using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Foundations;
using GapTraderCore.Interfaces;

namespace GapTraderCore.ViewModels
{
    public enum TradeStatus
    {
        Open,
        Closed,
        Both
    }

    public sealed class TradeFilterSelectorViewModel : BindableBase
    {
        public EventHandler FiltersChanged;

        public ObservableCollection<ISelectableStrategy> Strategies { get; }

        public ObservableCollection<ISelectable> Markets { get; }

        public ObservableCollection<ISelectable> DaysOfWeek { get; } 

        public List<TradeStatus> TradeStatuses { get; } = new List<TradeStatus>();

        public TradeStatus SelectedTradeStatus { get; set; } = TradeStatus.Both;

        public DateTime TradesStartDate
        {
            get => _tradesStartDate;
            set => SetProperty(ref _tradesStartDate, value);
        }

        public DateTime TradesEndDate
        {
            get => _tradesEndDate;
            set => SetProperty(ref _tradesEndDate, value);
        }

        public DateTime FilterStartDate
        {
            get => _filterStartDate;
            set => SetProperty(ref _filterStartDate, value);
        }

        public DateTime FilterEndDate
        {
            get => _filterEndDate;
            set => SetProperty(ref _filterEndDate, value);
        }

        public DateTime FilterStartTime
        {
            get => _filterStartTime;
            set => SetProperty(ref _filterStartTime, value);
        }

        public DateTime FilterEndTime
        {
            get => _filterEndTime;
            set => SetProperty(ref _filterEndTime, value);
        }

        public double MinRiskRewardRatio { get; set; } = 0;
        public double MaxRiskRewardRatio { get; set; } = 1000;

        public ICommand ApplyTradeFiltersCommand => new BasicCommand(() => FiltersChanged.Raise(this));

        public TradeFilterSelectorViewModel(ObservableCollection<ISelectableStrategy> strategies, ObservableCollection<ISelectable> markets)
        {
            Strategies = strategies;
            Markets = markets;
            GetTradeStatuses();
            SelectAllMarkets();
            SelectAllStrategies();

            DaysOfWeek = GetDaysOfWeek();
        }

        private void GetTradeStatuses()
        {
            var statuses = (TradeStatus[]) Enum.GetValues(typeof(TradeStatus));

            foreach (var status in statuses)
            {
                TradeStatuses.Add(status);
            }
        }

        private void SelectAllMarkets()
        {
            foreach (var market in Markets)
            {
                market.IsSelected = true;
            }
        }

        private void SelectAllStrategies()
        {
            foreach (var strategy in Strategies)
            {
                strategy.IsSelected = true;
            }
        }

        private static ObservableCollection<ISelectable> GetDaysOfWeek()
        {
            var daysOfWeek = new ObservableCollection<ISelectable>();
            var days = (DayOfWeek[])Enum.GetValues(typeof(DayOfWeek));

            foreach (var day in days)
            {
                daysOfWeek.Add(new Day(day) { IsSelected = true });
            }

            return daysOfWeek;
        }

        private DateTime _tradesStartDate;
        private DateTime _tradesEndDate;
        private DateTime _filterStartDate;
        private DateTime _filterEndDate;
        private DateTime _filterStartTime = new DateTime(1,1,1,0,0,0);
        private DateTime _filterEndTime = new DateTime(1,1,1,23,59,59);
    }
}
