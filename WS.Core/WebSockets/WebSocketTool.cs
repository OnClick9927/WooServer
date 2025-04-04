﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WS.Core.Tool;

namespace WS.Core.WebSockets;

public class WebSocketTool
{
    private static Dictionary<Type, Delegate> msg_handlers;
    private static ILogger logger = LogTools.CreateLogger<WebSocketTool>();
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
    internal static void ExecuteMsg(WebSocketToken token, int id, int sid, object msg)
    {
        var msgType = msg.GetType();
        if (msg_handlers.TryGetValue(msgType, out var del))
        {
            logger.LogInformation($"Execute Msg {id}:sid:{sid}msg:{msgType}");
            del.DynamicInvoke(token, id, sid, msg);
        }
        else
        {
            logger.LogCritical($"Not Find Handler id{id}:sid:{sid}msg:{msgType}");
        }
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
                         return ps.Length == 4
                                   && ps[0].ParameterType == typeof(WebSocketToken)
                                   && ps[1].ParameterType == typeof(int)
                                   && ps[2].ParameterType == typeof(int);
                     })
                     .Select(
                         method =>
                         {
                             var del = method.ToDelegate(ins);
                             var type = del.GetType();
                             var arguments = type.GetGenericArguments().Last();
                             return new { arguments, del };
                         }
                     );
               })
               .ToDictionary(x => x.arguments, y => y.del);
            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }
    static Type tokenCollectionType, packerType, queueType_Binary_Type, queueType_Text_Type;
    internal static bool ConfigService(IServiceCollection services)
    {
        tokenCollectionType = typeof(IWebSocketTokenCollection).GetSubTypes().Single();
        packerType = typeof(IWebSocketMsgPacker).GetSubTypes().Single();
        queueType_Binary_Type = typeof(IWebSocketBinaryQueue).GetSubTypes().Single();
        queueType_Text_Type = typeof(IWebSocketTextQueue).GetSubTypes().Single();

        services.AddSingleton(typeof(IWebSocketTokenCollection), tokenCollectionType);
        services.AddTransient(typeof(IWebSocketMsgPacker), packerType);
        services.AddTransient(typeof(IWebSocketBinaryQueue), queueType_Binary_Type);
        services.AddTransient(typeof(IWebSocketTextQueue), queueType_Text_Type);

        //logger.LogInformation($"{nameof(IWebSocketTokenCollection)}---> {tokenCollectionType}");
        logger.LogInformation($"{nameof(IWebSocketMsgPacker)}---> {packerType}");
        logger.LogInformation($"{nameof(IWebSocketBinaryQueue)}---> {queueType_Binary_Type}");
        logger.LogInformation($"{nameof(IWebSocketTextQueue)}---> {queueType_Text_Type}");

        return !(tokenCollectionType == null || packerType == null || queueType_Binary_Type == null || queueType_Text_Type == null);


    }
    internal static void RefreshToken(WebSocketToken token)
    {
        token.LastTime = DateTime.Now;
        GetWebSocketTokenCollection().Refresh(token, DateTime.Now);
    }
    internal static IEnumerable<WebSocketToken> GetTokens()
    {
        return GetWebSocketTokenCollection().GetTokens();
    }
    internal static void RemoveToken(WebSocketToken token)
    {
        GetWebSocketTokenCollection().Remove(token);

    }

    public static void BindToken(WebSocketToken token, long userData)
    {
        GetWebSocketTokenCollection().Bind(token, userData);
    }
    public static WebSocketToken? FindToken(long userData)
    {
        return GetWebSocketTokenCollection().Find(userData);
    }
}
