using Microsoft.AspNetCore.Builder;
using System;

namespace MaxConcurrentRequestsMiddleware
{
    public static class MaxConcurrentRequestsMiddlewareExtensions
    {
        public static IApplicationBuilder UseMaxConcurrentRequests(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<MaxConcurrentRequestsMiddleware>();
        }
    }
}
