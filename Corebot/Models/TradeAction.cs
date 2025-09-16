namespace TradingBotAPI.CoreBot.Models
{
    /// <summary>
    /// Represents the type of trade action a strategy can generate.
    /// </summary>
    public enum TradeAction
    {
        Buy,   // Execute a buy order
        Sell,  // Execute a sell order
        Hold   // No action, hold position
    }
}
