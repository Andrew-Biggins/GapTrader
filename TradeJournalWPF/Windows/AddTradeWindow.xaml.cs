using System.Windows;

namespace TradeJournalWPF.Windows
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
