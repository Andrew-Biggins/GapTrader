using System;
using System.Collections.Generic;
using GapTraderCore.Interfaces;
using GapTraderCore.Trades;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace GapTraderCore.ViewModelAdapters
{
    public class TradePlot : PlotModel, ITradePlot
    {
        public List<DataPoint> Points { get; }

        public TradePlot()
        {
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Balance",
                AbsoluteMinimum = 0,
                AxisTitleDistance = 10,
                MaximumPadding = 0.1,
                MinimumPadding = 0.1
            };
            Axes.Add(yAxis);

            LinearAxis xAxis = new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,
                StringFormat = "MM/dd",
                Title = "Date/Time",
                AxisTitleDistance = 10
            };
            Axes.Add(xAxis);

            var lineSeries = new MonotonicLineSeries();
            Points = lineSeries.Points;
            Series.Add(lineSeries);
        }

        public void UpdateData(double balance, IEnumerable<ITrade> trades)
        {
            var tradeList = new List<SortableTrade>();

            foreach (var trade in trades)
            {
                trade.CloseTime.IfExistsThen(x =>
                    {
                        trade.CashProfit.IfExistsThen(y =>
                        {
                            tradeList.Add(new SortableTrade(x, y));
                        });
                    });
            }

            tradeList.Sort((x, y) => DateTime.Compare(x.CloseTime, y.CloseTime));

            Points.Clear();

            foreach (var trade in tradeList)
            {
                balance += trade.Profit;

                if (balance <= 0)
                {
                    Points.Add(new DataPoint(DateTimeAxis.ToDouble(trade.CloseTime), 0));
                    break;
                }

                Points.Add(new DataPoint(DateTimeAxis.ToDouble(trade.CloseTime), balance));
            }

            InvalidatePlot(true);
        }

        private sealed class MonotonicLineSeries : LineSeries
        {
            internal MonotonicLineSeries() => IsXMonotonic = true;
        }
    }
}