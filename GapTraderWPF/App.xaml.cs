using GapTraderCore.ViewModels;
using System.Threading;
using System.Windows;
using GapTraderWPF.Windows;
using TradingSharedCore;
using TradingSharedCore.Interfaces;

namespace GapTraderWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var context = GetDispatcherContext();
            var runner = new GapTraderRunner(context);
            Window window = new GapTraderMainWindow();
            window.Show();
            var mainViewModel = new GapTraderMainViewModel(runner);
            window.DataContext = mainViewModel;
        }

        private IContext GetDispatcherContext()
        {
            var dispatcherContext = Dispatcher?.Invoke(() => SynchronizationContext.Current);

            if (dispatcherContext == null)
            {
                throw new ThreadStateException("Could not get dispatcher synchronisation context.");
            }

            var context = new Context(dispatcherContext);
            return context;
        }
    }
}
