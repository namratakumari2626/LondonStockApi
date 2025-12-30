using LondonStockApi.Data;
using LondonStockApi.Dtos;
using LondonStockApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LondonStockApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly TradingDbContext _context;
        private readonly ILogger<StocksController> _logger;


        public StocksController(TradingDbContext context, ILogger<StocksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a single stock by its ticker symbol.
        /// </summary>
        /// <param name="ticker">
        /// The stock ticker symbol (e.g. AAPL, MSFT). Case-insensitive.
        /// </param>
        /// <returns>
        /// A <see cref="StockDto"/> containing the ticker symbol and average price.
        /// </returns>
        [HttpGet("{ticker}")]
        [ProducesResponseType(typeof(StockDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStock(string ticker)
        {
            if (string.IsNullOrWhiteSpace(ticker))
                return BadRequest("Ticker symbol is required.");

            try
            {
                ticker = ticker.ToUpperInvariant();

                var stock = await _context.Stocks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.TickerSymbol == ticker);

                if (stock == null)
                    return NotFound();

                var response = new StockDto
                {
                    TickerSymbol = stock.TickerSymbol,
                    AveragePrice = stock.AveragePrice
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock {Ticker}", ticker);
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.");
            }
        }

        /// <summary>
        /// Retrieves a list of stocks, optionally filtered by a comma-separated list of ticker symbols.
        /// </summary>
        /// <param name="tickers">
        /// A comma-separated list of ticker symbols (e.g. "AAPL,MSFT,GOOG").  
        /// Whitespace is trimmed and comparison is case-insensitive.
        /// </param>
        /// <returns>
        /// A collection of <see cref="StockDto"/> objects.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StockDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStocks([FromQuery] string? tickers)
        {
            try
            {
                IQueryable<Stock> query = _context.Stocks.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(tickers))
                {
                    var normalizedTickers = tickers
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim().ToUpperInvariant())
                        .Distinct()
                        .ToList();

                    query = query.Where(s => normalizedTickers.Contains(s.TickerSymbol));
                }

                var stocks = await query
                    .Select(s => new StockDto
                    {
                        TickerSymbol = s.TickerSymbol,
                        AveragePrice = s.AveragePrice
                    })
                    .ToListAsync();

                return Ok(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stocks");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
