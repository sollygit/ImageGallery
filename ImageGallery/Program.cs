using CoreImageGallery.Data;
using ImageGallery;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CoreImageGallery
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = BuildWebHost(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    var config = scope.ServiceProvider.GetService<IConfiguration>();
                    
                    // Azure Blob Init
                    await new BlobInitializer().InitAsync(config, dbContext);
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while initializing DB and Blob Containers.");
                }
            }

            host.Run();
        }

        private static IWebHostBuilder BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                // .UseApplicationInsights()
                .UseStartup<Startup>();
    }
}
