using GapTraderCore.Interfaces;
using System;

namespace GapTraderCore.Trades
{
    [Serializable]
    public sealed class SavedTrade
    {
        public string StrategyName { get; }

        public string StrategyShortName { get; }

        public string MarketName { get; }

        public double StrategyEntryLevel { get; }

        public double StopLevel { get; }

        public double Target { get; }

        public double OpenLevel { get; }

        public double CloseLevel { get; private set; } = -1;

        public DateTime OpenTime { get; }

        public DateTime CloseTime { get; private set; } = DateTime.MinValue;

        public double Size { get; }

        public SavedTrade(IJournalTrade trade)
        {
            StrategyEntryLevel = trade.StrategyEntryLevel;
            StopLevel = trade.StopLevel;
            Target = trade.Target;
            OpenLevel = trade.OpenLevel;
            OpenTime = trade.OpenTime;
            StrategyName = trade.Strategy.Name;
            StrategyShortName = trade.Strategy.ShortName;
            MarketName = trade.Market.Name;

            trade.CloseLevel.IfExistsThen(x =>
            {
                CloseLevel = x;
                trade.CloseTime.IfExistsThen(y =>
                {
                    CloseTime = y;
                });
            });

            Size = trade.Size;
        }
    }
}
