using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Input;
using Foundations;
using Foundations.Optional;
using GapTraderCore.Interfaces;
using static System.IO.Directory;
using static GapTraderCore.CsvServices;

namespace GapTraderCore.ViewModels
{
    public delegate void DataProcessDel(string minuteBidDataFilePath, string minuteAskDataFilePath,
        string dailyDataFilePath, bool deriveDailyFromMinute, bool isUkData);

    public delegate void Del();

    public sealed class MarketDetailsViewModel : BindableBase
    {
        public IMarket Market
        {
            get => _market;
            set => SetProperty(ref _market, value);
        }

        public List<SavedData> SavedMarkets
        {
            get => _savedMarkets;
            set => SetProperty(ref _savedMarkets, value);
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

        public ILoadable MarketStats
        {
            get => _marketStats;
            set => SetProperty(ref _marketStats, value);
        }

        public DataUploaderViewModel DataUploaderViewModel { get; private set; }

        public string SaveName { get; set; }

        public ICommand SaveDataCommand => new BasicCommand(GetName);

        public ICommand ConfirmSaveNameCommand => new BasicCommand(SaveData);

        public ICommand LoadDataCommand => new BasicCommand(LoadSavedData);

        public ICommand DeserializeDataCommand => new BasicCommand(DeserializeData);

        public ICommand UploadNewDataCommand => new BasicCommand(UploadNewData);

        public MarketDetailsViewModel(List<SavedData> savedData, IRunner runner, IMarket market)
        {
            Market = market;
            _runner = runner;
            SavedMarkets = savedData;
            MarketStats = new MarketStats();
        }

        private void GetName()
        {
            _runner.GetSaveName(this);
        }

        private void SaveData()
        {
            var data = new SavedData(SaveName, _market);

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            path += "\\Saved Data";
            CreateDirectory(path);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream($"{path}\\{SaveName}.txt", FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, data);
            stream.Close();
        }

        private void UploadNewData()
        {
            DataProcessDel processNewData = ProcessNewData;
            DataUploaderViewModel = new DataUploaderViewModel(processNewData);
            UnfilledGaps = new List<Gap>();
            _runner.ShowUploadNewDataWindow(DataUploaderViewModel);
        }

        private void LoadSavedData()
        {
            _runner.ShowLoadSavedDataWindow(this);
        }

        private void ProcessSavedData(SavedData data)
        {
            MarketStats = _loadingBar;
            _loadingBar.Maximum = File.ReadAllLines(data.MinuteDataFilePath).Length +
                                  File.ReadAllLines(data.DailyDataFilePath).Length;

            Market.ClearData();
            UnfilledGaps = new List<Gap>();

            Market.MinuteData = ReadInSavedMinuteData(data, () => { _loadingBar.Progress++; });

            var firstMinuteDataDate = Market.MinuteData.Keys.First();
            Market.DailyCandles = ReadInDailyData(data.DailyDataFilePath, () => { _loadingBar.Progress++; },
                firstMinuteDataDate);

            _previousClose = Option.Some(SelectedSavedMarket.PreviousDailyClose);
            Market.CalculateStats(SelectedSavedMarket.IsUkData, _previousClose);
            MarketStats = new MarketStats(Market.DataDetails);
            _loadingBar.Progress = 0;

            UpdateMarketDetails();
        }

        private void ProcessNewData(string minuteBidDataFilePath, string minuteAskDataFilePath,
            string dailyDataFilePath, bool deriveDailyFromMinute, bool isUkData)
        {
            MarketStats = _loadingBar;
            Market.ClearData();
            _loadingBar.Maximum = File.ReadAllLines(minuteBidDataFilePath).Length +
                                  File.ReadAllLines(minuteAskDataFilePath).Length * 2;

            if (deriveDailyFromMinute)
            {
                _loadingBar.Maximum += (double)File.ReadAllLines(minuteBidDataFilePath).Length / 1330;
            }
            else
            {
                _loadingBar.Maximum += File.ReadAllLines(dailyDataFilePath).Length;
            }

            var timezone = isUkData
                ? Timezone.Uk
                : Timezone.Us;

            Market.MinuteData = ReadInNewMinuteData(minuteBidDataFilePath, minuteAskDataFilePath, timezone,
                () => { _loadingBar.Progress++; });

            if (deriveDailyFromMinute)
            {
                Market.DeriveDailyFromMinute(() => { _loadingBar.Progress++; });
            }
            else
            {
                var firstMinuteDataDate = Market.MinuteData.Keys.First();
                Market.DailyCandles = ReadInDailyData(dailyDataFilePath, () => { _loadingBar.Progress++; },
                    firstMinuteDataDate);
            }

            _previousClose = Option.None<double>();
            Market.CalculateStats(isUkData, _previousClose);
            _loadingBar.Progress = 0;

            UpdateMarketDetails();
        }

        private void DeserializeData()
        {
            //Start upload new thread background to allow the UI to update
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                ProcessSavedData(SelectedSavedMarket);
            }).Start();
        }

        private void UpdateMarketDetails()
        {
            MarketStats = new MarketStats(Market.DataDetails);
            UnfilledGaps = Market.UnfilledGaps;
        }

        private readonly IRunner _runner;
        private readonly LoadingBarViewModel _loadingBar = new LoadingBarViewModel();

        private ILoadable _marketStats;
        private SavedData _selectedSavedMarket;
        private IMarket _market = new Market();
        private List<SavedData> _savedMarkets;
        private List<Gap> _unfilledGaps;
        private Optional<double> _previousClose = Option.None<double>();
    }
}
