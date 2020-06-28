using GapTraderCore.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace GapTraderWPF
{
    public partial class SaveDataWindow : Window
    {
        public SaveDataWindow()
        {
            InitializeComponent();
        }

        private void OnSaveDataClicked(object sender, MouseButtonEventArgs e)
        {
            var vm = (MarketDetailsViewModel)DataContext;
            vm.OverwriteSavedDataCommand.Execute(null);
            Close();
        }

        private void OnDeleteDataClicked(object sender, MouseButtonEventArgs e)
        {
            var vm = (MarketDetailsViewModel)DataContext;
            vm.DeleteDataCommand.Execute(null);
        }
    }
}
