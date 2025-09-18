namespace TradingBotAPI.CoreBot.Models
{
    /// <summary>
    /// Represents the request body for starting a Grid Bot.
    /// </summary>
    public class GridBotRequest
    {
        /// <summary>
        /// Lower price bound of the grid.
        /// </summary>
        public decimal Lower { get; set; }

        /// <summary>
        /// Upper price bound of the grid.
        /// </summary>
        public decimal Upper { get; set; }

        /// <summary>
        /// Number of grid levels.
        /// </summary>
        public int Grids { get; set; }

        /// <summary>
        /// Total investment amount allocated to the bot.
        /// </summary>
        public decimal Investment { get; set; }
    }
}
