using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GapAnalyser.ViewModels;

namespace GapAnalyserWPF.UserControls
{
    /// <summary>
    /// Interaction logic for StrategyFinder.xaml
    /// </summary>
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
