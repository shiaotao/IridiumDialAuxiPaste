using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace IridiumDialAuxiPaste
{
    public partial class MainForm : Form
    {
        // Windows API 声明
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        
        // 按键常量定义
        const byte VK_CONTROL = 0x11;
        const byte VK_LEFT = 0x25;
        const byte VK_DELETE = 0x2E;
        const byte VK_V = 0x56;
        const uint KEYEVENTF_KEYUP = 0x0002;

        private Timer pasteTimer;

        public MainForm()
        {
            InitializeComponent();

            // 设置定时器"pasteTimer"
            pasteTimer = new System.Windows.Forms.Timer();
            pasteTimer.Tick += PasteTimer_Tick;
        }

        private void Pick2CopyText(TextBox textBox_)
        {
            // 检查TextBox_中是否有内容，将内容复制到剪贴板
            if (!string.IsNullOrEmpty(textBox_.Text))
            {
                textBox_.Focus();
                textBox_.SelectAll();

                Clipboard.SetText(textBox_.Text);

                float intervalSeconds = (float)secUpDown.Value;

                // 设置定时器间隔并启动
                pasteTimer.Interval = Convert.ToInt16(intervalSeconds * 1000);
                pasteTimer.Start();

            }
            else
            {
                MessageBox.Show("Picked Empty!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PasteTimer_Tick(object sender, EventArgs e)
        {
            // This method is subscribed by pasteTimer.Tick
            pasteTimer.Stop();

            SendKeysPress();
        }

        private void SendKeysPress()
        {
            // 按下Ctrl
            keybd_event(VK_CONTROL, 0, 0, UIntPtr.Zero);
            // 短按Left
            keybd_event(VK_LEFT, 0, 0, UIntPtr.Zero);
            keybd_event(VK_LEFT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            // 短按DELETE
            keybd_event(VK_DELETE, 0, 0, UIntPtr.Zero);
            keybd_event(VK_DELETE, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            // 短按V
            keybd_event(VK_V, 0, 0, UIntPtr.Zero);
            keybd_event(VK_V, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            // 释放Ctrl
            keybd_event(VK_CONTROL, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 检查TextBox中是否有内容
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                // 将TextBox内容复制到剪贴板
                Clipboard.SetText(textBox1.Text);
            }
            else
            {
                // 提示TextBox为空
                MessageBox.Show("Picked Empty!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Pick2CopyText(textBox2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Pick2CopyText(textBox3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Pick2CopyText(textBox4);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Pick2CopyText(textBox5);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Pick2CopyText(textBox6);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Pick2CopyText(textBox7);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Pick2CopyText(textBox8);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Pick2CopyText(textBox9);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Pick2CopyText(textBox10);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form != this) // 避免立即关闭当前窗口
                {
                    form.Close();
                }
            }

            // 关闭可能正在运行的后台任务
            pasteTimer.Stop();
            pasteTimer.Dispose();

            // 退出应用程序
            Application.Exit();
        }
    }
}
