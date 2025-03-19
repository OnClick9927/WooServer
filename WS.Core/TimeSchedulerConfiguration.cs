using Coravel;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WS.Core.Config;
using WS.Core.Tool;

namespace WS.Core;


public interface ITimeEntityContext
{
    void Invoke();
}
public static class TimeTool
{
    private static List<TimeEntity> entities = new List<TimeEntity>();
    public class TimeEntity
    {
        private ITimeEntityContext context;
        internal void Invoke()
        {
            context.Invoke();
        }
        internal bool completed { get; private set; }
        public void InvokeComplete()
        {
            completed = true;
        }
        internal static TimeEntity Create(ITimeEntityContext context)
        {
            var entity = new TimeEntity()
            {
                context = context,
                completed = false,
            };
            return entity;
        }
    }
    public static TimeEntity Add(ITimeEntityContext context)
    {
        var entity = TimeEntity.Create(context);
        entities.Add(entity);
        return entity;
    }

    internal static void Update()
    {
        entities.RemoveAll(x => x.completed);
        for (var i = 0; i < entities.Count; i++)
        {
            var entity = entities[i];
            entity.Invoke();
        }
    }
}

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
    ServerType IApplicationConfiguration.Fit() => ServerType.All;
}

