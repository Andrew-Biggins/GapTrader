namespace GapTraderCore.Interfaces
{
    public interface IStrategyDetails
    {
        bool HasError { get; }

        ISelectableStrategy GetNewStrategy();
    }
}