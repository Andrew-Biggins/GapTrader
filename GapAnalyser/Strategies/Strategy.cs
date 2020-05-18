using GapAnalyser.Interfaces;

namespace GapAnalyser.Strategies
{
    internal class Strategy<TEntry, TTarget> : IStrategy
    {
        public StrategyStats Stats { get; }

        public double Stop { get; set; }

        public TEntry Entry { get; }

        object IStrategy.Entry => Entry;

        public TTarget Target { get; }

        object IStrategy.Target => Target;

        public Strategy(object entry, double stop, object target, StrategyStats stats)
        {
            Entry = (TEntry) entry;
            Stop = stop;
            Target = (TTarget) target;
            Stats = stats;
        }
    }
}
