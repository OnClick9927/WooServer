using Coravel;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WS.Core.Tool;

namespace WS.Core;



class TimeSchedulerConfiguration : IApplicationConfiguration
{
    private ILogger logger = LogTools.CreateLogger<TimeSchedulerConfiguration>();
    public void Configure(WebApplication application)
    {
        application.Services.UseScheduler(scheduler =>
        {
            scheduler.Schedule(TimeTool.Update).EverySecond();
        });
    }

    void IApplicationConfiguration.ConfigureServices(IServiceCollection services)
    {
        services.AddScheduler();
    }
}

