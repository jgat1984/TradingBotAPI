using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TradingBotAPI.CoreBot.Models;

namespace TradingBotAPI.CoreBot
{
    public class PnLRepository
    {
        // Save a Profit/Loss record
        public void AddPnL(decimal pnlValue)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = "INSERT INTO PnL (ProfitLoss) VALUES (@ProfitLoss);";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ProfitLoss", pnlValue);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Get all Profit/Loss records
        public List<PnLRecord> GetAllPnL()
        {
            var pnlList = new List<PnLRecord>();

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT Id, ProfitLoss, Timestamp FROM PnL;";

                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pnlList.Add(new PnLRecord
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            ProfitLoss = Convert.ToDecimal(reader["ProfitLoss"]),
                            Timestamp = Convert.ToDateTime(reader["Timestamp"])
                        });
                    }
                }
            }

            return pnlList;
        }

        // Get latest PnL record (optional helper)
        public PnLRecord? GetLatestPnL()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT Id, ProfitLoss, Timestamp FROM PnL ORDER BY Id DESC LIMIT 1;";

                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new PnLRecord
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            ProfitLoss = Convert.ToDecimal(reader["ProfitLoss"]),
                            Timestamp = Convert.ToDateTime(reader["Timestamp"])
                        };
                    }
                }
            }

            return null;
        }
    }
}
