using System.Threading;

namespace TradingSharedCore.Interfaces
{
    public interface IContext
    {
        void Send(SendOrPostCallback callback);
    }
}