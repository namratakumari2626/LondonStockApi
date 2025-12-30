namespace LondonStockApi.Models
{
    public class Stock
    {
        public long Id { get; set; }
        public string TickerSymbol { get; set; } = default!;
        public decimal AveragePrice { get; set; }
        public decimal TotalVolume { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
