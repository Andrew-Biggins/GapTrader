using GapTraderCore.Interfaces;
using GapTraderCore.Strategies;

namespace GapTraderCore.ViewModels
{
    public sealed class OtherStrategyDetailsViewModel : StrategyDetailsViewModel
    {
        public string Name { get; set; } = string.Empty;

        public OtherStrategyDetailsViewModel(StrategyType strategyType) : base(strategyType)
        {
        }

        public override ISelectableStrategy GetNewStrategy()
        {
            return new TriangleStrategy(Name, Name, IsStopTrailed, TrailedStopSize);
        }
    }
}
