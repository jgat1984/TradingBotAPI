using System;
using TradingBotAPI.CoreBot.Models;
using TradingBotAPI.CoreBot.Strategies;

namespace TradingBotAPI.CoreBot
{
    public class TradeBot
    {
        private readonly TradeRepository _repo;
        private readonly IStrategy _strategy;

        public TradeBot(TradeRepository repo, IStrategy strategy)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        public void SaveTrade(string symbol, decimal price, decimal qty, TradeAction action)
        {
            try
            {
                var trade = new TradeRecord
                {
                    Symbol = symbol,
                    Price = price,
                    Quantity = qty,
                    Timestamp = DateTime.UtcNow,
                    Action = action.ToString(),    // ✅ convert enum → string here
                    Strategy = _strategy.Name
                };

                _repo.AddTrade(trade);
                Console.WriteLine($"[TRADE SAVED] {action} {qty} {symbol} @ {price} using {_strategy.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DB ERROR] {ex.Message}");
            }
        }
    }
}
