using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Foundations;
using Foundations.Optional;
using GapTraderCore.Interfaces;
using GapTraderCore.Strategies;

namespace GapTraderCore.ViewModels
{
    public sealed class AddTradeViewModel : BindableBase
    {
        public EventHandler TradeAdded;

        public ICommand ConfirmNewTradeCommand => new BasicCommand(() => TradeAdded.Raise(this));

        public ICommand AddNewMarketCommand => new BasicCommand(() => _runner.GetName(this, "Enter Market Name"));

        public ICommand ConfirmNewNameCommand => new BasicCommand(AddMarket);

        public ICommand AddNewStrategyCommand => new BasicCommand(() => _runner.GetStrategyDetails(_addStrategyViewModel));

        public DateTime OpenDate
        {
            get => _openDate;
            set
            {
                _openDate = value;
                VerifyDateTimes();
            }
        }

        public DateTime OpenTime
        {
            get => _openTime;
            set
            {
                _openTime = value;
                VerifyDateTimes();
            }
        }

        public DateTime CloseDate
        {
            get => _closeDate;
            set
            {
                _closeDate = value;
                VerifyDateTimes();
            }
        }

        public DateTime CloseTime
        {
            get => _closeTime;
            set
            {
                _closeTime = value;
                VerifyDateTimes();
            }
        }

        public double OpenLevel { get; set; } = 6000;

        public Optional<double> CloseLevel { get; set; } = Option.Some<double>(6200);

        public double Size { get; set; } = 1;

        public double Entry { get; set; } = 6000;

        public double Stop { get; set; } = 5900;

        public double Target { get; set; } = 6200;

        public IMarket SelectedMarket { get; set; }

        public IStrategy SelectedStrategy { get; set; }

        public ObservableCollection<IStrategy> Strategies { get; }

        public ObservableCollection<IMarket> Markets { get; }

        public bool EntryHasError
        {
            get => _entryHasError;
            set
            {
                SetProperty(ref _entryHasError, value, nameof(EntryHasError));
                VerifyInputs();
            }
        }

        public bool TargetHasError
        {
            get => _targetHasError;
            set
            {
                SetProperty(ref _targetHasError, value, nameof(TargetHasError)); 
                VerifyInputs();
            }
        }

        public bool StopHasError
        {
            get => _stopHasError;
            set
            {
                SetProperty(ref _stopHasError, value, nameof(StopHasError));
                VerifyInputs();
            }
        }

        public bool OpenLevelHasError
        {
            get => _openLevelHasError;
            set
            {
                SetProperty(ref _openLevelHasError, value, nameof(OpenLevelHasError));
                VerifyInputs();
            }
        }

        public bool CloseLevelHasError
        {
            get => _closeLevelHasError;
            set
            {
                SetProperty(ref _closeLevelHasError, value, nameof(CloseLevelHasError));
                VerifyInputs();
            }
        }

        public bool SizeHasError
        {
            get => _sizeHasError;
            set
            {
                SetProperty(ref _sizeHasError, value, nameof(SizeHasError));
                VerifyInputs();
            }
        }

        public bool DatesHaveError
        {
            get => _datesHaveError;
            set
            {
                SetProperty(ref _datesHaveError, value, nameof(DatesHaveError));
                VerifyInputs();
            }
        }

        public bool IsAddEnabled
        {
            get => _isAddEnabled;
            set => SetProperty(ref _isAddEnabled, value, nameof(IsAddEnabled));
        }

        public string NewName { get; set; }

        public AddTradeViewModel(IRunner runner, ObservableCollection<IMarket> markets,
            ObservableCollection<IStrategy> strategies)
        {
            _runner = runner;
            Markets = markets;
            Strategies = strategies;
            _addStrategyViewModel = new AddStrategyViewModel();
            _addStrategyViewModel.StrategyAdded += AddStrategy;

            if (Strategies.Count > 0)
            {
                SelectedStrategy = Strategies[0];
            }

            if (Markets.Count > 0)
            {
                SelectedMarket = Markets[0];
            }

            VerifyInputs();
        }

        private void AddStrategy(object sender, EventArgs e)
        {
            var stats = new StrategyStats(0,0,0,0,0,0,0,0,0,0,0,0,0);
            var trades = new List<ITrade>();
            var title = _addStrategyViewModel.IsFixedStop
                ? $"Entry: {(double)_addStrategyViewModel.SelectedEntry / 10}% | Target: {(double)_addStrategyViewModel.SelectedTarget /10}% | Stop: {_addStrategyViewModel.Stop}pts"
                : $"Entry: {(double)_addStrategyViewModel.SelectedEntry / 10}% | Target: {(double)_addStrategyViewModel.SelectedTarget / 10}% | Stop: {_addStrategyViewModel.Stop}%";
            var strategy = new Strategy<FibonacciLevel, FibonacciLevel>(_addStrategyViewModel.SelectedEntry, _addStrategyViewModel.Stop, _addStrategyViewModel.SelectedTarget, stats, trades, title);

            Strategies.Add(strategy);
            RaisePropertyChanged(nameof(Strategies));
        }

        private void AddMarket()
        {
            foreach (var market in Markets)
            {
                if (NewName == market.Name)
                {
                    _runner.Run(this, new Message("Already Exists", "Market Already Added", Message.MessageType.Error));
                    NewName = string.Empty;
                    return;
                }
            }

            Markets.Add(new Market { Name = NewName });
            RaisePropertyChanged(nameof(Markets));
            NewName = string.Empty;
        }

        private void VerifyInputs()
        {
            if (EntryHasError || TargetHasError || StopHasError ||
                OpenLevelHasError || CloseLevelHasError || SizeHasError
                || DatesHaveError)
            {
                IsAddEnabled = false;
            }
            else
            {
                IsAddEnabled = true;
            }
        }

        private void VerifyDateTimes()
        {
            var openTime = new DateTime(OpenDate.Year, OpenDate.Month,
                OpenDate.Day, OpenTime.Hour, OpenTime.Minute,
                OpenTime.Second);
            var closeTime = new DateTime(CloseDate.Year, CloseDate.Month,
                CloseDate.Day, CloseTime.Hour,
                CloseTime.Minute, CloseTime.Second);

            DatesHaveError = openTime > closeTime;

            VerifyInputs();
        }

        private readonly IRunner _runner;
        private readonly AddStrategyViewModel _addStrategyViewModel;
        private bool _entryHasError;
        private bool _targetHasError;
        private bool _stopHasError;
        private bool _openLevelHasError;
        private bool _closeLevelHasError;
        private bool _sizeHasError;
        private bool _isAddEnabled;
        private DateTime _openDate = DateTime.Today;
        private DateTime _openTime = new DateTime(2000,1,1,8,0,0);
        private DateTime _closeDate = DateTime.Today;
        private DateTime _closeTime = new DateTime(2000, 1, 1, 16, 30, 0);
        private bool _datesHaveError;
    }
}
