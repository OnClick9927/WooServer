namespace WS.Core.Config
{
    public class SnowflakeConfig
    {

        public long workerId { get; set; } = 1;
        public long dataCenterId { get; set; } = 1;
        public long sequence { get; set; } = 0L;
    }
}
