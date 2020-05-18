using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Foundations.Interfaces;

namespace Foundations
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
