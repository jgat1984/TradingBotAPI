using System;
using System.Collections.Generic;

namespace TradingBotAPI.CoreBot.Strategies
{
    public class MovingAverageCrossoverStrategy : IStrategy
    {
        private readonly int _shortPeriod = 3;
        private readonly int _longPeriod = 7;
        private readonly Queue<decimal> _prices = new Queue<decimal>();

        public string Name => "MovingAverageCrossover";
        public string Description => "Buys when short MA crosses above long MA, sells on opposite.";

        public string GenerateSignal(decimal price)
        {
            _prices.Enqueue(price);
            if (_prices.Count > _longPeriod)
                _prices.Dequeue();

            if (_prices.Count < _longPeriod)
                return "HOLD";

            decimal[] arr = _prices.ToArray();
            decimal shortAvg = 0, longAvg = 0;
            for (int i = arr.Length - _shortPeriod; i < arr.Length; i++)
                shortAvg += arr[i];
            shortAvg /= _shortPeriod;

            foreach (var p in arr) longAvg += p;
            longAvg /= arr.Length;

            if (shortAvg > longAvg) return "BUY";
            if (shortAvg < longAvg) return "SELL";

            return "HOLD";
        }
    }
}
