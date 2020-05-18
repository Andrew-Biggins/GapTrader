namespace GapAnalyser.Interfaces
{
    public interface IMinuteCandle : ICandle
    {
        bool IsCash { get; }
    }
}