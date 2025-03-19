namespace WS.Core.Config
{
    public class SnowflakeConfig
    {

        public long WorkerId { get; set; } = 1;
        public long DataCenterId { get; set; } = 1;
        public long Sequence { get; set; } = 0L;
    }
}
