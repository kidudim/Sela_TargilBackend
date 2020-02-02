namespace MaxConcurrentRequestsMiddleware.Config
{
    public class EndpointRule
    {
        public const int ConcurrentRequestsUnlimited = -1;

        private int _limit;

        public string Endpoint { get; set; }

        public int Limit
        {
            get { return _limit; }

            set { _limit = (value < ConcurrentRequestsUnlimited) ? ConcurrentRequestsUnlimited : value; }
        }

        public EndpointRule()
        {
            _limit = ConcurrentRequestsUnlimited;
            Endpoint = "*";
        }
    }
}
