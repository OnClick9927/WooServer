using Coravel.Scheduling.Schedule.Interfaces;
using Coravel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using WS.Core.Config;

namespace WS.Core;

public class Context
{
    public static IOptionsSnapshot<RootConfig> config;

    public static IServiceProvider Services { get; private set; }
    public static IWebHostEnvironment Environment { get; private set; }
    public static IHostApplicationLifetime Lifetime { get; private set; }

    public static ISchedulerConfiguration UseScheduler(Action<IScheduler> assignScheduledTasks)
    {
        return Services.UseScheduler(assignScheduledTasks);
    }

    internal static void Config(WebApplication web_application)
    {
        Services = web_application.Services;
        Environment = web_application.Environment;
        Lifetime = web_application.Lifetime;
    }
    public static void StopApplication()
    {
        Lifetime.StopApplication();
    }

}
