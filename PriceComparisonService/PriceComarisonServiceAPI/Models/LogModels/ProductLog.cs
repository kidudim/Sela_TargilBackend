using Newtonsoft.Json;

namespace PriceComarisonServiceAPI.Models
{
    public class ProductLog
    {
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "newPrice")]
        public float NewPrice { get; set; }

        [JsonProperty(PropertyName = "previousPrice")]
        public float? PreviousPrice { get; set; }

        [JsonProperty(PropertyName = "TimeStamp")]
        public long timestamp { get; set; } // Epoch time
    }
}
