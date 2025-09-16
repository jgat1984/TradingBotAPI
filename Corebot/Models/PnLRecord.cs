using System;

namespace TradingBotAPI.CoreBot.Models
{
    public class PnLRecord
    {
        public int Id { get; set; }               // Primary key
        public decimal ProfitLoss { get; set; }   // PnL value (+/-)
        public DateTime Timestamp { get; set; }   // Time it was logged
    }
}
