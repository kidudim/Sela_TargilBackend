using PriceComarisonServiceAPI.Models;
using System.Threading.Tasks;

namespace PriceComarisonServiceAPI.Modules
{
    public interface IProductLogger
    {
        Task LogAsync(ProductLog log);
    }
}