using System;
using GapTraderCore.ViewModels;
using TradingSharedCore;

namespace GapTraderCore.VariableSelectors
{
    public sealed class DynamicTestVariableSelector : VariableSelector
    {
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
