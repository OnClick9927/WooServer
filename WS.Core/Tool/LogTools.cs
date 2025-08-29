using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.Logging;
using WS.Core.Config;

namespace WS.Core.Tool;


public static class LogTools
{
    private static ILoggerFactory factory;
    internal static void InitLog(LogConfig log)
    {
        factory = LoggerFactory.Create(x => x.AddConsole().AddSimpleConsole(options =>
        {
            options.IncludeScopes = false;
            options.SingleLine = true;
            options.TimestampFormat = log.TimeStampFormat;
        })
           .SetMinimumLevel(log.LogLevel).AddFile(e =>
           {
               e.RootPath = AppContext.BaseDirectory;
               e.Files = new[] { new LogFileOptions { Path = "logs/<date:yyyy>_<date:MM>_<date:dd>-<counter>.txt" } };
               e.MaxFileSize = 10 * 1024 * 1024;
               e.FileAccessMode = LogFileAccessMode.KeepOpenAndAutoFlush;
           }));
    }
    public static ILogger CreateLogger<T>() => CreateLogger(typeof(T));
    public static ILogger CreateLogger(Type type) => factory.CreateLogger(type.Name);

    public static bool AlertLog(this ILogger logger, bool condition, string? message, LogLevel level, params object?[] args)
    {
        if (condition)
            logger.Log(level, message, args);
        return condition;
    }
}

