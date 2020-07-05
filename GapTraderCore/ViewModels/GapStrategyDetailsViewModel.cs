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
                return new OutOfGapStrategy<FibonacciLevel, FibonacciLevel>(SelectedEntry, Stop, SelectedTarget, 200, IsFixedStop, IsStopTrailed, TrailedStopSize);
            }
            else
            {
                return new IntoGapStrategy<FibonacciLevel, FibonacciLevel>(SelectedEntry, Stop, SelectedTarget, 200, IsFixedStop, IsStopTrailed, TrailedStopSize);
            }
        }

        private bool _isFixedStop;
        private bool _stopHasError;
    }
}