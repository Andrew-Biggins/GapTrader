using System.Windows;

namespace GapTraderWPF
{
    public partial class AddTradeWindow : Window
    {
        public AddTradeWindow()
        {
            InitializeComponent();
        }

        private void OnTradeAdded(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
