﻿using Microsoft.Extensions.Logging;

namespace WS.Core.Config
{
    public class ServerConfig
    {
        public string Name {  get; set; }
        public ServerType Type { get; set; }
        public string Url { get; set; }

        public SnowflakeConfig Snowflake { get; set; } = new SnowflakeConfig();

    }
}
