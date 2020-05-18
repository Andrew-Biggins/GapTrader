using System;
using System.Collections.Generic;
using System.Text;

namespace Foundations
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
