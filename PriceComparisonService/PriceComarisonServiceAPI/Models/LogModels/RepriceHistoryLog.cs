using Newtonsoft.Json;

namespace PriceComarisonServiceAPI.Models
{
    public class RepriceHistoryLog
    {
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "previousPrice")]
        public float PreviousPrice { get; set; }

        [JsonProperty(PropertyName = "newPrice")]
        public float NewPrice { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public int Timestamp { get; set; }  // epoch time
    }
}
