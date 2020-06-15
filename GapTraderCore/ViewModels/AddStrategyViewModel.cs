using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Foundations;
using static GapTraderCore.FibonacciServices;

namespace GapTraderCore.ViewModels
{
    public enum StrategyType
    {
        OutOfGap,
        IntoGap
    }

    public sealed class AddStrategyViewModel : BindableBase
    {
        public EventHandler StrategyAdded;

        public List<FibonacciLevel> EntryFibs { get; private set; } 
        public List<FibonacciLevel> TargetFibs { get; private set; }

        public ICommand ConfirmNewStrategyCommand => new BasicCommand(() => StrategyAdded.Raise(this));

        public FibonacciLevel SelectedEntry { get; set; }
        public FibonacciLevel SelectedTarget { get; set; }

        public StrategyType SelectedStrategyType
        {
            get => _selectedStrategyType;
            set
            {
                if (value != _selectedStrategyType)
                {
                    _selectedStrategyType = value;
                    UpdateFibs();
                }
            }
        }

        public bool IsFixedStop
        {
            get => _isFixedStop;
            set => SetProperty(ref _isFixedStop, value);
        }

        public double Stop { get; set; } = 20;

        public bool StopHasError
        {
            get => _stopHasError;
            set => SetProperty(ref _stopHasError, value, nameof(StopHasError));
        }

        public List<StrategyType> StrategyTypes { get; }

        public AddStrategyViewModel()
        {
            StrategyTypes = GetStrategyTypes();
            UpdateFibs();
        }

        private void UpdateFibs()
        {
            if (SelectedStrategyType == StrategyType.OutOfGap)
            {
                EntryFibs = _retraceFibs;
                TargetFibs = _extensionFibs;
                SelectedEntry = FibonacciLevel.FivePointNine;
                SelectedTarget = FibonacciLevel.OneHundredAndTwentySevenPointOne;
            }
            else if (SelectedStrategyType == StrategyType.IntoGap)
            {
                EntryFibs = _extensionFibs;
                TargetFibs = _retraceFibs;
                SelectedEntry = FibonacciLevel.OneHundredAndTwentySevenPointOne;
                SelectedTarget = FibonacciLevel.FivePointNine;
            }

            RaisePropertyChanged(nameof(EntryFibs));
            RaisePropertyChanged(nameof(TargetFibs));
            RaisePropertyChanged(nameof(SelectedEntry));
            RaisePropertyChanged(nameof(SelectedTarget));
        }

        private static List<StrategyType> GetStrategyTypes()
        {
            var list = new List<StrategyType>();

            var strategies = (StrategyType[])Enum.GetValues(typeof(StrategyType));

            foreach (var strategy in strategies)
            {
                list.Add(strategy);
            }

            return list;
        }

        private readonly List<FibonacciLevel> _retraceFibs = GetFibRetraceLevels();
        private readonly List<FibonacciLevel> _extensionFibs = GetFibExtensionLevels();

        private StrategyType _selectedStrategyType;
        private bool _isFixedStop;
        private bool _stopHasError;
        
    }
}
