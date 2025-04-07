
//#define Migration
using WS;
using WS.Core;
using WS.Core.Tool;



if (BuildDataBase()) return;
var result = GetArgs(args);
if (!result.succ) return;


WSApplicationTool.Run<TestApplication>(args, result.serverName, "app.json");




bool BuildDataBase()
{
    //Add-Migration Initial
    // Update-DataBase
#if Migration
    var builder = WebApplication.CreateBuilder(args);
    TestApplication.ConfigDB(builder.Services);
    builder.Build();
    return true;
#endif
    return false;
}

(bool succ, string? serverName) GetArgs(string[] args)
{
    string serverName = string.Empty;

    var succ = true;

    foreach (var arg in args)
    {
        if (arg.Contains("--h"))
            break;
        if (arg.Contains("--s"))
            serverName = arg.Split(':')[1];
    }

    succ = !string.IsNullOrEmpty(serverName);
    if (Context.mode == Context.RunMode.VS)
        succ = true;
    if (!succ) PrintMenu();
    return (succ, serverName);
}
void PrintMenu()
{
    Console.WriteLine("--h              --> for help ");
    Console.WriteLine("--s:{ServerName} --> choose Server[not null]");
}