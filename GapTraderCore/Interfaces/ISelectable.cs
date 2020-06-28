namespace GapTraderCore.Interfaces
{
    public interface ISelectable
    {
        string Name { get; }

        bool IsSelected { get; set; }
    }
}