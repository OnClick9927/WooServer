using Coravel.Scheduling.Schedule.Interfaces;
using Coravel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using WS.Core.Config;
using System.Diagnostics;

namespace WS.Core;

public class Context
{
    public static string CurrentDirectory => System.Environment.CurrentDirectory;





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


    public static IOptionsSnapshot<RootConfig> config;
    private static WebApplication webApplication;

    public static IServiceProvider Services => webApplication.Services;
    public static IWebHostEnvironment Environment => webApplication.Environment;
    public static IHostApplicationLifetime Lifetime => webApplication.Lifetime;

    public static ISchedulerConfiguration UseScheduler(Action<IScheduler> assignScheduledTasks)
    {
        return Services.UseScheduler(assignScheduledTasks);
    }

    internal static void Config(WebApplication web_application) => Context.webApplication = web_application;
    public static void StopApplication() => Lifetime.StopApplication();

}
