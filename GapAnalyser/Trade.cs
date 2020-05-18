using System;
using GapAnalyser.Interfaces;

namespace GapAnalyser
{
    public struct Trade : ITrade
    {
        public double Stop { get; }

        public double Target { get; }

        public double OpenLevel { get; }

        public double CloseLevel { get; }

        public DateTime OpenTime { get; }

        public DateTime CloseTime { get; }

        public double Profit { get; }

        public Trade(double stop, double target, double openLevel, double closeLevel, DateTime openTime, DateTime closeTime, double profit)
        {
            Stop = stop;
            Target = target;
            OpenLevel = openLevel;
            CloseLevel = closeLevel;
            OpenTime = openTime;
            CloseTime = closeTime;
            Profit = profit;
        }
    }
}