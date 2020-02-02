using System;

namespace PriceComarisonServiceAPI.Models.Cache
{
    public class CachedPriceValue
    {
        public float NewPrice { get; set; }
        public float? PreviousPrice { get; set; }
        public DateTime DateTime { get; set; }
    }
}
