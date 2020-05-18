using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GapAnalyser.Interfaces
{
    public interface IRunnable : INotifyPropertyChanged
    {
        event EventHandler? CloseRequested;

        string Title { get; }

        bool IsRunning { get; set; }

        void CleanUp();
    }
}
