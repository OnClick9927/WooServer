using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Snowflake.Core;
using WS.Core.Config;

namespace WS.Core.Tool;

public class IDTools
{
    private static Lazy<IdWorker> worker = new Lazy<IdWorker>(() =>
    {
        var snow_cfg = Context.config.Value.Current.snowflake;
        return new IdWorker(snow_cfg.workerId, snow_cfg.dataCenterId, snow_cfg.sequence);
    }, true);


    public static long NewId()
    {
        return worker.Value.NextId();
    }
}
