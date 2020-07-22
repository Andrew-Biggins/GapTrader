using System;
using System.ComponentModel;

namespace GapTraderCore.Interfaces
{
    public interface IRunnable : INotifyPropertyChanged
    {
        event EventHandler? CloseRequested;

        string Title { get; }

        bool IsRunning { get; set; }

        void CleanUp();
    }
}
