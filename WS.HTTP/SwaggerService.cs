using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using WS.Core;
using WS.Core.Tool;

namespace WS.HTTP;

class SwaggerService : IApplicationServiceConfig, IApplicationConfig
{
    private ILogger logger = LogTools.CreateLogger<SwaggerService>();

    void IApplicationConfig.Config(WebApplication application)
    {
        application.MapSwagger().RequireAuthorization();
        if (application.Environment.IsDevelopment())
        {
            application.UseSwagger();
            //app.UseSwaggerUI();
            application.UseKnife4UI();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger v1"));
        }
    }

    private IList<string> DefaultTagsSelector(ApiDescription apiDescription)
    {

        string _default = apiDescription.ActionDescriptor.RouteValues["controller"];
        var list = new List<string>() { _default };
        HttpServiceAttribute httpService = TypeTools.GetTypesWithAttribute(typeof(HttpServiceAttribute), false)
            .Single(x => x.Name == _default)
            .GetCustomAttribute<HttpServiceAttribute>();
        string route = string.IsNullOrEmpty(httpService.route) ? _default : httpService.route;
        list.Add(route);
        HttpMethodAttribute method_attr = apiDescription.ActionDescriptor.EndpointMetadata.LastOrDefault(x => x is HttpMethodAttribute) as HttpMethodAttribute;
        if (method_attr.rpc)
        {
            list.Add("RPC");
            list.Remove(route);
        }
        if (list.Count > 1)
            list.Remove(_default);
        return list;
    }
    private string DefaultOperationIdSelector(ApiDescription apiDescription)
    {
        ActionDescriptor actionDescriptor = apiDescription.ActionDescriptor;
        var last = actionDescriptor.EndpointMetadata.Last();

        var p = last.GetType().GetProperty("Route");
        string str = (string)p.GetValue(last);
        return str.Replace("/", "_");
    }
    void IApplicationServiceConfig.ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerGeneratorOptions.TagsSelector = DefaultTagsSelector;
            options.SwaggerGeneratorOptions.OperationIdSelector = DefaultOperationIdSelector;
        });



    }
}
