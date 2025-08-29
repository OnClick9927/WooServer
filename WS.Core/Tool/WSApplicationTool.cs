using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using WS.Core.Config;
namespace WS.Core.Tool;

public static class WSApplicationTool
{

    private static void AddAttributeServices(this IServiceCollection services, ILogger logger, int serverType)
    {
        TypeTools.GetTypesWithAttribute(typeof(ServiceAttribute), false).ToList().ForEach(type =>
        {


            var attr = type.GetCustomAttributes<ServiceAttribute>()
                .FirstOrDefault(x => x.FitAllServer || x.ServerType == serverType);
            if (attr == null) return;
            logger.LogInformation($"ServiceAttribute {attr.Lifetime}--->{type}");
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


    private static void ConfigApplicationServices(this IServiceCollection services, IEnumerable<Type> serviceTypes, IServiceProvider provider, ILogger logger, int serverType, IApplication application)
    {
        services.AddAttributeServices(logger, serverType);
        foreach (var type in serviceTypes)
        {
            if (!application.Fit(serverType, type)) continue;

            var cfg = provider.GetRequiredService(type) as IApplicationConfiguration;
            //if (!cfg.Fit(serverType)) continue;
            logger.LogInformation($"---> {type}");
            cfg.ConfigureServices(services);
        }

    }

    private static void ConfigApplication(this WebApplication web_application, IEnumerable<Type> configTypes,
        IServiceProvider provider, ILogger logger, int serverType, IApplication application)
    {
        foreach (var type in configTypes)
        {
            if (!application.Fit(serverType, type)) continue;
            var cfg = provider.GetRequiredService(type) as IApplicationConfiguration;

            logger.LogInformation($"---> {type}");
            cfg.Configure(web_application);
        }
    }

    public static void Run<TApplication>(string[] args, string? serverCfg, string rootConfigName = "app.json")
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
        //Context.config = config_root;
        var app = provider.GetRequiredService<TApplication>();
        try
        {
            ServerConfig server_config = ConfigTools.LoadConfig<ServerConfig>(new ServerConfig(), serverCfg);
            bool succ = app.LegalServerConfig(server_config);
            if (!succ)
            {
                logger.LogCritical($"服务器配置错误{server_config}");

                return;
            }
            Context.SetCurrentServer(config_root, server_config);
        }
        catch (Exception ex)
        {

            logger.LogCritical($"SetCurrent Server Err {ex}");
            return;
        }

        logger.LogInformation($"启动服务器{Context.CurrentServer}");
        IDTools.Init(Context.CurrentServer.Snowflake);




        ///////////////////////////////////////////////////////////////
        var builder = WebApplication.CreateBuilder(args);

        int type = Context.ServerType;


        logger.LogInformation("---配置服务 开始------------------------------");
        builder.Services.Configure<RootConfig>(config, e => config.Bind(e));
        builder.Services.AddHostedService<GracefulShutdownService>();
        builder.Services.AddSingleton(app);

        builder.Services.ConfigApplicationServices(configTypes, provider, logger, type, app);
        app.ConfigureApplicationServices(builder.Services);
        logger.LogInformation("---配置服务 结束------------------------------");
        var web_application = builder.Build();
        Context.Config(app, web_application);





        logger.LogInformation("---配置服务 开始------------------------------");
        web_application.ConfigApplication(configTypes, provider, logger, type, app);
        app.ConfigureApplication(web_application);
        logger.LogInformation("---配置服务 结束------------------------------");
        web_application.Run(Context.CurrentServer.LaunchUrl);
    }



}


