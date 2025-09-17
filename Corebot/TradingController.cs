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

        // --------------------------
        // START GRID BOT
        // --------------------------
        [HttpPost("start-gridbot")]
        public IActionResult StartGridBot([FromBody] GridBotRequest request)
        {
            if (request == null || request.Grids <= 0 || request.Lower >= request.Upper)
                return BadRequest("Invalid bot parameters");

            _tradingService.StartGridBot(request.Lower, request.Upper, request.Grids, request.Investment);
            return Ok(new { message = "Grid bot started", request });
        }

        // --------------------------
        // STOP GRID BOT
        // --------------------------
        [HttpPost("stop-gridbot")]
        public IActionResult StopGridBot()
        {
            _tradingService.StopGridBot();
            return Ok(new { message = "Grid bot stopped" });
        }

        // --------------------------
        // GET TRADES
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
        // GET GRID LEVELS (new)
        // --------------------------
        [HttpGet("grid-levels")]
        public IActionResult GetGridLevels(decimal lower, decimal upper, int grids)
        {
            if (grids <= 0 || lower >= upper)
            {
                return BadRequest("Invalid parameters");
            }

            decimal stepSize = (upper - lower) / grids;
            var levels = new List<decimal>();

            for (int i = 0; i <= grids; i++)
            {
                levels.Add(lower + (stepSize * i));
            }

            return Ok(new
            {
                Lower = lower,
                Upper = upper,
                Grids = grids,
                Levels = levels
            });
        }
    }
}
