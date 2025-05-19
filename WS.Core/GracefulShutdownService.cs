using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WS.Core.Tool;
namespace WS.Core;

class GracefulShutdownService : BackgroundService
{
    ILogger logger = LogTools.CreateLogger(typeof(GracefulShutdownService));
    public async override Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        logger.LogInformation("---进入APP------------------------------");
        Context.app.OnEnter();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(1000, stoppingToken);

            }
            catch (Exception)
            {

            }
        }

        logger.LogInformation("---ShutDown------------------------------");
        await Context.app.OnShutDown();
        logger.LogInformation("---ShutDown------------------------------");
    }

}


