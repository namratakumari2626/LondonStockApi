using LondonStockApi.Data;
using LondonStockApi.Dtos;
using LondonStockApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LondonStockApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradesController : ControllerBase
    {
        private readonly TradingDbContext _context;
        private readonly ILogger<TradesController> _logger;

        public TradesController(TradingDbContext context, ILogger<TradesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new trade and updates the associated stock’s average price and total volume.
        /// </summary>
        /// <param name="dto">
        /// The trade details, including ticker symbol, price, quantity, and broker identifier.
        /// </param>
        /// <returns>
        /// The newly created <see cref="Trade"/> entity.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(typeof(Trade), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTrade(CreateTradeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var strategy = _context.Database.CreateExecutionStrategy();

            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction =
                        await _context.Database.BeginTransactionAsync();

                    var ticker = dto.TickerSymbol.Trim().ToUpperInvariant();

                    var stock = await _context.Stocks
                        .FirstOrDefaultAsync(s => s.TickerSymbol == ticker);

                    if (stock == null)
                    {
                        stock = new Stock
                        {
                            TickerSymbol = ticker,
                            TotalVolume = 0,
                            AveragePrice = 0,
                            LastUpdated = DateTime.UtcNow
                        };

                        _context.Stocks.Add(stock);
                        await _context.SaveChangesAsync();
                    }

                    var trade = new Trade
                    {
                        StockId = stock.Id,
                        Price = dto.Price,
                        Quantity = dto.Quantity,
                        BrokerId = dto.BrokerId,
                        TradedAt = DateTime.UtcNow
                    };

                    _context.Trades.Add(trade);

                    var totalValue =
                        (stock.AveragePrice * stock.TotalVolume) +
                        (trade.Price * trade.Quantity);

                    stock.TotalVolume += trade.Quantity;
                    stock.AveragePrice = totalValue / stock.TotalVolume;
                    stock.LastUpdated = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                });

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trade for {Ticker}", dto.TickerSymbol);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.");
            }
        }
    }
}
