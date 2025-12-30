namespace LondonStockApi.Models
{
    public class Trade
    {
        public long Id { get; set; }
        public long StockId { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string BrokerId { get; set; } = default!;
        public DateTime TradedAt { get; set; } = DateTime.UtcNow;
    }
}
