using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices.JavaScript;
using WS.Core.Tool;

namespace WS.Core.WebSockets;

public class WebSocketTool
{
    private static Dictionary<Type, Delegate> msg_handlers;
    private static ILogger logger = LogTools.CreateLogger<WebSocketTool>();
    static IWebSocketMessageErrHandler messageErrHandler;
    static IWebSocketDisConnectHandler disConnectHandler;
    static IWebSocketMessageLogLevel msgLev;

    internal static IWebSocketMsgPacker CreateMsagPacker()
    {
        return Context.Services.GetRequiredService<IWebSocketMsgPacker>();
    }
    internal static IWebSocketBinaryQueue? CreateNewBinaryQueue(int size)
    {
        var queue = Context.Services.GetRequiredService<IWebSocketBinaryQueue>();
        queue?.Init(size);
        return queue;
    }
    internal static IWebSocketTextQueue? CreateNewTextQueue(int size)
    {
        var queue = Context.Services.GetRequiredService<IWebSocketTextQueue>();
        queue?.Init(size);
        return queue;
    }
    static IWebSocketTokenCollection GetWebSocketTokenCollection() => Context.Services.GetRequiredService<IWebSocketTokenCollection>();

    internal static void LogMsg(bool rec, int id, int sid, object msg)
    {
        var lev = msgLev.GetMessageLogLevel(id, sid, msg);
        if (rec)
            logger.Log(lev, $"Rec  {id}-{sid}-{msg.GetType()}\t{msg}");
        else
            logger.Log(lev, $"Send {id}-{sid}-{msg.GetType()}\t{msg}");

    }

    internal static async Task ExecuteMsg(WebSocketToken token, int id, int sid, object msg)
    {
        if (token.socket.State == System.Net.WebSockets.WebSocketState.Closed)
        {
        }
        else
        {

            var msgType = msg.GetType();
            if (msg_handlers.TryGetValue(msgType, out var del))
            {
                LogMsg(true, id, sid,msg);
                var _task = Task.Run(async () =>
                {

                    Task task = del.DynamicInvoke(token, id, sid, msg) as Task;
                    await task;
                    var _type = task.GetType();
                    var p = _type.GetProperty(nameof(Task<object>.Result));
                    if (p.PropertyType.FullName != "System.Threading.Tasks.VoidTaskResult")
                    {
                        var obj = p.GetValue(task);
                        await token.Send(id, sid, obj);
                    }

                });
                while (!_task.IsCompleted)
                {
                    await Task.Delay(1);
                    if (_task.Exception != null)
                    {
                        break;
                    }
                }
                Exception e = _task.Exception;
                //await _task;
                if (_task.Exception != null)
                {
                    if (!messageErrHandler.Handle(token, id, sid, msg, _task.Exception))
                    {
                        throw _task.Exception;
                    }
                }
            }
            else
            {
                logger.LogCritical($"Not Find Handler {id}-{sid}\n{msgType}\t{msg}");
            }
        }
        //return Task.CompletedTask;
    }
    internal static bool CreateMessageHandlers(IServiceProvider services)
    {
        try
        {
            msg_handlers = TypeTools.GetTypesWithAttribute(typeof(WebSocketHandlerAttribute), false)
                .Select(x => services.GetService(x))
               .SelectMany(ins =>
               {
                   return ins.GetType().GetMethods()
                     .Where(x =>
                     {
                         var ps = x.GetParameters();
                         return ((x.ReturnType.IsGenericType && x.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)) ||
                         x.ReturnType == typeof(Task)) && ps.Length == 4
                                   && ps[0].ParameterType == typeof(WebSocketToken)
                                   && ps[1].ParameterType == typeof(int)
                                   && ps[2].ParameterType == typeof(int);
                     })
                     .Select(
                         method =>
                         {
                             var del = method.ToDelegate(ins);
                             var type = del.GetType();
                             var arguments = type.GetGenericArguments();
                             var msgType = arguments[arguments.Length - 2];
                             return new { msgType, del };
                         }
                     );
               })
               .ToDictionary(x => x.msgType, y => y.del);
            messageErrHandler = services.GetService(err_handler) as IWebSocketMessageErrHandler;
            msgLev = services.GetService(msgLevHandler) as IWebSocketMessageLogLevel;
            disConnectHandler = services.GetService(disconHandler) as IWebSocketDisConnectHandler;

            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }
    static Type tokenCollectionType, packerType, queueType_Binary_Type, queueType_Text_Type, err_handler, msgLevHandler, disconHandler;
    internal static bool ConfigService(IServiceCollection services)
    {
        tokenCollectionType = typeof(IWebSocketTokenCollection).GetSubTypes().Single();
        packerType = typeof(IWebSocketMsgPacker).GetSubTypes().Single();
        queueType_Binary_Type = typeof(IWebSocketBinaryQueue).GetSubTypes().Single();
        queueType_Text_Type = typeof(IWebSocketTextQueue).GetSubTypes().Single();
        err_handler = typeof(IWebSocketMessageErrHandler).GetSubTypes().Single();
        msgLevHandler = typeof(IWebSocketMessageLogLevel).GetSubTypes().Single();
        disconHandler = typeof(IWebSocketDisConnectHandler).GetSubTypes().Single();

        services.AddSingleton(typeof(IWebSocketTokenCollection), tokenCollectionType);
        services.AddTransient(typeof(IWebSocketMsgPacker), packerType);
        services.AddTransient(typeof(IWebSocketBinaryQueue), queueType_Binary_Type);
        services.AddTransient(typeof(IWebSocketTextQueue), queueType_Text_Type);

        //logger.LogInformation($"{nameof(IWebSocketTokenCollection)}---> {tokenCollectionType}");
        logger.LogInformation($"{nameof(IWebSocketMsgPacker)}---> {packerType}");
        logger.LogInformation($"{nameof(IWebSocketBinaryQueue)}---> {queueType_Binary_Type}");
        logger.LogInformation($"{nameof(IWebSocketTextQueue)}---> {queueType_Text_Type}");
        logger.LogInformation($"{nameof(IWebSocketMessageErrHandler)}---> {err_handler}");

        return !(err_handler == null ||
            msgLevHandler == null ||
            tokenCollectionType == null ||
            packerType == null ||
            queueType_Binary_Type == null ||
            queueType_Text_Type == null || disconHandler == null);


    }
    static string ToString(ConnectionInfo info)
    {
        return $"{info.RemoteIpAddress}:{info.RemotePort}";
    }
    internal static void RefreshToken(WebSocketToken token, bool _new)
    {
        if (_new)
        {
            logger.LogInformation($"Token Connect {ToString(token.context.Connection)}");

        }
        token.LastTime = DateTime.Now;
        GetWebSocketTokenCollection().Refresh(token, DateTime.Now);
    }
    public static IEnumerable<WebSocketToken> GetTokens()
    {
        return GetWebSocketTokenCollection().GetTokens();
    }
    internal static void RemoveToken(WebSocketToken token)
    {
        logger.LogInformation($"Token Close {ToString(token.context.Connection)} {token.userData}");
        disConnectHandler?.OnDisConnect(token);
        GetWebSocketTokenCollection().Remove(token);

    }

    public static void BindToken(WebSocketToken token, string userData)
    {
        logger.LogInformation($"Token Bind {ToString(token.context.Connection)} {userData}");

        GetWebSocketTokenCollection().Bind(token, userData);
    }
    public static WebSocketToken? FindToken(string userData)
    {
        return GetWebSocketTokenCollection().Find(userData);
    }
}
