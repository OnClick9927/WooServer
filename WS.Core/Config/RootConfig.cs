namespace WS.Core.Config;

public class RootConfig
{
    public ServerType ServerType { get; set; }

    public List<ServerConfig> cfgs { get; set; }

    private ServerConfig _current;
    public ServerConfig Current
    {
        get
        {
            if (_current == null)
                _current = cfgs.FirstOrDefault(x => x.type == ServerType);
            return _current;
        }
    }
}
