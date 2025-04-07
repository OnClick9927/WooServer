using Microsoft.Extensions.Configuration;
namespace WS.Core.Tool;
public static class ConfigTools
{



    public static IConfiguration LoadConfig(string configName)
    {
        return new ConfigurationBuilder()
             .SetBasePath(Context.CurrentDirectory)
             .AddJsonFile(configName).Build();
    }
    public static T LoadConfig<T>(T t, string configName)
    {
        IConfiguration configuration = LoadConfig(configName);
        configuration.Bind(t);
        return t;
    }

}
