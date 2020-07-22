using System;
using System.Windows;
using System.Windows.Media;
using GapTraderCore.Interfaces;
using Microsoft.Win32;
using TradeJournalCore.Interfaces;
using TradeJournalWPF.Windows;
using TradingSharedCore;
using TradingSharedCore.Interfaces;
using TradingSharedCore.Optional;
using TradingSharedWPF;
using TradingSharedWPF.Windows;
using Option = TradingSharedCore.Optional.Option;

namespace TradeJournalWPF
{
    public class TradeJournalRunner : RunnerBase, ITradeJournalRunner
    {
        public TradeJournalRunner(IContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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

        public void GetTradeDetails(object sender)
        {
            _context.Send(_ =>
            {
                var owner = GetOwner(sender);
                var window = new AddTradeWindow { DataContext = sender, Owner = owner };
                window.ShowDialog();
            });
        }


        public Optional<string> OpenSaveDialog(object sender, string fileName, string filter)
        {
            var result = Option.None<string>();

            _context.Send(_ =>
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = filter,
                    Title = "Save Data",
                    FileName = fileName
                };

                if (saveFileDialog.ShowDialog() != false)
                {
                    result = Option.Some(saveFileDialog.FileName);
                }
            });

            return result;
        }

        private readonly IContext _context;
    }
}
