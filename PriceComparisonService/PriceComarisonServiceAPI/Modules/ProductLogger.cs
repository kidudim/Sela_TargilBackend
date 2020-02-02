using Newtonsoft.Json;
using PriceComarisonServiceAPI.Models;
using PriceComarisonServiceAPI.Models.Configs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PriceComarisonServiceAPI.Modules
{
    public sealed class ProductLogger : IProductLogger
    {
        private readonly ProductLoggingConfiguration _config;
        private static ProductLoggingConfiguration DEFAULT_CONFIG = new ProductLoggingConfiguration()
        {
            LogFilePath = @"c:/tmp/product/",
            LogFilePrefix = @"results",
        };

        public ProductLogger(ProductLoggingConfiguration config)
        {
            _config = config;
            ValidateConfig();
            CreateLogFolders();
        }

        private void CreateLogFolders()
        {
            if (!Directory.Exists(_config.LogFilePath))
            {
                Directory.CreateDirectory(_config.LogFilePath);
            }
        }

        private void ValidateConfig()
        {
            if (string.IsNullOrEmpty(_config.LogFilePrefix))
            {
                _config.LogFilePrefix = DEFAULT_CONFIG.LogFilePrefix;
            }

            if (string.IsNullOrEmpty(_config.LogFilePath))
            {
                _config.LogFilePath = DEFAULT_CONFIG.LogFilePath;
            }
        }

        public async Task LogAsync(ProductLog log)
        {
            if (log != null)
            {
                string logFilename = GetLogFileName(DateTime.UtcNow);
                using (StreamWriter streamWriter = new StreamWriter(logFilename))
                {
                    var jsonString = JsonConvert.SerializeObject(log);
                    await streamWriter.WriteLineAsync(jsonString);
                    streamWriter.Close();
                }
            }
        }

        // Example {folder}/results_2020.01.22__11.14.03_{GUID}.log 
        private string GetLogFileName(DateTime now)
        {
            string filename;
            filename = $"{_config.LogFilePath}{_config.LogFilePrefix}_{now:yyyy.MM.dd__HH.mm.ss}_{Guid.NewGuid()}.log";
            return filename;
        }
    }
}
