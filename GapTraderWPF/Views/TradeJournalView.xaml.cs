using GapTraderCore.ViewModels;
using GapTraderWPF.CustomSorters;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GapTraderWPF.Views
{
    public partial class TradeJournalView : UserControl
    {
        public TradeJournalView()
        {
            InitializeComponent();
            JDataGrid.Sorting += JDataGrid_Sorting;
        }

        private void JDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (e.Column == CloseTimeColumn || e.Column == CloseLevelColumn || e.Column == ResultInRColumn ||
                e.Column == TotalPointsColumn || e.Column == ProfitColumn || e.Column == MaeColumn ||
                e.Column == MfaColumn || e.Column == DrawdownColumn || e.Column == RealisedProfitColumn ||
                e.Column == UnrealisedProfitPointsColumn || e.Column == UnrealisedProfitCashColumn)
            {
                e.Handled = true;

                var direction = (e.Column.SortDirection != ListSortDirection.Ascending)
                    ? ListSortDirection.Ascending
                    : ListSortDirection.Descending;

                e.Column.SortDirection = direction;

                var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(JDataGrid.ItemsSource);

                if (e.Column == CloseTimeColumn)
                {
                    lcv.CustomSort = new CloseDateTimeSorter(direction);
                }

                if (e.Column == CloseLevelColumn)
                {
                    lcv.CustomSort = new OptionalDoubleSorter(direction, TradeListColumn.CloseLevel);
                }

                if (e.Column == ResultInRColumn)
                {
                    lcv.CustomSort = new OptionalDoubleSorter(direction, TradeListColumn.ResultInR);
                }

                if (e.Column == TotalPointsColumn)
                {
                    lcv.CustomSort = new OptionalDoubleSorter(direction, TradeListColumn.TotalPoints);
                }

                if (e.Column == ProfitColumn)
                {
                    lcv.CustomSort = new OptionalDoubleSorter(direction, TradeListColumn.CashProfit);
                }

                if (e.Column == MaeColumn)
                {
                    lcv.CustomSort = new OptionalDoubleSorter(direction, TradeListColumn.Mae);
                }

                if (e.Column == MfaColumn)
                {
                    lcv.CustomSort = new OptionalDoubleSorter(direction, TradeListColumn.Mfa);
                }

                if (e.Column == DrawdownColumn)
                {
                    lcv.CustomSort = new OptionalDoubleSorter(direction, TradeListColumn.Drawdown);
                }

                if (e.Column == RealisedProfitColumn)
                {
                    lcv.CustomSort = new OptionalDoubleSorter(direction, TradeListColumn.RealisedProfit);
                }

                if (e.Column == UnrealisedProfitPointsColumn)
                {
                    lcv.CustomSort = new OptionalDoubleSorter(direction, TradeListColumn.UnrealisedProfitPoints);
                }

                if (e.Column == UnrealisedProfitCashColumn)
                {
                    lcv.CustomSort = new OptionalDoubleSorter(direction, TradeListColumn.UnrealisedProfitCash);
                }
            }
        }

        private void OnEditTradeClicked(object sender, MouseButtonEventArgs e)
        {
            var vm = (TradeJournalViewModel) DataContext;
            vm.EditTradeCommand.Execute(null);
        }

        private void OnRemoveTradeClicked(object sender, MouseButtonEventArgs e)
        {
            var vm = (TradeJournalViewModel) DataContext;
            vm.RemoveTradeCommand.Execute(null);
        }

        private void OnGraphDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = (TradeJournalViewModel) DataContext;
            vm.PopOutGraphCommand.Execute(null);
        }
    }
}
