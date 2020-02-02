using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PriceComarisonServiceAPI.Models
{
    public class RepriceRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string ProductId { get; set; }

        [Required]
        public float Price { get; set; }
    }
}
