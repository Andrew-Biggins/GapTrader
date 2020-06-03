using Foundations;
using Foundations.Optional;
using GapAnalyser.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Input;
using static GapAnalyser.CsvServices;
using static System.IO.Directory;

namespace GapAnalyser.ViewModels
{
    public delegate void Del();

    public sealed class DataUploaderViewModel : BindableBase
    {
        public bool DeriveDailyFromMinute
        {
            get => _deriveDailyFromMinute;
            set
            {
                value = !_deriveDailyFromMinute;
                SetProperty(ref _deriveDailyFromMinute, value);
                _selector.DeriveDailyFromMinute = DeriveDailyFromMinute;
            }
        }

        public BindableBase FilepathSelector
        {
            get => _filepathSelector;
            private set => SetProperty(ref _filepathSelector, value);
        }

        public List<SavedData> SavedMarkets
        {
            get => _savedMarkets;
            private set => SetProperty(ref _savedMarkets, value);
        }

        public SavedData SelectedSavedMarket
        {
            get => _selectedSavedMarket;
            set => SetProperty(ref _selectedSavedMarket, value);
        }

        public List<Gap> UnfilledGaps
        {
            get => _unfilledGaps;
            private set => SetProperty(ref _unfilledGaps, value);
        } 

        public IMarket Market { get; }

        public bool IsUkData { get; set; }

        public string SaveName { get; set; }

        public string DataStartDate { get; private set; } = string.Empty;
        public string DataEndDate { get; private set; } = string.Empty;
        public string DataHigh { get; private set; } = string.Empty;
        public string HighDate { get; private set; } = string.Empty;
        public string DataLow { get; private set; } = string.Empty;
        public string LowDate { get; private set; } = string.Empty;
        public string AverageGapSize { get; private set; } = string.Empty;

        public ICommand SaveDataCommand => new BasicCommand(GetName);

        public ICommand ConfirmSaveNameCommand => new BasicCommand(SaveData);

        public ICommand LoadDataCommand => new BasicCommand(LoadData);

        public ICommand DeserializeDataCommand => new BasicCommand(DeserializeData);

        public ICommand SelectFileCommand => new BasicCommand(StartUpload);

        public DataUploaderViewModel(IMarket market, IRunner runner, List<SavedData> savedData)
        {
            Market = market;
            _runner = runner;
            SavedMarkets = savedData;
            FilepathSelector = _selector;
        }

        private void StartUpload()
        {
            if (_selector.DailyDataFileName == string.Empty && !DeriveDailyFromMinute)
            {
                _runner.Run(this,
                    new Message("", "Select Daily Data File or Derive From Minute Data", Message.MessageType.Error));
            }
            else if (_selector.MinuteDataFileName == string.Empty)
            {
                _runner.Run(this, new Message("", "Select Minute Data File", Message.MessageType.Error));
            }
            else
            {
                FilepathSelector = _loadingBar;

                // Start upload new thread to allow the UI to update  
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    _previousClose = Option.None<double>();
                    ProcessData(_selector.MinuteDataFileName, _selector.DailyDataFileName, DeriveDailyFromMinute, IsUkData);
                }).Start();
            }
        }

        private void ProcessData(string minuteDataFilePath, string dailyDataFilePath, bool deriveDailyFromMinute, bool isUkData)
        {
            Del progressCounter = IncrementLoadingBarProgress;
            Market.ClearData();
            UnfilledGaps = new List<Gap>();

            _loadingBar.Maximum = File.ReadAllLines(minuteDataFilePath).Length;

            if (deriveDailyFromMinute)
            {
                _loadingBar.Maximum += (double)File.ReadAllLines(minuteDataFilePath).Length / 1330;

            }
            else
            {
                _loadingBar.Maximum += File.ReadAllLines(dailyDataFilePath).Length;
            }

            Market.MinuteData = ReadInMinuteData(minuteDataFilePath, isUkData, progressCounter);

            if (deriveDailyFromMinute)
            {
                Market.DeriveDailyFromMinute(progressCounter);
            }
            else
            {
                var firstMinuteDataDate = Market.MinuteData.Keys.First();
                Market.DailyCandles = ReadInDailyData(dailyDataFilePath, progressCounter, firstMinuteDataDate);
            }

            Market.CalculateStats(isUkData, _previousClose);
            FilepathSelector = _selector;
            _loadingBar.Progress = 0;

            UpdateMarketDetails();
        }

        private void UpdateMarketDetails()
        {
            DataStartDate = $"{Market.DataDetails.StartDate:d/M/yy}";
            DataEndDate = $"{Market.DataDetails.EndDate:d/M/yy}";
            DataHigh = $"{Market.DataDetails.High:N1}";
            HighDate = $"{Market.DataDetails.HighDate:d/M/yy}";
            DataLow = $"{Market.DataDetails.Low:N1}";
            LowDate = $"{Market.DataDetails.LowDate:d/M/yy}";
            AverageGapSize = $"{Market.DataDetails.AverageGapSize:N1}"; 
            UnfilledGaps = Market.UnfilledGaps;
        }

        private void GetName()
        {
            _runner.GetSaveName(this);
        }

        private void SaveData()
        {
            var data = new SavedData(SaveName, Market);

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            path += "\\Saved Data";
            CreateDirectory(path);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream($"{path}\\{SaveName}.txt", FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, data);
            stream.Close();
        }

        private void LoadData()
        {
            _runner.ShowLoadDataWindow(this);
        }

        private void DeserializeData()
        {
            FilepathSelector = _loadingBar;

            // Start upload new thread to allow the UI to update  
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                _previousClose = Option.Some(SelectedSavedMarket.PreviousDailyClose);
                ProcessData(SelectedSavedMarket.MinuteDataFilePath, SelectedSavedMarket.DailyDataFilePath, false,
                    SelectedSavedMarket.IsUkData);
            }).Start();
        }

        private void IncrementLoadingBarProgress()
        {
             _loadingBar.Progress++;
        }

        private readonly LoadingBar _loadingBar = new LoadingBar();
        private readonly FilepathSelector _selector = new FilepathSelector();

        private Optional<double> _previousClose = Option.None<double>();
        private readonly IRunner _runner;
        private bool _deriveDailyFromMinute;
        private BindableBase _filepathSelector;
        private SavedData _selectedSavedMarket;
        private List<SavedData> _savedMarkets;
        private List<Gap> _unfilledGaps;
    }
}
