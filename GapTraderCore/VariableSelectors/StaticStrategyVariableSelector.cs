using System;
using System.Collections.Generic;
using System.Windows.Input;
using Foundations;
using GapTraderCore.ViewModels;
using static GapTraderCore.FibonacciServices;

namespace GapTraderCore.VariableSelectors
{
    public sealed class StaticStrategyVariableSelector : VariableSelector
    {
        public EventHandler? FiltersChanged;

        public bool IsFixedEntry
        {
            get => _isFixedEntry;
            set
            {
                value = !_isFixedEntry;
                SetProperty(ref _isFixedEntry, value);
                UpdateFixedFibs(_strategyType);
            }
        }

        public List<FibonacciLevel> EntryFibs { get; private set; } = GetFibRetraceLevels();
        public FibonacciLevel SelectedEntry { get; set; }

        public bool IsFixedTarget
        {
            get => _isFixedTarget;
            set
            {
                value = !_isFixedTarget;
                SetProperty(ref _isFixedTarget, value);
                UpdateFixedFibs(_strategyType);
            }
        }

        public List<FibonacciLevel> TargetFibs { get; private set; } = GetFibExtensionLevels();
        public FibonacciLevel SelectedTarget { get; set; }

        public int MaxMinGapSize { get; set; } = 200;
        public int MinMinGapSize { get; set; } = 50;
        public int GapSizeIncrement { get; set; } = 50;

        public bool ApplyDateTimeFilters
        {
            get => _applyDateTimeFilters;
            set
            {
                value = !_applyDateTimeFilters;
                SetProperty(ref _applyDateTimeFilters, value);
            }
        }

        public bool IsStopTrailed
        {
            get => _isStopTrailed;
            set
            {
                value = !_isStopTrailed;
                SetProperty(ref _isStopTrailed, value);

                // Set Min and Max to the values to prevent unnecessary iterations during the search
                if (!_isStopTrailed)
                {
                    MaxStopTrail = 10;
                    MinStopTrail = 10;
                }
            }
        }

        public int MaxStopTrail { get; set; } = 10;
        public int MinStopTrail { get; set; } = 10;
        public int StopTrailIncrement { get; set; } = 20;

        public double MinTrades { get; set; } = 10;
        public double MinProfitFactor { get; set; } = 2;

        public ICommand RefreshFiltersCommand => new BasicCommand(() => FiltersChanged.Raise(this));

        public void UpdateFixedFibs(StrategyType strategyType)
        {
            _strategyType = strategyType;

            if (strategyType == StrategyType.OutOfGap)
            {
                EntryFibs = GetFibRetraceLevels();
                TargetFibs = GetFibExtensionLevels();
                SelectedEntry = IsFixedEntry ? FibonacciLevel.FivePointNine : 0;
                SelectedTarget = IsFixedTarget ? FibonacciLevel.OneHundredAndTwentySevenPointOne : 0;
            }
            else if (strategyType == StrategyType.IntoGap)
            {
                EntryFibs = GetFibExtensionLevels();
                TargetFibs = GetFibRetraceLevels();
                SelectedEntry = IsFixedEntry ? FibonacciLevel.OneHundredAndTwentySevenPointOne : 0;
                SelectedTarget = IsFixedTarget ? FibonacciLevel.FivePointNine : 0;
            }

            RaisePropertyChanged(nameof(EntryFibs));
            RaisePropertyChanged(nameof(TargetFibs));
            RaisePropertyChanged(nameof(SelectedEntry));
            RaisePropertyChanged(nameof(SelectedTarget));
        }

        private bool _applyDateTimeFilters;
        private bool _isFixedEntry;
        private bool _isFixedTarget;
        private StrategyType _strategyType = StrategyType.OutOfGap;
        private bool _isStopTrailed;
    }
}
