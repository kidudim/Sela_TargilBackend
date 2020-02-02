using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PriceComarisonServiceAPI.Models
{
    public class QueryRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string ProductId { get; set; }

        [Required]
        public long From { get; set; }  // Epoch time

        [Required]
        public long To { get; set; }    // Epoch time

        [Required]
        public int PageSize { get; set; }

        [Required]
        public int PageNumber { get; set; }
    }
}
