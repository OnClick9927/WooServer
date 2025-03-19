using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WS.Core;
using WS.Core.Config;
using WS.Core.Tool;
using WS.DB;
using WS.Test;

namespace WS;


class TestApplication : IApplication
{
    ILogger logger = LogTools.CreateLogger(typeof(TestApplication));

    private readonly IOptionsSnapshot<RootConfig> root;
    public TestApplication(IOptionsSnapshot<RootConfig> root)
    {
        this.root = root;
    }
    public static void ConfigDB(IServiceCollection services)
    {
        services.AddDbContext<TodoDbContext>((builder) =>
        {
            string dir = $"{Environment.CurrentDirectory}/databases";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            builder.UseSqlite($"Data Source={dir}/_sqlite_{nameof(TodoDbContext)}.db");
        });
    }
    void IApplication.ConfigureApplicationServices(IServiceCollection services)
    {
        ConfigDB(services);
    }
    void IApplication.ConfigureApplication(WebApplication web_application)
    {

    }
    void IApplication.OnShutDown()
    {

    }
    async void IApplication.OnEnter()
    {
        logger.LogWarning($"snow flake Test {IDTools.NewId()}");
        await Task.Delay(1000);
        var result = await RPC.RPCCreateTodo(100);
        logger.LogInformation(result.Content.ToString());
    }




}
