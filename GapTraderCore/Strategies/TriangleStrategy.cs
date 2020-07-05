using GapTraderCore.Interfaces;

namespace GapTraderCore.Strategies
{
    public sealed class TriangleStrategy : ISelectableStrategy
    {
        public string Name { get; }

        public bool IsSelected { get; set; }

        public string ShortName { get; }

        public TriangleStrategy(string name, string shortName, bool isStopTrailed, double stopTrailSize)
        {
            Name = name;
            ShortName = shortName;

            if (isStopTrailed)
            {
                Name += $" | {stopTrailSize} Points Trailing Stop";
                ShortName += $" | {stopTrailSize} Points Trailing Stop";
            }
        }
    }
}
