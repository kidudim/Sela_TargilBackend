using PriceComarisonServiceAPI.Models.Cache;
using System;
using System.Collections;

namespace PriceComarisonServiceAPI.Modules
{
    public sealed class RepriceCache : IRepriceCache
    {
        private static Hashtable cache = new Hashtable();

        public void SetValue(string productId, float price, DateTime dateTime)
        {
            if (cache.ContainsKey(productId))
            {
                cache[productId] = new CachedPriceValue() { NewPrice = price, DateTime = dateTime, PreviousPrice = GetValue(productId).NewPrice };
            }
            else
            {
                cache[productId] = new CachedPriceValue() { NewPrice = price, DateTime = dateTime, PreviousPrice = null };
            }
            return;
        }

        public CachedPriceValue GetValue(string productId)
        {
            return (CachedPriceValue)cache[productId];
        }
    }
}
