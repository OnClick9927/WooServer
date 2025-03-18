using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WS.Core;
using WS.Core.Config;
using WS.Core.Tool;
using WS.DB;
using WS.Test;

namespace WS;


class Startup : IApplicationStartup
{
    ILogger logger = LogTools.CreateLogger(typeof(Startup));

    private readonly IOptionsSnapshot<RootConfig> root;
    public Startup(IOptionsSnapshot<RootConfig> root)
    {
        this.root = root;
    }
    public static void ConfigDB(IServiceCollection services)
    {
        DBServiceTool.AddDBContexts(services, (builder, Type) =>
        {
            //config?.Invoke(builder, typeof(TodoDbContext));
            string dir = $"{AppContext.BaseDirectory}/databases";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            builder.UseSqlite($"Data Source={dir}/_sqlite_{Type.Name}.db");
        });
    }
    void IApplicationStartup.ConfigApplicationServices(IServiceCollection services)
    {
        ConfigDB(services);
    }
    void IApplicationStartup.ConfigApplication(WebApplication web_application)
    {

    }
    void IApplicationStartup.OnShutDown()
    {

    }
    async void IApplicationStartup.OnEnter()
    {
        logger.LogWarning($"snow flake Test {IDTools.NewId()}");
        await Task.Delay(1000);
        var result = await RPC.RPCCreateTodo(new TodoItemDTO()
        {
            Name = "zz",
            IsComplete = false

        }, 100);
        logger.LogInformation(result.ToString());
    }




}
