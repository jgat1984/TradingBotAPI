using System.Data.SQLite;

namespace TradingBotAPI.CoreBot
{
    public static class DatabaseHelper
    {
        private static readonly string _dbPath = "tradingbot.db";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection($"Data Source={_dbPath};Version=3;");
        }

        public static void EnsureSchema()
        {
            using (var conn = GetConnection())
            {
                conn.Open();

                string sql = @"
                CREATE TABLE IF NOT EXISTS Crypto (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Symbol TEXT,
                    Action TEXT,
                    Price REAL,
                    Quantity REAL,
                    Investment REAL,
                    Profit REAL,
                    Strategy TEXT,
                    Timestamp TEXT,
                    LowerBound REAL,
                    UpperBound REAL
                );";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
