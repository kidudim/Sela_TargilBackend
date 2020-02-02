using Newtonsoft.Json;

namespace PriceComarisonServiceAPI.Models
{
    public class QueryResponse
    {
        public QueryResonseItem[] items { get; set; }
    }

    public class QueryResonseItem
    {
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public int Timestamp { get; set; }  // epoch time
    }
}
