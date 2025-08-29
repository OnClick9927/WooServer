namespace WS.Core.Config
{
    public class ServerConfig
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public int Channel { get; set; }
        public string? LaunchUrl { get; set; }
        public string? ClientUrl { get; set; }
        public bool PublicService { get; set; }
        public bool IsCenter { get; set; }

        public SnowflakeConfig Snowflake { get; set; } = new SnowflakeConfig();
        public WebSocketConfig WebSocket { get; set; } 

        public override string ToString()
        {
            return $"\n{nameof(Name)}:{Name}" +
                $"\t{nameof(Channel)}:{Channel}" +
                $"\t{nameof(Type)}:{Type}" +
                $"\n{nameof(LaunchUrl)}:{LaunchUrl}" +
                $"\n{nameof(PublicService)}:{PublicService}" +
                $"\n{nameof(IsCenter)}:{IsCenter}";
        }

    }
}
