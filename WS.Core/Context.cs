﻿using Coravel.Scheduling.Schedule.Interfaces;
using Coravel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using WS.Core.Config;
using System.Diagnostics;
using WS.Core.Tool;
using Microsoft.Extensions.DependencyInjection;

namespace WS.Core;

public static class Context
{
    public static string CurrentDirectory => System.Environment.CurrentDirectory;

    public static ServerConfig CurrentServer { get; private set; }
    public static ServerType ServerType => CurrentServer.Type;
    public static int ServerChannel => CurrentServer.Channel;

    public static List<ServerConfig> FindServers(ServerType type)
    {
        RootConfig cfg = config.Value;

        return cfg.Servers.Where(x => x.Type == type && x.Channel == ServerChannel).ToList();
    }
    public static ServerConfig? FindServerByUrl(string url)
    {
        RootConfig cfg = config.Value;
        return cfg.Servers.FirstOrDefault(x => x.RpcUrl == url);
    }
    public static ServerConfig? FindServerByName(string name)
    {
        RootConfig cfg = config.Value;
        return cfg.Servers.FirstOrDefault(x => x.Name == name);
    }
    public static bool SetCurrentServer(string? name)
    {
        RootConfig cfg = config.Value;
        if (string.IsNullOrEmpty(name))
            name = cfg.ServerName;
        CurrentServer = cfg.Servers.FirstOrDefault(s => s.Name == name);
        if (CurrentServer == null)
            CurrentServer = cfg.Servers.FirstOrDefault();
        return CurrentServer != null;
    }



    public enum RunMode
    {
        VS,
        DEBUG,
        RELEASE,
    }
    public static RunMode mode
    {
        get
        {
            if (Debugger.IsAttached)
            {
                return RunMode.VS;
            }
            else
            {
#if DEBUG
                return RunMode.DEBUG;
#else
                return Mode.RELEASE;
#endif
            }
        }
    }


    public static IOptionsSnapshot<RootConfig> config;
    private static WebApplication webApplication;
    public static IApplication app { get;private set; }

    public static IServiceProvider Services => webApplication.Services;
    public static IWebHostEnvironment Environment => webApplication.Environment;
    public static IHostApplicationLifetime Lifetime => webApplication.Lifetime;

    public static ISchedulerConfiguration UseScheduler(Action<IScheduler> assignScheduledTasks)
    {
        return Services.UseScheduler(assignScheduledTasks);
    }

    internal static void Config(IApplication app, WebApplication web_application)
    {
        Context.app = app;

        Context.webApplication = web_application;
    }

    public static void StopApplication() => Lifetime.StopApplication();




    public static IEnumerable<T?> GetServicesOfType<T>(this IServiceProvider provider) where T : class
    {
        return typeof(T).GetSubTypes()
            .Where(x => !x.IsAbstract)
            .Select(x => provider.GetRequiredService(x) as T);
    }

}
