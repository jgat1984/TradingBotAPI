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

        [HttpPost("start-gridbot")]
        public IActionResult StartGridBot([FromBody] GridBotRequest request)
        {
            _tradingService.StartGridBot(request.Lower, request.Upper, request.Grids, request.Investment);
            return Ok(new { message = "Grid Bot started", request.Lower, request.Upper, request.Grids, request.Investment });
        }

        [HttpPost("stop-gridbot")]
        public IActionResult StopGridBot()
        {
            _tradingService.StopGridBot();
            return Ok(new { message = "Grid Bot stopped" });
        }

        [HttpGet("trades")]
        public ActionResult<List<TradeRecord>> GetAllTrades()
        {
            var trades = _tradeRepo.GetLast100Trades();
            return Ok(trades);
        }

        [HttpGet("get-latest-price")]
        public async Task<IActionResult> GetLatestPrice([FromQuery] string pair = "XRPUSD")
        {
            var price = await _kraken.GetLatestPriceAsync(pair);
            if (price > 0)
                return Ok(new { pair, price });

            return Ok(new { pair, price = (decimal?)null, error = "Failed to fetch price" });
        }

        // ✅ Added session profit endpoint
        [HttpGet("session-profit")]
        public IActionResult GetSessionProfit()
        {
            var profit = _tradingService.GetSessionProfit();
            return Ok(new { sessionProfit = profit });
        }
    }

    public class GridBotRequest
    {
        public decimal Lower { get; set; }
        public decimal Upper { get; set; }
        public int Grids { get; set; }
        public decimal Investment { get; set; }
    }
}
