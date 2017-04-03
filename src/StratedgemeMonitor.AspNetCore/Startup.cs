using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Capital.GSG.FX.Monitoring.Server.Connector;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Capital.GSG.FX.Utils.Core.Logging;
using StratedgemeMonitor.AspNetCore.ControllerUtils;
using StratedgemeMonitor.AspNetCore.Controllers.Systems;

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
            string clientId = Configuration["Authentication:AzureAd:ClientId"];
            string key = Configuration["Authentication:AzureAd:ClientSecret"];
            string monitorBackendAddress = Configuration["MonitorServerBackend:Address"];
            string monitorBackendAppUri = Configuration["MonitorServerBackend:AppIdUri"];

            MonitoringServerConnector connector = new MonitoringServerConnector(clientId, key, monitorBackendAddress, monitorBackendAppUri);

            services.AddSingleton((serviceProvider) =>
            {
                return connector.AccountsConnector;
            });

            services.AddSingleton((serviceProvider) =>
            {
                return connector.AlertsConnector;
            });

            services.AddSingleton((serviceProvider) =>
            {
                return connector.DBLoggerConnector;
            });

            services.AddSingleton((serviceProvider) =>
            {
                return connector.ExecutionsConnector;
            });

            services.AddSingleton((serviceProvider) =>
            {
                return connector.FXEventsConnector;
            });

            services.AddSingleton((serviceProvider) =>
            {
                return connector.OrdersConnector;
            });

            services.AddSingleton((serviceProvider) =>
            {
                return connector.NewsBulletinsConnector;
            });

            services.AddSingleton((serviceProvider) =>
            {
                return connector.PnLsConnector;
            });

            services.AddSingleton((serviceProvider) =>
            {
                return connector.PositionsConnector;
            });

            services.AddSingleton((serviceProvider) =>
            {
                return connector.SystemConfigsConnector;
            });

            services.AddSingleton((serviceProvider) =>
            {
                return connector.SystemServicesConnector;
            });

            services.AddSingleton((serviceProvider) =>
            {
                return connector.SystemStatusesConnector;
            });

            services.AddSingleton((serviceProvider) =>
            {
                return connector.TradeEngineConnector;
            });

            services.AddControllerUtils();

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

            GSGLoggerFactory.Instance.AddConsole(Configuration.GetSection("Logging"));
            GSGLoggerFactory.Instance.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

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

    internal static class ServiceExtensions
    {
        public static void AddControllerUtils(this IServiceCollection services)
        {
            services.AddSingleton((serviceProvider) =>
            {
                var alertsConnector = serviceProvider.GetService<BackendAlertsConnector>();
                var pnlsConnector = serviceProvider.GetService<BackendPnLsConnector>();
                var systemStatusesConnector = serviceProvider.GetService<BackendSystemStatusesConnector>();
                var systemServicesConnector = serviceProvider.GetService<BackendSystemServicesConnector>();

                return new AlertsControllerUtils(alertsConnector, systemStatusesConnector, systemServicesConnector, pnlsConnector);
            });

            services.AddSingleton((serviceProvider) =>
            {
                var systemStatusesConnector = serviceProvider.GetService<BackendSystemStatusesConnector>();
                var systemServicesConnector = serviceProvider.GetService<BackendSystemServicesConnector>();

                return new SystemsControllerUtils(systemStatusesConnector, systemServicesConnector);
            });
        }
    }
}
