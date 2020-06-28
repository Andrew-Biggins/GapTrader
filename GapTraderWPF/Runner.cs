using System;
using System.Windows;
using System.Windows.Media;
using Foundations.Interfaces;
using GapTraderCore;
using GapTraderCore.Interfaces;

namespace GapTraderWPF
{
    internal sealed class Runner : IRunner
    {
        public Runner(IContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void ShowTrades(object sender, IStrategy strategy)
        {
            _context.Send(_ =>
            {
                var owner = GetOwner(sender);
                var window = new TradeListWindow { DataContext = strategy, Owner = owner };
                window.ShowDialog();
            });
        }

        public void GetName(object sender, string title)
        {
            _context.Send(_ =>
            {
                var owner = GetOwner(sender);
                var window = new GetNameWindow { DataContext = sender, Owner = owner, Title = title};
                window.ShowDialog();
            });
        }

        public void GetStrategyDetails(object sender)
        {
            _context.Send(_ =>
            {
                var owner = GetOwner(sender);
                var window = new AddStrategyWindow { DataContext = sender, Owner = owner };
                window.ShowDialog();
            });
        }

        public void ShowLoadSavedDataWindow(object sender)
        {
            _context.Send(_ =>
            {
                var owner = GetOwner(sender);
                var window = new LoadSavedDataWindow { DataContext = sender, Owner = owner };
                window.ShowDialog();
            });
        }

        public void ShowGraphWindow(object sender)
        {
            _context.Send(_ =>
            {
                var owner = GetOwner(sender);
                var window = new GraphWindow { DataContext = sender, Owner = owner };
                window.ShowDialog();
            });
        }

        public void ShowSaveDataWindow(object sender)
        {
            _context.Send(_ =>
            {
                var owner = GetOwner(sender);
                var window = new SaveDataWindow { DataContext = sender, Owner = owner };
                window.ShowDialog();
            });
        }

        public void ShowUploadNewDataWindow(object sender)
        {
            _context.Send(_ =>
            {
                var owner = GetOwner(sender);
                var window = new UploadDataWindow { DataContext = sender, Owner = owner };
                window.ShowDialog();
            });
        }

        public void GetTradeDetails(object sender)
        {
            _context.Send(_ =>
            {
                var owner = GetOwner(sender);
                var window = new AddTradeWindow { DataContext = sender, Owner = owner };
                window.ShowDialog();
            });
        }

        public void Run(object sender, IRunnable runnable)
        {
            _context.Send(_ =>
            {
                var owner = GetOwner(sender);
                var window = new GenericWindow { DataContext = runnable, Owner = owner };
                window.ShowDialog();
            });
        }

        public void RunConcurrently(object sender, IRunnable runnable)
        {
            _context.Send(_ =>
            {
                var owner = GetOwner(sender);
                var window = new GenericWindow { DataContext = runnable, Owner = owner };
                window.Show();
            });
        }

        public void Run(object sender, Message message)
        {
            _context.Send(_ =>
            {
                var owner = GetOwner(sender);
                MessageBox.Show(owner, message.ContentKey, message.NameKey, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            });
        }

        public bool RunForResult(object sender, Message message)
        {
            var result = false;

            _context.Send(_ =>
            {
                var owner = GetOwner(sender);

                result = MessageBox.Show(owner, message.ContentKey, message.NameKey,
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Information) == MessageBoxResult.Yes;
            });

            return result;
        }

        /// <summary>
        /// Searches the visual tree for an element whose data context is the sender, and
        /// returns the containing window. If no such element is found, then returns the 
        /// first window in the application's collection of windows. If there are no windows,
        /// returns null.
        /// </summary>
        private static Window? GetOwner(object sender)
        {
            bool HasDataContext(DependencyObject d) => d is FrameworkElement element &&
                                                       element.DataContext == sender;

            bool SenderIsInTree(DependencyObject parent)
            {
                if (HasDataContext(parent))
                {
                    return true;
                }

                var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

                for (var index = 0; index < childrenCount; ++index)
                {
                    var child = VisualTreeHelper.GetChild(parent, index);

                    if (HasDataContext(child) ||
                        SenderIsInTree(child))
                    {
                        return true;
                    }
                }

                return false;
            }

            Window? SearchWindows()
            {
                foreach (var o in Application.Current.Windows)
                {
                    if (o is MainWindow root && SenderIsInTree(root))
                    {
                        return root;
                    }
                }

                return Application.Current.Windows.Count > 0
                   ? Application.Current.Windows[0]
                   : null;
            }

            var owner = SearchWindows();
            return owner;
        }

        private readonly IContext _context;
    }
}
