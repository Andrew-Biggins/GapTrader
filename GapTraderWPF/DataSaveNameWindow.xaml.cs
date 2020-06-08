using System.Windows;

namespace GapTraderWPF
{
    public partial class DataSaveNameWindow : Window
    {
        public DataSaveNameWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
