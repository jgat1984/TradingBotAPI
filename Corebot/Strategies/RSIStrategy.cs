using System;
using System.Collections.Generic;

namespace TradingBotAPI.CoreBot.Strategies
{
    public class RsiStrategy : IStrategy
    {
        private readonly int _period;
        private readonly decimal _overbought;
        private readonly decimal _oversold;
        private readonly Queue<decimal> _prices = new Queue<decimal>();

        public string Name => "RSI";
        public string Description => $"RSI Strategy (Period={_period}, OB={_overbought}, OS={_oversold})";

        public RsiStrategy(int period = 14, decimal overbought = 70m, decimal oversold = 30m)
        {
            _period = period;
            _overbought = overbought;
            _oversold = oversold;
        }

        public string GenerateSignal(decimal price)
        {
            _prices.Enqueue(price);
            if (_prices.Count > _period)
                _prices.Dequeue();

            if (_prices.Count < _period)
                return "HOLD";

            // stub RSI calculation
            if (price > _overbought) return "SELL";
            if (price < _oversold) return "BUY";

            return "HOLD";
        }
    }
}
