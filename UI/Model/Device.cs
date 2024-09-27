using System.Reflection;
using ObtSDK.AutoAndroidVm;
using Size = System.Windows.Size;

namespace UI.Model;

[Obfuscation(Exclude = false, Feature = "-rename")]
public class Device : BaseDeviceInfo
{
    protected override Size FixedWindowSize => Conf.Instance.VmScreenSize;
    public Thread TestThread { get; set; }
}