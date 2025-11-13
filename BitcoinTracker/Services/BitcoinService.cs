using BitcoinTracker.Services;
using Newtonsoft.Json.Linq;

namespace BitcoinTracker.Models
{
    // Provides functionality for retrieving and processing Bitcoin price and volume data from the CoinGecko API using the ApiClient.cs
    public class BitcoinService
    {
        // Instance of the ApiClient used to handle HTTP communication
        private readonly ApiClient _apiClient;

        // Initializes a new instance of the BitcoinService class

        // An ApiClient object responsible for making HTTP requests.
        public BitcoinService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }


        // Asynchronously retrieves Bitcoin's daily price and volume data for the last 365 days.
        // The data is fetched from the CoinGecko API and converted into a list of DailySummary objects containing date, price, and volume for each day.
        public async Task<List<DailySummary>> GetDailyBitcoinPricesAsync()
        {
            try
            {
                // CoinGecko's free API restricts data queries to a 365-day range.
                // Define UNIX timestamps for 'from' (365 days ago) and 'to' (current time).
                var to = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var from = DateTimeOffset.UtcNow.AddDays(-365).ToUnixTimeSeconds();

                // Build the endpoint URL for CoinGecko API.
                // The vs_currency is set to EUR to match European market data.
                string endpoint =
                    $"coins/bitcoin/market_chart/range?vs_currency=eur&from={from}&to={to}&x_cg_demo_api_key=CG-2uzQyVNvqpNtTpJi15C8bgUd";

                // Send request and get raw JSON string response.
                string json = await _apiClient.GetAsync(endpoint);

                // Parse the JSON data into a JObject for structured access.
                var data = JObject.Parse(json);

                // Extract the "prices" and "total_volumes" arrays from the JSON structure.
                var prices = data["prices"];
                var volumes = data["total_volumes"];

                // Prepare the list to hold processed daily summaries.
                var summaries = new List<DailySummary>();

                // Iterate through each data point and map JSON values into strongly-typed objects.
                for (int i = 0; i < prices.Count(); i++)
                {
                    // Convert the UNIX timestamp (milliseconds) into a DateTime object.
                    var timestamp = (long)prices[i][0];
                    var date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime.Date;

                    // Extract the price and volume values from the arrays.
                    var price = (double)prices[i][1];
                    var volume = (double)volumes[i][1];

                    // Create a new summary entry for the current day.
                    summaries.Add(new DailySummary
                    {
                        Date = date,
                        Price = price,
                        Volume = volume
                    });
                }

                // Return the completed list of daily summaries.
                return summaries;
            }
            catch (Exception ex)
            {
                // Wrap and rethrow any exception with additional context information.
                throw new Exception($"Error fetching Bitcoin data: {ex.Message}", ex);
            }
        }
    }
}
