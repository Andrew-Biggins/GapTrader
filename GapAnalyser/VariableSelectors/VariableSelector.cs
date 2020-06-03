using Foundations;

namespace GapAnalyser.VariableSelectors
{
    public abstract class VariableSelector : BindableBase
    {
        public int MaxStop { get; set; } = 1000;
        public int MinStop { get; set; } = 10;
        public int StopIncrement { get; set; } = 100;

        public AccountSizer AccountSizer { get; } = new AccountSizer();
    }
}
