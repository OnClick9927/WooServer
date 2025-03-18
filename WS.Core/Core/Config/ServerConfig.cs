using Microsoft.Extensions.Logging;

namespace WS.Core.Config
{
    public class ServerConfig
    {
        public ServerType type { get; set; }
        public string url { get; set; }
        public int webSocketQueueSize { get; set; } = 4 * 1024;
        public LogLevel logLevel { get; set; }
        public string timeStampFormat { get; set; } = "K yyyy-MM-dd HH:mm:ss:fffffff ";
        public SnowflakeConfig snowflake { get; set; } = new SnowflakeConfig();
    }
}
