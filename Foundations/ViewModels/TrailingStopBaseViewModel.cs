namespace TradingSharedCore.ViewModels
{
    public abstract class TrailingStopBaseViewModel : BindableBase
    {
        public bool IsStopTrailedForwarder
        {
            get => _isStopTrailedForwarder;
            set
            {
                value = !_isStopTrailedForwarder;
                SetProperty(ref _isStopTrailedForwarder, value);
                IsStopTrailed = _isStopTrailedForwarder;
            }
        }

        public bool IsStopTrailed { get; set; }

        public double TrailedStopSize { get; set; } = 20;

        private bool _isStopTrailedForwarder;
    }
}
