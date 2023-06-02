using System;
using System.Windows;
using System.Windows.Input;

namespace mgtv_fulllscreen
{
    /// <summary>`
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        { 
            InitializeComponent();
           urlTextBox.Text = Properties.Settings.Default.url;
        }
        private void open(Object sender, RoutedEventArgs e)
        {
            Browser b = new Browser(urlTextBox.Text, passwordTextBox.Text);
            b.Closed += B_Closed;
            this.Hide();
            b.Show();
        }

        private void B_Closed(object sender, EventArgs e)
        {
           this.Show();
        }

        private void urlTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Properties.Settings.Default.url = urlTextBox.Text;
                Properties.Settings.Default.Save();
                this.open(sender, e);
            }
        }
    }
}
