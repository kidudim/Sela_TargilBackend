using Newtonsoft.Json;
using System;

namespace PriceComarisonServiceAPI.Models
{
    public class RepriceLog
    {
        [JsonProperty(PropertyName = "productId")]
        public string ProductId { get; set; }

        [JsonProperty(PropertyName = "price")]
        public float Price { get; set; }

        [JsonIgnore]
        public DateTime Datetime { get; set; }
    }
}
