using Grpc.AspNetCore.Server;
using LsProxyServerGrpcNewLib.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LsProxyServerGrpcNewLib
{
    internal class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGrpcServiceStatisticalData, GrpcServiceStatisticalData>();
            services.AddSingleton<IGrpcStatisticsDataProcessor, GrpcStatisticsDataProcessor>();

            services.AddGrpc(ConfigureOptions);
        }

        private void ConfigureOptions(GrpcServiceOptions obj)
        {
            obj.EnableDetailedErrors = true;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IGrpcStatisticsDataProcessor processor)
        {
            processor.Start();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ProxyService>();
            });
        }
    }
}
