using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradingBotAPI.CoreBot;
using TradingBotAPI.CoreBot.Models;
using TradingBotAPI.Services;

namespace TradingBotAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   // Base route: /api/trading
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

        // --------------------------
        // START GRID BOT (POST)
        // --------------------------
        [HttpPost("start-gridbot")]
        public IActionResult StartGridBot([FromBody] GridBotRequest request)
        {
            if (request == null)
                return BadRequest(new { error = "Request body is missing." });

            if (request.Grids <= 0 || request.Lower >= request.Upper)
                return BadRequest(new { error = "Invalid bot parameters." });

            _tradingService.StartGridBot(
                request.Lower,
                request.Upper,
                request.Grids,
                request.Investment
            );

            return Ok(new
            {
                message = "Grid bot started",
                lower = request.Lower,
                upper = request.Upper,
                grids = request.Grids,
                investment = request.Investment
            });
        }

        // --------------------------
        // START GRID BOT (GET - for browser testing)
        // Example:
        // https://tradingbotapi.onrender.com/api/trading/start-gridbot?lower=1&upper=10&grids=4&investment=1000
        // --------------------------
        [HttpGet("start-gridbot")]
        public IActionResult StartGridBotQuery(
            [FromQuery] decimal lower,
            [FromQuery] decimal upper,
            [FromQuery] int grids,
            [FromQuery] decimal investment)
        {
            if (grids <= 0 || lower >= upper)
                return BadRequest(new { error = "Invalid parameters." });

            _tradingService.StartGridBot(lower, upper, grids, investment);

            return Ok(new
            {
                message = "Grid bot started (via GET)",
                lower,
                upper,
                grids,
                investment
            });
        }

        // --------------------------
        // STOP GRID BOT (POST)
        // --------------------------
        [HttpPost("stop-gridbot")]
        public IActionResult StopGridBot()
        {
            _tradingService.StopGridBot();
            return Ok(new { message = "Grid bot stopped" });
        }

        // --------------------------
        // STOP GRID BOT (GET - for browser testing)
        // Example:
        // https://tradingbotapi.onrender.com/api/trading/stop-gridbot
        // --------------------------
        [HttpGet("stop-gridbot")]
        public IActionResult StopGridBotQuery()
        {
            _tradingService.StopGridBot();
            return Ok(new { message = "Grid bot stopped (via GET)" });
        }

        // --------------------------
        // GET ALL TRADES
        // --------------------------
        [HttpGet("trades")]
        public IActionResult GetTrades()
        {
            var trades = _tradeRepo.GetAllTrades();
            return Ok(trades);
        }

        // --------------------------
        // GET LATEST PRICE
        // --------------------------
        [HttpGet("get-latest-price")]
        public async Task<IActionResult> GetLatestPrice(string pair = "XRPUSD")
        {
            var price = await _kraken.GetLatestPriceAsync(pair);
            return Ok(new { Pair = pair, Price = price });
        }

        // --------------------------
        // GET SESSION PROFIT
        // --------------------------
        [HttpGet("session-profit")]
        public IActionResult GetSessionProfit()
        {
            var profit = _tradingService.GetSessionProfit();
            return Ok(new { sessionProfit = profit });
        }

        // --------------------------
        // PREVIEW GRID LEVELS
        // --------------------------
        [HttpGet("preview-gridbot")]
        public IActionResult PreviewGridLevels(decimal lower, decimal upper, int grids)
        {
            if (grids <= 0 || lower >= upper)
                return BadRequest(new { error = "Invalid parameters" });

            decimal stepSize = (upper - lower) / grids;
            var levels = new List<decimal>();

            for (int i = 0; i <= grids; i++)
            {
                levels.Add(lower + (stepSize * i));
            }

            return Ok(new
            {
                lower,
                upper,
                grids,
                levels
            });
        }
    }
}
