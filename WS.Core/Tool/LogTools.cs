using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WS.Core.Config;

namespace WS.Core.Tool;


[ToolInitAttribute]
public static class LogTools
{
    private static ILoggerFactory factory;
    internal static void InitLog(LogLevel lev, string timeStampFormat)
    {
        factory = LoggerFactory.Create(x => x.AddConsole().AddSimpleConsole(options =>
        {
            options.IncludeScopes = false;
            options.SingleLine = true;
            options.TimestampFormat = timeStampFormat;
        })
           .SetMinimumLevel(lev).AddFile(e =>
           {
               e.RootPath = AppContext.BaseDirectory;
               e.Files = new[] { new LogFileOptions { Path = "logs/<date:yyyy>_<date:MM>_<date:dd>-<counter>.txt" } };
               e.MaxFileSize = 10 * 1024 * 1024;
               e.FileAccessMode = LogFileAccessMode.KeepOpenAndAutoFlush;
           }));
    }
    public static ILogger CreateLogger<T>() => factory.CreateLogger<T>();
    public static ILogger CreateLogger(Type type) => factory.CreateLogger(type);
    private static void Init(IServiceProvider service)
    {
        var cfg = service.GetRequiredService<IOptionsSnapshot<RootConfig>>().Value;

        LogTools.InitLog(cfg.Current.logLevel, cfg.Current.timeStampFormat);
    }
    public static bool AlertLog(this ILogger logger, bool condition, string? message, LogLevel level, params object?[] args)
    {
        if (condition)
            logger.Log(level, message, args);
        return condition;
    }
}

