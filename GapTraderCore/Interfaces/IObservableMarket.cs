using System.ComponentModel;

namespace GapTraderCore.Interfaces
{
    public interface IObservableMarket : INotifyPropertyChanged
    {
        string Name { get; set; }

        bool IsSelected { get; set; }
    }
}