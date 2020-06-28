using System;
using GapTraderCore.Interfaces;

namespace GapTraderCore
{
    public sealed class Day : ISelectable
    {
        public string Name { get; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                value = !_isSelected;
                _isSelected = value;
            }
        }

        public Day(DayOfWeek day)
        {
            Name = day.ToString();
        }

        private bool _isSelected;
    }
}
