using WS.Core.Tool;

namespace WS;

class Program
{
    private static void Main(string[] args)
    {
        WSApplicationTool.Run<Startup>(args, "root.json");
    }
}