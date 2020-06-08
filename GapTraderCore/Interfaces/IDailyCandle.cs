namespace GapTraderCore.Interfaces
{
    public interface IDailyCandle : ICandle
    {
        Gap Gap { get; set; }
    }
}