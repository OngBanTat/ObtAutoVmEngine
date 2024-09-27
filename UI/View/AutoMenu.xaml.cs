using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;
using ObtSDK;
using ObtSDK.AutoAndroidVm;
using UI.Controller;
using UI.Model;

namespace UI.View;

[Obfuscation(Exclude = false, Feature = "-rename")]
public sealed partial class AutoMenu : INotifyPropertyChanged
{
    private readonly DispatcherTimer _timerCheckDevices = new();

    private bool _isSelectAll;

    private ObservableCollection<Device> _listDevices = [];


    private Thread t;

    public AutoMenu()
    {
        InitializeComponent();
        DataContext = this;
        // HookMouse();
    }


    public BaseDeviceInfo? SelectedDevice
    {
        get => null;
        set
        {
            if (value != null) value.IsSelected = !value.IsSelected;
            _isSelectAll = ListDevices.All(x => x.IsSelected);
        }
    }


    public ObservableCollection<Device> ListDevices
    {
        get => _listDevices;
        set => SetField(ref _listDevices, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;


    // private void HookMouse()
    // {
    //     MouseHook.Start((@event, _) =>
    //     {
    //         if (@event != MouseHook.MouseEvent.RUp) return;
    //         var hWnd = WindowHelper.GetForegroundWindow();
    //         var clzName = new StringBuilder(256);
    //         WindowHelper.GetClassName(hWnd, clzName, 256);
    //         Console.WriteLine("parent: " + ProcessHelper.GetWindowTitle(hWnd) + " -- " + clzName);
    //         var a = AutoCtrl.GetChildHandle(hWnd);
    //         foreach (var ptr in a)
    //         {
    //             WindowHelper.GetClassName(ptr, clzName, 256);
    //             Console.WriteLine("Child: " + ProcessHelper.GetWindowTitle(ptr) + " -- " + clzName);
    //         }
    //     });
    // }

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

    private void LoadDevices()
    {
        var maxTab = Conf.Instance.ProjectTypeMaxTab[Config.Session.User.Additional.Additional];
        VmHelper.LoadDevices(ListDevices, maxTab);
    }


    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        LoadDevices();
        _timerCheckDevices.Interval = TimeSpan.FromSeconds(10.0);
        _timerCheckDevices.Tick += TimerCheckDevices_Tick;
        _timerCheckDevices.Start();
    }

    private void TimerCheckDevices_Tick(object? sender, EventArgs e)
    {
        LoadDevices();
    }


    private void OnBtnAllClick(object sender, RoutedEventArgs e)
    {
        _isSelectAll = !_isSelectAll;
        foreach (var item in ListDevices) item.IsSelected = _isSelectAll;
    }

    private void BtnStopAllClick(object sender, RoutedEventArgs e)
    {
        foreach (var item in ListDevices)
        {
            if (!item.IsSelected) continue;
            item.StopAuto();
        }
    }

    private void BtnRunAllClick(object sender, RoutedEventArgs e)
    {
        foreach (var item in ListDevices)
        {
            if (!item.IsSelected) continue;
            item.StartAuto(BuildAutoThread(item));
        }
    }

    private Action BuildAutoThread(Device device)
    {
        return () =>
        {
            var ctl = new AutoController(device);
            ctl.Run();
        };
    }

    private void ButtonStart_Click(object sender, RoutedEventArgs e)
    {
        var clickedButton = (Button)sender;
        // Use the button's DataContext to find out which item it was in the ListView
        var item = clickedButton.DataContext as Device;
        item.StartAuto(BuildAutoThread(item));
    }

    private void ButtonStop_Click(object sender, RoutedEventArgs e)
    {
        var item =  (sender as Button)?.DataContext as Device;
        item.StopAuto();
    }

    private void ButtonPause_Click(object sender, RoutedEventArgs e)
    {
        var item = (sender as Button)?.DataContext as Device;
        item?.PauseAuto();
    }

    private void ButtonResume_Click(object sender, RoutedEventArgs e)
    {
        var item = (sender as Button)?.DataContext as Device;
        item?.ResumeAuto();
    }

    private void ButtonScreenshot_Click(object sender, RoutedEventArgs e)
    {
        var device = (sender as Button)?.DataContext as Device;
        // device?.SetWindowsPositionAndSize();
        var imageViewer = new ImageViewer(device);
        imageViewer.EnableMirrorDevice();
        imageViewer.ShowDialog();
    }

    private void ButtonTest_OnClick(object sender, RoutedEventArgs e)
    {
        var device = (sender as Button)?.DataContext as Device;
        if (device.TestThread != null) device.TestThread.Abort();
        device.TestThread = new Thread(() => { new AutoController(device).Test(); });
        device.TestThread.Start();
        // device.Home();
    }


    private void Button_MirrorAll_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var windowName = "Multi monitor - Created by K9 from Kteam";
            WindowHelper.CloseAllWindowsByTitle(windowName);
            // Specify the path to the exe file
            var directoryInfo = new DirectoryInfo("assets\\monitor");
            var startInfo = new ProcessStartInfo
            {
                FileName = "app.exe",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
                WorkingDirectory = directoryInfo.FullName
            };

            // Start the process
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}