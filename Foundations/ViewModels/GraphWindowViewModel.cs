using System.Collections.Generic;
using GapTraderCore.Interfaces;
using TradingSharedCore.ViewModelAdapters;

namespace TradingSharedCore.ViewModels
{
    public class GraphWindowViewModel
    {
        public ITradePlot Plot { get; }

        public GraphWindowViewModel(double accountStartSize, IEnumerable<ITrade> trades)
        {
            Plot = new TradePlot();
            Plot.UpdateData(accountStartSize, trades);
        }
    }
}