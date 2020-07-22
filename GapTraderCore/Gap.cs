using System;
using TradingSharedCore.Optional;
using Option = TradingSharedCore.Optional.Option;

namespace GapTraderCore
{
    public class Gap
    {
        public double GapPoints { get; }

        public double AbsoluteGapPoints { get; }

        public double Open { get; }

        public double Close { get; }

        public double GapFillPercentage { get; set; }

        public double GapExtensionPercentage { get; set; }

        public bool HasGapBeenFilled { get; set; }

        public double FiftyPercentGapFillLevel { get; set; }

        public Optional<DateTime> GapFillDate { get; set; } = Option.None<DateTime>();

        public DateTime Date { get; }

        public Gap(double previousClose, double open, DateTime date)
        {
            Close = previousClose;
            Open = open;
            GapPoints = open - previousClose;
            AbsoluteGapPoints = Math.Abs(GapPoints);
            Date = date;
        }
    }
}
