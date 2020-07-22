using System.Threading;
using System.Windows;
using TradeJournalCore.ViewModels;
using TradingSharedCore;
using TradingSharedCore.Interfaces;

namespace TradeJournalWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var context = GetDispatcherContext();
            var runner = new TradeJournalRunner(context);
            Window window = new TradeJournalMainWindow();
            window.Show();
            var mainViewModel = new TradeJournalViewModel(runner);
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
