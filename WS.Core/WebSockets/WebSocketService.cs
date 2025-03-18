using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WS.Core;
using WS.Core.Config;
using WS.Core.Tool;

namespace WS.WebSockets;


public class WebSocketService : IApplicationServiceConfig, IApplicationConfig
{
    private ILogger logger = LogTools.CreateLogger<WebSocketService>();
    private readonly IOptionsSnapshot<RootConfig> root;

    public WebSocketService(IOptionsSnapshot<RootConfig> root)
    {
        this.root = root;
    }

    private async Task Accept(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path == "/ws")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var context_ws = new WebSocketChannel(new WebSocketToken(webSocket, context), root.Value.Current.webSocket.queueSize);
                await context_ws.BeginRec();
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            await next(context);
        }
    }
    void IApplicationConfig.Config(WebApplication application)
    {
        Context.UseScheduler(scheduler =>
        {
            IScheduleInterval interval = scheduler.Schedule(CheckTokenAlive);
            interval.EverySecond();
        });


        application.UseWebSockets(new WebSocketOptions()
        {
            KeepAliveInterval = TimeSpan.FromDays(5)
        });
        application.Use(Accept);
        WebSocketTool.CreateMessageHandlers(application.Services);
    }

    private void CheckTokenAlive()
    {
        var tokens = WebSocketTool.GetTokens();

        foreach (var token in tokens)
        {
            if ((DateTime.Now - token.LastTime).TotalSeconds > root.Value.Current.webSocket.autoDisconnectTime)
            {
                token.Close(string.Empty, System.Net.WebSockets.WebSocketCloseStatus.NormalClosure);
            }
        }
    }

    void IApplicationServiceConfig.ConfigureServices(IServiceCollection services)
    {
        WebSocketTool.ConfigService(services);
    }
}
