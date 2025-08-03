using Launcher.Model;

namespace Launcher.Controller;

public class AutoController(Device d)
{
    private async Task DelayAsync(int milliseconds)
    {
        while (true)
        {
            await d.DelayAsync(milliseconds);
            Console.WriteLine($"Delay for {milliseconds} milliseconds");
        }
    }

    public async Task<bool> Run()
    {
        Console.WriteLine("Run");
        d.Status = "Running";
        int count = 0;
        while (true)
        {
            await DelayAsync(1000);
            Console.WriteLine("Still running...");
            d.Status = "Still running..." + count++;
        }

        Console.WriteLine("Done");
        return true;
    }


    public dynamic Test()
    {
        Console.WriteLine("Test");

        return true;
    }
}