using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Input;
using Foundations;
using GapAnalyser.Interfaces;
using Microsoft.VisualBasic;
using static System.IO.Directory;
using static GapAnalyser.CsvServices;

namespace GapAnalyser.ViewModels
{
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

        public string SaveName { get; set; }

        public string DataStartDate { get; } = string.Empty;
        public string DataEndDate { get; } = string.Empty;
        public string DataHigh { get; } = string.Empty;
        public string HighDate { get; } = string.Empty;
        public string DataLow { get; } = string.Empty;
        public string LowDate { get; } = string.Empty;
        public string AverageGapSize { get; } = string.Empty;

        public ICommand SaveDataCommand => new BasicCommand(GetName);

        public ICommand ConfirmSaveNameCommand => new BasicCommand(SaveData);

        public ICommand LoadDataCommand => new BasicCommand(LoadData);

        public ICommand DeserializeDataCommand => new BasicCommand(DeserializeData);

        public MarketDetailsViewModel(List<SavedData> savedData, IRunner runner)
        {
            SavedMarkets = savedData;
            _runner = runner;
        }

        public MarketDetailsViewModel(List<SavedData> savedData, IMarket market, IRunner runner)
        {
            Market = market;
            _runner = runner;
            SavedMarkets = savedData;

            DataStartDate = $"{market.DataDetails.StartDate:d/M/yy}";
            DataEndDate = $"{market.DataDetails.EndDate:d/M/yy}";
            DataHigh = $"{market.DataDetails.High:N1}";
            HighDate = $"{market.DataDetails.HighDate:d/M/yy}";
            DataLow = $"{market.DataDetails.Low:N1}";
            LowDate = $"{market.DataDetails.LowDate:d/M/yy}";
            AverageGapSize = $"{market.DataDetails.AverageGapSize:N1}";
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

        private void LoadData()
        {
            _runner.ShowLoadDataWindow(this);
        }

        private void DeserializeData()
        {
            // todo add first to XML
            //_market.ClearData();
            //_market.MinuteData = ReadInMinuteData(SelectedSavedMarket.MinuteDataFilePath, SelectedSavedMarket.IsUkData, progressCounter);
            //var firstMinuteDataDate = _market.MinuteData.Keys.First();
            //_market.DailyCandles = ReadInDailyData(SelectedSavedMarket.MinuteDataFilePath, progressCounter, firstMinuteDataDate);
        }

        private readonly IRunner _runner;

        private SavedData _selectedSavedMarket;
        private IMarket _market = new Market();
        private List<SavedData> _savedMarkets;
    }
}
