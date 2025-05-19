using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Text;
using WS.Core.Config;
using WS.Core.Tool;

namespace WS.Core.HTTP;

public static partial class HTTPTool
{
    public class HttpPostResult<T>
    {
        public bool Success { get; set; }
        public string? Content { get; set; }
        public HttpStatusCode Code { get; set; }
        public string? ReasonPhrase { get; set; }
        public T? Result { get; set; }

        public static HttpPostResult<T> Succeed { get; private set; } = new HttpPostResult<T>()
        {

            Success = true,
            Code = HttpStatusCode.OK,

        };
        public static HttpPostResult<T> Empty { get; private set; } = new HttpPostResult<T>()
        {

            Success = false,
            Code = HttpStatusCode.NotFound,

        };
    }

    private static RootConfig rootCfg => Context.config.Value;

    private static Dictionary<Type, ServerType> service_type_map = new Dictionary<Type, ServerType>();
    static Dictionary<Type, Dictionary<string, string>> Rpcmap = new Dictionary<Type, Dictionary<string, string>>();

    public static async Task<HttpPostResult<T>> HTTPPost<T>(string url, Dictionary<string, object>? headers = null)
    {
        var result = new HttpPostResult<T>();
        using (HttpClient client = new HttpClient())
        {

            var content = new StringContent("", Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);
            if (headers != null)
                foreach (var head in headers)
                    if (head.Value.GetType() == typeof(string))
                        content.Headers.Add(head.Key, (string)head.Value);
                    else
                        content.Headers.Add(head.Key, JsonConvert.SerializeObject(head.Value));
            try
            {
                var resp = await client.PostAsync(url, content);
                result.Code = resp.StatusCode;
                result.Success = resp.IsSuccessStatusCode;
                result.ReasonPhrase = resp.ReasonPhrase;
                if (resp.IsSuccessStatusCode)
                {
                    result.Content = await resp.Content.ReadAsStringAsync();
                    result.Result = JsonConvert.DeserializeObject<T>(result.Content);
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Code = HttpStatusCode.NotFound;
                result.ReasonPhrase = e.Message;
            }

            return result;

        };

    }

    public static async Task<HttpPostResult<T>> RpcPost<T>(Type type, string method, Dictionary<string, object>? headers = null)
    {
        var fit = FindRpcServers(type);
        if (fit.Count == 0)
            return HttpPostResult<T>.Empty;
        foreach (var server in fit)
        {
            var result = await HTTPPost<T>(GetRpcUrl(server, type, method), headers);
            if (result.Success)
                return result;
        }
        return HttpPostResult<T>.Empty;
    }
    public static async Task<HttpPostResult<T>> RpcPost<T>(string url, Type type, string method, Dictionary<string, object>? headers = null)
    {
        var server = Context.FindServerByUrl(url);
        if (server == null)
            return HttpPostResult<T>.Empty;
        var result = await HTTPPost<T>(GetRpcUrl(server, type, method), headers);
        if (result.Success)
            return result;
        return HttpPostResult<T>.Empty;
    }
    public static List<ServerConfig> FindRpcServers(Type type)
    {
        var serverType = service_type_map[type];
        var fit = Context.FindServers(serverType);
        return fit;
    }

    public static string GetRpcUrl(ServerConfig server, Type type, string method) => $"{server.RpcUrl}/{Rpcmap[type][method]}";
    public static async Task<HttpPostResult<T>> RpcBroadcast<T>(Type type, string method, Dictionary<string, object>? headers = null)
    {
        var fit = FindRpcServers(type);
        if (fit == null || fit.Count == 0)
            return HttpPostResult<T>.Empty;
        foreach (var server in fit)
        {
            var result = await HTTPPost<T>(GetRpcUrl(server, type, method), headers);
            if (!result.Success)
                return result;
        }
        return HttpPostResult<T>.Succeed;

    }

    internal static void CollectRPC(ILogger logger)
    {
        var types = TypeTools.GetTypesWithAttribute(typeof(RpcControllerAttribute), false);

        foreach (var type in types)
        {
            Dictionary<string, string>? routeMap;
            if (!Rpcmap.TryGetValue(type, out routeMap))
            {
                routeMap = new Dictionary<string, string>();
                Rpcmap.Add(type, routeMap);
            }

            var attr = type.GetCustomAttribute<RpcControllerAttribute>();
            var route = type.GetCustomAttribute<RouteAttribute>();

            if (route.Template != "[controller]/[action]")
            {
                logger.LogError($"RPC ERR-->Type:{type} Attribute:{nameof(RouteAttribute)}'s {nameof(RouteAttribute.Template)} must be->[controller]/[action]");
                return;
            }



            service_type_map[type] = attr.serverType;
            var methods = type.GetMethods().Where(method => method.GetCustomAttribute<HttpPostAttribute>() != null);
            foreach (var method in methods)
            {
                string name = method.Name;
                routeMap[name] = $"{type.Name.Replace("Controller", "")}/{name}";
            }
        }
    }

}

partial class HTTPTool
{
    public class Entity
    {
        public string key { get; set; }
        public string serverName { get; set; }

        public long time { get; set; }
        public Action<Entity> call;
    }
    private static List<Entity> contexts = new List<Entity>();
    public static void RegisterTimer(string serverName, string key, long time, Action<Entity> action)
    {
        if (contexts.Any(x => x.serverName == serverName && x.key == key && x.time == time)) return;
        var context = new Entity() { key = key, serverName = serverName, time = time, call = action };

        contexts.Add(context);
    }
    public static void RemoveTimer(string serverName, string key)
    {
        contexts.RemoveAll(x => x.serverName == serverName && x.key == key);
    }
    public static void RemoveTimer(string serverName)
    {
        contexts.RemoveAll(x => x.serverName == serverName);
    }



   public static void Update()
    {
        var now = TimeTool.GetTimeStamp_Now();
        var find = contexts.FindAll(x => x.time <= now);
        if (find != null && find.Count > 0)
        {
            foreach (var context in find)
            {
                context.call?.Invoke(context);
            }
            contexts.RemoveAll(x => find.Contains(x));
        }
    }
}