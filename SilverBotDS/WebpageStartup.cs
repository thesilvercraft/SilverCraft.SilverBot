using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilverBotDS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;

using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.IO;

namespace SilverBotDS
{
    public class BrowserService
    {
        private readonly IJSRuntime _js;

        public BrowserService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<BrowserDimension> GetDimensions()
        {
            return await _js.InvokeAsync<BrowserDimension>("getDimensions");
        }
    }

    public class BrowserDimension
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class WebpageStartup
    {
        public WebpageStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddScoped<BrowserService>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
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
            // var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //var assetDirectory = Path.Combine(assemblyDirectory, "wwwroot");

            // use it
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();

                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}