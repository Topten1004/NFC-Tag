using BqsClinoTag.Hubs;
using BqsClinoTag.Models;
using BqsClinoTag.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


namespace BqsClinoTag
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddScoped<NotificationHub>();
            services.AddAutoMapper(typeof(AutoMapperProfiles));
        }
    }
}
