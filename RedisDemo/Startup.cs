using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using RedisDemo.Data;
using RedisDemo.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddHealthChecksUI().AddInMemoryStorage();

            services.AddServerSideBlazor();
            //configure health checks
            services.AddHealthChecks()
                .AddCheck<ResponseTimeHealthCheck>("Network speed test",null,new[] { "service" })
            //    .AddCheck("foo service", () =>
            //{
            //    return HealthCheckResult.Degraded("서비스 작동안함");
            //},new[] {"searvice"}
            //)
            //.AddCheck("bar service", () => HealthCheckResult.Healthy("서비스 작동"))
            .AddCheck("Database", () => HealthCheckResult.Healthy("서비스 작동"),new[] {"database", "sql" });

            services.AddSingleton<WeatherForecastService>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                options.InstanceName = "RedisDemo_";
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("quickhealth", new HealthCheckOptions()
                {
                    Predicate = _ => false
                });
                endpoints.MapHealthChecks("/health/services", new HealthCheckOptions()
                { 
                  Predicate=reg =>reg.Tags.Contains("service"),
                  ResponseWriter =UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/health",new HealthCheckOptions() {
                ResponseWriter =UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapBlazorHub();
                endpoints.MapHealthChecksUI();
                endpoints.MapFallbackToPage("/_Host");
            });

             
        }
    }
}
