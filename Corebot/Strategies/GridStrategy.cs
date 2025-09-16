using System;
using TradingBotAPI.CoreBot.Models;

namespace TradingBotAPI.CoreBot.Strategies
{
    /// <summary>
    /// Adaptive Medium Strategy
    /// ✅ Recalculates a new medium after every trade decision.
    /// - If price > medium → SELL and update medium = price
    /// - If price < medium → BUY and update medium = price
    /// - If price == medium → HOLD
    /// </summary>
    public class GridStrategy : IStrategy
    {
        public string Name => "GridBot";
        public string Description => "Adaptive: recalculates medium each time for buy/sell.";

        private decimal _medium;

        public GridStrategy(decimal lower, decimal upper, int grids)
        {
            // initial medium = midpoint of lower/upper
            _medium = (lower + upper) / 2;
        }

        public string GenerateSignal(decimal price)
        {
            if (price > _medium)
            {
                _medium = price; // update medium
                return "SELL";
            }
            else if (price < _medium)
            {
                _medium = price; // update medium
                return "BUY";
            }
            else
            {
                return "HOLD";
            }
        }
    }
}
