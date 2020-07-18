using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GapTraderCore.Candles;
using GapTraderCore.Interfaces;
using GapTraderCore.ViewModels;
using Microsoft.VisualBasic.FileIO;

namespace GapTraderCore
{
    public static class CsvServices
    {
        public static Dictionary<DateTime, List<BidAskCandle>> ReadInNewMinuteData(
            string bidFilePath, string askFilePath, Timezone timezone, Del progressCounter)
        {
            var data = new Dictionary<DateTime, List<BidAskCandle>>();
            data = ReadMinuteDataFile(data, bidFilePath, timezone, progressCounter);

            return AddAskData(data, askFilePath, progressCounter, timezone);
        }

        public static Dictionary<DateTime, List<BidAskCandle>> ReadInSavedMinuteData(SerializableMarketData marketData, Del progressCounter)
        {
            var minuteData = new Dictionary<DateTime, List<BidAskCandle>>();
            var timezone = marketData.IsUkData ? Timezone.Uk : Timezone.Us;

            var date = DateTime.MinValue;
            using var csvParser = SetUpParser(marketData.MinuteDataFilePath);
            var minuteCandles = new List<BidAskCandle>();

            while (!csvParser.EndOfData)
            {
                var values = csvParser.ReadFields();

                var candle = ParseBidCandle(values, timezone);
                candle.AddAskPrices(double.Parse(values[6]), double.Parse(values[7]), double.Parse(values[8]),
                    double.Parse(values[9]));

                progressCounter();

                if (candle.DateTime.Date != date.Date)
                {
                    if (minuteCandles.Count != 0)
                    {
                        minuteData.Add(date.Date, minuteCandles);
                    }

                    minuteCandles = new List<BidAskCandle>();
                }

                date = candle.DateTime.Date;
                minuteCandles.Add(candle);
            }

            if (minuteCandles.Count != 0)
            {
                minuteData.Add(date.Date, minuteCandles);
            }

            return minuteData;
        }

        public static List<DailyCandle> ReadInDailyData(string filePath, Del progressCounter, DateTime firstMinuteDataDate)
        {
            using var csvParser = SetUpParser(filePath);
            var data = new List<DailyCandle>();

            while (!csvParser.EndOfData)
            {
                // Check data is valid (bank holidays have null entries on some markets)
                var x = csvParser.PeekChars(12);
                if (x[^1].ToString().Equals("n"))
                {
                    csvParser.ReadLine();
                    continue;
                }

                var candle = ParseDailyCandle(csvParser.ReadFields());

                progressCounter();

                if (candle.Date >= firstMinuteDataDate)
                {
                    data.Add(candle);
                }
            }

            return data;
        }

        public static void WriteMinuteCsv(Dictionary<DateTime, List<BidAskCandle>> data, string filePath)
        {
            var csv = new StringBuilder();

            csv.AppendLine(Headings);

            foreach (var (_, candles) in data)
            {
                foreach (var candle in candles)
                {
                    var newLine =
                        $"{candle.DateTime},{candle.BidOpen},{candle.BidHigh},{candle.BidLow},{candle.BidClose},{candle.Volume},{candle.AskOpen},{candle.AskHigh},{candle.AskLow},{candle.AskClose}";
                    csv.AppendLine(newLine);
                }
            }

            File.WriteAllText(filePath, csv.ToString());
        }

        public static void WriteDailyDataCsv(List<DailyCandle> data, string filePath)
        {
            var csv = new StringBuilder();

            csv.AppendLine(Headings);

            foreach (var candle in data)
            {
                var newLine = $"{candle.Date},{candle.Open},{candle.High},{candle.Low},{candle.Close},{candle.Volume}";
                csv.AppendLine(newLine);
            }

            File.WriteAllText(filePath, csv.ToString());
        }

        public static void WriteTradeDataCsv(IEnumerable<IJournalTrade> trades, string filePath)
        {
            var csv = new StringBuilder();

            csv.AppendLine(
                "Market, Strategy, Strategy Entry, Stop, Target, Long/Short, Risk Reward Ratio, Open Date/Time, Open Level, Size, Close Date/Time, Close Level, Total Points, Profit/Loss, Result in R, MAE, MFA, Drawdown, Realised Profit %, Unrealised Profit Points, Unrealised Profit Cash"); 

            foreach (var trade in trades)
            {
                var closeLevel = string.Empty;
                var closeDateTime = string.Empty;
                var totalPoints = string.Empty;
                var profit = string.Empty;
                var resultInR = string.Empty;
                var mae = string.Empty;
                var mfe = string.Empty;
                var drawdown = string.Empty;
                var realisedProfitPercentage = string.Empty;
                var unrealisedProfitPoints = string.Empty;
                var unrealisedProfitCash = string.Empty;

                trade.CloseLevel.IfExistsThen(x => { closeLevel = x.ToString(CultureInfo.CurrentCulture); });
                trade.CloseTime.IfExistsThen(x => { closeDateTime = x.ToString(CultureInfo.CurrentCulture); });
                trade.PointsProfit.IfExistsThen(x => { totalPoints = x.ToString(CultureInfo.CurrentCulture); });
                trade.CashProfit.IfExistsThen(x => { profit = x.ToString(CultureInfo.CurrentCulture); });
                trade.ResultInR.IfExistsThen(x => { resultInR = x.ToString(CultureInfo.CurrentCulture); });
                trade.MaximumAdverseExcursionPoints.IfExistsThen(x => { mae = x.ToString(CultureInfo.CurrentCulture); });
                trade.MaximumFavourableExcursionPoints.IfExistsThen(x => { mfe = x.ToString(CultureInfo.CurrentCulture); });
                trade.MaximumAdverseExcursionPercentageOfStop.IfExistsThen(x => { drawdown = x.ToString(CultureInfo.CurrentCulture); });
                trade.PointsProfitPercentageOfMaximumFavourableExcursion.IfExistsThen(x => { realisedProfitPercentage = x.ToString(CultureInfo.CurrentCulture); });
                trade.UnrealisedProfitPoints.IfExistsThen(x => { unrealisedProfitPoints = x.ToString(CultureInfo.CurrentCulture); });
                trade.UnrealisedProfitCash.IfExistsThen(x => { unrealisedProfitCash = x.ToString(CultureInfo.CurrentCulture); });


                var newLine =
                    $"{trade.Market},{trade.Strategy.Name},{trade.StrategyEntryLevel},{trade.StopLevel},{trade.Target},{trade.Direction},{trade.RiskRewardRatio},{trade.OpenTime},{trade.OpenLevel},{trade.Size},{closeDateTime},{closeLevel},{totalPoints},{profit},{resultInR},{mae},{mfe},{drawdown},{realisedProfitPercentage},{unrealisedProfitPoints},{unrealisedProfitCash}";
                csv.AppendLine(newLine);
            }

            File.WriteAllText(filePath, csv.ToString());
        }

        public static string[] WriteSafeReadAllLines(string path)
        {
            using var csv = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(csv);
            var file = new List<string>();

            while (!sr.EndOfStream)
            {
                file.Add(sr.ReadLine());
            }

            return file.ToArray();
        }

        private static TextFieldParser SetUpParser(string filePath)
        {
            var csvParser = new TextFieldParser(filePath) { CommentTokens = new[] { "#" } };
            csvParser.SetDelimiters(",");
            csvParser.HasFieldsEnclosedInQuotes = true;

            // Skip the row with the column names
            csvParser.ReadLine();

            return csvParser;
        }

        private static Dictionary<DateTime, List<BidAskCandle>> ReadMinuteDataFile(
            Dictionary<DateTime, List<BidAskCandle>> data, string filePath,
            Timezone timezone, Del progressCounter)
        {
            var date = DateTime.MinValue;
            using var csvParser = SetUpParser(filePath);
            var minuteCandles = new List<BidAskCandle>();

            while (!csvParser.EndOfData)
            {
                var candle = ParseBidCandle(csvParser.ReadFields(), timezone);

                progressCounter();

                if (Math.Abs(candle.Volume) > 0)
                {
                    if (candle.DateTime.Date != date.Date)
                    {
                        if (minuteCandles.Count != 0)
                        {
                            data.Add(date.Date, minuteCandles);
                        }

                        minuteCandles = new List<BidAskCandle>();
                    }

                    date = candle.DateTime.Date;

                    if (candle.IsCash)
                    {
                        minuteCandles.Add(candle);
                    }
                }
            }

            if (minuteCandles.Count != 0)
            {
                data.Add(date.Date, minuteCandles);
            }

            return data;
        }

        private static Dictionary<DateTime, List<BidAskCandle>> AddAskData(
            Dictionary<DateTime, List<BidAskCandle>> data, string askFilePath, Del progressCounter, Timezone timezone)
        {
            using var askParser = SetUpParser(askFilePath);
            var askCandles = new List<AskCandle>();

            while (!askParser.EndOfData)
            {
                var candle = ParseAskCandle(askParser.ReadFields(), timezone);
                askCandles.Add(candle);
                progressCounter();
            }

            foreach (var (_, bidCandles) in data)
            {
                for (var i = bidCandles.Count - 1; i >= 0; i--)
                {
                    var candleDataComplete = false;

                    foreach (var askCandle in askCandles)
                    {
                        if (bidCandles[i].DateTime == askCandle.DateTime)
                        {
                            bidCandles[i].AddAskPrices(askCandle.AskOpen, askCandle.AskHigh, askCandle.AskLow,
                                askCandle.AskClose);
                            askCandles.Remove(askCandle);
                            candleDataComplete = true;
                            progressCounter();
                            break;
                        }
                    }

                    // Candles with no matching bid and ask data are removed and completely omitted
                    // Means some data loss and possible trade inaccuracy 
                    if (!candleDataComplete)
                    {
                        bidCandles.RemoveAt(i);
                    }
                }
            }

            return data;
        }

        private static DailyCandle ParseDailyCandle(IReadOnlyList<string> fields)
        {
            var (dateTime, open, high, low, close, volume) = Parse(fields);
            return new DailyCandle(dateTime, open, high, low, close, volume);
        }

        private static BidAskCandle ParseBidCandle(IReadOnlyList<string> fields, Timezone timezone)
        {
            var (dateTime, open, high, low, close, volume) = Parse(fields);
            return new BidAskCandle(dateTime, open, high, low, close, volume, timezone);
        }

        private static AskCandle ParseAskCandle(IReadOnlyList<string> fields, Timezone timezone)
        {
            var (dateTime, open, high, low, close, _) = Parse(fields);
            return new AskCandle(dateTime, open, high, low, close, timezone);
        }

        private static (DateTime, double, double, double, double, double) Parse(IReadOnlyList<string> fields)
        {
            var dateTimeString = fields[0].Replace("GMT+0100", "+1").Replace("GMT-0000", "+0");

            DateTime.TryParseExact(dateTimeString, new[] { "yyyy-MM-dd", "dd.MM.yyyy HH:mm:ss.fff z", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy" },
                CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt);

            var open = double.Parse(fields[1]);
            var high = double.Parse(fields[2]);
            var low = double.Parse(fields[3]);
            var close = double.Parse(fields[4]);
            var volume = double.Parse(fields[5]);

            return (dt, open, high, low, close, volume);
        }

        private const string Headings = "Date, Bid Open, Bid High, Bid Low, Bid Close, Volume, Ask Open, Ask High, Ask Low, Ask Close";
    }
}
