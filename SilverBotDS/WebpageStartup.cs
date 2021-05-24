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
using NetTools;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Radzen;

namespace SilverBotDS
{
    internal static class CloudFlareConnectingIpMiddleware
    {
        public const string CLOUDFLARE_CONNECTING_IP_HEADER_NAME = "CF_CONNECTING_IP";

        private static string[] GetStrings(string url)
        {
            return new WebClient().DownloadString(url).Split('\n').Select(s => s.Trim()).ToArray();
        }

        private static string[] GetCloudflareIP()
        {
            try
            {
                return GetStrings("https://www.cloudflare.com/ips-v4").Union(GetStrings("https://www.cloudflare.com/ips-v6")).ToArray();
            }
            catch
            {
                return @"173.245.48.0/20
103.21.244.0/22
103.22.200.0/22
103.31.4.0/22
141.101.64.0/18
108.162.192.0/18
190.93.240.0/20
188.114.96.0/20
197.234.240.0/22
198.41.128.0/17
162.158.0.0/15
104.16.0.0/12
172.64.0.0/13
131.0.72.0/22
2400:cb00::/32
2606:4700::/32
2803:f800::/32
2405:b500::/32
2405:8100::/32
2a06:98c0::/29
2c0f:f248::/32
".Split('\n').Select(s => s.Trim()).ToArray();
            }
        }

        /// <summary>
        /// Add cloudflare forward header options
        /// </summary>
        /// <param name="builder">Application builder</param>
        public static void UseCloudflareForwardHeaderOptions(this IApplicationBuilder builder)
        {
            ForwardedHeadersOptions options = new()
            {
                ForwardedForHeaderName = CLOUDFLARE_CONNECTING_IP_HEADER_NAME,
                ForwardedHeaders = ForwardedHeaders.All
            };
            try
            {
                ICollection<string> urls = builder.ServerFeatures.Get<IServerAddressesFeature>().Addresses;
                if (urls != null && urls.Count != 0)
                {
                    string[] cloudFlareIP = GetCloudflareIP();
                    foreach (string line in cloudFlareIP)
                    {
                        if (IPAddressRange.TryParse(line, out IPAddressRange range))
                        {
                            options.KnownNetworks.Add(new IPNetwork(range.Begin, range.GetPrefixLength()));
                        }
                    }
                }
            }
            catch
            {
                //oh no something went wrong
            }
            builder.UseForwardedHeaders(options);
        }
    }

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
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();
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

            // use it
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCloudflareForwardHeaderOptions();
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