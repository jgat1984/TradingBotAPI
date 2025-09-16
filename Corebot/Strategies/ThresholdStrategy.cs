using System;

namespace TradingBotAPI.CoreBot.Strategies
{
    public class ThresholdStrategy : IStrategy
    {
        private readonly decimal _buyThreshold;
        private readonly decimal _profitTarget;
        private decimal _lastBuyPrice = 0m;
        private bool _holding = false;

        public string Name => "Threshold";
        public string Description => "Buys if price drops below a threshold and sells after profit target.";

        public ThresholdStrategy(decimal buyThreshold = 27000m, decimal profitTarget = 0.05m)
        {
            _buyThreshold = buyThreshold;
            _profitTarget = profitTarget;
        }

        public string GenerateSignal(decimal price)
        {
            if (!_holding && price < _buyThreshold)
            {
                _holding = true;
                _lastBuyPrice = price;
                return "BUY";
            }

            if (_holding && price > _lastBuyPrice * (1 + _profitTarget))
            {
                _holding = false;
                return "SELL";
            }

            return "HOLD";
        }
    }
}
