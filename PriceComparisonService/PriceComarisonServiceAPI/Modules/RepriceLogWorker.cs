using System;
using System.Threading;

namespace PriceComarisonServiceAPI.Modules
{
    public class RepriceLogWorker : IRepriceLogWorker
    {
        private bool _enabled = false;
        private int _interval_in_milliseconds = 1000;
        private readonly IRepriceLogger _logger;

        public RepriceLogWorker(IRepriceLogger logger)
        {
            _logger = logger;
        }

        public void Start(int? intervalMS = 1000)
        {
            if (intervalMS != null)
            {
                _interval_in_milliseconds = (int)intervalMS;
            }

            Console.WriteLine("Starting LogWorker...");

            _enabled = true;

            Thread worker = new Thread(
                () =>
                {
                    while (true)
                    {
                        if (_enabled)
                        {
                            try
                            {
                                DoWork();
                            }
                            catch (Exception ex)
                            {
                            }
                        }

                        Thread.Sleep(_interval_in_milliseconds);
                    }
                });

            worker.Start();

        }

        public void Stop()
        {
            Console.WriteLine("Stopping LogWorker...");

            _enabled = false;
        }

        public async void DoWork()
        {
            Console.WriteLine("Working " + DateTime.UtcNow);

            await _logger.FlushLogAsync();
        }

        public void SetInterval(int intervalMS)
        {
            _interval_in_milliseconds = intervalMS;
        }
    }
}
