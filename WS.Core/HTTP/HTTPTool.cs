using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Text;
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


    private static Dictionary<Type, int> service_type_map = new Dictionary<Type, int>();
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

    public static async Task<HttpPostResult<T>> RpcPost<T>(MethodInfo info, Dictionary<string, object>? headers = null)
    {
        if (context == null) return HttpPostResult<T>.Empty;
        Type type = info.DeclaringType;
        //string method = info.Name;
        var serverType = service_type_map[type];
        var fit = context.FindServerNames(serverType);
        if (fit.Count == 0)
            return HttpPostResult<T>.Empty;
        foreach (var server in fit)
        {
            var result = await RpcPost<T>(server, info, headers); ;
            if (result.Success)
                return result;
        }
        return HttpPostResult<T>.Empty;
    }
    public static async Task<HttpPostResult<T>> RpcPost<T>(string serverName, MethodInfo info, Dictionary<string, object>? headers = null)
    {
        Type type = info.DeclaringType;
        string method = info.Name;
        if (context == null || !context.Exist(serverName))
            return HttpPostResult<T>.Empty;
        var result = await HTTPPost<T>(GetRpcUrl(serverName, type, method), headers);
        if (result.Success)
            return result;
        return HttpPostResult<T>.Empty;
    }
    public static async Task<HttpPostResult<T>> RpcPostByUrl<T>(string url, MethodInfo info, Dictionary<string, object>? headers = null)
    {
        Type type = info.DeclaringType;
        string method = info.Name;
        var result = await HTTPPost<T>(GetRpcUrlByBase(url, type, method), headers);
        if (result.Success)
            return result;
        return HttpPostResult<T>.Empty;
    }

    public static string GetRpcUrlByBase(string url, Type type, string method) => $"{url}/{Rpcmap[type][method]}";

    public static string GetRpcUrl(string serverName, Type type, string method) => GetRpcUrlByBase(context.GetHttpRpcUrl(serverName), type, method);

    private static IHttpRPCContext? context;


    internal static void CollectRPC(ILogger logger, IServiceProvider services)
    {

        context = services.GetServicesOfType<IHttpRPCContext>().FirstOrDefault();

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


    //public static string GetClientIP(this HttpContext context)
    //{
    //    // 1. 尝试从X-Forwarded-For获取
    //    if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
    //        return forwardedFor.FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();

    //    var remoteIp = context.Connection.RemoteIpAddress;
    //    if (remoteIp.IsIPv4MappedToIPv6)
    //    {
    //        return remoteIp.MapToIPv4().ToString();
    //    }
    //    return remoteIp?.ToString();
    //}

}
partial class HTTPTool
{

    private static List<TimerRecord> recorders = new List<TimerRecord>();




    public static void ReadRecords()
    {
        var rs = context?.ReadRecords();
        recorders.AddRange(rs);
        context?.OnRecordChange(recorders);
    }
    public static void RegisterTimer(string serverName, string key, long time)
    {
        if (recorders.Any(x => x.serverName == serverName && x.key == key && x.time == time)) return;
        var record = new TimerRecord() { key = key, serverName = serverName, time = time };
        recorders.Add(record);
        context?.OnRecordChange(recorders);
    }

    public static void RemoveTimer(string serverName)
    {
        recorders.RemoveAll(x => x.serverName == serverName);
        context?.OnRecordChange(recorders);

    }


    public static void Update()
    {
        context?.RegisterServer();
        var now = TimeTool.GetTimeStamp_Now();
        var find = recorders.FindAll(x => x.time <= now);
        if (find != null && find.Count > 0)
        {
            foreach (var record in find)
            {
                context?.CallRecord(record);
            }
            recorders.RemoveAll(x => find.Contains(x));
            context?.OnRecordChange(recorders);

        }
    }
}