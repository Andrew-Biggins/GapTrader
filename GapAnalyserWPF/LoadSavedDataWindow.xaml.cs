using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GapAnalyser.ViewModels;

namespace GapAnalyserWPF
{
    /// <summary>
    /// Interaction logic for LoadSavedDataWindow.xaml
    /// </summary>
    public partial class LoadSavedDataWindow : Window
    {
        public LoadSavedDataWindow()
        {
            InitializeComponent();
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = (DataUploaderViewModel)DataContext;
            vm.DeserializeDataCommand.Execute(null);
            Close();
        }
    }
}
