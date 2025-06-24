using Launcher.Model;

namespace Launcher.Controller;

public class AutoController(Device d)
{
    public async Task<bool> Run()
    {
        d.DoneAuto();
        var screen = d.AdbDeviceClient.DumpScreen();
        screen.Save("screen.xml");
        throw new Exception("Test");
        return true;
    }


    public dynamic Test()
    {
        Console.WriteLine("Test");
        d.OpenApp("com.wildlife.games.battle.royale.free.zooba");
        return true;
    }
}