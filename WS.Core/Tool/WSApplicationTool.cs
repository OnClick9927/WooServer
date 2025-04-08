using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using WS.Core.Config;
using Newtonsoft.Json;
namespace WS.Core.Tool;

public static class WSApplicationTool
{

    private static void AddAttributeServices(this IServiceCollection services, ILogger logger, ServerType serverType)
    {
        TypeTools.GetTypesWithAttribute(typeof(ServiceAttribute), false).ToList().ForEach(type =>
        {


            var attr = type.GetCustomAttribute<ServiceAttribute>();
            if (!attr.ServerType.HasFlag(serverType)) return;
            logger.LogInformation($"AddServiceByAttr---> {type}");
            switch (attr.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(type); break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(type); break;
                case ServiceLifetime.Transient:
                    services.AddTransient(type); break;
                default:
                    break;
            }
        });

    }


    private static void ConfigApplicationServices(this IServiceCollection services, IEnumerable<Type> serviceTypes, IServiceProvider provider, ILogger logger, ServerType serverType)
    {
        services.AddAttributeServices(logger, serverType);
        foreach (var type in serviceTypes)
        {
            var cfg = provider.GetRequiredService(type) as IApplicationConfiguration;
            if (!cfg.Fit().HasFlag(serverType)) continue;
            logger.LogInformation($"---> {type}");
            cfg.ConfigureServices(services);
        }

    }

    private static void ConfigApplication(this WebApplication web_application, IEnumerable<Type> configTypes, IServiceProvider provider, ILogger logger, ServerType serverType)
    {
        foreach (var type in configTypes)
        {
            var cfg = provider.GetRequiredService(type) as IApplicationConfiguration;
            if (!cfg.Fit().HasFlag(serverType)) continue;

            logger.LogInformation($"---> {type}");
            cfg.Configure(web_application);
        }
    }

    public static void Run<TApplication>(string[] args, string? serverName, string rootConfigName = "app.json")
        where TApplication : class, IApplication
    {

        IServiceCollection services = new ServiceCollection();
        IConfiguration config = ConfigTools.LoadConfig(rootConfigName);
        services.Configure<RootConfig>(config, e => config.Bind(e));
        var configTypes = typeof(IApplicationConfiguration).GetSubTypes().ToList();
        configTypes.ForEach(x => services.AddSingleton(x));
        services.AddTransient<TApplication>();

        IServiceProvider provider = services.BuildServiceProvider();

        var config_root = provider.GetRequiredService<IOptionsSnapshot<RootConfig>>();
        LogTools.InitLog(config_root.Value.Log);
        ILogger logger = LogTools.CreateLogger<TApplication>();
        Context.config = config_root;



        if (!Context.SetCurrentServer(serverName))
        {
            logger.LogCritical("SetCurrent Server Err");
            return;
        }
        logger.LogInformation($"启动服务器{Context.CurrentServer}");
        IDTools.Init(Context.CurrentServer.Snowflake);

        var app = provider.GetRequiredService<TApplication>();



        ///////////////////////////////////////////////////////////////
        var builder = WebApplication.CreateBuilder(args);

        ServerType type = Context.ServerType;


        logger.LogInformation("---配置服务 开始------------------------------");
        builder.Services.Configure<RootConfig>(config, e => config.Bind(e));
        builder.Services.ConfigApplicationServices(configTypes, provider, logger, type);
        app.ConfigureApplicationServices(builder.Services);
        logger.LogInformation("---配置服务 结束------------------------------");
        var web_application = builder.Build();
        Context.Config(web_application);





        logger.LogInformation("---配置服务 开始------------------------------");
        web_application.ConfigApplication(configTypes, provider, logger, type);
        app.ConfigureApplication(web_application);
        logger.LogInformation("---配置服务 结束------------------------------");

        WaitLife(app, logger, web_application, TimeSpan.FromSeconds(config_root.Value.ServerLaunchTime));
        web_application.Run(Context.CurrentServer.LaunchUrl);
    }

    private async static void WaitLife<TApplication>(TApplication startup, ILogger logger, WebApplication application, TimeSpan span) where TApplication : IApplication
    {
        await Task.Delay(span);
        logger.LogInformation("---进入APP------------------------------");
        startup.OnEnter();
        await application.WaitForShutdownAsync();
        logger.LogInformation("---ShutDown------------------------------");
        startup.OnShutDown();
    }

}
