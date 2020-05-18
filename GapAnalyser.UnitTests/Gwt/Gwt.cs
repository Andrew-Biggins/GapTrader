using Xunit;

namespace GapAnalyser.MicroTests.Gwt
{
    public sealed class Gwt : FactAttribute
    {
        public Gwt(string given, string when, string then)
        {
            DisplayName = $"{given}, {when}, {then}.";
        }
    }

    public sealed class GwtTheory : TheoryAttribute
    {
        public GwtTheory(string given, string when, string then)
        {
            DisplayName = $"{given}, {when}, {then}.";
        }
    }

    public sealed class GwtWpf : WpfFactAttribute
    {
        public GwtWpf(string given, string when, string then)
        {
            DisplayName = $"{given}, {when}, {then}.";
        }
    }

    public sealed class GwtWpfTheory : WpfTheoryAttribute
    {
        public GwtWpfTheory(string given, string when, string then)
        {
            DisplayName = $"{given}, {when}, {then}.";
        }
    }
}
