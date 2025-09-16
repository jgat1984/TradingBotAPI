using System;

namespace TradingBotAPI.CoreBot.Strategies
{
    public class MomentumStrategy : IStrategy
    {
        private decimal _lastPrice = 0m;

        public string Name => "Momentum";
        public string Description => "Buys if price is rising, sells if price is falling.";

        public string GenerateSignal(decimal price)
        {
            if (_lastPrice == 0m)
            {
                _lastPrice = price;
                return "HOLD";
            }

            string signal = "HOLD";
            if (price > _lastPrice) signal = "BUY";
            else if (price < _lastPrice) signal = "SELL";

            _lastPrice = price;
            return signal;
        }
    }
}
