using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ModularApplication
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webEnv;

        public Startup(IWebHostEnvironment webEnv)
        {
            _webEnv = webEnv;
        }

        // Module1 needs this AddMvc call and the OrchardCore.Application.Mvc.Targets PackageReference.
        // Module2 only needs the OrchardCore.Application.Targets PackageReference and of course AddOrchardCore and UseOrchardCore.
        public void ConfigureServices(IServiceCollection services)
        {
            ////OK => Default
            //services
            //    .AddOrchardCore()
            //    .AddMvc();

            //OK => access IMvcBuilder
            //AddRazorRuntimeCompilation for both dev and publish 
            services
                .AddOrchardCore()
                .AddMvc()
                .ConfigureServices(cfgServices =>
                {
                    cfgServices.AddMvcCore().AddRazorRuntimeCompilation();
                });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseOrchardCore();
        }
    }
}
