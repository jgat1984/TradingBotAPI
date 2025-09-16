using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TradingBotAPI.CoreBot.Models;

namespace TradingBotAPI.CoreBot
{
    public class TradeRepository
    {
        // Insert a trade record
        public void AddTrade(TradeRecord trade)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
                INSERT INTO Crypto 
                (Symbol, Action, Price, Quantity, Investment, Profit, Strategy, Timestamp, LowerBound, UpperBound)
                VALUES 
                (@Symbol, @Action, @Price, @Quantity, @Investment, @Profit, @Strategy, @Timestamp, @LowerBound, @UpperBound);";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Symbol", trade.Symbol);
                    cmd.Parameters.AddWithValue("@Action", trade.Action);
                    cmd.Parameters.AddWithValue("@Price", trade.Price);
                    cmd.Parameters.AddWithValue("@Quantity", trade.Quantity);
                    cmd.Parameters.AddWithValue("@Investment", trade.Investment);
                    cmd.Parameters.AddWithValue("@Profit", trade.Profit);
                    cmd.Parameters.AddWithValue("@Strategy", trade.Strategy);
                    cmd.Parameters.AddWithValue("@Timestamp", trade.Timestamp.ToString("o"));
                    cmd.Parameters.AddWithValue("@LowerBound", trade.LowerBound);
                    cmd.Parameters.AddWithValue("@UpperBound", trade.UpperBound);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Fetch the last 100 trades
        public List<TradeRecord> GetLast100Trades()
        {
            var trades = new List<TradeRecord>();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = @"SELECT Id, Symbol, Action, Price, Quantity, Investment, Profit, Strategy, Timestamp, LowerBound, UpperBound
                               FROM Crypto
                               ORDER BY Id DESC
                               LIMIT 100;";

                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        trades.Add(new TradeRecord
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Symbol = reader["Symbol"].ToString(),
                            Action = reader["Action"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]),
                            Quantity = Convert.ToDecimal(reader["Quantity"]),
                            Investment = Convert.ToDecimal(reader["Investment"]),
                            Profit = Convert.ToDecimal(reader["Profit"]),
                            Strategy = reader["Strategy"].ToString(),
                            Timestamp = DateTime.Parse(reader["Timestamp"].ToString()),
                            LowerBound = Convert.ToDecimal(reader["LowerBound"]),
                            UpperBound = Convert.ToDecimal(reader["UpperBound"])
                        });
                    }
                }
            }

            return trades;
        }
    }
}
