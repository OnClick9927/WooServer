using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Text;
using WS.Core;
using WS.Core.Config;
using WS.Core.Tool;

namespace WS.HTTP;

public static class HTTPTool
{
    public class HttpPostResult<T>
    {
        public bool Success { get; set; }
        public string Content { get; set; }
        public HttpStatusCode Code { get; set; }
        public string? ReasonPhrase { get; set; }
        public T Result { get; set; }
    }

    private static RootConfig rootCfg => Context.config.Value;

    private static Dictionary<Type, ServerType> service_type_map = new Dictionary<Type, ServerType>();
    static Dictionary<Type, Dictionary<string, string>> Rpcmap = new Dictionary<Type, Dictionary<string, string>>();

    public static async Task<HttpPostResult<T>> HTTPPost<T>(string url, Dictionary<string, object> headers = null)
    {
        var result = new HttpPostResult<T>();
        using (HttpClient client = new HttpClient())
        {

            var content = new StringContent("", Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);
            if (headers != null)
                foreach (var head in headers)
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

    public static async Task<HttpPostResult<T>> RpcHTTPPost<T>(Type type, string method, Dictionary<string, object> headers = null)
    {
        ServerConfig? find = rootCfg.cfgs.FirstOrDefault(x => x.type == service_type_map[type]);
        return await HTTPPost<T>($"{find.url}/{Rpcmap[type][method]}", headers);
    }

    public static void CollectRPC()
    {
        var types = TypeTools.GetTypesWithAttribute(typeof(RpcControllerAttribute), false);

        foreach (var type in types)
        {
            Dictionary<string, string> routeMap;
            if (!Rpcmap.TryGetValue(type, out routeMap))
            {
                routeMap = new Dictionary<string, string>();
                Rpcmap.Add(type, routeMap);
            }

            var attr = type.GetCustomAttribute<RpcControllerAttribute>();
            var route = type.GetCustomAttribute<RouteAttribute>();
            service_type_map[type] = attr.serverType;
            var methods = type.GetMethods().Where(method => method.GetCustomAttribute<HttpPostAttribute>() != null);


            foreach (var method in methods)
            {
                string name = method.Name;
                //string target = $"{type.FullName}/{name}";
                //target = route.Template.Replace("[controller]", name);

                routeMap[name] = route.Template.Replace("[controller]", name); ;
                //Console.WriteLine(target);
            }
        }
    }

}
