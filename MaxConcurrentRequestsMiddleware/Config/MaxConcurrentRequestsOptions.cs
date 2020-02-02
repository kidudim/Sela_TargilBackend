using System.Collections.Generic;

namespace MaxConcurrentRequestsMiddleware.Config
{
    public class MaxConcurrentRequestsOptions
    {
        public List<EndpointRule> EndpointRules { get; set; }
    }
}
