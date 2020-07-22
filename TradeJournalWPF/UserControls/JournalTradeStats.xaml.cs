using System.Windows.Controls;
using System.Windows.Input;
using TradingSharedCore.ViewModels;

namespace TradeJournalWPF.UserControls
{
    public partial class JournalTradeStats : UserControl
    {
        public JournalTradeStats()
        {
            InitializeComponent();
        }

        private void OnMouseLeftButtonUpOnStrategyResults(object sender, MouseButtonEventArgs e)
        {
            var vm = (StrategyResultsStatsViewModel)DataContext;
            vm.MoreDetailsCommand.Execute(null);
        }
    }
}
