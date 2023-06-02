
using System.ComponentModel;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
            this.Closing += MainWindow_Closing;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EnableHook();
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
            
        }


        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int VK_LWIN = 0x5B;
        private const int VK_RWIN = 0x5C;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);



        private IntPtr _hookHandle = IntPtr.Zero;
        private LowLevelKeyboardProc _hookProc;

      
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            DisableHook();
        }

        private void EnableHook()
        {
            if (_hookHandle == IntPtr.Zero)
            {
                _hookProc = HookCallback;
                _hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, GetModuleHandle(null), 0);
                if (_hookHandle == IntPtr.Zero)
                {
                    // 处理钩子安装失败的情况
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        private void DisableHook()
        {
            if (_hookHandle != IntPtr.Zero)
            {
                bool result = UnhookWindowsHookEx(_hookHandle);
                _hookHandle = IntPtr.Zero;
                _hookProc = null;
                if (!result)
                {
                    // 处理钩子卸载失败的情况
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                KBDLLHOOKSTRUCT hookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (hookStruct.vkCode == VK_LWIN || hookStruct.vkCode == VK_RWIN)
                {
                    // 返回非零值以禁用 Windows 键
                    return (IntPtr)1;
                }
            }
            if (wParam == (IntPtr)0x00000104) { 
                return (IntPtr)1;
            }

            return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }
    }
}
