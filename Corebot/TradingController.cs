using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradingBotAPI.CoreBot;
using TradingBotAPI.CoreBot.Models;
using TradingBotAPI.Services;

namespace TradingBotAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradingController : ControllerBase
    {
        private readonly TradeRepository _tradeRepo;
        private readonly TradingService _tradingService;
        private readonly KrakenClient _kraken;

        public TradingController(
            TradeRepository tradeRepo,
            TradingService tradingService)
        {
            _tradeRepo = tradeRepo;
            _tradingService = tradingService;
            _kraken = new KrakenClient();
        }

        // ✅ Start Grid Bot
        [HttpPost("start-gridbot")]
        public async Task<IActionResult> StartGridBot([FromBody] GridBotRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            // ✅ fetch current price from Kraken
            var currentPrice = await _kraken.GetLatestPriceAsync("XRPUSD");

            // ✅ auto-populate defaults if missing or 0
            if (request.Lower <= 0)
                request.Lower = decimal.Round(currentPrice * 0.98m, 4); // -2%
            if (request.Upper <= 0)
                request.Upper = decimal.Round(currentPrice * 1.02m, 4); // +2%
            if (request.Grids <= 0)
                request.Grids = 4;

            _tradingService.StartGridBot(request.Lower, request.Upper, request.Grids, request.Investment);

            // ✅ return values to frontend
            return Ok(new
            {
                lower = request.Lower,
                upper = request.Upper,
                grids = request.Grids,
                investment = request.Investment,
                message = "Grid Bot started"
            });
        }

        // ✅ Stop Grid Bot
        [HttpPost("stop-gridbot")]
        public IActionResult StopGridBot()
        {
            _tradingService.StopGridBot();
            return Ok(new { message = "Grid Bot stopped" });
        }

        // ✅ Preview Grid Bot (new endpoint)
        [HttpGet("preview-gridbot")]
        public async Task<IActionResult> PreviewGridBot([FromQuery] decimal investment = 0)
        {
            // fetch current price from Kraken
            var currentPrice = await _kraken.GetLatestPriceAsync("XRPUSD");

            // calculate suggested defaults
            var lower = decimal.Round(currentPrice * 0.98m, 4); // -2%
            var upper = decimal.Round(currentPrice * 1.02m, 4); // +2%
            var grids = 4;

            return Ok(new
            {
                currentPrice,
                lower,
                upper,
                grids,
                investment
            });
        }

        // ✅ Get Trade History
        [HttpGet("trades")]
        public ActionResult<List<TradeRecord>> GetAllTrades()
        {
            var trades = _tradeRepo.GetLast100Trades();
            return Ok(trades);
        }

        // ✅ Get Latest Price
        [HttpGet("get-latest-price")]
        public async Task<IActionResult> GetLatestPrice([FromQuery] string pair = "XRPUSD")
        {
            var price = await _kraken.GetLatestPriceAsync(pair);
            if (price > 0)
                return Ok(new { pair, price });

            return Ok(new { pair, price = (decimal?)null, error = "Failed to fetch price" });
        }

        // ✅ Get Session Profit
        [HttpGet("session-profit")]
        public IActionResult GetSessionProfit()
        {
            var profit = _tradingService.GetSessionProfit();
            return Ok(new { sessionProfit = profit });
        }
    }

    // ✅ Strongly typed request model
    public class GridBotRequest
    {
        public decimal Lower { get; set; }
        public decimal Upper { get; set; }
        public int Grids { get; set; }
        public decimal Investment { get; set; }
    }
}
