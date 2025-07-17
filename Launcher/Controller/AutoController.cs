using Launcher.Model;

namespace Launcher.Controller;

public class AutoController(Device d)
{
    
    public async Task<bool> Run()
    {
        d.TriggerStop();
        var ret =  d.FindAndClick(d.Assets("icongame.png"), 0.7);
        await d.DelayAsync(10000);
        Console.WriteLine(ret);
        return true;
    }


    public dynamic Test()
    {
        Console.WriteLine("Test");

        return true;
    }
}