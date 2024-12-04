using Coravel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WS.Core.Tool;

namespace WS.Core.Service;

class TimeSchedulerService : IApplicationServiceConfig
{
    private ILogger logger = LogTools.CreateLogger<TimeSchedulerService>();

    public void ConfigureServices(IServiceCollection services)
    {
        throw new NotImplementedException();
    }

    void IApplicationServiceConfig.ConfigureServices(IServiceCollection services)
    {
        services.AddScheduler();
    }
}
