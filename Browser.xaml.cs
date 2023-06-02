
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace mgtv_fulllscreen
{
    /// <summary>
    /// Browser.xaml 的交互逻辑
    /// </summary>
    public partial class Browser : Window
    {
        private string url;
        private string password = "mgtv";
        private string input = "";
        public Browser(string url, string password)
        {
            InitializeComponent();

            Keyboard.AddKeyDownHandler(this, OnKeyDown);

            this.url = url;
            this.password = password.ToLower();
            this.Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            browser.Address = this.url;
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            
            // 判断按下的是否连续按下了密码
            if (char.IsDigit((char)KeyInterop.VirtualKeyFromKey(e.Key)) || char.IsLetter((char)KeyInterop.VirtualKeyFromKey(e.Key)))
            {
                this.input = (this.input + e.Key).ToLower();
                if(!password.StartsWith(this.input))
                {
                    this.input = "";
                }
                else if (this.input == this.password)
                {
                    this.Close();
                }
            }
            if (Keyboard.Modifiers != ModifierKeys.None)
            {
                e.Handled = true;
                return;
            }
            
        }
    }
}
