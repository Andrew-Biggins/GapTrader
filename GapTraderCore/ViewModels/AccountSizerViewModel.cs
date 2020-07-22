using TradingSharedCore;

namespace GapTraderCore.ViewModels
{
    public sealed class AccountSizerViewModel : BindableBase
    {
        public double AccountStartSize { get; set; } = 10000;
        public double RiskPercentage { get; set; } = 1;

        public bool AccountStartSizeHasError
        {
            get => _accountStartSizeHasError;
            set => SetProperty(ref _accountStartSizeHasError, value, nameof(AccountStartSizeHasError));
        }

        public bool RiskPercentageHasError
        {
            get => _riskPercentageHasError;
            set => SetProperty(ref _riskPercentageHasError, value, nameof(RiskPercentageHasError));
        }

        public bool Compound
        {
            get => _compound;
            set
            {
                value = !_compound;
                SetProperty(ref _compound, value);
            }
        }

        private bool _compound = true;
        private bool _accountStartSizeHasError;
        private bool _riskPercentageHasError;
    }
}
