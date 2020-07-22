using System.Windows;
using System.Windows.Input;
using GapTraderCore.ViewModels;
using TradeJournalCore.ViewModels;

namespace GapTraderWPF
{
    public partial class GetNameWindow : Window
    {
        public GetNameWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && TextBox.Text != string.Empty)
            {
                if (DataContext is MarketDetailsViewModel marketDetailsViewModel)
                {
                    marketDetailsViewModel.ConfirmNewNameCommand.Execute(null);
                    Close();
                }

                if (DataContext is AddTradeViewModel addTradeViewModel)
                {
                    addTradeViewModel.ConfirmNewNameCommand.Execute(null);
                    Close();
                }

            }
        }
    }
}
