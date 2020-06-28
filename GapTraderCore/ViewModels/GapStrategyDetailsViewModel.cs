using System.Collections.Generic;
using Foundations;
using GapTraderCore.Interfaces;
using GapTraderCore.Strategies;
using static GapTraderCore.FibonacciServices;

namespace GapTraderCore.ViewModels
{
    public class GapStrategyDetailsViewModel : BindableBase, IStrategyDetails
    {
        public List<FibonacciLevel> EntryFibs { get; }
        public List<FibonacciLevel> TargetFibs { get; }

        public bool IsFixedStop
        {
            get => _isFixedStop;
            set => SetProperty(ref _isFixedStop, value);
        }

        public double Stop { get; set; } = 20;

        public bool StopHasError
        {
            get => _stopHasError;
            set
            {
                SetProperty(ref _stopHasError, value, nameof(StopHasError));
                HasError = value;
            }
        }

        public bool HasError { get; private set; }

        public FibonacciLevel SelectedEntry { get; set; }
        public FibonacciLevel SelectedTarget { get; set; }

        public GapStrategyDetailsViewModel(StrategyType strategyType)
        {
            _strategyType = strategyType;

            if (_strategyType == StrategyType.OutOfGap)
            {
                EntryFibs = GetFibRetraceLevels();
                TargetFibs = GetFibExtensionLevels();
                SelectedEntry = FibonacciLevel.FivePointNine;
                SelectedTarget = FibonacciLevel.OneHundredAndTwentySevenPointOne;
            }
            else if (_strategyType == StrategyType.IntoGap)
            {
                EntryFibs = GetFibExtensionLevels();
                TargetFibs = GetFibRetraceLevels();
                SelectedEntry = FibonacciLevel.OneHundredAndTwentySevenPointOne;
                SelectedTarget = FibonacciLevel.FivePointNine;
            }
        }

        public ISelectableStrategy GetNewStrategy()
        {
            if (_strategyType == StrategyType.OutOfGap)
            {
                return new OutOfGapStrategy<FibonacciLevel, FibonacciLevel>(SelectedEntry, Stop, SelectedTarget, 200, IsFixedStop);
            }
            else
            {
                return new IntoGapStrategy<FibonacciLevel, FibonacciLevel>(SelectedEntry, Stop, SelectedTarget, 200, IsFixedStop);
            }
        }

        private readonly StrategyType _strategyType;

        private bool _isFixedStop;
        private bool _stopHasError;
    }
}