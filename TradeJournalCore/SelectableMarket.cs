using TradeJournalCore.Interfaces;
using TradingSharedCore;
using TradingSharedCore.Interfaces;

namespace TradeJournalCore
{
    public sealed class SelectableMarket : SerializableBase, ISelectable
    {
        public SelectableMarket(string name = "") : base(name)
        {
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                value = !_isSelected;
                _isSelected = value;
            }
        }

        private bool _isSelected;

    }
}
