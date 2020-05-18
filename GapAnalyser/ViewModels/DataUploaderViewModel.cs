using System;
using Foundations;
using GapAnalyser.Interfaces;
using System.Windows.Input;

namespace GapAnalyser.ViewModels
{
    public sealed class DataUploaderViewModel : BindableBase
    {
        public string DailyDataFileName
        {
            get => _dailyDatafileName;
            set => SetProperty(ref _dailyDatafileName, value);
        }

        public string MinuteDataFileName
        {
            get => _minuteDatafileName;
            set => SetProperty(ref _minuteDatafileName, value);
        }

        public bool DeriveDailyFromMinute
        {
            get => _deriveDailyFromMinute;
            set
            {
                value = !_deriveDailyFromMinute;
                SetProperty(ref _deriveDailyFromMinute, value);
            }
        }

        public bool UkData { get; set; }

        public ICommand SelectFileCommand => new BasicCommand(ProcessData);

        public DataUploaderViewModel(IMarket market, IRunner runner)
        {
            _market = market;
            _runner = runner;
        }

        private void ProcessData()
        {
            if (DailyDataFileName == string.Empty && !DeriveDailyFromMinute)
            {
                _runner.Run(this, new Message("", "Select Daily Data File", Message.MessageType.Error));
            }
            else if (MinuteDataFileName == string.Empty)
            {
                _runner.Run(this, new Message("", "Select Minute Data File", Message.MessageType.Error));
            }
            else
            {
                _market.ClearData();
                _market.ReadInMinuteData(MinuteDataFileName, UkData);

                if (DeriveDailyFromMinute)
                {
                    _market.DeriveDailyFromMinute();
                }
                else
                {
                    _market.ReadInData(DailyDataFileName);
                }

                _market.CalculateGapFibLevelPreHitAdverseExcursions();
            }
        }

        private readonly IRunner _runner;
        private readonly IMarket _market;
        private string _dailyDatafileName;
        private string _minuteDatafileName;
        private bool _deriveDailyFromMinute;
    }
}
