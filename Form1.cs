using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Blog_robbingwithlight  
{
    public partial class robbingwithlight : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;

        private delegate IntPtr LowLevelKeyProc(int nCode, IntPtr wParam, IntPtr lParam);


        private static LowLevelKeyProc keyboardProc = KeyboardHookProc;

        private static IntPtr keyHook = IntPtr.Zero;
        private static IntPtr keyHookInstance = IntPtr.Zero;

        public static void SetHook()
        {
            if (keyHook == IntPtr.Zero)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    keyHook = SetWindowsHookEx(WH_KEYBOARD_LL, keyboardProc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        public static void UnHook()
        {
            UnhookWindowsHookEx(keyHook);
        }
                
        private static TextBox textBox;

        public robbingwithlight()
        {
            SetHook();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {            
            textBox = log;
        }
       
        private static IntPtr KeyboardHookProc(int code, IntPtr wParam, IntPtr lParam)
        {           
            if (code >= 0 && (int)wParam == 256)
            {                
                KeyCheck(Marshal.ReadInt32(lParam));
            }
            return CallNextHookEx(keyHook, code, wParam, lParam);
        }

        public static void KeyCheck(int keyCode)
        {            
            KeysConverter converter = new KeysConverter();
            textBox.AppendText(converter.ConvertToInvariantString(keyCode));        
            if (textBox.TextLength > 10000)
            {
                textBox.Text = "";
            }
        }

        private void Form1_Closing(object sender, EventArgs e)
        {            
            UnHook();
        }
    }
}