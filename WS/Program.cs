//#define Mid
using WS.Core.Tool;

namespace WS;

class Program
{
    private static void Main(string[] args)
    {
#if Mid
        var builder = WebApplication.CreateBuilder(args);
        Startup.ConfigDB(builder.Services);
        builder.Build();
#endif
        WSApplicationTool.Run<Startup>(args, "root.json");
    }
}