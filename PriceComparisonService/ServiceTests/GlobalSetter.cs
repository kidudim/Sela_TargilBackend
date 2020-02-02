using MaxConcurrentRequestsMiddleware.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceComarisonServiceAPI;
using PriceComarisonServiceAPI.Controllers;
using PriceComarisonServiceAPI.Models.Configs;
using PriceComarisonServiceAPI.Modules;
using System;
using System.Collections.Generic;
using System.Net;

namespace PriceComarisonServiceTests
{
    public static class GlobalSetter
    {
        private static RepriceLoggingConfiguration REPRICE_DEFAULT_CONFIG = new RepriceLoggingConfiguration()
        {
            LogFilePath = @"c:/tmp/repricer/",
            LogFilePrefix = @"results",
            CreateEveryXSeconds = 1,
            MaximumLogsInAFile = 10
        };

        private static ProductLoggingConfiguration PRODUCT_DEFAULT_CONFIG = new ProductLoggingConfiguration()
        {
            LogFilePath = @"c:/tmp/product/",
            LogFilePrefix = @"results"
        };

        private static RepriceCache repriceCache = new RepriceCache();
        private static RepriceLogger repriceLogger = new RepriceLogger(REPRICE_DEFAULT_CONFIG);
        private static RepriceLogWorker logWorker = new RepriceLogWorker(repriceLogger);
        private static ProductLogger productLogger = new ProductLogger(PRODUCT_DEFAULT_CONFIG);
        private static PriceComparisonController controller = new PriceComparisonController(repriceLogger, productLogger, repriceCache);

        public static PriceComparisonController GetPriceComparisonControllerAndRestartLoggerWorker()
        {
            logWorker.Stop();
            logWorker.Start(REPRICE_DEFAULT_CONFIG.CreateEveryXSeconds * 1000);
            return controller;
        }

        #region Prepare SUT
        public static TestServer PrepareTestServer(IEnumerable<KeyValuePair<string, string>> configuration = null)
        {
            IWebHostBuilder webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>();

            if (configuration != null)
            {
                ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddInMemoryCollection(configuration);
                IConfiguration buildedConfiguration = configurationBuilder.Build();

                webHostBuilder.UseConfiguration(buildedConfiguration);
                webHostBuilder.ConfigureServices((services) =>
                {
                    services.Configure<MaxConcurrentRequestsOptions>(options => buildedConfiguration.GetSection("MaxConcurrentRequestsOptions").Bind(options));
                });
            }

            return new TestServer(webHostBuilder);
        }
        #endregion
    }

    public struct HttpResponseInformation
    {
        public HttpStatusCode StatusCode { get; set; }

        public TimeSpan Timing { get; set; }

        public override string ToString()
        {
            return $"StatusCode: {StatusCode} | Timing {Timing}";
        }
    }
}
