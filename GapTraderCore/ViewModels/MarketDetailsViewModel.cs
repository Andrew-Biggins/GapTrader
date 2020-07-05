using System;
using Foundations;
using Foundations.Optional;
using GapTraderCore.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Input;
using static GapTraderCore.CsvServices;
using static GapTraderCore.Strings;
using static System.IO.Directory;

namespace GapTraderCore.ViewModels
{
    public delegate void DataProcessDel(string minuteBidDataFilePath, string minuteAskDataFilePath,
        string dailyDataFilePath, bool deriveDailyFromMinute, bool isUkData);

    public delegate void Del();

    public sealed class MarketDetailsViewModel : BindableBase
    {
        public EventHandler MarketDataChanged;

        public GapFillStatsViewModel GapFillStatsViewModel
        {
            get => _gapFillStatsViewModel;
            private set => SetProperty(ref _gapFillStatsViewModel, value);
        }

        public IMarket Market
        {
            get => _market;
            set => SetProperty(ref _market, value);
        }

        public List<SerializableMarketData> SavedMarkets
        {
            get => _savedMarkets;
            set => SetProperty(ref _savedMarkets, value);
        }

        public SerializableMarketData SelectedSerializableMarket
        {
            get => _selectedSerializableMarket;
            set => SetProperty(ref _selectedSerializableMarket, value);
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

        public bool DataInUse
        {
            get => _dataInUse;
            private set => SetProperty(ref _dataInUse, value);
        }

        public DataUploaderViewModel DataUploaderViewModel { get; private set; }

        public string NewName { get; set; }

        public bool DataExists { get; private set; }

        public ICommand NewSaveDataCommand => new BasicCommand(() => _runner.GetName(this, "Enter Save Name"));

        public ICommand SaveDataCommand => new BasicCommand(() => _runner.ShowSaveDataWindow(this));

        public ICommand ConfirmNewNameCommand => new BasicCommand(SaveData);

        public ICommand LoadDataCommand => new BasicCommand(() => _runner.ShowLoadSavedDataWindow(this));

        public ICommand DeserializeDataCommand => new BasicCommand(DeserializeData);

        public ICommand UploadNewDataCommand => new BasicCommand(UploadNewData);

        public ICommand DeleteDataCommand => new BasicCommand(DeleteData);

        public ICommand AddDataCommand => new BasicCommand(AddData);

        public ICommand OverwriteSavedDataCommand => new BasicCommand(OverwriteSavedData);

        public MarketDetailsViewModel(IRunner runner, IMarket market)
        {
            Market = market;
            _runner = runner;

            SavedMarkets = GetSavedMarkets(_savedDataPath);
            MarketStats = new MarketStats();
            GapFillStatsViewModel = new GapFillStatsViewModel();
            CheckForData();
        }

        public void ToggleDataInUse() => DataInUse = !DataInUse;

        private void UpdateStats()
        {
            GapFillStatsViewModel = new GapFillStatsViewModel(Market);
            UnfilledGaps = Market.UnfilledGaps;
        }

        private void CheckForData()
        {
            DataExists = Market.DailyCandles.Count > 0;
        }

        private void SaveData()
        {
            foreach (var savedMarket in SavedMarkets)
            {
                if (savedMarket.SaveName == NewName)
                {
                    _runner.Run(this,
                        new Message("Already Exists", "Data Name Already Exists", Message.MessageType.Error));
                    NewName = string.Empty;
                    return;
                }
            }

            var data = new SerializableMarketData(NewName, _market);

            CreateDirectory(_savedDataPath);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream($"{_savedDataPath}\\{NewName}.txt", FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, data);
            stream.Close();

            UpdateMarketDetails(NewName);
            Market.UpdateName(NewName);
            RaisePropertyChanged(nameof(Market));
            NewName = string.Empty;
            SavedMarkets = GetSavedMarkets(_savedDataPath);
        }

        private void OverwriteSavedData()
        {
            var name = SelectedSerializableMarket.SaveName;
            DeleteData();
            NewName = name;
            SaveData();
        }

        private void UploadNewData()
        {
            DataProcessDel processNewData = ProcessNewData;
            DataUploaderViewModel = new DataUploaderViewModel(processNewData)
            {
                IsNewData = true,
                Title = "Upload New Data"
            };
            _runner.ShowUploadNewDataWindow(DataUploaderViewModel);
        }

        private void AddData()
        {
            DataProcessDel addData = AddDataToExisting;
            DataUploaderViewModel = new DataUploaderViewModel(addData)
            {
                IsNewData = false,
                Title = "Add to Existing Data"
            };
            _runner.ShowUploadNewDataWindow(DataUploaderViewModel);
        }

        private void AddDataToExisting(string minuteBidDataFilePath, string minuteAskDataFilePath,
            string dailyDataFilePath, bool deriveDailyFromMinute, bool isUkData)
        {
            ClearExistingData();
            MarketStats = _loadingBar;

            _loadingBar.Maximum = WriteSafeReadAllLines(minuteBidDataFilePath).Length +
                                  WriteSafeReadAllLines(minuteAskDataFilePath).Length * 2;

            if (deriveDailyFromMinute)
            {
                _loadingBar.Maximum += (double)WriteSafeReadAllLines(minuteBidDataFilePath).Length / 1330;
            }
            else
            {
                _loadingBar.Maximum += WriteSafeReadAllLines(dailyDataFilePath).Length;
            }

            var timezone = Market.IsUkData
                ? Timezone.Uk
                : Timezone.Us;

            var newMinuteData = ReadInNewMinuteData(minuteBidDataFilePath, minuteAskDataFilePath, timezone,
                () => { _loadingBar.Progress++; });

            foreach (var (key, value) in newMinuteData)
            {
                if (!Market.MinuteData.ContainsKey(key))
                {
                    Market.MinuteData.Add(key, value);
                }
            }

            if (deriveDailyFromMinute)
            {
                Market.DeriveDailyFromMinute(() => { _loadingBar.Progress++; });
            }
            else
            {
                var firstMinuteDataDate = Market.MinuteData.Keys.First();
                var newDailyCandles = ReadInDailyData(dailyDataFilePath, () => { _loadingBar.Progress++; },
                    firstMinuteDataDate);

                // Remove duplicate/overlapping dates before joining the lists
                foreach (var candle in Market.DailyCandles)
                {
                    for (var i = newDailyCandles.Count - 1; i >= 0; i--)
                    {
                        if (newDailyCandles[i].Date == candle.Date)
                        {
                            newDailyCandles.Remove(newDailyCandles[i]);
                        }
                    }
                }

                Market.DailyCandles.AddRange(newDailyCandles);
            }

            var pc = Market.DailyCandles[0].Open + Market.DailyCandles[0].Gap.GapPoints;

            _previousClose = Option.Some(pc);
            Market.CalculateStats(Market.IsUkData, _previousClose);
            _loadingBar.Progress = 0;

            UpdateMarketDetails(Market.Name);
        }

        private void ProcessSavedData(SerializableMarketData marketData)
        {
            MarketStats = _loadingBar;
            _loadingBar.Maximum = WriteSafeReadAllLines(marketData.MinuteDataFilePath).Length +
                                  WriteSafeReadAllLines(marketData.DailyDataFilePath).Length;

            ClearExistingData();

            Market.MinuteData = ReadInSavedMinuteData(marketData, () => { _loadingBar.Progress++; });

            var firstMinuteDataDate = Market.MinuteData.Keys.First();
            Market.DailyCandles = ReadInDailyData(marketData.DailyDataFilePath, () => { _loadingBar.Progress++; },
                firstMinuteDataDate);

            _previousClose = Option.Some(SelectedSerializableMarket.PreviousDailyClose);
            Market.CalculateStats(SelectedSerializableMarket.IsUkData, _previousClose);
            MarketStats = new MarketStats(Market.DataDetails);
            _loadingBar.Progress = 0;

            UpdateMarketDetails(marketData.SaveName);
            CheckForData();
        }

        private void ProcessNewData(string minuteBidDataFilePath, string minuteAskDataFilePath,
            string dailyDataFilePath, bool deriveDailyFromMinute, bool isUkData)
        {
            ClearExistingData();
            MarketStats = _loadingBar;
            _loadingBar.Maximum = WriteSafeReadAllLines(minuteBidDataFilePath).Length +
                                  WriteSafeReadAllLines(minuteAskDataFilePath).Length * 2;

            if (deriveDailyFromMinute)
            {
                _loadingBar.Maximum += (double)WriteSafeReadAllLines(minuteBidDataFilePath).Length / 1330;
            }
            else
            {
                _loadingBar.Maximum += WriteSafeReadAllLines(dailyDataFilePath).Length;
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
            Market.UpdateName("Unsaved Data Set");
            CheckForData();
        }

        private void DeserializeData()
        {
            //Start upload new thread background to allow the UI to update
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                ProcessSavedData(SelectedSerializableMarket);
            }).Start();
        }

        private void UpdateMarketDetails()
        {
            MarketStats = new MarketStats(Market.DataDetails);
            UpdateStats();
        }

        private void UpdateMarketDetails(string name)
        {
            MarketStats = new MarketStats(Market.DataDetails, name);
            UpdateStats();
            Market.UpdateName(name);
        }

        private void ClearExistingData()
        {
            Market.ClearData();
            MarketDataChanged.Raise(this);
            UnfilledGaps = new List<Gap>();
            GapFillStatsViewModel = new GapFillStatsViewModel();
        }

        private void DeleteData()
        {
            var dataFile = $"{SelectedSerializableMarket.SaveName}.txt";
            var minData = $"{SelectedSerializableMarket.SaveName}{MinuteDataFileName}";
            var dailyData = $"{SelectedSerializableMarket.SaveName}{DailyDataFileName}";

            if (File.Exists(Path.Combine(_savedDataPath, dataFile)))
            {
                File.Delete(Path.Combine(_savedDataPath, dataFile));
            }

            if (File.Exists(Path.Combine(_savedDataPath, minData)))
            {
                File.Delete(Path.Combine(_savedDataPath, minData));
            }

            if (File.Exists(Path.Combine(_savedDataPath, dailyData)))
            {
                File.Delete(Path.Combine(_savedDataPath, dailyData));
            }

            SavedMarkets = GetSavedMarkets(_savedDataPath);
        }

        private static List<SerializableMarketData> GetSavedMarkets(string dataPath)
        {
            var markets = new List<SerializableMarketData>();

            CreateDirectory(dataPath);

            var d = new DirectoryInfo(dataPath);
            IFormatter formatter = new BinaryFormatter();

            foreach (var file in d.GetFiles("*.txt"))
            {
                var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                var savedData = (SerializableMarketData)formatter.Deserialize(stream);
                markets.Add(savedData);
            }

            return markets;
        }

        private readonly string _savedDataPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Saved Data\\";

        private readonly IRunner _runner;
        private readonly LoadingBarViewModel _loadingBar = new LoadingBarViewModel();

        private ILoadable _marketStats;
        private SerializableMarketData _selectedSerializableMarket;
        private IMarket _market = new Market();
        private List<SerializableMarketData> _savedMarkets;
        private List<Gap> _unfilledGaps;
        private Optional<double> _previousClose = Option.None<double>();
        private GapFillStatsViewModel _gapFillStatsViewModel;
        private bool _dataInUse;
    }
}
