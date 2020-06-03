using System;
using System.Collections.Generic;
using System.ComponentModel;
using Foundations.Optional;
using GapAnalyser.Candles;
using GapAnalyser.ViewModels;

namespace GapAnalyser.Interfaces
{
    public interface IMarket : INotifyPropertyChanged
    {
        Dictionary<FibonacciLevel, FibLevel> GapFibLevels { get; }

        List<DailyCandle> DailyCandles { get; set; }

        Dictionary<DateTime, List<MinuteCandle>> MinuteData { get; set; }

        List<Gap> UnfilledGaps { get; }

        DataDetails DataDetails { get; }

        void DeriveDailyFromMinute(Del counter);

        void ClearData();

        void CalculateStats(bool ukData, Optional<double> previousClose);

        bool IsUkData { get; }
    }
}