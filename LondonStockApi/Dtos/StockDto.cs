namespace LondonStockApi.Dtos
{
    public class StockDto
    {
        public string TickerSymbol { get; set; } = default!;
        public decimal AveragePrice { get; set; }
    }
}
