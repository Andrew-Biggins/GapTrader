using System.Collections.Generic;
using GapTraderCore.Interfaces;

namespace GapTraderCore.Strategies
{
    internal class Strategy<TEntry, TTarget> : SerializableStrategy, IStrategy, ISelectableStrategy
    {
        public StrategyStats Stats { get; }

        public double Stop { get; set; }

        public TEntry Entry { get; }

        object IStrategy.Entry => Entry;

        public TTarget Target { get; }

        object IStrategy.Target => Target;

        public List<ITrade> Trades { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                value = !_isSelected;
                _isSelected = value;
            }
        }

        public Strategy(object entry, double stop, object target, StrategyStats stats, List<ITrade> trades) : base()
        {
            Entry = (TEntry) entry;
            Stop = stop;
            Target = (TTarget) target;
            Stats = stats;
            Trades = trades;
        }

        public Strategy(object entry, double stop, object target) : base()
        {
            Entry = (TEntry)entry;
            Stop = stop;
            Target = (TTarget)target;
        }

        public Strategy(string name, string shortName) : base(name, shortName)
        {
            
        }

        private bool _isSelected;
    }
}
