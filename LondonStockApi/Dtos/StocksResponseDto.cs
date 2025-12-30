namespace LondonStockApi.Dtos
{
    public class StocksResponseDto
    {
        public IReadOnlyCollection<StockDto> Stocks { get; set; } = [];
    }
}
