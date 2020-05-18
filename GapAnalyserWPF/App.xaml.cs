using Foundations;
using Foundations.Interfaces;
using GapAnalyser.ViewModels;
using System.Threading;
using System.Windows;

namespace GapAnalyserWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var context = GetDispatcherContext();
            var runner = new Runner(context);
            Window window = new MainWindow();
            window.Show();
            var mainViewModel = new MainViewModel(runner);
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
