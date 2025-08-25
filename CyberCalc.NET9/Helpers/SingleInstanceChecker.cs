namespace WPF_CALC_NET_9.Helpers;

public static class SingleInstanceChecker
{
    private static Mutex? _mutex;

    public static bool CheckSingleInstance()
    {
        string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "WPF_CALC_NET_9";
        _mutex = new Mutex(true, appName, out bool createdNew);
        return createdNew;
    }

    public static void ReleaseMutex()
    {
        _mutex?.ReleaseMutex();
    }
}