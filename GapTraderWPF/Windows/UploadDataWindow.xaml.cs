using GapTraderCore;
using Microsoft.Win32;
using System.Threading;
using System.Windows;
using GapTraderCore.ViewModels;
using TradingSharedCore;
using TradingSharedCore.Interfaces;

namespace GapTraderWPF
{
    public partial class UploadDataWindow : Window
    {
        public UploadDataWindow()
        {
            InitializeComponent();
        }

        private void OnProcessButtonClick(object sender, RoutedEventArgs e)
        {
            var runner = new GapTraderRunner(GetDispatcherContext());

            if (DailyDataFileNameTextBox.Text == string.Empty && !DeriveFromDaily.IsChecked == true)
            {
                runner.Run(this,
                    new Message("", "Select Daily Data File or Derive From Minute Data", Message.MessageType.Error));
            }
            else if (BidMinuteDataFileNameTextBox.Text == string.Empty || AskMinuteDataFileNameTextBox.Text == string.Empty)
            {
                runner.Run(this, new Message("", "Select Minute Data Files", Message.MessageType.Error));
            }
            else
            {
                var vm = (DataUploaderViewModel)DataContext;
                vm.StartUploadCommand.Execute(null);
                Close();
            }
        }

        private void SelectDailyDataFileButtonClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "CSV Files|*.csv",
                DefaultExt = "*.csv"
            };

            if (fileDialog.ShowDialog() == true)
            {
                DailyDataFileNameTextBox.Text = fileDialog.FileName;
            }
        }

        private void SelectBidMinuteDataFileButtonClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "CSV Files|*.csv",
                DefaultExt = "*.csv"
            };

            if (fileDialog.ShowDialog() == true)
            {
                BidMinuteDataFileNameTextBox.Text = fileDialog.FileName;
            }
        }

        private void SelectAskMinuteDataFileButtonClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "CSV Files|*.csv",
                DefaultExt = "*.csv"
            };

            if (fileDialog.ShowDialog() == true)
            {
                AskMinuteDataFileNameTextBox.Text = fileDialog.FileName;
            }
        }

        private IContext GetDispatcherContext()
        {
            var dispatcherContext = Dispatcher?.Invoke(() => SynchronizationContext.Current);

            if (dispatcherContext == null)
            {
                throw new ThreadStateException("Could not get dispatcher synchronisation context.");
            }

            var context = new Context(dispatcherContext);
            return context;
        }
    }
}
