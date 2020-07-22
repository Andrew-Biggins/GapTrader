using System.Collections.Generic;
using GapTraderCore.Interfaces;
using TradingSharedCore.Interfaces;
using TradingSharedCore.Optional;
using Option = TradingSharedCore.Optional.Option;

namespace TradingSharedCore.Strategies
{
    public class Strategy<TEntry, TTarget> : SerializableStrategy, IStrategy, ISelectableStrategy
    {
        public StrategyStats Stats { get; }

        public double Stop { get; set; }

        public TEntry Entry { get; }

        object IStrategy.Entry => Entry;

        public TTarget Target { get; }

        object IStrategy.Target => Target;

        public List<ITrade> Trades { get; }

        public Optional<double> TrailedStopSize { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                value = !_isSelected;
                _isSelected = value;
            }
        }

        public Strategy(object entry, double stop, object target, StrategyStats stats, List<ITrade> trades,
            bool isStopTrailed, double trailedStopSize)
        {
            Entry = (TEntry) entry;
            Stop = stop;
            Target = (TTarget) target;
            Stats = stats;
            Trades = trades;
            TrailedStopSize = isStopTrailed ? Option.Some(trailedStopSize) : Option.None<double>();
        }

        public Strategy(object entry, double stop, object target, bool isStopTrailed, double trailedStopSize)
        {
            Entry = (TEntry)entry;
            Stop = stop;
            Target = (TTarget)target;
            TrailedStopSize = isStopTrailed ? Option.Some(trailedStopSize) : Option.None<double>();
        }

        public Strategy(string name, string shortName) : base(name, shortName)
        {
            
        }

        private bool _isSelected;
    }
}
