using Microsoft.Extensions.Configuration;

namespace WS.Core.Tool;
public static class ConfigTools
{
    public static IConfiguration LoadConfig(string configName)
    {
        return new ConfigurationBuilder()
             .SetBasePath(Path.Combine(Environment.CurrentDirectory, "Configs"))
             .AddJsonFile(configName).Build();
    }
    public static T LoadConfig<T>(T t, string configName)
    {
        IConfiguration configuration = LoadConfig(configName);
        configuration.Bind(t);
        return t;
    }

    private static Dictionary<Type, object> cfgs = new Dictionary<Type, object>();
    public static void SetConfig<T>(T t)
    {
        cfgs[typeof(T)] = t;
    }
    public static T GetConfig<T>()
    {
        if (cfgs.TryGetValue(typeof(T), out var cfg))
            return (T)cfg;
        return default;
    }
}
