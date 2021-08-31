using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Module1.Helpers;
using OrchardCore.Modules;

namespace Module1
{
    public class Startup : StartupBase
    {
        private readonly ILogger<Startup> _logger;

        public Startup(ILogger<Startup> logger)
        {
            _logger = logger;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation(">>>> Module1 ConfigureServices");
            base.ConfigureServices(services);
            services.AddSingleton<SingleViewCodeUpdateHelper>();
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            _logger.LogInformation(">>>> Module1 Configure");
            base.Configure(app, routes, serviceProvider);
            routes.MapAreaControllerRoute
            (
                name: "Module1_Default",
                areaName: "Module1",
                pattern: "Module1/{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}
