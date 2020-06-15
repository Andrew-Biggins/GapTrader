using System.Collections.Generic;
using GapTraderCore.Interfaces;

namespace GapTraderCore.Strategies
{
    internal class Strategy<TEntry, TTarget> : IStrategy
    {
        public StrategyStats Stats { get; private set; }

        public double Stop { get; set; }

        public TEntry Entry { get; }

        object IStrategy.Entry => Entry;

        public TTarget Target { get; }

        object IStrategy.Target => Target;

        public List<ITrade> Trades { get; }

        public string Title { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                value = !_isSelected;
                _isSelected = value;
            }
        }

        public Strategy(object entry, double stop, object target, StrategyStats stats, List<ITrade> trades, string title)
        {
            Entry = (TEntry) entry;
            Stop = stop;
            Target = (TTarget) target;
            Stats = stats;
            Trades = trades;
            Title = title;
        }

        public void UpdateStats()
        {
           // Stats = DataProcessor.GetStrategyStats(Trades);
        }

        private bool _isSelected;
    }
}
