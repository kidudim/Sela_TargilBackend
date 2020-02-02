using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PriceComarisonServiceTests;
using PriceComarisonServiceTests.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PriceComparisonControllerTests
{
    [TestClass]
    public class MaxConcurrentRequests_Query_Tests
    {
        [TestMethod]
        public void Query_less_than_50_parallel_requests_no_service_unavailable()
        {
            // Arrange
            int concurrentCalls = 10;
            int maxConcurrentCalls = 50;
            HttpResponseInformation[] responseInformation;
            Dictionary<string, string> configuration = new Dictionary<string, string>
            {
                {"RepriceLogging:CreateEveryXSeconds", "1" },
                {"RepriceLogging:MaximumLogsInAFile", "10" },
                {"RepriceLogging:LogFilePrefix", "results" },
                {"RepriceLogging:LogFilePath", "/tmp/repricer/" },
                {"ProductLogging:LogFilePrefix", "results" },
                {"ProductLogging:LogFilePath", "/tmp/product/" },
                {"MaxConcurrentRequestsOptions:EndpointRules:0:Endpoint", "/api/PriceComparison/Query" },
                {"MaxConcurrentRequestsOptions:EndpointRules:0:Limit", $"{maxConcurrentCalls }" }
            };

            using (TestServer server = GlobalSetter.PrepareTestServer(configuration))
            {
                List<HttpClient> clients = new List<HttpClient>();
                for (int i = 0; i < concurrentCalls; i++)
                {
                    clients.Add(server.CreateClient());
                }

                List<Task<HttpResponseMessageWithTiming>> responsesWithTimingsTasks = new List<Task<HttpResponseMessageWithTiming>>();
                foreach (HttpClient client in clients)
                {
                    responsesWithTimingsTasks.Add(Task.Run(async () => { return await client.GetWithTimingAsync("api/PriceComparison/Query?ProductId=1&From=2&To=3&PageSize=4&PageNumber=5"); }));
                }
                Task.WaitAll(responsesWithTimingsTasks.ToArray());

                clients.ForEach(client => client.Dispose());

                responseInformation = responsesWithTimingsTasks.Select(task => new HttpResponseInformation
                {
                    StatusCode = task.Result.Response.StatusCode,
                    Timing = task.Result.Timing
                }).ToArray();
            }

            bool bHadServiceUnavailable = responseInformation.Any(i => i.StatusCode == HttpStatusCode.ServiceUnavailable);
            Assert.AreEqual(false, bHadServiceUnavailable, "Should not have service unavailable");
        }

        [TestMethod]
        public void Query_more_than_50_parallel_requests()
        {
            // Arrange
            int concurrentCalls = 53;
            int maxConcurrentCalls = 50;
            HttpResponseInformation[] responseInformation;
            Dictionary<string, string> configuration = new Dictionary<string, string>
            {
                {"RepriceLogging:CreateEveryXSeconds", "1" },
                {"RepriceLogging:MaximumLogsInAFile", "10" },
                {"RepriceLogging:LogFilePrefix", "results" },
                {"RepriceLogging:LogFilePath", "/tmp/repricer/" },
                {"ProductLogging:LogFilePrefix", "results" },
                {"ProductLogging:LogFilePath", "/tmp/product/" },
                {"MaxConcurrentRequestsOptions:EndpointRules:0:Endpoint", "/api/PriceComparison/Query" },
                {"MaxConcurrentRequestsOptions:EndpointRules:0:Limit", $"{maxConcurrentCalls }" }
            };

            using (TestServer server = GlobalSetter.PrepareTestServer(configuration))
            {
                List<HttpClient> clients = new List<HttpClient>();
                for (int i = 0; i < concurrentCalls; i++)
                {
                    clients.Add(server.CreateClient());
                }

                List<Task<HttpResponseMessageWithTiming>> responsesWithTimingsTasks = new List<Task<HttpResponseMessageWithTiming>>();
                foreach (HttpClient client in clients)
                {
                    responsesWithTimingsTasks.Add(Task.Run(async () => { return await client.GetWithTimingAsync("api/PriceComparison/Query?ProductId=1&From=2&To=3&PageSize=4&PageNumber=5"); }));
                }
                Task.WaitAll(responsesWithTimingsTasks.ToArray());

                clients.ForEach(client => client.Dispose());

                responseInformation = responsesWithTimingsTasks.Select(task => new HttpResponseInformation
                {
                    StatusCode = task.Result.Response.StatusCode,
                    Timing = task.Result.Timing
                }).ToArray();
            }

            var results = responseInformation;
            Assert.AreEqual(concurrentCalls - maxConcurrentCalls, responseInformation.Count(i => i.StatusCode == HttpStatusCode.ServiceUnavailable));
        }
    }
}
