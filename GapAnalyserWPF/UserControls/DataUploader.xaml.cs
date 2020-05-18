using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace GapAnalyserWPF.UserControls
{
    /// <summary>
    /// Interaction logic for DataUploader.xaml
    /// </summary>
    public partial class DataUploader : UserControl
    {
        public DataUploader()
        {
            InitializeComponent();
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

        private void SelectMinuteDataFileButtonClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "CSV Files|*.csv",
                DefaultExt = "*.csv"
            };

            if (fileDialog.ShowDialog() == true)
            {
                MinuteDataFileNameTextBox.Text = fileDialog.FileName;
            }
        }
    }
}
