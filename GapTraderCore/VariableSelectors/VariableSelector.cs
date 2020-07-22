using GapTraderCore.Interfaces;
using TradingSharedCore;

namespace GapTraderCore.VariableSelectors
{
    public abstract class VariableSelector : BindableBase, ILoadable
    {
        public int MaxStopSize { get; set; } = 100;
        public int MinStopSize { get; set; } = 20;
        public int StopSizeIncrement { get; set; } = 20;

        public bool IsFixedStop { get; set; }

        public double MinWinProbability { get; set; } = 40;
    }
}
