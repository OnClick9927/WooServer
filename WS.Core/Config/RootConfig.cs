namespace WS.Core.Config;





public class RootConfig
{
    public string ServerName { get; set; }
    public ServerType ServerType => Current.Type;
    public double ServerLaunchTime { get; set; } = 1;

    public List<ServerConfig> Servers { get; set; } = new List<ServerConfig>();

    public WebSocketConfig WebSocket { get; set; } = new WebSocketConfig();

    public LogConfig Log { get; set; } = new LogConfig();

    public ServerConfig Current { get; private set; }
    public bool SetCurrent(string? name)
    {
        if (string.IsNullOrEmpty(name))
            name = ServerName;
        Current = Servers.FirstOrDefault(s => s.Name == name);
        if (Current == null)
            Current = Servers.FirstOrDefault();
        return Current != null;
    }

    public List<ServerConfig> FindServers(ServerType type)
    {
        return Servers.Where(x => x.Type == type).ToList();
    }

}
