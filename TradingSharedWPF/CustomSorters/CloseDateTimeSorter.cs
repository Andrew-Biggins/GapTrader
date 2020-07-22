using System.Collections;
using System.ComponentModel;
using GapTraderCore.Interfaces;

namespace TradingSharedWPF.CustomSorters
{
    public class CloseDateTimeSorter : IComparer
    {
        public CloseDateTimeSorter(ListSortDirection direction)
        {
            _direction = direction;
        }

        public int Compare(object x, object y)
        {
            var result = 0;

            var tx = (ITrade)x;
            var ty = (ITrade)y;

            var xData = tx.CloseTime;
            var yData = ty.CloseTime;

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
    }
}