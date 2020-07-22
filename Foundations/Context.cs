using System;
using System.Threading;
using TradingSharedCore.Interfaces;

namespace TradingSharedCore
{
    public sealed class Context : IContext
    {
        public Context(SynchronizationContext context)
        {
            _actual = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Send(SendOrPostCallback callback) => _actual.Send(callback, null);

        private readonly SynchronizationContext _actual;
    }
}
