namespace WS.Core.Config;





public class RootConfig
{
    public string ServerName { get; set; }

    public List<ServerConfig> Servers { get; set; } = new List<ServerConfig>();

    public WebSocketConfig WebSocket { get; set; } = new WebSocketConfig();

    public LogConfig Log { get; set; } = new LogConfig();




}
