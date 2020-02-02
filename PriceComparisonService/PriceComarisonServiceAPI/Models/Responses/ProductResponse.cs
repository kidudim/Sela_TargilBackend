using Newtonsoft.Json;

namespace PriceComarisonServiceAPI.Models
{
    public class ProductResponse
    {
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }  // epoch time
    }
}
