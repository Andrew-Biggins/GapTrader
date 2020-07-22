using System;

namespace TradingSharedCore.Optional
{
    public static partial class Option
    {
        public static Optional<T> Some<T>(T value)
        {
            return value is null
               ? throw new ArgumentNullException(nameof(value))
               : new Option.OptionSome<T>(value);
        }

        public static Optional<T> None<T>() => new Option.OptionNone<T>();
    }
}
