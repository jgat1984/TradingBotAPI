using System;
using System.Collections.Generic;
using System.Linq;

namespace TradingBotAPI.CoreBot.Strategies
{
    /// <summary>
    /// Micro mean reversion strategy for crypto.
    /// Buys when price < mean - threshold, sells when price > mean + threshold.
    /// Designed for XRP micro-movements in the 10,000ths range.
    /// </summary>
    public class MeanReversionMicroStrategy : IStrategy
    {
        private readonly Queue<decimal> _priceWindow;
        private readonly int _windowSize;
        private readonly decimal _threshold;

        public string Name => "MeanReversionMicro";

        public string Description =>
            "A simplified micro mean reversion strategy. " +
            "Buys if price is below the moving average by more than the threshold, " +
            "sells if above by more than the threshold, otherwise holds.";

        public MeanReversionMicroStrategy(int windowSize = 5, decimal threshold = 0.0001m)
        {
            _windowSize = windowSize;
            _threshold = threshold;
            _priceWindow = new Queue<decimal>();
        }

        public string GenerateSignal(decimal currentPrice)
        {
            _priceWindow.Enqueue(currentPrice);

            if (_priceWindow.Count > _windowSize)
                _priceWindow.Dequeue();

            if (_priceWindow.Count < _windowSize)
                return $"HOLD @ {currentPrice:F6} (collecting data)";

            decimal mean = _priceWindow.Average();

            if (currentPrice > mean + _threshold)
                return $"SELL @ {currentPrice:F6} — MicroReversion trigger met";

            if (currentPrice < mean - _threshold)
                return $"BUY @ {currentPrice:F6} — MicroReversion trigger met";

            return $"HOLD @ {currentPrice:F6} (no trigger)";
        }

        public Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "WindowSize", _windowSize },
                { "Threshold", _threshold }
            };
        }
    }
}
