using HikingTrailsApi.Application;
using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Services;
using HikingTrailsApi.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.IO;

namespace HikingTrailsApi.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();
            services.AddInfrastructure(Configuration);

            services.ConfigureApplicationIdentity(Configuration);

            services.AddSingleton(Configuration);

            services.AddMvc();

            services.AddScoped<IImageStorageService, ImageStorageService>();

            services.ConfigureSwagger();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //CULTURE SETUP
            var cultureInfo = new CultureInfo("en-GB");
            var lithuanianCultureInfo = new CultureInfo("lt-LT");

            cultureInfo.DateTimeFormat = lithuanianCultureInfo.DateTimeFormat;

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hiking Trails v1"));
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            //Create Image directory if it doesn't already exist before any requests are made
            var staticFileDirectory = Path.Combine(env.ContentRootPath, "Images");
            Directory.CreateDirectory(staticFileDirectory);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(staticFileDirectory),
                RequestPath = "/images"
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
