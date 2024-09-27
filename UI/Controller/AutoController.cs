using System.Drawing;
using System.IO;
using System.Windows.Input;
using KAutoHelper;
using ObtSDK;
using ObtSDK.AutoAndroidVm;
using UI.Model;
using UI.View;
using Point = System.Windows.Point;

namespace UI.Controller;

public class AutoController
{
    private static readonly TesseractClient tesseract = new();
    private readonly string assets = @"assets\data\LDPlayer\";
    private readonly Device d;

    public AutoController(Device d)
    {
        this.d = d;
        assets = d.DType switch
        {
            BaseDeviceInfo.DeviceType.LdPlayer => @"assets\data\LDPlayer\",
            BaseDeviceInfo.DeviceType.MEmu => @"assets\data\Memu\",
            _ => assets
        };
    }

    private void NhapNoiDung(string noidung, bool clearField = false)
    {
        if (clearField)
            for (var i = 0; i < 40; i++)
            {
                if (d.DType == BaseDeviceInfo.DeviceType.LdPlayer)
                {
                    d.SendKeyBoardPress(Key.Back);
                }
                else
                {
                    d.SendKeyBoardPress(Key.Left);
                    Delay(10);
                    d.SendKeyBoardPress(Key.Delete);
                }

                Delay(10);
            }


        Delay(100);
        d.SendText(noidung);
        Delay(100);
        d.ClickByPercent(2.40, 11.85);
        Delay();
    }

    private void Delay(int delay = 200)
    {
        while (delay > 0)
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(1.0));
            delay--;
        }
    }


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


    public bool Run()
    {
        return true;
    }


    public void Test()
    {
      
    }
}