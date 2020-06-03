namespace GapAnalyser.VariableSelectors
{
    public sealed class StaticStrategyVariableSelector : VariableSelector
    {
        public int MaxMinGapSize { get; set; } = 1000;
        public int MinMinGapSize { get; set; } = 10;
        public int GapSizeIncrement { get; set; } = 100;

        public bool ApplyDateTimeFilters
        {
            get => _applyDateTimeFilters;
            set
            {
                value = !_applyDateTimeFilters;
                SetProperty(ref _applyDateTimeFilters, value);
            }
        }

        private bool _applyDateTimeFilters;
    }
}
