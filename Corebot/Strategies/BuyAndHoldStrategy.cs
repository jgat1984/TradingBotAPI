using System;

namespace TradingBotAPI.CoreBot.Strategies
{
    public class BuyAndHoldStrategy : IStrategy
    {
        private bool _hasBought = false;

        public string Name => "BuyAndHold";
        public string Description => "Buys once at the beginning and holds regardless of price.";

        public string GenerateSignal(decimal price)
        {
            if (!_hasBought)
            {
                _hasBought = true;
                return "BUY";
            }

            return "HOLD";
        }
    }
}
