using Coravel.Scheduling.Schedule.Interfaces;
using Coravel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;

namespace WS.Core;

public class Context
{
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
