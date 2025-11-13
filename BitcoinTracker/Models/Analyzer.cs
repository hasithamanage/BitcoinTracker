namespace BitcoinTracker.Models
{

    // Analyzer class provides methods to analyze cryptocurrency market data.
    public static class Analyzer
    {
        // A) Longest bearish streak (price decreases compared to previous day)
        public static int GetLongestBearishStreak(List<DailyData> data)
        {
            // Return 0 if data is null or too short to compare
            if (data == null || data.Count < 2) return 0;

            int longest = 0;
            int current = 0;

            // Loop through list comparing current day's price to previous day's price
            for (int i = 1; i < data.Count; i++)
            {
                // If today's price is lower, continue streak
                if (data[i].Price < data[i - 1].Price)
                {
                    current++;
                    // Update longest streak if current exceeds previous record
                    if (current > longest)
                        longest = current;
                }
                else
                {
                    // Reset streak when price increases or stays the same
                    current = 0;
                }
            }

            return longest;
        }

        // B) Finds the date with the highest trading volume.
        // Returns a tuple (Date, Volume).
        public static (DateTime date, decimal volume) GetHighestVolumeDay(List<DailyData> data)
        {
            // Handle empty or null list
            if (data == null || data.Count == 0) return (DateTime.MinValue, 0m);

            // Order by descending volume and take the first (max)
            var max = data.OrderByDescending(d => d.Volume).First();
            return (max.Date, max.Volume);
        }

        // C) Finds the best days to buy and sell for the maximum possible profit.
        public static (DateTime? buyDate, DateTime? sellDate, decimal profit) GetBestBuySellPair(List<DailyData> data)
        {
            // At least two data points needed for comparison
            if (data == null || data.Count < 2)
                return (null, null, 0m);

            decimal minPrice = data[0].Price;
            DateTime minDate = data[0].Date;

            decimal bestProfit = 0m;
            DateTime? bestBuy = null;
            DateTime? bestSell = null;

            // Loop through all days after the first
            foreach (var day in data.Skip(1))
            {
                // Calculate profit if bought at minPrice and sold today
                var potentialProfit = day.Price - minPrice;
                if (potentialProfit > bestProfit)
                {
                    bestProfit = potentialProfit;
                    bestBuy = minDate;
                    bestSell = day.Date;
                }

                // Update min price if current day's price is lower
                if (day.Price < minPrice)
                {
                    minPrice = day.Price;
                    minDate = day.Date;
                }
            }

            // If no positive profit found, return zero result
            if (bestProfit <= 0)
                return (null, null, 0m); // all decreasing

            return (bestBuy, bestSell, bestProfit);
        }
    }
}
