using System;
using TradingBotAPI.CoreBot;

namespace TradingBotAPI.CoreBot.Strategies
{
    /// <summary>
    /// Mean Reversion Strategy:
    /// Buys when price is significantly below the running average,
    /// sells when price is significantly above.
    /// </summary>
    public class MeanReversionStrategy : IStrategy
    {
        private decimal _meanPrice = 0m;
        private int _count = 0;

        // Deviation threshold (2% default)
        private readonly decimal _deviationThreshold;

        public MeanReversionStrategy(decimal deviationThreshold = 0.02m)
        {
            _deviationThreshold = deviationThreshold;
        }

        // === IStrategy members ===
        public string Name => "MeanReversion";

        public string Description =>
            $"Buys when price is {(_deviationThreshold * 100):F1}% below average, sells when above.";

        public string GenerateSignal(decimal price)
        {
            // Incrementally update running mean
            _count++;
            _meanPrice = ((_meanPrice * (_count - 1)) + price) / _count;

            // Buy if price is below mean by threshold
            if (price < _meanPrice * (1 - _deviationThreshold))
                return "BUY";

            // Sell if price is above mean by threshold
            if (price > _meanPrice * (1 + _deviationThreshold))
                return "SELL";

            return "HOLD";
        }
    }
}
