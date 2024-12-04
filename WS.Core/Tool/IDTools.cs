using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Snowflake.Core;
using WS.Core.Config;

namespace WS.Core.Tool;

[ToolInitAttribute]
public class IDTools
{

    private static IdWorker worker;

    private static void Init(IServiceProvider service)
    {
        var cfg = service.GetRequiredService<IOptionsSnapshot<RootConfig>>().Value;
        var snow_cfg = cfg.Current.snowflake;
        worker = new IdWorker(snow_cfg.workerId, snow_cfg.dataCenterId, snow_cfg.sequence);
    }

    public static long NewId()
    {
        return worker.NextId();
    }
}
