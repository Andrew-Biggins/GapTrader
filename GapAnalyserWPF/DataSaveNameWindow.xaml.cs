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

namespace GapAnalyserWPF
{
    /// <summary>
    /// Interaction logic for DataSaveNameWindow.xaml
    /// </summary>
    public partial class DataSaveNameWindow : Window
    {
        public DataSaveNameWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
