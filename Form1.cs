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
        const byte VK_SHIFT = 0x10;
        const byte VK_TAB = 0x09;
        const byte VK_SPACE = 0x20;
        const byte VK_LEFT = 0x25;
        const byte VK_DELETE = 0x2E;
        const byte VK_V = 0x56;
        const uint KEYEVENTF_KEYUP = 0x0002;

        private Timer pasteTimer;
        private Timer progressBarTimer;
        private DateTime pasteTimerStartStamp;

        private int totalMilliseconds = 0;
        private int elapsedMilliseconds = 0;

        public MainForm()
        {
            InitializeComponent();

            // 初始化剪贴板
            InitializeClipboard();

            checkProgToolStripMenuItem.CheckedChanged += CheckProgStatus_CheckedChanged;
            autoClearToolStripMenuItem.CheckedChanged += AutoClearStatus_CheckedChanged;
            autoLockToolStripMenuItem.CheckedChanged += AutoLockStatus_CheckedChanged;
            autoDialToolStripMenuItem.CheckedChanged += AutoDialStatus_CheckedChanged;

            // 设置关键操作定时器"pasteTimer"
            pasteTimer = new System.Windows.Forms.Timer();
            pasteTimer.Tick += PasteTimer_Tick;

            // Set the progress bar timer
            progressBarTimer = new System.Windows.Forms.Timer();
            progressBarTimer.Interval = 9;
            progressBarTimer.Tick += ProgressBarTimer_Tick;

            toolStripProgressBar.Maximum = totalMilliseconds;
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
                pasteTimerStartStamp = DateTime.Now;

                totalMilliseconds = pasteTimer.Interval;
                toolStripProgressBar.Maximum = totalMilliseconds;
                toolStripProgressBar.Visible = true;

                progressBarTimer.Start();

                toolStripStatusLabel1.Text = "CCHC " + textBox_.Name.Substring(textBox_.Name.Length - 2).Replace("x", "0") + " Picked";
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

            toolStripStatusLabel1.Text = toolStripStatusLabel1.Text.Substring(0, 8) + "Pasted";
        }

        private void ProgressBarTimer_Tick(object sender, EventArgs e)
        {
            // 计算已经过的时间
            elapsedMilliseconds = totalMilliseconds - (int)(DateTime.Now - pasteTimerStartStamp).TotalMilliseconds;

            // 检查计时是否结束
            if (elapsedMilliseconds <= 0)
            {
                progressBarTimer.Stop();

                toolStripProgressBar.Value = 0;
                toolStripProgressBar.Visible = false;

                return;
            }

            // 更新进度条
            toolStripProgressBar.Value = Math.Min(elapsedMilliseconds, totalMilliseconds);
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

            if (autoLockToolStripMenuItem.Checked)
            {
                // 按下并释放Tab键
                keybd_event(VK_TAB, 0, 0, UIntPtr.Zero);
                keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                // 按下并释放空格键
                keybd_event(VK_SPACE, 0, 0, UIntPtr.Zero);
                keybd_event(VK_SPACE, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                // 按下Shift键
                keybd_event(VK_SHIFT, 0, 0, UIntPtr.Zero);
                // 按下并释放Tab键
                keybd_event(VK_TAB, 0, 0, UIntPtr.Zero);
                keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                // 释放Shift键
                keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            }

            if (autoLockToolStripMenuItem.Checked && autoDialToolStripMenuItem.Checked)
            {
                ;
            }

            if (autoDialToolStripMenuItem.Checked)
            {
                ;
            }
        }

        private void InitializeClipboard()
        {
            try
            {
                // 检查剪贴板是否已有内容
                bool hasText = Clipboard.ContainsText();
                bool hasImage = Clipboard.ContainsImage();
                bool hasData = Clipboard.ContainsData(DataFormats.CommaSeparatedValue) ||
                                Clipboard.ContainsData(DataFormats.Html) ||
                                Clipboard.ContainsData(DataFormats.Rtf);

                // 如果剪贴板为空，才进行初始化
                if (!hasText && !hasImage && !hasData)
                {
                    // 设置一个Low Line 作为初始内容,然后清除
                    Clipboard.SetText("_");
                    Clipboard.Clear();
                }
                else
                {
                    // 剪贴板已有内容，不需要初始化，仅通过读取唤醒剪贴板
                    var format = Clipboard.GetText();
                }
            }
            catch (Exception)
            {
                // 剪贴板可能暂时不可用，稍后再试
                System.Threading.Thread.Sleep(99);
            }
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

        private void button1_DoubleClick(object sender, EventArgs e)
        {
            autoDialToolStripMenuItem.Checked = true;
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

        private void button4_DoubleClick(object sender, EventArgs e)
        {
            autoDialToolStripMenuItem.Checked = true;
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

        private void abstractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbstractMessageBox.Show(this, "包的(๑•̀ㅂ•)و✧́", "Pumping elephant", MessageBoxIcon.Question);
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AbstractMessageBox.Show(this, "请按下[PICK]选择您要拨打的铱星号码，在设定时间内将光标定位到甲板软件的拨号文本框控件内，稍等片刻即能为您准备好新的号码。", "How to use?", MessageBoxIcon.Information);
        }

        private void cheakProgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkProgToolStripMenuItem.Checked = !checkProgToolStripMenuItem.Checked;
        }

        private void autoClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ;
        }

        private void autoLockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoLockToolStripMenuItem.Checked = !autoLockToolStripMenuItem.Checked;
        }

        private void autoDialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoDialToolStripMenuItem.Checked = !autoDialToolStripMenuItem.Checked;
            //textBox1.Text = DateTime.Now.Ticks.ToString();
        }

        private void alwaysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (alwaysToolStripMenuItem.Checked)
            {
                alwaysToolStripMenuItem.Checked = false;
            }
            else
            {
                whileTimingToolStripMenuItem.Checked = false;
                alwaysToolStripMenuItem.Checked = true;
            }
        }

        private void whileTimingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (whileTimingToolStripMenuItem.Checked)
            {
                whileTimingToolStripMenuItem.Checked = false;
            }
            else
            {
                alwaysToolStripMenuItem.Checked = false;
                whileTimingToolStripMenuItem.Checked = true;
            }
        }

        private void CheckProgStatus_CheckedChanged(object sender, EventArgs e)
        {
            toolStripButtonCP.Checked = checkProgToolStripMenuItem.Checked;
        }

        private void AutoClearStatus_CheckedChanged(object sender, EventArgs e)
        {
            ;
        }

        private void AutoLockStatus_CheckedChanged(object sender, EventArgs e)
        {
            if(toolStripButtonAL.Checked = autoLockToolStripMenuItem.Checked)
            {
                textBox1.Text = DateTime.Now.Ticks.ToString();
            }

        }

        private void AutoDialStatus_CheckedChanged(object sender, EventArgs e)
        {
            toolStripButtonAD.Checked = autoDialToolStripMenuItem.Checked;
        }
    }

    public class CheckedChanged
    {
        private int _value2Check;

        public event EventHandler CheckValueChanged;

        public int CheckingValue
        {
            get { return _value2Check; }
            set
            {
                if (_value2Check != value)
                {
                    _value2Check = value;
                    // 触发事件
                    OnceValueChanged(EventArgs.Empty);
                }
            }
        }

        // 触发事件的方法
        protected virtual void OnceValueChanged(EventArgs e)
        {
            CheckValueChanged?.Invoke(this, e);
        }
    }
}
