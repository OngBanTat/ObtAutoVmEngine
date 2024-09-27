using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using ObtSDK;
using UI.Model;
using MessageBox = System.Windows.Forms.MessageBox;

namespace UI;

public class Conf : Config
{
    public static Config Instance => _instance ??= new Conf();
    public override Size VmScreenSize => new(960, 540);
    public override string APP_TITLE => "Auto game by XYZ";

    #region Liên hệ admin OBT lấy thông tin cấu hình riêng cho app thu phí
    // public override Dictionary<string, int> ProjectTypeMaxTab => new()
    // {
    //     { "62513d40-65d7-11ef-84ed-f5078c2d4879", 50 },
    //     { "c6498430-67ab-11ef-84ed-f5078c2d4879", 40 },
    //     { "d03f90b0-67ab-11ef-84ed-f5078c2d4879", 30 },
    //     { "e0097740-67ab-11ef-84ed-f5078c2d4879", 20 },
    //     { "f5a69ba0-67ab-11ef-84ed-f5078c2d4879", 10 },
    //     { "9c786100-67ae-11ef-84ed-f5078c2d4879", 1 }
    // };
    // public override string ProjectId => "AUTO_TGPT";
    #endregion
  


    // public override string? SentryURL =>"https://d2b902d5bd435517a301bed11a5f8f08@o930542.ingest.us.sentry.io/4507864935694336";
   
}