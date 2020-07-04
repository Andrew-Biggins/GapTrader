using System;
using System.Collections.Generic;
using System.Text;
using GapTraderCore.Interfaces;
using GapTraderCore.Strategies;

namespace GapTraderCore.ViewModels
{
    public class StrategyDetailsViewModel : TrailingStopBaseViewModel, IStrategyDetails
    {
        public bool HasError { get; protected set; }

        public StrategyDetailsViewModel(StrategyType strategyType)
        {
            StrategyType = strategyType;
        }

        public virtual ISelectableStrategy GetNewStrategy()
        {
            switch (StrategyType)
            {
                case StrategyType.Triangle:
                    return new TriangleStrategy("Triangle", "Triangle", IsStopTrailed, TrailedStopSize);
                case StrategyType.FailedTriangle:
                    return new TriangleStrategy("Failed Triangle", "Failed Triangle", IsStopTrailed, TrailedStopSize);
                default:
                    return new TriangleStrategy("Other", "Other", IsStopTrailed, TrailedStopSize);
            }
        }

        protected readonly StrategyType StrategyType;
    }

}

