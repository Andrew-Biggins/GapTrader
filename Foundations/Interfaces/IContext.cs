using System.Threading;

namespace Foundations.Interfaces
{
    public interface IContext
    {
        void Send(SendOrPostCallback callback);
    }
}