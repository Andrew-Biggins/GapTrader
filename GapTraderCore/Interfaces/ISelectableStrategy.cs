namespace GapTraderCore.Interfaces
{
    public interface ISelectableStrategy : ISelectable
    {
        string ShortName { get; }
    }
}