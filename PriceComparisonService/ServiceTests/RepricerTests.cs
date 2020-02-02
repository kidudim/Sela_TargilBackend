using Microsoft.VisualStudio.TestTools.UnitTesting;
using PriceComarisonServiceAPI.Controllers;
using PriceComarisonServiceAPI.Models;
using PriceComarisonServiceTests;
using System.Threading.Tasks;

namespace PriceComparisonControllerTests
{
    [TestClass]
    public class RepricerTests
    {
        static PriceComparisonController controller = GlobalSetter.GetPriceComparisonControllerAndRestartLoggerWorker();

        private async Task SendRepriceRequest(PriceComparisonController controller, float i)
        {
            await controller.Reprice(new RepriceRequest()
            {
                Price = i,
                ProductId = "product" + (i)
            });
        }

        [TestMethod]
        public void Repricing_50_parallel_requests()
        {
            // Arrange
            int requestCount = 50;

            // Act
            Parallel.For(0, requestCount, i =>
            {
                SendRepriceRequest(controller, i + 1);
            });

            // Assert
        }

        [TestMethod]
        public void Repricing_4__requests_500ms_interval()
        {
            // Arrange
            var controller = GlobalSetter.GetPriceComparisonControllerAndRestartLoggerWorker();
            int requestCount = 4;
            int intervalMS = 500;

            for (int i = 0; i < requestCount; i++)
            {
                Task task = new TaskFactory().StartNew(() =>
                {
                    SendRepriceRequest(controller, i + 1);
                });
                Task.Delay(intervalMS).Wait();
            }
            Task.WaitAll();
        }
    }
}
