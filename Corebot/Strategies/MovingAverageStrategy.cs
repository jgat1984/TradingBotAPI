using System;
using System.Collections.Generic;

namespace TradingBotAPI.CoreBot.Strategies
{
    public class MovingAverageStrategy : IStrategy
    {
        private readonly int _period = 5;
        private readonly Queue<decimal> _prices = new Queue<decimal>();

        public string Name => "MovingAverage";
        public string Description => "Trades when price is above or below a moving average.";

        public string GenerateSignal(decimal price)
        {
            _prices.Enqueue(price);
            if (_prices.Count > _period)
                _prices.Dequeue();

            if (_prices.Count < _period)
                return "HOLD";

            decimal avg = 0m;
            foreach (var p in _prices) avg += p;
            avg /= _prices.Count;

            if (price > avg) return "BUY";
            if (price < avg) return "SELL";

            return "HOLD";
        }
    }
}
