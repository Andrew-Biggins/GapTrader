using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using GapTraderCore.ViewModels;

namespace GapTraderWPF.Views
{
    public partial class TradeJournalView : UserControl
    {
        public TradeJournalView()
        {
            InitializeComponent();
        }

        private void OnEditTradeClicked(object sender, MouseButtonEventArgs e)
        {
            var vm = (TradeJournalViewModel)DataContext;
            vm.EditTradeCommand.Execute(null);
        }

        private void OnRemoveTradeClicked(object sender, MouseButtonEventArgs e)
        {
            var vm = (TradeJournalViewModel)DataContext;
            vm.RemoveTradeCommand.Execute(null);
        }

        private void OnGraphDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = (TradeJournalViewModel)DataContext;
            vm.PopOutGraphCommand.Execute(null);
        }
    }
}
