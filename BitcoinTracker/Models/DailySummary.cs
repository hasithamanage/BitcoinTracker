namespace BitcoinTracker.Models
{

    // Represents a simplified summary of daily Bitcoin data, used for analysis and charting purposes.
    public class DailySummary
    {
        public DateTime Date { get; set; }  // Date of the record (UTC)
        public double Price { get; set; }    // Closing price of Bitcoin for the day (in EUR)
        public double Volume { get; set; }  // Trading volume for the day (in EUR)
    }
}
