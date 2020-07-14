using System.Collections.Generic;
using GapTraderCore.Interfaces;
using GapTraderCore.Strategies;
using static GapTraderCore.FibonacciServices;

namespace GapTraderCore.ViewModels
{
    public class GapStrategyDetailsViewModel : StrategyDetailsViewModel
    {
        public List<FibonacciLevel> EntryFibs { get; }
        public List<FibonacciLevel> TargetFibs { get; }

        public double MinimumGapSize { get; set; } = 200;

        public bool MinimumGapHasError
        {
            get => _minimumGapHasError;
            set
            {
                SetProperty(ref _minimumGapHasError, value, nameof(MinimumGapHasError));
                VerifyInputs();
            }
        }

        public bool IsFixedStop
        {
            get => _isFixedStop;
            set => SetProperty(ref _isFixedStop, value);
        }

        public double Stop { get; set; } = 20;

        public bool StopSizeHasError
        {
            get => _stopSizeHasError;
            set
            {
                SetProperty(ref _stopSizeHasError, value, nameof(StopSizeHasError));
                VerifyInputs();
            }
        }

        public bool StopTrailHasError
        {
            get => _stopTrailHasError;
            set
            {
                SetProperty(ref _stopTrailHasError, value, nameof(StopTrailHasError));
                VerifyInputs();
            }
        }

        public FibonacciLevel SelectedEntry { get; set; }
        public FibonacciLevel SelectedTarget { get; set; }

        public GapStrategyDetailsViewModel(StrategyType strategyType) : base(strategyType)
        {
            if (StrategyType == StrategyType.OutOfGap)
            {
                EntryFibs = GetFibRetraceLevels();
                TargetFibs = GetFibExtensionLevels();
                SelectedEntry = FibonacciLevel.FivePointNine;
                SelectedTarget = FibonacciLevel.OneHundredAndTwentySevenPointOne;
            }
            else if (StrategyType == StrategyType.IntoGap)
            {
                EntryFibs = GetFibExtensionLevels();
                TargetFibs = GetFibRetraceLevels();
                SelectedEntry = FibonacciLevel.OneHundredAndTwentySevenPointOne;
                SelectedTarget = FibonacciLevel.FivePointNine;
            }
        }

        public override ISelectableStrategy GetNewStrategy()
        {
            if (StrategyType == StrategyType.OutOfGap)
            {
                return new OutOfGapStrategy<FibonacciLevel, FibonacciLevel>(SelectedEntry, Stop, SelectedTarget, MinimumGapSize,
                    IsFixedStop, IsStopTrailed, TrailedStopSize);
            }

            return new IntoGapStrategy<FibonacciLevel, FibonacciLevel>(SelectedEntry, Stop, SelectedTarget, MinimumGapSize,
                IsFixedStop, IsStopTrailed, TrailedStopSize);
        }

        private void VerifyInputs()
        {
            if (MinimumGapHasError || StopSizeHasError || StopTrailHasError)
            {
                HasError = true;
            }
            else
            {
                HasError = false;
            }
        }

        private bool _isFixedStop;
        private bool _stopSizeHasError;
        private bool _stopTrailHasError;
        private bool _minimumGapHasError;
    }
}