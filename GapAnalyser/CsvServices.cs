using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using GapAnalyser.Candles;

namespace GapAnalyser
{
    public static class CsvServices
    {
        public static TextFieldParser SetUpParser(string filePath)
        {
            var csvParser = new TextFieldParser(filePath) { CommentTokens = new[] { "#" } };
            csvParser.SetDelimiters(",");
            csvParser.HasFieldsEnclosedInQuotes = true;

            // Skip the row with the column names
            csvParser.ReadLine();

            return csvParser;
        }

        public static DailyCandle ParseDailyCandle(string[] fields)
        {
            var (dateTime, open, high, low, close, volume) = Parse(fields);
            return new DailyCandle(dateTime, open, high, low, close, volume);
        }

        public static MinuteCandle ParseUkMinuteCandle(string[] fields)
        {
            var (dateTime, open, high, low, close, volume) = Parse(fields);
            return new UkMinuteCandle(dateTime, open, high, low, close, volume);
        }

        public static MinuteCandle ParseUsMinuteCandle(string[] fields)
        {
            var (dateTime, open, high, low, close, volume) = Parse(fields);
            return new UsMinuteCandle(dateTime, open, high, low, close, volume);
        }

        private static (DateTime, double, double, double, double, double) Parse(IReadOnlyList<string> fields)
        {
            var dateTimeString = fields[0].Replace("GMT+0100", "+1").Replace("GMT-0000", "+0");

            DateTime.TryParseExact(dateTimeString, new[] { "yyyy-MM-dd", "dd.MM.yyyy HH:mm:ss.fff z" }, System.Globalization.CultureInfo.CurrentCulture,
                System.Globalization.DateTimeStyles.None, out var dt);

            var open = double.Parse(fields[1]);
            var high = double.Parse(fields[2]);
            var low = double.Parse(fields[3]);
            var close = double.Parse(fields[4]);
            var volume = double.Parse(fields[5]);

            return (dt, open, high, low, close, volume);
        }
    }
}
