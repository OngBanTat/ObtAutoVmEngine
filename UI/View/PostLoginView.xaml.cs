using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Navigation;
using ObtSDK;

namespace UI.View;

[ObfuscationAttribute(Exclude = false, Feature = "-rename")]
public sealed partial class PostLoginView : INotifyPropertyChanged
{
    private string _current;
    private string _expireDate;
    private string _username;

    public PostLoginView()
    {
        InitializeComponent();
        DataContext = this;
        Title = new Conf().APP_TITLE;
        if (Common.CheckMutant(Title))
        {
            MessageBox.Show("Multi instance detected! Close Application!");
            Environment.Exit(0);
        }

        new MainWindow().ShowDialog();
    }

    public string Current
    {
        get => _current;
        set => SetField(ref _current, value);
    }

    public string Username
    {
        get => _username;
        set => SetField(ref _username, value);
    }

    public string ExpireDate
    {
        get => _expireDate;
        set => SetField(ref _expireDate, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void PostLoginView_OnLoaded(object sender, RoutedEventArgs e)
    {
        Common.SetInterval(() =>
        {
            if (Config.LastCheck.AddMinutes(6) < DateTime.Now) Environment.Exit(0);
            Current = DateTime.Now.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss");
        }, 1000);
        var epochMilliseconds = Config.Session.User.Additional.ExpiredAt; // Epoch time in milliseconds
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var date = epoch.AddMilliseconds(epochMilliseconds);
        var dateString = date.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss");
        Username = "Login as: " + Config.Session.User.Username + " -- Max: " +
                   Conf.Instance.ProjectTypeMaxTab[Config.Session.User.Additional.Additional] + " Tabs";
        ExpireDate = "Expired at: " + dateString;
        Current = DateTime.Now.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss");
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(e.Uri.AbsoluteUri);
        e.Handled = true;
    }

    private void PostLoginView_OnClosed(object sender, EventArgs e)
    {
        var windowName = "Multi monitor - Created by K9 from Kteam";
        WindowHelper.CloseAllWindowsByTitle(windowName);
        Environment.Exit(0);
    }
}