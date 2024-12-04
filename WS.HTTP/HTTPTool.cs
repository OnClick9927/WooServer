using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Snowflake.Core;
using System.Net;
using System.Reflection;
using System.Text;
using WS.Core.Config;
using WS.Core.Tool;

namespace WS.HTTP;

[Core.Tool.ToolInit]
public static class HTTPTool
{
    private static string RemapRoute(string route)
    {
        if (route_map.TryGetValue(route, out var result))
        {
            return result;
        }
        return string.Empty;
    }
    private static ServerType Service2ServerType(Type type) => service_type_map[type];
    private static Dictionary<string, string> route_map = new Dictionary<string, string>();
    private static Dictionary<Type, ServerType> service_type_map = new Dictionary<Type, ServerType>();


    internal static void InitRoute(IServiceProvider services, IEndpointRouteBuilder route, ServerType serverType)
    {
        TypeTools.GetTypesWithAttribute(typeof(HttpServiceAttribute), false)
              .Select(x => services.GetService(x))
              .ToList()
              .ForEach(http =>
              {
                  var http_type = http.GetType();
                  var _attr = http_type.GetCustomAttribute<HttpServiceAttribute>(false);
                  service_type_map.Add(http_type, _attr.rpcServerType);


                  var base_route = string.IsNullOrEmpty(_attr.route) ? http_type.Name : _attr.route;


                  http_type.GetMethods()
                      .Where(x => x.IsDefined(typeof(HttpMethodAttribute), false))
                      .ToList()
                      .ForEach(x =>
                      {
                          var attr = x.GetCustomAttribute<HttpMethodAttribute>(false);
                          var _name = string.IsNullOrEmpty(attr.name) ? x.Name : attr.name;
                          _name = $"{base_route}/{_name}";
                          route_map.Add($"{http_type.Name}/{x.Name}", _name);
                          if (_attr.rpcServerType != serverType) return;
                          var del = x.ToDelegate(http);
                          
                          switch (attr.type)
                          {
                              case HttpMethodAttribute.MethodType.GET:
                                  route.MapGet(_name, del);
                                  break;
                              case HttpMethodAttribute.MethodType.Post:
                                  var handler = route.MapPost(_name, del);
                                  break;
                              case HttpMethodAttribute.MethodType.Put:
                                  route.MapPut(_name, del);
                                  break;
                              case HttpMethodAttribute.MethodType.Delete:
                                  route.MapDelete(_name, del);
                                  break;
                              default:
                                  break;
                          }
                      });
              });
    }

    public class HttpPostResult<T>
    {
        public bool Success { get; set; }
        public string Content { get; set; }
        public HttpStatusCode Code { get; set; }
        public string? ReasonPhrase { get; set; }
        public T Result { get; set; }
    }

    private static RootConfig rootCfg { get; set; }

    private static void Init(IServiceProvider service)
    {
        var cfg = service.GetRequiredService<IOptionsSnapshot<RootConfig>>().Value;
        rootCfg = cfg;
    }


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

        var serverType = Service2ServerType(type);
        ServerConfig? find = rootCfg.cfgs.FirstOrDefault(x => x.type == serverType);
        return await HTTPPost<T>($"{find.url}/{RemapRoute($"{type.Name}/{method}")}", headers);
    }
}
