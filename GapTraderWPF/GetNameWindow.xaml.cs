using System.Windows;

namespace GapTraderWPF
{
    public partial class GetNameWindow : Window
    {
        public GetNameWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
