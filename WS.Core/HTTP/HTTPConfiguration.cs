using Coravel;
using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using WS.Core.Tool;


namespace WS.Core.HTTP;

class HTTPConfiguration : IApplicationConfiguration
{
    private ILogger logger = LogTools.CreateLogger<HTTPConfiguration>();
    void IApplicationConfiguration.Configure(WebApplication application)
    {


        application.Services.UseScheduler(scheduler =>
        {
            scheduler.Schedule(HTTPTool.Update).EverySecond();
        });

        //////////////////////////////////////////////////////////////////////////

        application.UseDefaultFiles();
        application.UseStaticFiles();
        //////////////////////////////////////////////////////////////////////////



        //HTTPTool.InitRoute(application.Services, application, root.Value.ServerType);

        HTTPTool.CollectRPC(logger, application.Services);
        //application.UseMvc();
        //application.UseHttpsRedirection();
        application.UseCors();

        application.UseAuthorization();
        application.MapControllers();


        //if (application.Environment.IsDevelopment())
        {
            application.MapSwagger();
            application.MapOpenApi();
            application.MapScalarApiReference(); // scalar/v1
            application.UseSwagger();
            application.UseKnife4UI();
            //application.UseSwaggerUI();
            //application.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger v1"));
        }

    }

    void IApplicationConfiguration.ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
    options.AddDefaultPolicy(builder => builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                //.AllowCredentials()
                                ));

        services.AddScheduler();

        //services.AddMvc();
        services.AddControllers(options =>
        {
            options.Conventions.Add(new DisableControllerConvention());
        });
        services.AddControllers();
        //services.AddAuthentication();
        services.AddEndpointsApiExplorer();

        services.AddEndpointsApiExplorer();


        //////////////////////////////////////////////////////////////////////////

        services.AddOpenApi();
        //services.AddSwaggerGen();

        services.AddSwaggerGen(c =>
        {
            // ... 其他配置

            // 自定义 OperationId 的生成逻辑，直接使用的方法名
            c.CustomOperationIds(apiDesc =>
            {
                if (apiDesc.TryGetMethodInfo(out var methodInfo))
                {
                    return methodInfo.Name;
                }
                return apiDesc.RelativePath;
                // 返回方法信息的名称
                //return apiDesc.ActionDescriptor.RouteValues["action"]; // 或者 apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null;
            });
        });

    }
}

