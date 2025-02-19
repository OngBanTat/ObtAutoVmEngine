using Launcher.Model;

namespace Launcher.Controller;

public class AutoController(Device d)
{
    public async Task<bool> Run()
    {
        d.DoneAuto();
        return true;
    }


    public dynamic Test()
    {
        Console.WriteLine("Test");
        d.OpenApp("com.tepaylink.tamgioiphantranhmobile");
        return true;
    }
}