using System;
using GapAnalyser.ViewModels;

namespace GapAnalyser.VariableSelectors
{
    public sealed class DynamicTestVariableSelector : VariableSelector
    {
        public double MinWinProbability { get; set; } = 40;
        public double GapSizeTolerance { get; set; } = 50;

        public TradeType TradeType { get; set; } = TradeType.Both;

        public DateTime TestStartDate { get; set; }

        public DateTime DataStartDate { get; }

        public DateTime DataEndDate { get; set; }

        public DynamicTestVariableSelector(DataDetails marketDetails)
        {
            DataStartDate = marketDetails.StartDate;
            DataEndDate = marketDetails.EndDate;
            TestStartDate = DataEndDate;
        }
    }
}
