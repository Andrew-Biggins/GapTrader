using System;

namespace Foundations.Optional
{
    public static partial class Option
    {
        private class OptionNone<T> : Optional<T>
        {
            public override Optional<T> IfExistsThen(Action<T> action) => this;

            public override void IfEmpty(Action whenNone) => whenNone();
        }
    }
}
