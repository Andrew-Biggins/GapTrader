using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using Foundations;
using Foundations.Interfaces;
using GapTraderCore;
using GapTraderCore.ViewModels;
using static System.IO.Directory;

namespace GapTraderWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var context = GetDispatcherContext();
            var runner = new Runner(context);
            Window window = new MainWindow();
            window.Show();
            var savedData = GetSavedMarkets();
            var mainViewModel = new MainViewModel(runner, savedData);
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

        private static List<SavedData> GetSavedMarkets()
        {
            var markets = new List<SavedData>();

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            path += "\\Saved Data";
            CreateDirectory(path);

            var d = new DirectoryInfo(path);
            IFormatter formatter = new BinaryFormatter();

            foreach (var file in d.GetFiles("*.txt"))
            {
                var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                var savedData = (SavedData) formatter.Deserialize(stream);
                markets.Add(savedData);
            }

            return markets;
        }
    }
}
