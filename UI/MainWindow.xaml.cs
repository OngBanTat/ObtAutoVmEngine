using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using ObtSDK;
using ObtSDK.Services;

namespace UI;

[Obfuscation(Exclude = false, Feature = "-rename")]
public sealed partial class MainWindow : INotifyPropertyChanged
{
    private string? _password;
    private string? _username;


    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        Closing += OnClosing;
    }

    public string? Username
    {
        get => _username;
        set => SetField(ref _username, value);
    }

    public string? Password
    {
        get => _password;
        set => SetField(ref _password, value);
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnClosing(object sender, CancelEventArgs e)
    {
        if (string.IsNullOrEmpty(Config.AccessToken))
            Dispatcher.Invoke(delegate { Environment.Exit(0); });
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        BtnLogin.IsEnabled = false;
        try
        {
            await ObtObtApiServicesImp.GetInstance().GetLic<Conf>(Username, Password);
            Close();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message);
        }

        BtnLogin.IsEnabled = true;
    }

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

    private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && BtnLogin.IsEnabled)
            // The Enter key is pressed
            LoginButton_Click(sender, null);
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Username = LocalStorage.GetItem("username") ?? "";
            Password = LocalStorage.GetItem("password") ?? "";
            // if (Username != "" && Password != "")
            //     LoginButton_Click(sender, e);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(e.Uri.AbsoluteUri);
        e.Handled = true;
    }
}