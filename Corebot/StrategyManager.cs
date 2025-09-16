using System;
using System.Collections.Generic;
using TradingBotAPI.CoreBot.Strategies;

namespace TradingBotAPI.CoreBot.Strategies
{
    public static class StrategyManager
    {
        // === Register all strategies with descriptions ===
        public static readonly Dictionary<int, (Func<IStrategy> Factory, string Description)> Strategies =
            new Dictionary<int, (Func<IStrategy> Factory, string Description)>
            {
                { 1, (() => new ThresholdStrategy(),
                      "Buys when price drops below 27k and sells after rising more than 5%.") },

                { 2, (() => new MovingAverageStrategy(),
                      "Trades when the short-term moving average crosses above or below the long-term moving average.") },

                { 3, (() => new MovingAverageCrossoverStrategy(),
                      "Uses short vs long moving average crossover signals.") },

                { 4, (() => new RsiStrategy(2, 50.2m, 49.8m),
                      "Uses a 3-period RSI with tight bounds (OB=52, OS=48) for rapid buy/sell signals.") },

                { 5, (() => new RsiStrategy(2, 50.2m, 49.8m),
                      "Uses a 5-period RSI with wider bounds (OB=65, OS=35) for more conservative trades.") },

                { 6, (() => new MomentumStrategy(),
                      "Buys if recent price momentum is positive and sells if momentum weakens.") },

                { 7, (() => new BuyAndHoldStrategy(),
                      "Buys once at the beginning and holds the position regardless of price.") },

                { 8, (() => new MeanReversionStrategy(),
                      "Buys when price deviates significantly from the mean, sells when it returns.") },

                { 9, (() => new RandomStrategy(),
                      "Generates random buy/sell signals (for testing only).") }
            };

        // === Helper for API: get descriptions as dictionary ===
        public static Dictionary<string, string> GetStrategyDescriptions()
        {
            var dict = new Dictionary<string, string>();
            foreach (var kvp in Strategies)
            {
                string name = kvp.Value.Factory().GetType().Name.Replace("Strategy", "");
                dict[name] = kvp.Value.Description;
            }
            return dict;
        }

        // === Helper for API: create a strategy by number ===
        public static IStrategy GetStrategy(int choice)
        {
            if (Strategies.ContainsKey(choice))
                return Strategies[choice].Factory();
            return new RsiStrategy(3, 52m, 48m); // default fallback
        }
    }

    /// <summary>
    /// Base interface for all trading strategies.
    /// </summary>
    public interface IStrategy
    {
        string Name { get; }
        string Description { get; }
        string GenerateSignal(decimal price); // BUY / SELL / HOLD
    }
}
