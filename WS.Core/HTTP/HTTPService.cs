using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using WS.Core;
using WS.Core.Config;
using WS.Core.Tool;


namespace WS.HTTP;

class HTTPService : IApplicationConfig, IApplicationServiceConfig
{
    private ILogger logger = LogTools.CreateLogger<HTTPService>();
    private readonly IOptionsSnapshot<RootConfig> root;

    public HTTPService(IOptionsSnapshot<RootConfig> root)
    {
        this.root = root;
    }

    void IApplicationConfig.Config(WebApplication application)
    {
        //////////////////////////////////////////////////////////////////////////

        application.UseDefaultFiles();
        application.UseStaticFiles();
        //////////////////////////////////////////////////////////////////////////



        //HTTPTool.InitRoute(application.Services, application, root.Value.ServerType);

        HTTPTool.CollectRPC();
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

    public void ConfigureServices(IServiceCollection services)
    {
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
