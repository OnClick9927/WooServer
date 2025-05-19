using Coravel;
using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scalar.AspNetCore;
using WS.Core.Tool;
using static System.Net.Mime.MediaTypeNames;


namespace WS.Core.HTTP;

class HTTPConfiguration : IApplicationConfiguration
{
    ServerType IApplicationConfiguration.Fit() => ServerType.All;
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

        HTTPTool.CollectRPC(logger);
        //application.UseMvc();
        application.UseHttpsRedirection();
        application.UseAuthorization();
        application.MapControllers();


        if (application.Environment.IsDevelopment())
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
        services.AddScheduler();

        //services.AddMvc();
        services.AddControllers();
        //services.AddAuthentication();
        services.AddEndpointsApiExplorer();

        services.AddEndpointsApiExplorer();


        //////////////////////////////////////////////////////////////////////////

        services.AddOpenApi();
        services.AddSwaggerGen();
        services.AddSwaggerGen();

    }
}
