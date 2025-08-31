using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using WPF_CALC_NET_9.Properties;

namespace WPF_CALC_NET_9
{
    public partial class App : Application
    {
        private const string MutexName = "WPF_CALC_NET_9_Mutex";
        private const long MaxLogSizeBytes = 10 * 1024 * 1024; // 10 MB log rotation threshold
        private const int MaxErrorDialogs = 3; // Maximum number of error dialogs to display

        private int _errorDialogCount = 0;
        private static readonly SemaphoreSlim _logLock = new(1, 1);
        private Mutex? _mutex;

        // Log directory in user-local application data
        private readonly string logDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WPF_CALC_NET_9",
            "Logs");

        private string LogFilePath => Path.Combine(logDirectory, "error.log");

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                string savedTheme = Settings.Default.ThemeName;
                if (!string.IsNullOrEmpty(savedTheme))
                {
                    Helpers.ThemeManager.ApplyTheme(savedTheme);
                }

                var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
                Resources["AppVersion"] = $"v{version}";
                var titleAttr = Assembly.GetExecutingAssembly()
                                        .GetCustomAttribute<AssemblyTitleAttribute>();
                Resources["AppTitle"] = titleAttr?.Title ?? "CYBERPUNK CALC";

                if (!EnsureSingleInstance())
                {
                    MessageBox.Show("The application is already running!", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    Shutdown();
                    return;
                }

                DispatcherUnhandledException += App_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
            catch (Exception ex)
            {
                _ = LogExceptionAsync(ex, nameof(OnStartup));
                MessageBox.Show($"Failed to start the application: {ex.Message}",
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Unregister global exception handlers
            DispatcherUnhandledException -= App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;

            ReleaseMutex();
            base.OnExit(e);
        }

        /// <summary>
        /// Ensures only a single instance of the application is running.
        /// </summary>
        private bool EnsureSingleInstance()
        {
            try
            {
                _mutex = new Mutex(true, MutexName, out bool createdNew);
                return createdNew;
            }
            catch (Exception ex)
            {
                _ = LogExceptionAsync(ex, nameof(EnsureSingleInstance));
                MessageBox.Show($"Error while checking application instance:\n{ex}",
                    "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Releases the mutex when the application exits.
        /// </summary>
        private void ReleaseMutex()
        {
            if (_mutex != null)
            {
                try
                {
                    _mutex.ReleaseMutex();
                }
                catch (Exception ex)
                {
                    _ = LogExceptionAsync(ex, nameof(ReleaseMutex));
                }
                finally
                {
                    _mutex.Dispose();
                    _mutex = null;
                }
            }
        }

        /// <summary>
        /// Handles unhandled exceptions from the UI thread.
        /// </summary>
        private async void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (!Dispatcher.HasShutdownStarted)
            {
                if (Interlocked.Increment(ref _errorDialogCount) <= MaxErrorDialogs)
                {
                    MessageBox.Show($"An error has occurred:\n{e.Exception.Message}",
                        "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                await LogExceptionAsync(e.Exception, nameof(App_DispatcherUnhandledException));
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles unhandled exceptions from non-UI threads.
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                if (Current?.Dispatcher != null &&
                    !Current.Dispatcher.HasShutdownStarted &&
                    Interlocked.Increment(ref _errorDialogCount) <= MaxErrorDialogs)
                {
                    _ = Current.Dispatcher.InvokeAsync(() =>
                    {
                        MessageBox.Show($"An unexpected error occurred:\n{ex.Message}",
                            "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }

                _ = LogExceptionAsync(ex, nameof(CurrentDomain_UnhandledException));
            }
        }

        /// <summary>
        /// Logs exception details asynchronously with rotation and concurrency safety.
        /// </summary>
        private async Task LogExceptionAsync(Exception ex, string methodName, CancellationToken cancellationToken = default)
        {
            try
            {
                Directory.CreateDirectory(logDirectory);

                // Rotate the log file if it exceeds the defined size limit
                if (File.Exists(LogFilePath) && new FileInfo(LogFilePath).Length > MaxLogSizeBytes)
                {
                    string backupLog = Path.Combine(logDirectory, $"error_{DateTime.Now:yyyyMMddHHmmss}.log");
                    File.Move(LogFilePath, backupLog);
                }

                // Create a detailed log entry
                string logEntry = $@" 
                    [{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}]
                    OS: {Environment.OSVersion}
                    User: {Environment.UserName}
                    Version: {Assembly.GetExecutingAssembly().GetName().Version}
                    Thread: {Environment.CurrentManagedThreadId}
                    Method: {methodName}
                    Exception type: {ex.GetType().FullName}
                    Message: {ex.Message}
                    Source: {ex.Source}
                    Target site: {ex.TargetSite}
                    Stack trace:
                    {ex.StackTrace}
                    ";

                if (ex.InnerException is not null)
                {
                    logEntry += $"\nInner Exception: {ex.InnerException.GetType().FullName}\n"
                              + $"Message: {ex.InnerException.Message}\n"
                              + $"StackTrace: {ex.InnerException.StackTrace}\n";
                }

                logEntry += "------------------------------------------------------------\n";

                await _logLock.WaitAsync(cancellationToken);
                try
                {
                    await File.AppendAllTextAsync(LogFilePath, logEntry, cancellationToken);
                }
                finally
                {
                    _logLock.Release();
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore cancellation requests during logging
            }
            catch (Exception logEx)
            {
                Debug.WriteLine($"Error while writing logs: {logEx}");
            }
        }
    }
}
