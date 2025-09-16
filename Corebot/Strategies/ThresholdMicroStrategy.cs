using System;
using System.Collections.Generic;

namespace TradingBotAPI.CoreBot.Strategies
{
    /// <summary>
    /// Micro threshold strategy:
    /// - Trades on tiny 0.0001 moves above/below a reference price
    /// - Requires 3 ticks in same direction (momentum confirmation)
    /// - Enforces 5s cooldown between trades
    /// </summary>
    public class ThresholdMicroStrategy : IStrategy
    {
        private const decimal MicroStep = 0.0001m;      // trade threshold
        private const int MomentumWindow = 3;           // confirm ticks
        private const int MinSecondsBetweenTrades = 5;  // cooldown

        private decimal _referencePrice = 0m;
        private bool _initialized = false;
        private readonly Queue<decimal> _recentPrices = new Queue<decimal>();
        private DateTime _lastTradeTime = DateTime.MinValue;

        public string Name => "ThresholdMicro";
        public string Description => "Micro threshold strategy with momentum confirmation and cooldown.";

        public string GenerateSignal(decimal price)
        {
            if (!_initialized)
            {
                _referencePrice = price;
                _initialized = true;
                return "HOLD";
            }

            // Add to momentum queue
            _recentPrices.Enqueue(price);
            if (_recentPrices.Count > MomentumWindow)
                _recentPrices.Dequeue();

            // Respect cooldown
            if ((DateTime.UtcNow - _lastTradeTime).TotalSeconds < MinSecondsBetweenTrades)
                return "HOLD";

            // Momentum check
            bool upward = true, downward = true;
            decimal[] prices = _recentPrices.ToArray();
            for (int i = 1; i < prices.Length; i++)
            {
                if (prices[i] <= prices[i - 1]) upward = false;
                if (prices[i] >= prices[i - 1]) downward = false;
            }

            // Trading logic with micro threshold
            if (price < _referencePrice - MicroStep && upward)
            {
                _referencePrice = price; // reset anchor
                _lastTradeTime = DateTime.UtcNow;
                return "BUY";
            }

            if (price > _referencePrice + MicroStep && downward)
            {
                _referencePrice = price; // reset anchor
                _lastTradeTime = DateTime.UtcNow;
                return "SELL";
            }

            return "HOLD";
        }
    }
}
