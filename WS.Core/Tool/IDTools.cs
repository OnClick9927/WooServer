using Snowflake.Core;
using WS.Core.Config;

namespace WS.Core.Tool;

public class IDTools
{
    private static IdWorker worker;

    internal static void Init(SnowflakeConfig snow_cfg)
    {
        worker = new IdWorker(snow_cfg.WorkerId, snow_cfg.DataCenterId, snow_cfg.Sequence);
    }
    public static long NewId()
    {
        return worker.NextId();
    }
}
