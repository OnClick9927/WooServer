using Coravel;
using Coravel.Invocable;
using Coravel.Scheduling.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WS.Core.Tool;

namespace WS.Core;

class TimeSchedulerService : IApplicationServiceConfig
{
    private ILogger logger = LogTools.CreateLogger<TimeSchedulerService>();

    void IApplicationServiceConfig.ConfigureServices(IServiceCollection services)
    {
        services.AddScheduler();
    }
}

