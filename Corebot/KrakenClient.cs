using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TradingBotAPI.CoreBot
{
    public class KrakenClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public KrakenClient()
        {
            // No BaseAddress — we pass full URLs
        }

        /// <summary>
        /// Gets the latest trade price for the given pair (e.g., BTCUSD, ETHUSD, XRPUSD).
        /// </summary>
        public async Task<decimal> GetLatestPriceAsync(string pair = "BTCUSD")
        {
            try
            {
                string url = $"https://api.kraken.com/0/public/Ticker?pair={pair}";
                var response = await _httpClient.GetStringAsync(url);

                var json = JObject.Parse(response);
                var key = GetResultKey(pair);

                string lastTradePrice = (string)json["result"]?[key]?["c"]?[0];
                if (decimal.TryParse(lastTradePrice, out var price))
                {
                    return price;
                }

                return 0m;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to fetch price from Kraken: {ex.Message}");
                return 0m;
            }
        }

        /// <summary>
        /// Gets OHLC candles (open, high, low, close) for the given pair and interval.
        /// </summary>
        public async Task<JArray> GetOhlcAsync(string pair = "BTCUSD", int interval = 1)
        {
            try
            {
                string url = $"https://api.kraken.com/0/public/OHLC?pair={pair}&interval={interval}";
                var response = await _httpClient.GetStringAsync(url);

                var json = JObject.Parse(response);
                var key = GetResultKey(pair);
                return (JArray)json["result"]?[key];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to fetch OHLC from Kraken: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Gets the most recent trades for the given pair.
        /// </summary>
        public async Task<JArray> GetRecentTradesAsync(string pair = "BTCUSD")
        {
            try
            {
                string url = $"https://api.kraken.com/0/public/Trades?pair={pair}";
                var response = await _httpClient.GetStringAsync(url);

                var json = JObject.Parse(response);
                var key = GetResultKey(pair);
                return (JArray)json["result"]?[key];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to fetch trades from Kraken: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Returns the last N trade prices as a decimal list.
        /// Useful for feeding strategies with recent market data.
        /// </summary>
        public async Task<List<decimal>> GetRecentPricesAsync(string pair = "BTCUSD", int count = 10)
        {
            var trades = await GetRecentTradesAsync(pair);
            var prices = new List<decimal>();

            if (trades != null)
            {
                foreach (var trade in trades)
                {
                    if (decimal.TryParse((string)trade[0], out decimal price))
                    {
                        prices.Add(price);
                    }

                    if (prices.Count >= count)
                        break;
                }
            }

            return prices;
        }

        /// <summary>
        /// Maps friendly symbols to Kraken API keys.
        /// </summary>
        private string GetResultKey(string pair)
        {
            switch (pair.ToUpper())
            {
                case "BTCUSD": return "XXBTZUSD";
                case "ETHUSD": return "XETHZUSD";
                case "XRPUSD": return "XXRPZUSD";
                default: return pair.ToUpper(); // fallback, let Kraken handle other pairs
            }
        }
    }
}
