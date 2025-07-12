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


    public void NhapNoiDung(string noidung, bool clearField = false)
    {
        if (clearField)
            DelChars(200);
        Delay(100);
        SendText(noidung, 2);
        Delay(500);
        ClickOnPosition(899, 500);
        Delay();
    }

    #endregion
}