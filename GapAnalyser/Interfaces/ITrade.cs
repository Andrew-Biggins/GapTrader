using System;
using Microsoft.VisualBasic;

namespace GapAnalyser.Interfaces
{
    public interface ITrade
    {
        double Stop { get; }

        double Target { get; }

        double OpenLevel { get; }

        double CloseLevel { get; }

        DateTime OpenTime { get; }

        DateTime CloseTime { get; }

        double Profit { get; }
    }
}