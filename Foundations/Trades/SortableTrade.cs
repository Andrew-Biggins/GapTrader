using System;

namespace TradingSharedCore.Trades
{
    public class SortableTrade
    {
        public DateTime CloseTime { get; }

        public double Profit { get; }

        public SortableTrade(DateTime closeTime, double profit)
        {
            CloseTime = closeTime;
            Profit = profit;
        }
    }
}
