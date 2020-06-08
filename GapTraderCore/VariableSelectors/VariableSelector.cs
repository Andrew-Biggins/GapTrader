using Foundations;
using GapTraderCore.Interfaces;

namespace GapTraderCore.VariableSelectors
{
    public abstract class VariableSelector : BindableBase, ILoadable
    {
        public int MaxStop { get; set; } = 1000;
        public int MinStop { get; set; } = 10;
        public int StopIncrement { get; set; } = 100;

        public bool IsFixedStop { get; set; }

        public double MinWinProbability { get; set; } = 40;
    }
}
