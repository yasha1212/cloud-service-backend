using CloudService.Configurations;
using CloudService.DAL;
using CloudService.DAL.Extensions;
using CloudService.Entities;
using CloudService.Impl;
using CloudService.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace CloudService.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public PortalConfiguration PortalConfiguration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            PortalConfiguration = configuration.Get<PortalConfiguration>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataAccess(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString(ConfigurationKeys.ConnectionStringName);
            });

            services.AddConfiguration(Configuration);

            services.AddSecurity();

            services.AddImpl();

            services.AddControllers();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddJwtAuthentication(PortalConfiguration);

            services.AddSwagger();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CloudService.Web v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder => builder.WithOrigins(PortalConfiguration.Cors.Origins)
                            .AllowAnyMethod()
                            .WithHeaders("accept", "content-type", "origin", "custom-header", "Authorization"));

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                if (PortalConfiguration.Cors.Origins.Any(o => o.Equals(context.Request.Host.Host)))
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", context.Request.Host.Host);
                }

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
