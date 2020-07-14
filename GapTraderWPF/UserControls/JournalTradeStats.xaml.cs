using GapTraderCore.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace GapTraderWPF.UserControls
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
