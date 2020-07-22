using System;

namespace TradingSharedCore
{
    public sealed class FibLevel
    {
        public double NextDayHitPercentage { get; set; }

        public double RetracementFactor { get; }

        public double AveragePreHitAdverseExcursion { get; set; }

        public double HighestPreHitAdverseExcursion { get; set; }

        public DateTime DateOfHighestPreHitAdverseExcursion { get; set; }

        public double AveragePostHitFavourableExcursion { get; set; }

        public double HighestPostHitFavourableExcursion { get; set; }

        public DateTime DateOfHighestPostHitFavourableExcursion { get; set; }

        public double AveragePostHitAdverseExcursion { get; set; }

        public double HighestPostHitAdverseExcursion { get; set; }

        public DateTime DateOfHighestPostHitAdverseExcursion { get; set; }

        public FibLevel(double retracementFactor)
        {
            RetracementFactor = retracementFactor / 1000;
        }
    }
}
