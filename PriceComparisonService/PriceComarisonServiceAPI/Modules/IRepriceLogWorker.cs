namespace PriceComarisonServiceAPI.Modules
{
    public interface IRepriceLogWorker
    {
        public void Start(int? intervalMS);
        public void Stop();
        public void SetInterval(int intervalMS);
    }
}
