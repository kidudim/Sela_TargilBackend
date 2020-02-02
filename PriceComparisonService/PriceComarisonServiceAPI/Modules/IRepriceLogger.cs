using PriceComarisonServiceAPI.Models;
using System.Threading.Tasks;

namespace PriceComarisonServiceAPI.Modules
{
    public interface IRepriceLogger
    {
        public Task AddLogAsync(RepriceLog log);
        public Task FlushLogAsync();
    }
}
