using System;

namespace GapTraderCore
{
    public struct StrategyTestFilters
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        public StrategyTestFilters(DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime)
        {
            StartDate = startDate;
            EndDate = endDate;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}