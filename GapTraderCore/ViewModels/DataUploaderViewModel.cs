using Foundations;
using System.Threading;
using System.Windows.Input;

namespace GapTraderCore.ViewModels
{
    public sealed class DataUploaderViewModel : BindableBase
    {
        public bool DeriveDailyFromMinute
        {
            get => _deriveDailyFromMinute;
            set
            {
                value = !_deriveDailyFromMinute;
                SetProperty(ref _deriveDailyFromMinute, value);
            }
        }

        public string DailyDataFileName
        {
            get => _dailyDatafileName;
            set => SetProperty(ref _dailyDatafileName, value);
        }

        public string MinuteBidDataFileName
        {
            get => _minuteBidDatafileName;
            set => SetProperty(ref _minuteBidDatafileName, value);
        }

        public string MinuteAskDataFileName
        {
            get => _minuteAskDatafileName;
            set => SetProperty(ref _minuteAskDatafileName, value);
        }

        public bool IsUkData { get; set; }

        public ICommand StartUploadCommand => new BasicCommand(StartUpload);

        public DataUploaderViewModel(DataProcessDel processNewData)
        {
            _processNewData = processNewData;
        }

        private void StartUpload()
        {
            // Start upload new background thread to allow the UI to update  
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                _processNewData(MinuteBidDataFileName, MinuteAskDataFileName,
                    DailyDataFileName, DeriveDailyFromMinute, IsUkData);
            }).Start();
        }

        private readonly DataProcessDel _processNewData;
        private bool _deriveDailyFromMinute;
        private string _dailyDatafileName;
        private string _minuteBidDatafileName;
        private string _minuteAskDatafileName;
    }
}
