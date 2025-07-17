using System.IO;
using System.Reflection;
using ObtSDK.AutoAndroidVm;
using ObtSDK.ObtApis.Services;
using Size = System.Windows.Size;

namespace Launcher.Model;

public class Device : BaseDeviceInfo
{
    private readonly IObtApiServices _obtApiServices = ObtApiServicesImp.GetInstance();
    protected override Size FixedWindowSize => Conf.Instance.VmScreenSize;

    /// <summary>
    /// Stops the current operation or process on the device by throwing an exception
    /// if a cancellation has been requested through the associated cancellation token.
    /// This method ensures any ongoing operation adheres to the requested cancellation state.
    /// </summary>
    /// <exception cref="System.OperationCanceledException">
    /// Thrown when the cancellation token signals a cancellation request.
    /// </exception>
    public void TriggerStop()
    {
        CancellationTokenSource?.Token.ThrowIfCancellationRequested();
    }

    public string Assets(string path)
    {
        return DType switch
        {
            DeviceType.LdPlayer => @"assets\data\LDPlayer\" + path,
            DeviceType.MEmu => @"assets\data\Memu\" + path,
            DeviceType.Adb => @"assets\data\Adb\" + path,
            DeviceType.LdAdb => @"assets\data\LDPlayer\" + path,
            DeviceType.MemuAdb => @"assets\data\Memu\" + path,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public new async Task Delay(int milliseconds = 200)
    {
        TriggerStop();
        await DelayAsync(milliseconds);
    }


    #region Common Utils

    public static void ClearTempFolder()
    {
        var pngFiles = Directory.GetFiles("assets\\temp", "*.png");
        if (pngFiles.Length <= 1000) return;
        foreach (var file in pngFiles)
            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file {file}: {ex.Message}");
            }
    }

    public async Task NhapNoiDung(string noidung, bool clearField = false)
    {
        if (clearField)
            DelChars(200);
        await Delay(100);
        SendText(noidung, 2);
        await Delay(500);
    }

    #endregion
}