using System.IO;
using ObtSDK.AutoAndroidVm;
using ObtSDK.Utils;
using Size = System.Windows.Size;

namespace Launcher.Model;

public class Device : BaseDeviceInfo
{
    protected override Size FixedWindowSize => Conf.Instance.VmScreenSize;

    public new CancellationTokenSource? CancellationTokenSource => base.CancellationTokenSource;

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
        ThrowIfStop();        
        if (clearField)
            await DelCharsAsync(40);
        await DelayAsync();
        await SendTextAsync(noidung);
        await DelayAsync();
        while (await FindAndClickAsync(Assets("btn_xong.png"), 0.7, 3, 3, 0, 49, 61, 32)) await DelayAsync(1000);
    }


    #endregion
}