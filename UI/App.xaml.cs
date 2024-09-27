using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using ObtSDK;
using Sentry;

namespace UI;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
[Obfuscation(Exclude = false, Feature = "-rename")]
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
        SentrySdk.Init(o =>
        {
            o.Dsn = Conf.Instance.SentryURL;
            o.Debug = true;
            o.TracesSampleRate = 1.0;
        });
        SentrySdk.StartSession();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SentrySdk.EndSession();
        SentrySdk.Close();

        base.OnExit(e);
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        SentrySdk.AddBreadcrumb("Unhandled exception occurred");
        SentrySdk.CaptureException(e.ExceptionObject as Exception, scope =>
        {
            scope.SetTag("Environment", "Production");
            scope.SetExtra("User ID", Config.Session.User.Username);
            scope.SetExtra("Token", Config.Session.Token);
            scope.SetExtra("ProjectID", Conf.Instance.ProjectId);
            var maxTab = Conf.Instance.ProjectTypeMaxTab[Config.Session.User.Additional.Additional];
            scope.SetExtra("maxTab", maxTab);
        });
    }

    private void OnDispatcherUnhandledException(object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        SentrySdk.AddBreadcrumb("A dispatcher unhandled exception occurred");
        SentrySdk.CaptureException(e.Exception, scope =>
        {
            scope.SetTag("Environment", "Production");
            scope.SetExtra("User ID", Config.Session.User.Username);
            scope.SetExtra("Token", Config.Session.Token);
            scope.SetExtra("ProjectID", Conf.Instance.ProjectId);
            var maxTab = Conf.Instance.ProjectTypeMaxTab[Config.Session.User.Additional.Additional];
            scope.SetExtra("maxTab", maxTab);
        });
        e.Handled = true; // Prevent application from crashing
    }

    private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        SentrySdk.AddBreadcrumb("An unobserved task exception occurred");
        SentrySdk.CaptureException(e.Exception, scope =>
        {
            scope.SetTag("Environment", "Production");
            scope.SetExtra("User ID", Config.Session.User.Username);
            scope.SetExtra("Token", Config.Session.Token);
            scope.SetExtra("ProjectID", Conf.Instance.ProjectId);
            var maxTab = Conf.Instance.ProjectTypeMaxTab[Config.Session.User.Additional.Additional];
            scope.SetExtra("maxTab", maxTab);
        });
        e.SetObserved(); // Prevent application from crashing
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
    }
}