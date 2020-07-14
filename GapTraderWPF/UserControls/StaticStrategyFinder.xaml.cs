using System.Windows.Controls;
using System.Windows.Input;
using GapTraderCore.ViewModels;

namespace GapTraderWPF.UserControls
{
    public partial class StaticStrategyFinder : UserControl
    {
        public StaticStrategyFinder()
        {
            InitializeComponent();
        }

        private void OnViewTradesClicked(object sender, MouseButtonEventArgs e)
        {
            var vm = (StaticStrategyFinderViewModel)DataContext;
            vm.ViewTradesCommand.Execute(null);
        }

        private void OnViewGraphClicked(object sender, MouseButtonEventArgs e)
        {
            var vm = (StaticStrategyFinderViewModel)DataContext;
            vm.ViewGraphCommand.Execute(null);
        }

        private void OnRowClicked(object sender, MouseButtonEventArgs e)
        {
            var vm = (StaticStrategyFinderViewModel)DataContext;
            vm.MoreDetailsCommand.Execute(null);
        }
    }
}
