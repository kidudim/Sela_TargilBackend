using PriceComarisonServiceAPI.Models.Cache;
using System;

namespace PriceComarisonServiceAPI.Modules
{
    public interface IRepriceCache
    {
        void SetValue(string productId, float price, DateTime dateTime);
        CachedPriceValue GetValue(string productId);
    }
}