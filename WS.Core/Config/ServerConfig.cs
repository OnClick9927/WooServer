using Microsoft.Extensions.Logging;

namespace WS.Core.Config
{
    public class ServerConfig
    {
        public string Name { get; set; }
        public ServerType Type { get; set; }
        public string LaunchUrl { get; set; }
        public string RpcUrl { get; set; }


        public SnowflakeConfig Snowflake { get; set; } = new SnowflakeConfig();

        public override string ToString()
        {
            return $"{nameof(Name)}:{Name} {nameof(Type)}:{Type}\n{nameof(LaunchUrl)}:{LaunchUrl}\n{nameof(RpcUrl)}:{RpcUrl}";
        }

    }
}
