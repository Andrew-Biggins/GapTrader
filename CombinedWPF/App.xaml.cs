using CombinedCore.ViewModels;
using System.Threading;
using System.Windows;
using TradingSharedCore;
using TradingSharedCore.Interfaces;

namespace CombinedWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var context = GetDispatcherContext();
            var runner = new CombinedRunner(context);
            Window window = new CombinedMainWindow();
            window.Show();
            var mainViewModel = new CombinedMainViewModel(runner);
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
