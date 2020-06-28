using System;
using System.Collections.Generic;
using System.Text;

namespace GapTraderCore.Trades
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
