using Newtonsoft.Json;
using PriceComarisonServiceAPI.Models;
using PriceComarisonServiceAPI.Models.Configs;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace PriceComarisonServiceAPI.Modules
{
    public sealed class RepriceLogger : IRepriceLogger
    {
        private readonly RepriceLoggingConfiguration _options;
        //private readonly IOptions<RepriceLoggingConfiguration> _config;
        private readonly object lockObj = new object();
        private ConcurrentQueue<RepriceLog> logQueue = new ConcurrentQueue<RepriceLog>();

        private static RepriceLoggingConfiguration DEFAULT_CONFIG = new RepriceLoggingConfiguration()
        {
            LogFilePath = @"c:/tmp/repricer/",
            LogFilePrefix = @"results",
            MaximumLogsInAFile = 10
        };

        public RepriceLogger(RepriceLoggingConfiguration options/*IOptions<RepriceLoggingConfiguration> config*/)
        {
            //if (config == null)
            //{
            //    throw new ArgumentNullException(nameof(config));
            //};
            //_options = config?.Value ?? throw new ArgumentNullException(nameof(config.Value));
            //_config = config;
            _options = options ?? throw new ArgumentNullException(nameof(options));
            ValidateConfig();
            CreateLogFolders();
        }

        private void CreateLogFolders()
        {
            if(!Directory.Exists(_options.LogFilePath))
            {
                Directory.CreateDirectory(_options.LogFilePath);
            }
        }

        private void ValidateConfig()
        {
            if (_options.MaximumLogsInAFile <= 0)
            {
                _options.MaximumLogsInAFile = DEFAULT_CONFIG.MaximumLogsInAFile;
            }

            if (string.IsNullOrEmpty(_options.LogFilePrefix))
            {
                _options.LogFilePrefix = DEFAULT_CONFIG.LogFilePrefix;
            }

            if (string.IsNullOrEmpty(_options.LogFilePath))
            {
                _options.LogFilePath = DEFAULT_CONFIG.LogFilePath;
            }
        }

        public async Task AddLogAsync(RepriceLog log)
        {
            bool needFlushing;
            var now = DateTime.UtcNow;
            string filenameToFlush = GetLogFileName(now);
            RepriceLog[] logsToFlush = null;

            lock (lockObj)
            {
                logQueue.Enqueue(log);
                needFlushing = CheckMaxQuataReached();

                if (needFlushing)
                {
                    // Save logs and filename for async file flushing method
                    logsToFlush = logQueue.ToArray();
                    logQueue.Clear();
                }
            }

            if (needFlushing)
            {
                await FlushLogsAsync(logsToFlush, filenameToFlush);
            }
        }

        public async Task FlushLogAsync()
        {
            if (logQueue.Count > 0)
            {
                // Save logs and filename for async file flushing method
                string filenameToFlush = GetLogFileName(DateTime.UtcNow);
                RepriceLog[] logsToFlush = null;

                lock (lockObj)
                {
                    logsToFlush = logQueue.ToArray();
                    logQueue.Clear();
                }

                await FlushLogsAsync(logsToFlush, filenameToFlush);
            }
        }

        private async Task FlushLogsAsync(RepriceLog[] logs, string logfile)
        {
            if (logs.Length > 0)
            {
                using (StreamWriter streamWriter = new StreamWriter(logfile))
                {
                    for (int i = 0; i < logs.Length; i++)
                    {
                        var jsonString = JsonConvert.SerializeObject(logs[i]);
                        await streamWriter.WriteLineAsync(jsonString);
                    }
                    streamWriter.Close();
                }
            }
            return;
        }

        private bool CheckMaxQuataReached()
        {
            return (logQueue.Count >= _options.MaximumLogsInAFile);
        }

        // Example {folder}/results_2020.01.22__11.14.03_{GUID}.log 
        private string GetLogFileName(DateTime now)
        {
            string filename;
            filename = $"{_options.LogFilePath}{_options.LogFilePrefix}_{now:yyyy.MM.dd__HH.mm.ss}_{Guid.NewGuid()}.log";
            return filename;
        }
    }
}
