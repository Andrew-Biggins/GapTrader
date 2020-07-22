using System;
using System.ComponentModel;

namespace TradingSharedCore
{
#nullable enable
    public static class EventRaiser
    {
        public static void Raise(this EventHandler? handler, object sender)
        {
            handler?.Invoke(sender, EventArgs.Empty);
        }

        public static void Raise<T>(this EventHandler<EventArgs<T>>? handler, object sender, T value)
        {
            handler?.Invoke(sender, new EventArgs<T>(value));
        }

        public static void Raise<T>(this EventHandler<EventArgs<T>>? handler, object sender, EventArgs<T> args)
        {
            handler?.Invoke(sender, args);
        }

        public static void Raise<T>(this EventHandler<T>? handler, object sender, T args)
        {
            handler?.Invoke(sender, args);
        }

        public static void Raise(this PropertyChangedEventHandler? handler, object sender, string propertyName)
        {
            handler?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
        }
    }
#nullable disable
}
