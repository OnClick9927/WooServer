namespace WS.Core.Config;





public class RootConfig
{
    public string ServerName { get; set; }
    public double ServerLaunchTime { get; set; } = 1;

    public List<ServerConfig> Servers { get; set; } = new List<ServerConfig>();

    public WebSocketConfig WebSocket { get; set; } = new WebSocketConfig();

    public LogConfig Log { get; set; } = new LogConfig();




}
