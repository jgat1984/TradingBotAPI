using System;

namespace TradingBotAPI.CoreBot.Models
{
    public class TradeRecord
    {
        public int Id { get; set; }            // Auto-increment in DB
        public string Symbol { get; set; }
        public string Action { get; set; }     // Buy / Sell
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Investment { get; set; } // Cash left after trade
        public decimal Profit { get; set; }     // Cumulative session profit
        public string Strategy { get; set; }
        public DateTime Timestamp { get; set; } // UTC timestamp
        public decimal LowerBound { get; set; }
        public decimal UpperBound { get; set; }
    }
}
