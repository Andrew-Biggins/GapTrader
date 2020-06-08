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

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = (StaticStrategyFinderViewModel)DataContext;
            vm.ViewTradesCommand.Execute(null);
        }
    }
}
