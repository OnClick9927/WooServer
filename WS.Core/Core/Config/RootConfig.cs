namespace WS.Core.Config;





public class RootConfig
{
    public int ServerIndex { get; set; }

    public double ServerLaunchTime { get; set; } = 1;

    public List<ServerConfig> Servers { get; set; } = new List<ServerConfig>();

    public WebSocketConfig WebSocket { get; set; } = new WebSocketConfig();

    public LogConfig Log { get; set; } = new LogConfig();

    public ServerConfig Current => Servers[ServerIndex];


    public List<ServerConfig> FindServers(ServerType type)
    {
        return Servers.Where(x => x.Type == type).ToList();
    }
 
}
