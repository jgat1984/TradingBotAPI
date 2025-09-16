using System;

namespace TradingBotAPI.CoreBot.Strategies
{
    public class RandomStrategy : IStrategy
    {
        private readonly Random _rand = new Random();

        public string Name => "Random";
        public string Description => "Generates random BUY/SELL/HOLD signals (for testing).";

        public string GenerateSignal(decimal price)
        {
            int roll = _rand.Next(3);
            return roll switch
            {
                0 => "BUY",
                1 => "SELL",
                _ => "HOLD",
            };
        }
    }
}
