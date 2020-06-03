using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GapAnalyser.Candles;
using GapAnalyser.ViewModels;

namespace GapAnalyser
{
    public static class CsvServices
    {
        public static Dictionary<DateTime, List<MinuteCandle>> ReadInMinuteData(string filePath, bool ukData, Del progressCounter)
        {
            using var csvParser = SetUpParser(filePath);

            var data = new Dictionary<DateTime, List<MinuteCandle>>();
            var date = DateTime.MinValue;
            var mc = new List<MinuteCandle>();

            while (!csvParser.EndOfData)
            {
                var candle = ukData
                    ? ParseUkMinuteCandle(csvParser.ReadFields())
                    : ParseUsMinuteCandle(csvParser.ReadFields());

                progressCounter();

                if (Math.Abs(candle.Volume) > 0)
                {
                    if (candle.Date.Date != date.Date)
                    {
                        if (mc.Count != 0)
                        {
                            data.Add(date.Date, mc);
                        }

                        mc = new List<MinuteCandle>();
                    }

                    date = candle.Date.Date;
                    mc.Add(candle);
                }
            }

            if (mc.Count != 0)
            {
                data.Add(date.Date, mc);
            }

            return data;
        }

        public static List<DailyCandle> ReadInDailyData(string filePath, Del progressCounter, DateTime firstMinuteDataDate)
        {
            using var csvParser = SetUpParser(filePath);
            var data = new List<DailyCandle>();

            while (!csvParser.EndOfData)
            {
                var x = (csvParser.PeekChars(12));
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

        public static void WriteMinuteCsv(Dictionary<DateTime, List<MinuteCandle>> data, string filePath)
        {
            var csv = new StringBuilder();

            csv.AppendLine(Headings);

            foreach (var (_, minuteCandles) in data)
            {
                foreach (var candle in minuteCandles)
                {
                    var newLine = $"{candle.Date},{candle.Open},{candle.High},{candle.Low},{candle.Close},{candle.Volume}";
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

        private static TextFieldParser SetUpParser(string filePath)
        {
            var csvParser = new TextFieldParser(filePath) { CommentTokens = new[] { "#" } };
            csvParser.SetDelimiters(",");
            csvParser.HasFieldsEnclosedInQuotes = true;

            // Skip the row with the column names
            csvParser.ReadLine();

            return csvParser;
        }

        private static DailyCandle ParseDailyCandle(IReadOnlyList<string> fields)
        {
            var (dateTime, open, high, low, close, volume) = Parse(fields);
            return new DailyCandle(dateTime, open, high, low, close, volume);
        }

        private static MinuteCandle ParseUkMinuteCandle(IReadOnlyList<string> fields)
        {
            var (dateTime, open, high, low, close, volume) = Parse(fields);
            return new UkMinuteCandle(dateTime, open, high, low, close, volume);
        }

        private static MinuteCandle ParseUsMinuteCandle(IReadOnlyList<string> fields)
        {
            var (dateTime, open, high, low, close, volume) = Parse(fields);
            return new UsMinuteCandle(dateTime, open, high, low, close, volume);
        }

        private static (DateTime, double, double, double, double, double) Parse(IReadOnlyList<string> fields)
        {
            var dateTimeString = fields[0].Replace("GMT+0100", "+1").Replace("GMT-0000", "+0");

            DateTime.TryParseExact(dateTimeString, new[] { "yyyy-MM-dd", "dd.MM.yyyy HH:mm:ss.fff z", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy"},
                System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out var dt);

            var open = double.Parse(fields[1]);
            var high = double.Parse(fields[2]);
            var low = double.Parse(fields[3]);
            var close = double.Parse(fields[4]);
            var volume = double.Parse(fields[5]);

            return (dt, open, high, low, close, volume);
        }

        private const string Headings = "Date, Open, High, Low, Close, Volume";
    }
}
