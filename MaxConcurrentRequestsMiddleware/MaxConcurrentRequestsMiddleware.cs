using MaxConcurrentRequestsMiddleware.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaxConcurrentRequestsMiddleware
{

    public class MaxConcurrentRequestsMiddleware
    {
        private Hashtable EndpointStatus = new Hashtable();
        private readonly RequestDelegate _next;
        private readonly IOptions<MaxConcurrentRequestsOptions> _config;
        private MaxConcurrentRequestsOptions _options;

        public MaxConcurrentRequestsMiddleware(RequestDelegate next, IOptions<MaxConcurrentRequestsOptions> config)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            };
            _config = config;
            _options = _config?.Value ?? throw new ArgumentNullException(nameof(_config.Value));

            SettingEndpointStatus(_options.EndpointRules);
        }

        private void SettingEndpointStatus(List<EndpointRule> rules)
        {
            foreach (var rule in rules)
            {
                EndpointStatus[GetLoweredTrimedPath(rule.Endpoint)] =
                    new EndpointStatus()
                    {
                        ConcurrentRequestsCount = 0,
                        Limit = rule.Limit
                    };
            }
        }

        private string GetLoweredTrimedPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            else
            {
                char[] charsToTrim = { ' ', '/', '\\' };
                return path.ToLower().Trim(charsToTrim);
            }
        }

        public async Task Invoke(HttpContext context)
        {
            string endpoint = GetLoweredTrimedPath(context.Request.Path);
            var status = (EndpointStatus)EndpointStatus[endpoint];
            if (status == null)
            {
                status = (EndpointStatus)EndpointStatus["*"];
            }
            if (status != null &&
                CheckLimitExceeded(status))
            {
                if (!context.RequestAborted.IsCancellationRequested)
                {
                    IHttpResponseFeature responseFeature = context.Features.Get<IHttpResponseFeature>();
                    responseFeature.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    responseFeature.ReasonPhrase = "Concurrent request limit exceeded.";
                }
            }
            else
            {
                try
                {
                    await _next(context);
                }
                finally
                {
                    if (ShouldDecrementConcurrentRequestsCountAsync(status.Limit))
                    {
                        Interlocked.Decrement(ref status.ConcurrentRequestsCount);
                    }
                }
            }
        }

        private bool CheckLimitExceeded(EndpointStatus status)
        {
            bool limitExceeded;

            if (status.Limit == EndpointRule.ConcurrentRequestsUnlimited)
            {
                limitExceeded = false;
            }
            else
            {
                int initialConcurrentRequestsCount, incrementedConcurrentRequestsCount;
                do
                {
                    limitExceeded = true;

                    initialConcurrentRequestsCount = status.ConcurrentRequestsCount;
                    if (initialConcurrentRequestsCount >= status.Limit)
                    {
                        break;
                    }

                    limitExceeded = false;
                    incrementedConcurrentRequestsCount = initialConcurrentRequestsCount + 1;
                }
                while (initialConcurrentRequestsCount != Interlocked.CompareExchange(ref status.ConcurrentRequestsCount, incrementedConcurrentRequestsCount, initialConcurrentRequestsCount));
            }

            return limitExceeded;
        }

        private bool ShouldDecrementConcurrentRequestsCountAsync(int limit)
        {
            return (limit != EndpointRule.ConcurrentRequestsUnlimited);
        }
    }

    public class EndpointStatus
    {
        public int Limit;
        public int ConcurrentRequestsCount;
    }
}
