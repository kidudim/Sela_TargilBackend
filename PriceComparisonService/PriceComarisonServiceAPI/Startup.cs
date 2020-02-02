using MaxConcurrentRequestsMiddleware;
using MaxConcurrentRequestsMiddleware.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PriceComarisonServiceAPI.Models.Configs;
using PriceComarisonServiceAPI.Modules;
using System;
using System.IO;
using System.Reflection;

namespace PriceComarisonServiceAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigMaxConcurrentCallsService(services);
            ConfigureRepricerService(services);
            ConfigureProductService(services);

            services.AddControllers();

            #region swagger

            //This line adds Swagger generation services to our container.
            services.AddSwaggerGen(c =>
            {
                //The generated Swagger JSON file will have these properties.
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Price Comparison API",
                    Version = "v1",
                    Description = "Einat Rosenberg's example project for Sela - Backend Consultant",
                    Contact = new OpenApiContact
                    {
                        Name = "Einat Rosenberg",
                        Email = "einat@kidudim.com",
                        Url = new Uri("https://www.kidudim.com"),
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            #endregion
        }

        #region service_configurations

        private void ConfigureProductService(IServiceCollection services)
        {
            var ProductLogConfig = GetProductLoggingConfiguration();
            var productLogger = new ProductLogger(ProductLogConfig);
            services.AddSingleton(typeof(IProductLogger), productLogger);
        }

        private void ConfigureRepricerService(IServiceCollection services)
        {
            services.Configure<RepriceLoggingConfiguration>(Configuration.GetSection("RepriceLogging"));
            var config = GetRepriceLoggingConfiguration();
            var repriceLogger = new RepriceLogger(config);
            services.AddSingleton(typeof(IRepriceLogger), repriceLogger);

            // Cache
            services.AddSingleton(typeof(IRepriceCache), new RepriceCache());

            // Logger worker
            var logWorker = new RepriceLogWorker(repriceLogger);
            services.AddSingleton(typeof(IRepriceLogWorker), logWorker);
            logWorker.Start(config.CreateEveryXSeconds * 1000);
        }
        private void ConfigMaxConcurrentCallsService(IServiceCollection services)
        {
            services.Configure<MaxConcurrentRequestsOptions>(Configuration.GetSection("MaxConcurrentRequestsOptions"));
        }

        #endregion

        #region get_configs

        public RepriceLoggingConfiguration GetRepriceLoggingConfiguration()
        {
            try
            {
                return Configuration.GetSection("RepriceLogging").Get<RepriceLoggingConfiguration>();
            }
            catch
            {
                return null;
            }
        }

        public ProductLoggingConfiguration GetProductLoggingConfiguration()
        {
            try
            {
                return Configuration.GetSection("ProductLogging").Get<ProductLoggingConfiguration>();
            }
            catch
            {
                return null;
            }
        }

        public QueryConfiguration GetQueryConfiguration()
        {
            try
            {
                return Configuration.GetSection("QueryLogging").Get<QueryConfiguration>();
            }
            catch
            {
                return null;
            }
        }

        #endregion

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMaxConcurrentRequests();

            #region swagger_ui
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Einat Rosenberg's example project for Sela");
                c.RoutePrefix = string.Empty;
            });
            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
