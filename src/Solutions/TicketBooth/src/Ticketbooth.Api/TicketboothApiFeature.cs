using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stratis.Bitcoin;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Builder.Feature;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Ticketbooth.Api
{
    public class TicketboothApiFeature : FullNodeFeature
    {
        private readonly FullNode _fullNode;
        private readonly IFullNodeBuilder _fullNodeBuilder;
        private readonly IOptions<TicketboothApiOptions> _apiOptions;
        private readonly ILogger<TicketboothApiFeature> _logger;

        private IWebHost _webHost;

        public TicketboothApiFeature(
            FullNode fullNode,
            IFullNodeBuilder fullNodeBuilder,
            ILoggerFactory loggerFactory,
            IOptions<TicketboothApiOptions> ticketboothApiOptions)
        {
            _fullNode = fullNode;
            _fullNodeBuilder = fullNodeBuilder;
            _apiOptions = ticketboothApiOptions;
            _logger = loggerFactory.CreateLogger<TicketboothApiFeature>();
        }

        public override async Task InitializeAsync()
        {
            _logger.LogInformation("API starting...");

            _webHost = WebHost.CreateDefaultBuilder()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, _apiOptions.Value.HttpsPort, config =>
                    {
                        config.UseHttps();
                    });
                })
                .ConfigureServices(services =>
                {
                    // copies all the services defined for the full node.
                    // also copies over singleton instances already defined
                    foreach (ServiceDescriptor service in _fullNodeBuilder.Services)
                    {
                        // open types can't be singletons
                        if (service.ServiceType.IsGenericType || service.Lifetime == ServiceLifetime.Scoped)
                        {
                            services.Add(service);
                            continue;
                        }

                        var serviceObj = _fullNode.Services.ServiceProvider.GetService(service.ServiceType);
                        if (serviceObj != null && service.Lifetime == ServiceLifetime.Singleton && service.ImplementationInstance == null)
                        {
                            services.AddSingleton(service.ServiceType, serviceObj);
                        }
                        else
                        {
                            services.Add(service);
                        }
                    }
                })
                .UseStartup<Startup>()
                .Build();
            await _webHost.StartAsync();

            _logger.LogInformation($"API listening on port {_apiOptions.Value.HttpsPort}");
        }

        public override void Dispose()
        {
            _logger.LogInformation($"API stopping on port {_apiOptions.Value.HttpsPort}");
            _webHost.StopAsync().Wait();
        }
    }

    /// <summary>
    /// Options for the Ticketbooth API
    /// </summary>
    public class TicketboothApiOptions
    {
        /// <summary>
        /// The port to access the API. Set to 39200 by default.
        /// </summary>
        public int HttpsPort { get; set; } = 39200;
    }

    /// <summary>
    /// A class providing extension methods for to configure Ticketbooth API full node extension.
    /// </summary>
    public static class TicketboothSetupExtensions
    {
        /// <summary>
        /// Adds the Ticketbooth API to the full node.
        /// </summary>
        /// <param name="fullNodeBuilder">Full node builder</param>
        /// <param name="setupAction">Configuration options</param>
        /// <returns>The full node builder</returns>
        public static IFullNodeBuilder AddTicketboothApi(this IFullNodeBuilder fullNodeBuilder, Action<TicketboothApiOptions> setupAction = null)
        {
            return fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<TicketboothApiFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddOptions<TicketboothApiOptions>();

                        if (setupAction != null)
                        {
                            services.ConfigureTicketboothApi(setupAction);
                        }

                        services.AddSingleton(fullNodeBuilder);
                    });
            });
        }

        /// <summary>
        /// Configures the Ticketbooth API.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="setupAction">Configuration options</param>
        public static void ConfigureTicketboothApi(this IServiceCollection services, Action<TicketboothApiOptions> setupAction = null)
        {
            services.Configure(setupAction);
        }
    }
}
