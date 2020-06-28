using System;
using System.IO;
using System.Reflection;
using GapTraderCore.Interfaces;
using static System.IO.Directory;
using static GapTraderCore.Strings;

namespace GapTraderCore
{
    [Serializable]
    public sealed class SerializableMarketData : SerializableBase
    {
        public string MinuteDataFilePath { get; set; }

        public string DailyDataFilePath { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime SavedDate { get; set; }

        public string SaveName { get; set; }

        public bool IsUkData { get; set; }

        public double PreviousDailyClose { get; set; }

        public SerializableMarketData(string saveName, IMarket market) : base(market.Name)
        {
            SaveName = saveName;
            StartDate = market.DataDetails.StartDate;
            EndDate = market.DataDetails.EndDate;
            SavedDate = DateTime.Now;
            IsUkData = market.IsUkData;

            PreviousDailyClose = market.DailyCandles[0].Open + market.DailyCandles[0].Gap.GapPoints;

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            path += "\\Saved Data";

            CreateDirectory(path);

            MinuteDataFilePath = $"{path}\\{SaveName}{MinuteDataFileName}";
            DailyDataFilePath = $"{path}\\{SaveName}{DailyDataFileName}";

            CsvServices.WriteMinuteCsv(market.MinuteData, MinuteDataFilePath);
            CsvServices.WriteDailyDataCsv(market.DailyCandles, DailyDataFilePath);
        }
    }
}
