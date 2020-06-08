using System.Windows;
using System.Windows.Input;
using GapTraderCore.ViewModels;

namespace GapTraderWPF
{
    public partial class LoadSavedDataWindow : Window
    {
        public LoadSavedDataWindow()
        {
            InitializeComponent();
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = (MarketDetailsViewModel)DataContext;
            vm.DeserializeDataCommand.Execute(null);
            Close();
        }
    }
}
