using System;
using System.Collections.Generic;
using System.Text;

namespace TradingSharedCore
{
    public enum StrategyType
    {
        OutOfGap,
        IntoGap,
        Triangle,
        FailedTriangle,
        Other
    }

    public enum TradeDirection
    {
        Long,
        Short,
        Both
    }

    public enum TradeType
    {
        IntoGap,
        OutOfGap,
        Both
    }
}
