using BitcoinTracker.Models;
using BitcoinTracker.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BitcoinTracker.ViewModels
{
    // The ViewModel for the main view. Handles data fetching, analysis, exposes and properties for data binding in the WPF UI.
    public class MainViewModel : INotifyPropertyChanged
    {
        // The service responsible for communicating with the CoinGecko API.
        private readonly BitcoinService _service = new BitcoinService(new ApiClient());

        // Initializes a new instance of the MainViewModel class and sets default start and end dates (last 30 days)
        public MainViewModel()
        {
            EndDate = DateTime.UtcNow.Date;
            StartDate = EndDate.AddDays(-30); // default to last 30 days
        }

        // --- Properties bound to the UI ---

        private string _bearishDays;
        
        // Gets or sets a text summary of the longest bearish streak (downtrend days).
        public string BearishDays
        {
            get => _bearishDays;
            set { _bearishDays = value; OnPropertyChanged(); }
        }

        private string _highestVolumeInfo;

        // Gets or sets a formatted string containing the date and value of the highest trading volume during the selected period.
        public string HighestVolumeInfo
        {
            get => _highestVolumeInfo;
            set { _highestVolumeInfo = value; OnPropertyChanged(); }
        }

        private string _bestTradeInfo;

        // Gets or sets a text summary describing the best buy-sell pair and profit.
        public string BestTradeInfo
        {
            get => _bestTradeInfo;
            set { _bestTradeInfo = value; OnPropertyChanged(); }
        }

        private string _statusMessage;
        
        // Gets or sets a message for UI feedback (e.g., loading, success, or error).
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        // Start date for filtering the Bitcoin price data.
        public DateTime StartDate { get; set; }

        // End date for filtering the Bitcoin price data.
        public DateTime EndDate { get; set; }

        // --- INotifyPropertyChanged implementation ---

        public event PropertyChangedEventHandler PropertyChanged;

        // Triggers the PropertyChanged event for UI updates when a property value changes.

        // The property name (auto-filled by CallerMemberName).
        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        // Fetches Bitcoin data, performs trend and volume analysis, and updates bound UI properties.
        // A list of DailySummary containing analyzed Bitcoin price and volume data.
        public async Task<List<DailySummary>> AnalyzeAndGetDataAsync()
        {
            try
            {
                // --- Fetch data ---
                StatusMessage = "Fetching data from CoinGecko...";
                var data = await _service.GetDailyBitcoinPricesAsync();

                if (data == null || data.Count == 0)
                {
                    StatusMessage = "No data found for the selected range.";
                    return null;
                }

                // Filter data based on selected dates
                data = data.Where(d => d.Date >= StartDate && d.Date <= EndDate).ToList();

                // A) Find the longest bearish streak
                int bearish = Analyzer.GetLongestBearishStreak(data.Select(d => new DailyData { Date = d.Date, Price = (decimal)d.Price }).ToList());
                BearishDays = $"Longest bearish streak: {bearish} days";

                // B) Find the highest volume day
                var (volDate, volValue) = Analyzer.GetHighestVolumeDay(data.Select(d => new DailyData { Date = d.Date, Volume = (decimal)d.Volume, Price = (decimal)d.Price }).ToList());
                HighestVolumeInfo = $"Highest volume: {volValue:N0} € on {volDate:yyyy-MM-dd}";

                // C) Identify the most profitable buy-sell pair 
                var (buy, sell, profit) = Analyzer.GetBestBuySellPair(data.Select(d => new DailyData { Date = d.Date, Price = (decimal)d.Price }).ToList());

                // Display results or fallback message if no profitable trade exists
                BestTradeInfo = (buy == null)
                    ? "No profitable trade in this range."
                    : $"Best bitcoin trade: Buy {buy:yyyy-MM-dd}, Sell {sell:yyyy-MM-dd}, Profit {profit:F2} €";

                // --- Final status update ---
                StatusMessage = "Analysis completed successfully!";
                return data;
            }
            catch (Exception ex)
            {
                // Catch any unexpected runtime or network errors and update UI message
                StatusMessage = $"Error: {ex.Message}";
                return null;
            }
        }
    }

}
