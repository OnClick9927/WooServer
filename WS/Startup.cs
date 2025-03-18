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
        //services.AddDbContext<TodoDbContext>((builder) =>
        //{
        //    //config?.Invoke(builder, typeof(TodoDbContext));
        //    string dir = $"{AppContext.BaseDirectory}/databases";
        //    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);


        //    builder.UseSqlite($"Data Source={dir}/_sqlite.db");


        //}, ServiceLifetime.Singleton);
    }
    void IApplicationStartup.ConfigApplicationServices(IServiceCollection services)
    {
        ConfigDB(services);
    }
    async void IApplicationStartup.ConfigApplication(WebApplication web_application)
    {
        var logger = LogTools.CreateLogger<Startup>();

        await Task.Delay(10);
        logger.LogInformation("---进入APP------------------------------");
        Enter();
        await web_application.WaitForShutdownAsync();
        logger.LogInformation("---ShutDown------------------------------");
        ShutDown();
    }
     void ShutDown()
    {

    }
     async void Enter()
    {
        ILogger logger = LogTools.CreateLogger(typeof(Startup));
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
