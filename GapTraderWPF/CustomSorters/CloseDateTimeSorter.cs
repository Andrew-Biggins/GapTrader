using GapTraderCore.Trades;
using System.Collections;
using System.ComponentModel;

namespace GapTraderWPF.CustomSorters
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

            var tx = (JournalTrade)x;
            var ty = (JournalTrade)y;

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