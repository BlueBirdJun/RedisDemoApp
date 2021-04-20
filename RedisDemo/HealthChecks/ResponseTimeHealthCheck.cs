using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RedisDemo.HealthChecks
{
    public class ResponseTimeHealthCheck : IHealthCheck
    {
        private Random rnd = new Random();
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            int responseTimeInMS = rnd.Next(1, 300);
            if(responseTimeInMS<100)
            {
                return Task.FromResult(HealthCheckResult.Healthy($"the response time looks good ({responseTimeInMS})."));
            }
            else if(responseTimeInMS <200)
            {
                return Task.FromResult(HealthCheckResult.Degraded($"the response time bit slow ({responseTimeInMS})."));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"the response time is unacceptable ({responseTimeInMS})."));
            }
        }
    }

}
