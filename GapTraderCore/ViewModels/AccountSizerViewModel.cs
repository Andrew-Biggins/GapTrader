using Foundations;

namespace GapTraderCore.ViewModels
{
    public sealed class AccountSizerViewModel : BindableBase
    {
        public double AccountStartSize { get; set; } = 10000;
        public double RiskPercentage { get; set; } = 1;

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
    }
}
