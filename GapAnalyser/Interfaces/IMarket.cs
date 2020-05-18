using System;
using System.Collections.Generic;
using System.ComponentModel;
using GapAnalyser.Candles;

namespace GapAnalyser.Interfaces
{
    public interface IMarket : INotifyPropertyChanged
    {
        Dictionary<FibonacciLevel, FibLevel> GapFibLevels { get; }

        List<DailyCandle> DailyCandles { get; }

        Dictionary<DateTime, List<MinuteCandle>> MinuteData { get; }

        List<Gap> UnfilledGaps { get; }

        DataDetails DataDetails { get; }

        bool UkData { get; }

        void ReadInMinuteData(string filePath, bool ukData);

        void DeriveDailyFromMinute();

        void ReadInData(string filePath);

        void CalculateGapFibLevelPreHitAdverseExcursions();

        void ClearData();
    }
}