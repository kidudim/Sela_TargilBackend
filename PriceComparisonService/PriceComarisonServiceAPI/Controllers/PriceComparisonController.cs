using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PriceComarisonServiceAPI.Models;
using PriceComarisonServiceAPI.Modules;
using System;
using System.Threading.Tasks;

namespace PriceComarisonServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriceComparisonController : ControllerBase
    {
        private readonly IRepriceLogger _repriceLogger;
        private readonly IProductLogger _productLogger;
        private readonly IRepriceCache _priceCache;

        public PriceComparisonController(IRepriceLogger repriceLogger, IProductLogger productLogger, IRepriceCache priceCache)
        {
            _repriceLogger = repriceLogger;
            _priceCache = priceCache;
            _productLogger = productLogger;
        }

        /// <summary>
        /// trigger a repricing flow for a single product
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Reprice")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> Reprice(RepriceRequest req)
        {
            await UpdatePrice(req.ProductId, req.Price);
            return Accepted();
        }

        private async Task UpdatePrice(string productId, float price)
        {
            var datetime = DateTime.UtcNow;
            _priceCache.SetValue(productId, price, datetime);
            await _repriceLogger.AddLogAsync(new RepriceLog() { ProductId = productId, Price = price, Datetime = datetime });
            return;
        }

        /// <summary>
        /// return the latest price for a given product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/product/{id}/price")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Product(string id)
        {
            var result = _priceCache.GetValue(id);

            if (result == null)
            {
                return NotFound();
            }
            else
            {
                TimeSpan t = result.DateTime - new DateTime(1970, 1, 1);
                var epochTime = (long)t.TotalSeconds;

                ProductResponse response = new ProductResponse()
                {
                    Price = result.NewPrice,
                    ProductId = id,
                    Timestamp = epochTime
                };

                _productLogger.LogAsync(new ProductLog()
                {
                    NewPrice = result.NewPrice,
                    PreviousPrice = result.PreviousPrice,
                    ProductId = id,
                    timestamp = epochTime
                });

                return Ok(response);
            }
        }

        /// <summary>
        /// return products from the log files
        /// according to query filters and enable paging
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Query")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult> Query([FromQuery]QueryRequest req)  // [FromQuery] was added for swagger
        {
            System.Threading.Thread.Sleep(1000);  // simulate a lot of processing
            return Ok();
        }
    }
}
