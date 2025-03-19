//#define GEN_DATABAASE
using WS.Core.Tool;

namespace WS;

class Program
{
    static void PrintMenu()
    {
        Console.WriteLine("--h              --> for help ");
        Console.WriteLine("--s:{ServerName} --> choose Server[not null]");
    }


    private static (bool succ, string? serverName) GetArgs(string[] args)
    {
        string serverName = string.Empty;
#if !DEBUG

        foreach (var arg in args)
        {
            if (arg.Contains("--h"))
                break;
            if (arg.Contains("--s"))
                serverName = arg.Split(':')[1];
        }
        return (!string.IsNullOrEmpty(serverName), serverName);
#endif
        return (true, serverName);
    }
    private static void Main(string[] args)
    {
#if GEN_DATABAASE
        var builder = WebApplication.CreateBuilder(args);
        TestApplication.ConfigDB(builder.Services);
        builder.Build();
        return;
#endif

        var result = GetArgs(args);
        if (!result.succ)
        {
            PrintMenu();
            return;
        }
        WSApplicationTool.Run<TestApplication>(args, result.serverName, "root.json");
    }
}