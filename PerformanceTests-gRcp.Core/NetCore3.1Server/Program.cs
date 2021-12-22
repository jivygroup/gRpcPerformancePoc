using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;

namespace LSProxyService_Grpc_console
{
    class Program
    {
        const int _port = 23456;

        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            Console.WriteLine("started - press any key to quit...");
            Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ConfigureEndpointDefaults(o =>
                        {
                            o.Protocols = HttpProtocols.Http2;
                            
                        });
                        options.ListenAnyIP(_port);
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
