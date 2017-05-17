using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Capital.GSG.FX.Monitoring.Server.Connector;
using StratedgemeMonitor.Controllers.Alerts;
using StratedgemeMonitor.Controllers.Systems;
using StratedgemeMonitor.Controllers.DBLoggers;
using StratedgemeMonitor.Controllers.Executions;
using StratedgemeMonitor.Controllers.FXEvents;
using StratedgemeMonitor.Controllers.NewsBulletins;
using StratedgemeMonitor.Controllers.Orders;
using StratedgemeMonitor.Controllers.Positions;
using StratedgemeMonitor.Controllers.TradeEngines;
using Capital.GSG.FX.Utils.Core.Logging;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using StratedgemeMonitor.Controllers.MonitorBackend;
using StratedgemeMonitor.Controllers.FAConfigurations;
using StratedgemeMonitor.Controllers.Stratedgeme.Strategies;

namespace StratedgemeMonitor
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBackendConnectors(Configuration);

            services.AddControllerUtils(Configuration.GetSection("Grafana"));

            // Add framework services.
            services.AddMvc();

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

            app.UseCookieAuthentication();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                ClientId = Configuration["Authentication:AzureAd:ClientId"],
                Authority = Configuration["Authentication:AzureAd:AADInstance"] + Configuration["Authentication:AzureAd:TenantId"],
                CallbackPath = Configuration["Authentication:AzureAd:CallbackPath"]
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
        public static void AddBackendConnectors(this IServiceCollection services, IConfigurationRoot configuration)
        {
            string clientId = configuration["Authentication:AzureAd:ClientId"];
            string key = configuration["Authentication:AzureAd:ClientSecret"];
            string monitorBackendAddress = configuration["MonitorServerBackend:Address"];
            string monitorBackendAppUri = configuration["MonitorServerBackend:AppIdUri"];

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
                return connector.FAConfigurationsConnector;
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
                return connector.StrategiesConnector;
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

            services.AddSingleton((serviceProvider) =>
            {
                return connector.TwsAccountsConnector;
            });
        }

        public static void AddControllerUtils(this IServiceCollection services, IConfigurationSection grafanaConfigSection)
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
                var dbLoggerConnector = serviceProvider.GetService<BackendDBLoggerConnector>();
                var systemStatusesConnector = serviceProvider.GetService<BackendSystemStatusesConnector>();
                var systemServicesConnector = serviceProvider.GetService<BackendSystemServicesConnector>();
                var systemConfigsConnector = serviceProvider.GetService<BackendSystemConfigsConnector>();

                return new DBLoggerControllerUtils(dbLoggerConnector, systemStatusesConnector, systemServicesConnector, systemConfigsConnector);
            });

            services.AddSingleton((serviceProvider) =>
            {
                var executionsConnector = serviceProvider.GetService<BackendExecutionsConnector>();
                var grafanaEndpoint = grafanaConfigSection.GetValue<string>("Endpoint");
                var pnlDashboard = grafanaConfigSection.GetValue<string>("PnlDashboard");

                return new ExecutionsControllerUtils(executionsConnector, grafanaEndpoint, pnlDashboard);
            });

            services.AddSingleton((serviceProvider) =>
            {
                var faConfigurationsConnector = serviceProvider.GetService<BackendFAConfigurationsConnector>();

                return new FAConfigurationsControllerUtils(faConfigurationsConnector);
            });

            services.AddSingleton((serviceProvider) =>
            {
                var fxEventsConnector = serviceProvider.GetService<BackendFXEventsConnector>();

                return new FXEventsControllerUtils(fxEventsConnector);
            });

            services.AddSingleton((serviceProvider) =>
            {
                var executionsConnector = serviceProvider.GetService<BackendExecutionsConnector>();

                return new MonitorBackendControllerUtils(executionsConnector);
            });

            services.AddSingleton((serviceProvider) =>
            {
                var newsBulletinsConnector = serviceProvider.GetService<BackendNewsBulletinsConnector>();

                return new NewsBulletinsControllerUtils(newsBulletinsConnector);
            });

            services.AddSingleton((serviceProvider) =>
            {
                var ordersConnector = serviceProvider.GetService<BackendOrdersConnector>();

                return new OrdersControllerUtils(ordersConnector);
            });

            services.AddSingleton((serviceProvider) =>
            {
                var positionsConnector = serviceProvider.GetService<BackendPositionsConnector>();
                var accountsConnector = serviceProvider.GetService<BackendAccountsConnector>();

                return new PositionsControllerUtils(positionsConnector, accountsConnector);
            });

            services.AddSingleton((serviceProvider) =>
            {
                var strategiesConnector = serviceProvider.GetService<BackendStrategiesConnector>();

                return new StrategiesControllerUtils(strategiesConnector);
            });

            services.AddSingleton((serviceProvider) =>
            {
                var systemStatusesConnector = serviceProvider.GetService<BackendSystemStatusesConnector>();
                var systemServicesConnector = serviceProvider.GetService<BackendSystemServicesConnector>();

                return new SystemsControllerUtils(systemStatusesConnector, systemServicesConnector);
            });

            services.AddSingleton((serviceProvider) =>
            {
                var tradeEngineConnector = serviceProvider.GetService<BackendTradeEngineConnector>();
                var systemStatusesConnector = serviceProvider.GetService<BackendSystemStatusesConnector>();
                var systemServicesConnector = serviceProvider.GetService<BackendSystemServicesConnector>();
                var systemConfigsConnector = serviceProvider.GetService<BackendSystemConfigsConnector>();

                return new TradeEngineControllerUtils(tradeEngineConnector, systemStatusesConnector, systemServicesConnector, systemConfigsConnector);
            });
        }
    }
}
