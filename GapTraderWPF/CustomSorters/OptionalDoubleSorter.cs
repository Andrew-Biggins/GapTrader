using System;
using Foundations.Optional;
using GapTraderCore.Trades;
using System.Collections;
using System.ComponentModel;
using GapTraderCore.Interfaces;

namespace GapTraderWPF.CustomSorters
{
    public enum TradeListColumn
    {
        CloseLevel,
        ResultInR,
        TotalPoints,
        CashProfit,
        Mae,
        Mfa,
        Drawdown,
        RealisedProfit,
        UnrealisedProfitPoints,
        UnrealisedProfitCash
    }

    public sealed class OptionalDoubleSorter : IComparer
    {
        public OptionalDoubleSorter(ListSortDirection direction, TradeListColumn column)
        {
            _direction = direction;
            _column = column;
        }

        public int Compare(object x, object y)
        {
            var result = 0;

            var tx = (ITrade)x;
            var ty = (ITrade)y;

            var xData = Option.None<double>();
            var yData = Option.None<double>();

            switch (_column)
            {
                case TradeListColumn.CloseLevel:
                    xData = tx.CloseLevel;
                    yData = ty.CloseLevel;
                    break;
                case TradeListColumn.ResultInR:
                    xData = tx.ResultInR;
                    yData = ty.ResultInR;
                    break;
                case TradeListColumn.TotalPoints:
                    xData = tx.PointsProfit;
                    yData = ty.PointsProfit;
                    break;
                case TradeListColumn.CashProfit:
                    xData = tx.CashProfit;
                    yData = ty.CashProfit;
                    break;
                case TradeListColumn.Mae:
                    xData = tx.MaximumAdverseExcursionPoints;
                    yData = ty.MaximumAdverseExcursionPoints;
                    break;
                case TradeListColumn.Mfa:
                    xData = tx.MaximumFavourableExcursionPoints;
                    yData = ty.MaximumFavourableExcursionPoints;
                    break;
                case TradeListColumn.Drawdown:
                    xData = tx.MaximumAdverseExcursionPercentageOfStop;
                    yData = ty.MaximumAdverseExcursionPercentageOfStop;
                    break;
                case TradeListColumn.RealisedProfit:
                    xData = tx.PointsProfitPercentageOfMaximumFavourableExcursion;
                    yData = ty.PointsProfitPercentageOfMaximumFavourableExcursion;
                    break;
                case TradeListColumn.UnrealisedProfitPoints:
                    xData = tx.UnrealisedProfitPoints;
                    yData = ty.UnrealisedProfitPoints;
                    break;
                case TradeListColumn.UnrealisedProfitCash:
                    xData = tx.UnrealisedProfitCash;
                    yData = ty.UnrealisedProfitCash;
                    break;
            }


            xData.IfExistsThen(px =>
            {
                yData.IfExistsThen(py =>
                {
                    if (px.CompareTo(py) < 0)
                    {
                        result = -1;
                    }

                    if (px.CompareTo(py) > 0)
                    {
                        result = 1;
                    }

                }).IfEmpty(() => { result = -1; });
            }).IfEmpty(() =>
            {
                yData.IfExistsThen(py =>
                {
                    result = 1;
                });
            });

            if (_direction == ListSortDirection.Ascending)
            {
                result *= -1;
            }

            return result;
        }

        private readonly ListSortDirection _direction;
        private readonly TradeListColumn _column;
    }
}
