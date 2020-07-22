using System.Windows;

namespace TradeJournalWPF.Windows
{
    public partial class AddStrategyWindow : Window
    {
        public AddStrategyWindow()
        {
            InitializeComponent();
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
