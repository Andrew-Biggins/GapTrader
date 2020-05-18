using System;

namespace Foundations.Optional
{
    public static partial class Option
    {
        public static Optional<T> Some<T>(T value)
        {
            return value is null
               ? throw new ArgumentNullException(nameof(value))
               : new OptionSome<T>(value);
        }

        public static Optional<T> None<T>() => new OptionNone<T>();
    }
}
