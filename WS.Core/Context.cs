using Coravel.Scheduling.Schedule.Interfaces;
using Coravel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using WS.Core.Config;
using System.Diagnostics;
using WS.Core.Tool;
using Microsoft.Extensions.DependencyInjection;

namespace WS.Core;

public static class Context
{
    public static string CurrentDirectory => System.Environment.CurrentDirectory;

    public static ServerConfig CurrentServer { get; private set; }
    public static string ServerName => CurrentServer.Name;

    public static int ServerType => CurrentServer.Type;
    public static int ServerChannel => CurrentServer.Channel;
    public static string? ClientUrl => CurrentServer.ClientUrl;
    public static bool PublicService => CurrentServer.PublicService;
    public static bool IsCenter => CurrentServer.IsCenter;


    //public static bool Https { get; private set; }

    public static string CenterServer {  get; private set; }

    public static void SetCurrentServer(IOptionsSnapshot<RootConfig> rootcfg, ServerConfig config)
    {
        CurrentServer = config;
        RootConfig root = rootcfg.Value;
        CenterServer = rootcfg.Value.CenterServer;


    }



    public enum RunMode
    {
        VS,
        DEBUG,
        RELEASE,
    }
    public static RunMode mode
    {
        get
        {
            if (Debugger.IsAttached)
            {
                return RunMode.VS;
            }
            else
            {
#if DEBUG
                return RunMode.DEBUG;
#else
                return Mode.RELEASE;
#endif
            }
        }
    }


    //public static IOptionsSnapshot<RootConfig> config;
    private static WebApplication webApplication;
    public static IApplication app { get; private set; }

    public static IServiceProvider Services => webApplication.Services;
    public static IWebHostEnvironment Environment => webApplication.Environment;
    public static IHostApplicationLifetime Lifetime => webApplication.Lifetime;

    public static ISchedulerConfiguration UseScheduler(Action<IScheduler> assignScheduledTasks)
    {
        return Services.UseScheduler(assignScheduledTasks);
    }

    internal static void Config(IApplication app, WebApplication web_application)
    {
        Context.app = app;

        Context.webApplication = web_application;
    }

    public static void StopApplication() => Lifetime.StopApplication();




    public static IEnumerable<T?> GetServicesOfType<T>(this IServiceProvider provider) where T : class
    {
        var list= typeof(T).GetSubTypes()
            .Where(x => !x.IsAbstract)
            .Select(x => provider.GetService(x) as T)
            .Where(x => x != null);

        return list;
    }

}
