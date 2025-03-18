using Microsoft.Extensions.Logging;

namespace WS.Core.Config;

public class LogConfig
{
    public LogLevel LogLevel { get; set; }
    public string TimeStampFormat { get; set; } = "K yyyy-MM-dd HH:mm:ss:fffffff ";
}
