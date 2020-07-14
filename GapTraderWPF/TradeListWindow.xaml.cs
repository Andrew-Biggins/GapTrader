using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using GapTraderWPF.CustomSorters;

namespace GapTraderWPF
{
    public partial class TradeListWindow : Window
    {
        public TradeListWindow()
        {
            InitializeComponent();
            TradeListDataGrid.Sorting += TradeListDataGrid_Sorting;
        }

        private void TradeListDataGrid_Sorting(object sender, System.Windows.Controls.DataGridSortingEventArgs e)
        {
            if (e.Column == CloseTimeColumn || e.Column == CloseLevelColumn || e.Column == TotalPointsColumn ||
                e.Column == ProfitColumn || e.Column == MaeColumn || e.Column == MfaColumn ||
                e.Column == DrawdownColumn || e.Column == RealisedProfitColumn ||
                e.Column == UnrealisedProfitPointsColumn || e.Column == UnrealisedProfitCashColumn)
            {
                e.Handled = true;

                var direction = (e.Column.SortDirection != ListSortDirection.Ascending)
                    ? ListSortDirection.Ascending
                    : ListSortDirection.Descending;

                e.Column.SortDirection = direction;

                var lcv = (ListCollectionView) CollectionViewSource.GetDefaultView(TradeListDataGrid.ItemsSource);

                if (e.Column == CloseTimeColumn)
                {
                    lcv.CustomSort = new CloseDateTimeSorter(direction);
                }

                if (e.Column == CloseLevelColumn)
                {
                    lcv.CustomSort = new OptionalDoubleSorter(direction, TradeListColumn.CloseLevel);
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
    }
}
