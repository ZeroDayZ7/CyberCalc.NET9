using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace WPF_CALC_NET_9
{
    public partial class App : Application
    {
        private readonly string logFile = "error.log";
        private Mutex? _mutex;
        private const string MutexName = "WPF_CALC_NET_9_Mutex";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Sprawdź, czy aplikacja jest już uruchomiona
            if (!CheckSingleInstance())
            {
                MessageBox.Show("Aplikacja jest już uruchomiona!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                Shutdown();
                return;
            }

            // Globalne handlery dla wyjątków
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Wyczyść subskrypcje zdarzeń
            DispatcherUnhandledException -= App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;

            // Zwolnij Mutex
            ReleaseMutex();

            base.OnExit(e);
        }

        // Sprawdzenie pojedynczej instancji
        private bool CheckSingleInstance()
        {
            try
            {
                _mutex = new Mutex(true, MutexName, out bool createdNew);
                return createdNew;
            }
            catch (Exception ex)
            {
                LogException(ex);
                MessageBox.Show($"Błąd podczas sprawdzania instancji aplikacji:\n{ex}", "Błąd krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        // Zwolnienie Mutex
        private void ReleaseMutex()
        {
            try
            {
                _mutex?.ReleaseMutex();
                _mutex?.Dispose();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        // UI thread exceptions
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (!Dispatcher.HasShutdownStarted)
            {
                MessageBox.Show($"Wystąpił błąd:\n{e.Exception}", "Błąd aplikacji", MessageBoxButton.OK, MessageBoxImage.Error);
                LogException(e.Exception);
                e.Handled = true;
            }
        }

        // Non-UI thread exceptions
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                // Sprawdź, czy Dispatcher jest dostępny i aplikacja nie została zamknięta
                if (Application.Current != null && Application.Current.Dispatcher != null && !Application.Current.Dispatcher.HasShutdownStarted)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show($"Nieoczekiwany błąd:\n{ex}", "Błąd krytyczny", MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                }

                // Zawsze loguj do pliku
                LogException(ex);
            }
        }

        private void LogException(Exception ex)
        {
            try
            {
                File.AppendAllText(logFile, $"{DateTime.Now}: {ex}\n\n");
            }
            catch
            {
                // Ignoruj błędy logowania, aby nie crashować aplikacji
            }
        }
    }
}