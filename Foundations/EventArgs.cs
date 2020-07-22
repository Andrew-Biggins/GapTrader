using System;

namespace TradingSharedCore
{
    public sealed class EventArgs<T> : EventArgs
    {
        public T Value { get; }

        public EventArgs(T value)
        {
            Value = value != null
               ? value
               : throw new ArgumentNullException(nameof(value));
        }
    }
}
