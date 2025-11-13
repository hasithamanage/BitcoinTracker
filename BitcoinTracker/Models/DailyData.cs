namespace BitcoinTracker.Models
{

    // The DailyData class represents a single day's cryptocurrency market data.
    public class DailyData
    {
        public DateTime Date { get; set; }     // Date of the data record in UTC (time part is 00:00:00)
        public decimal Price { get; set; }     // Closing price of cripto currency for this day (in EUR)
        public decimal MarketCap { get; set; } // Market capitalization value for this day (in EUR)
        public decimal Volume { get; set; }    // Total trading volume for this day (in EUR)

    }
}
