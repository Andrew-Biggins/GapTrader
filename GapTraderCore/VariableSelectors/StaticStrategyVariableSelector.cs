using System;
using System.Windows.Input;
using Foundations;

namespace GapTraderCore.VariableSelectors
{
    public sealed class StaticStrategyVariableSelector : VariableSelector
    {
        public EventHandler? FiltersChanged;

        public int MaxMinGapSize { get; set; } = 1000;
        public int MinMinGapSize { get; set; } = 10;
        public int GapSizeIncrement { get; set; } = 100;

        public bool ApplyDateTimeFilters
        {
            get => _applyDateTimeFilters;
            set
            {
                value = !_applyDateTimeFilters;
                SetProperty(ref _applyDateTimeFilters, value);
            }
        }

        public double MinTrades { get; set; } = 10;
        public double MinProfitFactor { get; set; } = 2;

        public ICommand RefreshFiltersCommand => new BasicCommand(() => FiltersChanged.Raise(this));

        private bool _applyDateTimeFilters;
    }
}
