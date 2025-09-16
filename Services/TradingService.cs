using System;
using System.Threading;
using TradingBotAPI.CoreBot;
using TradingBotAPI.CoreBot.Models;

namespace TradingBotAPI.Services
{
    public class TradingService
    {
        private readonly TradeRepository _tradeRepo;
        private Timer _gridBotTimer;

        // Bot state
        private decimal _remainingInvestment;
        private decimal _amount = 0m;               // current XRP holdings
        private decimal _lastBuyPrice = 0m;
        private decimal _sessionProfit = 0m;
        private int _lastZone = -1;

        private const decimal MinInvestmentThreshold = 0.00001m;

        public TradingService(TradeRepository tradeRepo)
        {
            _tradeRepo = tradeRepo;
        }

        public void StartGridBot(decimal lower, decimal upper, int grids, decimal investment)
        {
            StopGridBot();
            _remainingInvestment = investment;
            _sessionProfit = 0;
            _amount = 0;
            _lastBuyPrice = 0;
            _lastZone = -1;

            _gridBotTimer = new Timer(async _ =>
            {
                try
                {
                    var kraken = new KrakenClient();
                    decimal price = await kraken.GetLatestPriceAsync(SelectedCoinStore.ActiveCoin ?? "XRPUSD");
                    RunGridBot(lower, upper, grids, price);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GRID BOT ERROR] {ex.Message}");
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        public void StopGridBot()
        {
            _gridBotTimer?.Dispose();
            _gridBotTimer = null;
            _remainingInvestment = 0;
            _amount = 0;
            _lastBuyPrice = 0;
            _lastZone = -1;
            _sessionProfit = 0;

            Console.WriteLine("[GRID BOT] Bot stopped.");
        }

        private void RunGridBot(decimal lower, decimal upper, int grids, decimal currentPrice)
        {
            if (grids <= 0 || lower >= upper)
            {
                Console.WriteLine("[GRID BOT] Invalid params. Stopping bot...");
                StopGridBot();
                return;
            }

            TradeAction action = TradeAction.Hold;
            int currentZone = GetZoneIndex(currentPrice, lower, upper, grids);

            // SELL: moved up into higher zone while holding coins
            if (_lastZone != -1 && currentZone > _lastZone && _amount > 0)
            {
                decimal revenue = _amount * currentPrice;
                decimal profit = (currentPrice - _lastBuyPrice) * _amount;

                _sessionProfit += profit;
                _remainingInvestment += revenue;
                _amount = 0;

                action = TradeAction.Sell;
            }
            // BUY: moved down into lower zone while holding cash
            else if (_lastZone != -1 && currentZone < _lastZone && _remainingInvestment > MinInvestmentThreshold)
            {
                _amount = _remainingInvestment / currentPrice;
                _lastBuyPrice = currentPrice;
                _remainingInvestment = 0;

                action = TradeAction.Buy;
            }

            // ✅ Always log a trade (Buy, Sell, or Hold)
            var trade = new TradeRecord
            {
                Symbol = SelectedCoinStore.ActiveCoin ?? "XRPUSD",
                Strategy = "GridBot",
                Action = action.ToString(),
                Price = currentPrice,
                Quantity = _amount,
                Investment = _remainingInvestment,
                Profit = _sessionProfit,
                LowerBound = lower,
                UpperBound = upper,
                Timestamp = DateTime.UtcNow
            };

            _tradeRepo.AddTrade(trade);
            _lastZone = currentZone;

            Console.WriteLine($"[GRID BOT] {action} @ {currentPrice:F6} | Qty: {_amount:F6} | Cash: {_remainingInvestment:F2} | Profit: {_sessionProfit:F2}");
        }

        private int GetZoneIndex(decimal price, decimal lower, decimal upper, int grids)
        {
            if (price <= lower) return 0;
            if (price >= upper) return grids;

            decimal stepSize = (upper - lower) / grids;
            return (int)((price - lower) / stepSize);
        }

        public decimal GetSessionProfit() => _sessionProfit;
    }
}
