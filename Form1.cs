using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IridiumDialAuxiPaste
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Pick2CopyText(TextBox textBox_)
        {
            // 检查TextBox_中是否有内容，将内容复制到剪贴板
            if (!string.IsNullOrEmpty(textBox_.Text))
            {
                textBox_.Focus();

                Clipboard.SetText(textBox_.Text);
            }
            else
            {
                MessageBox.Show("Picked Empty!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

    }
}
