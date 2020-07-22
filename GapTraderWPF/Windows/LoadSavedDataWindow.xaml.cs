using System.Windows;
using System.Windows.Input;
using GapTraderCore.ViewModels;

namespace GapTraderWPF.Windows
{
    public partial class LoadSavedDataWindow : Window
    {
        public LoadSavedDataWindow()
        {
            InitializeComponent();
        }

        private void OnLoadDataClicked(object sender, MouseButtonEventArgs e)
        {
            var vm = (MarketDetailsViewModel)DataContext;
            vm.DeserializeDataCommand.Execute(null);
            Close();
        }

        private void OnDeleteDataClicked(object sender, MouseButtonEventArgs e)
        {
            var vm = (MarketDetailsViewModel)DataContext;
            vm.DeleteDataCommand.Execute(null);
        }
    }
}
