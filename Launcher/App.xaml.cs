using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using ObtSDK;
using ObtSDK.Utils;

namespace Launcher;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        // Register global exception handlers
#if RELEASE
        Config.Debug = false;
#endif
        // Check if the ADB_LOCAL_TRANSPORT_MAX_PORT environment variable is not set to "65000"
        if (Environment.GetEnvironmentVariable("ADB_LOCAL_TRANSPORT_MAX_PORT", EnvironmentVariableTarget.User) !=
            "65000")
        {
            // Terminate any running adb.exe processes
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("adb"))
            {
                try
                {
                    process.Kill(); // Attempt to kill the process
                    process.WaitForExit(); // Wait for the process to exit
                }
                catch (Exception ex)
                {
                    // Log any exception that occurs while trying to kill the process
                    Console.WriteLine($"Failed to terminate adb.exe process: {ex.Message}");
                }
            }

            // Set the ADB_LOCAL_TRANSPORT_MAX_PORT environment variable to "65000"
            Environment.SetEnvironmentVariable("ADB_LOCAL_TRANSPORT_MAX_PORT", "65000", EnvironmentVariableTarget.User);
        }

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        if (!Config.Debug)
        {
            new Thread(() => { PcInformation.GetAllPcInformation(); }).Start(); //Cache PC info
            SentrySdk.Init(o =>
            {
                o.Dsn = Conf.Instance.SentryURL;
                o.Debug = false;
                o.AutoSessionTracking = true;
                o.IsGlobalModeEnabled = true;
                o.TracesSampleRate = 1.0;
                o.CaptureFailedRequests = true;
                o.Release = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                o.Environment = "Production";
                o.AttachStacktrace = true;
                o.MaxBreadcrumbs = 100;
                o.MaxCacheItems = 100;
                o.MaxQueueItems = 100;
            });
            SentrySdk.StartSession();
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SentrySdk.EndSession();
        SentrySdk.Close();

        base.OnExit(e);
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (!Config.Debug)
        {
            SentrySdk.AddBreadcrumb("Unhandled exception occurred");
            SentrySdk.CaptureException(e.ExceptionObject as Exception, scope =>
            {
                scope.SetTag("Environment", "Production");
                scope.SetExtra("PC Info", PcInformation.GetAllPcInformation());
                scope.SetExtra("User ID", Config.Session.User.Username);
                scope.SetExtra("Token", Config.Session.Token);
                scope.SetExtra("ProjectID", Conf.Instance.ProjectId);
                scope.SetExtra("AccountConfigId", Config._instance?.AccountConfigId);
            });
        }
    }

    private void OnDispatcherUnhandledException(object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        if (!Config.Debug)
        {
            SentrySdk.AddBreadcrumb("A dispatcher unhandled exception occurred");
            SentrySdk.CaptureException(e.Exception, scope =>
            {
                scope.SetTag("Environment", "Production");
                scope.SetExtra("PC Info", PcInformation.GetAllPcInformation());
                scope.SetExtra("User ID", Config.Session.User.Username);
                scope.SetExtra("Token", Config.Session.Token);
                scope.SetExtra("ProjectID", Conf.Instance.ProjectId);
                scope.SetExtra("AccountConfigId", Config._instance?.AccountConfigId);
            });
            e.Handled = true; // Prevent application from crashing
        }
    }

    private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        if (!Config.Debug)
        {
            SentrySdk.AddBreadcrumb("An unobserved task exception occurred");
            SentrySdk.CaptureException(e.Exception, scope =>
            {
                scope.SetTag("Environment", "Production");
                scope.SetExtra("PC Info", PcInformation.GetAllPcInformation());
                scope.SetExtra("User ID", Config.Session.User.Username);
                scope.SetExtra("Token", Config.Session.Token);
                scope.SetExtra("ProjectID", Conf.Instance.ProjectId);
                scope.SetExtra("AccountConfigId", Config._instance?.AccountConfigId);
            });
            e.SetObserved(); // Prevent application from crashing
        }
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
    }
}