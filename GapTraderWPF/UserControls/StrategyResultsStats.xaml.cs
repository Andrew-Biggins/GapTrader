﻿using System.Windows.Controls;
using System.Windows.Input;
using GapTraderCore.ViewModels;
using TradingSharedCore.ViewModels;

namespace GapTraderWPF.UserControls
{
    public partial class StrategyResultsStats : UserControl
    {
        public StrategyResultsStats()
        {
            InitializeComponent();
        }

        private void OnMouseLeftButtonUpOnStrategyResults(object sender, MouseButtonEventArgs e)
        {
            var vm = (StrategyResultsStatsViewModel)DataContext;
            vm.MoreDetailsCommand.Execute(null);
        }
    }
}
