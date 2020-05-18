namespace GapAnalyser.Interfaces
{
    public interface IDailyCandle : ICandle
    {
        Gap Gap { get; set; }
    }
}