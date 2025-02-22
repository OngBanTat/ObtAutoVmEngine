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
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
#if RELEASE
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
#endif
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SentrySdk.EndSession();
        SentrySdk.Close();

        base.OnExit(e);
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
#if RELEASE
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
#endif
    }

    private void OnDispatcherUnhandledException(object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
#if RELEASE
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
#endif
    }

    private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
#if RELEASE
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
#endif
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
    }
}