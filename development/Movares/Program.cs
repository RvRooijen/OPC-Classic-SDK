using MovaresClientApp;

namespace Movares;

public class Program
{
    public static void Main()
    {
        var opcClient = new OpcClient();
        opcClient.CreateSession("http://localhost:8079/OPC/DA");
        opcClient.AddSubscription(1000);
        opcClient.LoadConfiguration("config.json");
        opcClient.StartUpdating(1000);

        while (true)
        {
            Thread.Sleep(1000);
        }
    }
}