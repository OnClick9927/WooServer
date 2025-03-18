using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using WS.Core;
using WS.Core.Config;
using WS.HTTP;

namespace WS.Core.Tool;

public static class WSApplicationTool
{

    private static void AddAttributeServices(this IServiceCollection services, ILogger logger)
    {
        TypeTools.GetTypesWithAttribute(typeof(ServiceAttribute), false).ToList().ForEach(type =>
        {

            logger.LogInformation($"---> {type}");

            var attr = type.GetCustomAttribute<ServiceAttribute>();
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


    private static void ConfigApplicationServices(this IServiceCollection services, IEnumerable<Type> serviceTypes, IServiceProvider provider, ILogger logger)
    {
        services.AddAttributeServices(logger);
        foreach (var type in serviceTypes)
        {
            logger.LogInformation($"---> {type}");
            var cfg = provider.GetRequiredService(type) as IApplicationServiceConfig;
            cfg.ConfigureServices(services);
        }

    }

    private static void ConfigApplication(this WebApplication web_application, IEnumerable<Type> configTypes, IServiceProvider provider, ILogger logger)
    {
        foreach (var type in configTypes)
        {
            logger.LogInformation($"---> {type}");
            var cfg = provider.GetRequiredService(type) as IApplicationConfig;
            cfg.Config(web_application);
        }
    }

    public static IServiceProvider Run<TStartup>(string[] args, string rootConfigName = "root.json") where TStartup : class, IApplicationStartup
    {

        IServiceCollection services = new ServiceCollection();

        IConfiguration config = ConfigTools.LoadConfig(rootConfigName);
        services.AddOptions().Configure<RootConfig>(config, e => config.Bind(e));

        var serviceTypes = typeof(IApplicationServiceConfig).GetSubTypes().ToList();
        var configTypes = typeof(IApplicationConfig).GetSubTypes().ToList();
        serviceTypes.ForEach(x => services.AddSingleton(x));
        configTypes.ForEach(x => services.AddSingleton(x));
        ;
        services.AddTransient<TStartup>();

        IServiceProvider provider = services.BuildServiceProvider();

        Context.config = provider.GetRequiredService<IOptionsSnapshot<RootConfig>>();
        LogTools.InitLog();


        var startup = provider.GetRequiredService<TStartup>();





        startup.FitConfigTypes(serviceTypes, configTypes);
        startup.BeforeBuildWebApplication();
        ///////////////////////////////////////////////////////////////
        var builder = WebApplication.CreateBuilder(args);




        ILogger logger = LogTools.CreateLogger<TStartup>();
        logger.LogInformation("---配置服务 开始------------------------------");
        builder.Services.ConfigApplicationServices(serviceTypes, provider, logger);
        logger.LogInformation("---配置服务 结束------------------------------");
        startup.ConfigServices(builder.Services);
        var web_application = builder.Build();
        Context.Config(web_application);





        logger.LogInformation("---配置服务 开始------------------------------");
        web_application.ConfigApplication(configTypes, provider, logger);
        logger.LogInformation("---配置服务 结束------------------------------");

        startup.BeforeRun(web_application);
        web_application.Run(provider.GetRequiredService<IOptionsSnapshot<RootConfig>>().Value.Current.url);

        return provider;
    }

}
