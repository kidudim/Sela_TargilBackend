namespace PriceComarisonServiceAPI.Models.Configs
{
    public class RepriceLoggingConfiguration
    {
        public int CreateEveryXSeconds { get; set; }

        public int MaximumLogsInAFile { get; set; }

        public string LogFilePrefix { get; set; }

        public string LogFilePath { get; set; }
    }
}
