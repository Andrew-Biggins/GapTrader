using System;
using GapAnalyser.Interfaces;
using System.IO;
using System.Reflection;
using GapAnalyser;
using static System.IO.Directory;

namespace GapAnalyser
{
    [Serializable]
    public sealed class SavedData
    {
        public string MinuteDataFilePath { get; set; }

        public string DailyDataFilePath { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime SavedDate { get; set; }

        public string Name { get; set; }

        public bool IsUkData { get; set; }

        public double PreviousDailyClose { get; set; }

        public SavedData(string name, IMarket market)
        {
            Name = name;
            StartDate = market.DataDetails.StartDate;
            EndDate = market.DataDetails.EndDate;
            SavedDate = DateTime.Now;
            IsUkData = market.IsUkData;

            PreviousDailyClose = market.DailyCandles[0].Open + market.DailyCandles[0].Gap.GapPoints;

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            path += "\\Saved Data";

            CreateDirectory(path);
            // todo handle if already exists
            MinuteDataFilePath = $"{path}\\{Name}_minute_data.csv";
            DailyDataFilePath = $"{path}\\{Name}_daily_data.csv";

            CsvServices.WriteMinuteCsv(market.MinuteData, MinuteDataFilePath);
            CsvServices.WriteDailyDataCsv(market.DailyCandles, DailyDataFilePath);
        }
    }
}
