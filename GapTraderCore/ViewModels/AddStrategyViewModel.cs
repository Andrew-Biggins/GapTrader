using Foundations;
using GapTraderCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace GapTraderCore.ViewModels
{
    public enum StrategyType
    {
        OutOfGap,
        IntoGap,
        Triangle,
        Other
    }

    public sealed class AddStrategyViewModel : BindableBase
    {
        public EventHandler StrategyAdded;

        public ICommand ConfirmNewStrategyCommand => new BasicCommand(() => StrategyAdded.Raise(this));

        public IStrategyDetails StrategyDetails
        {
            get => _strategyDetails;
            set => SetProperty(ref _strategyDetails, value);
        }

        public StrategyType SelectedStrategyType
        {
            get => _selectedStrategyType;
            set
            {
                if (value != _selectedStrategyType)
                {
                    _selectedStrategyType = value;
                    RaisePropertyChanged(nameof(SelectedStrategyType));
                    UpdateStrategyDetails();
                }
            }
        }

        public List<StrategyType> StrategyTypes { get; }

        public AddStrategyViewModel()
        {
            StrategyTypes = GetStrategyTypes();
            UpdateStrategyDetails();
        }

        private void UpdateStrategyDetails()
        {
            switch (SelectedStrategyType)
            {
                case StrategyType.OutOfGap:
                case StrategyType.IntoGap:
                    StrategyDetails = new GapStrategyDetailsViewModel(SelectedStrategyType);
                    break;
                case StrategyType.Triangle:
                    break;
                case StrategyType.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

        private StrategyType _selectedStrategyType;
        private IStrategyDetails _strategyDetails;
    }
}
