using System.ComponentModel.DataAnnotations;

namespace LondonStockApi.Dtos
{
    public class CreateTradeDto
    {
        [Required]
        [StringLength(10)]
        public string TickerSymbol { get; set; } = default!;

        [Range(0.0001, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0.0001, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        [StringLength(50)]
        public string BrokerId { get; set; } = default!;
    }
}
