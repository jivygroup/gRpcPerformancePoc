using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LSProxyService_Grpc_console
{
    class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGrpcServiceStatisticalData, GrpcServiceStatisticalData>();
            services.AddSingleton<IGrpcStatisticsDataProcessor, GrpcStatisticsDataProcessor>();
            
            services.AddGrpc(config =>
            {
                config.EnableDetailedErrors = true;
            });
        }

        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            IGrpcStatisticsDataProcessor processor)
        {
            processor.Start();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ProxyService>();
            });
        }
    }
}
