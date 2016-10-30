using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Capital.GSG.FX.Utils.Core;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

namespace StratedgemeMonitor.AspNetCore
{
    public class Startup
    {
        internal static string ClientId;
        internal static string ClientSecret;
        internal static string Authority;
        internal static string MonitorBackendServerAddress;
        internal static string MonitorBackendServerResourceId;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string monitorBackendAddress = Configuration["MonitorServerBackend:Address"];

            services.AddSingleton((serviceProvider) =>
            {
                return new BackendAccountsConnector(monitorBackendAddress);
            });

            services.AddSingleton((serviceProvider) =>
            {
                return new BackendAlertsConnector(monitorBackendAddress);
            });

            services.AddSingleton((serviceProvider) =>
            {
                return new BackendDBLoggerConnector(monitorBackendAddress);
            });

            services.AddSingleton((serviceProvider) =>
            {
                return new BackendExecutionsConnector(monitorBackendAddress);
            });

            services.AddSingleton((serviceProvider) =>
            {
                return new BackendFXEventsConnector(monitorBackendAddress);
            });

            services.AddSingleton((serviceProvider) =>
            {
                return new BackendOrdersConnector(monitorBackendAddress);
            });

            services.AddSingleton((serviceProvider) =>
            {
                return new BackendNewsBulletinsConnector(monitorBackendAddress);
            });

            services.AddSingleton((serviceProvider) =>
            {
                return new BackendPositionsConnector(monitorBackendAddress);
            });

            services.AddSingleton((serviceProvider) =>
            {
                return new BackendSystemConfigsConnector(monitorBackendAddress);
            });

            services.AddSingleton((serviceProvider) =>
            {
                return new BackendSystemServicesConnector(monitorBackendAddress);
            });

            services.AddSingleton((serviceProvider) =>
            {
                return new BackendSystemStatusesConnector(monitorBackendAddress);
            });

            services.AddSingleton((serviceProvider) =>
            {
                return new BackendTradeEngineConnector(monitorBackendAddress);
            });

            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();

            services.AddSession();

            services.AddAuthentication(
                SharedOptions => SharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            LoggingUtils.LoggerFactory.AddConsole(Configuration.GetSection("Logging"));
            LoggingUtils.LoggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseSession();

            // Populate AzureAd Configuration Values
            Authority = Configuration["Authentication:AzureAd:AADInstance"] + Configuration["Authentication:AzureAd:TenantId"];
            ClientId = Configuration["Authentication:AzureAd:ClientId"];
            ClientSecret = Configuration["Authentication:AzureAd:ClientSecret"];

            MonitorBackendServerAddress = Configuration["MonitorServerBackend:Address"];
            MonitorBackendServerResourceId = Configuration["MonitorServerBackend:AppIdUri"];

            app.UseCookieAuthentication();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Authority = Authority,
                CallbackPath = Configuration["Authentication:AzureAd:CallbackPath"],
                ResponseType = OpenIdConnectResponseType.CodeIdToken,
                GetClaimsFromUserInfoEndpoint = false
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Alerts}/{action=Index}/{id?}");
            });

            app.UseResponseCompression();
        }
    }
}
