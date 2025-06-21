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
    public partial class AbstractMessageBox : Form
    {
        private Button confermButton;
        private Label messageLabel;
        private PictureBox iconPictureBox;
        public AbstractMessageBox(string message, string title, MessageBoxIcon icon)
        {
            this.Text = title;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Size = new Size(300, 150);
            this.StartPosition = FormStartPosition.Manual; // 重要：手动设置位置

            // 创建消息标签
            messageLabel = new Label();
            messageLabel.Text = message;
            messageLabel.AutoSize = true;
            messageLabel.Location = new Point(70, 30);
            messageLabel.MaximumSize = new Size(200, 0);
            this.Controls.Add(messageLabel);

            // 创建图标
            iconPictureBox = new PictureBox();
            iconPictureBox.Size = new Size(32, 32);
            iconPictureBox.Location = new Point(20, 30);

            // 设置图标
            switch (icon)
            {
                case MessageBoxIcon.Warning:
                    iconPictureBox.Image = SystemIcons.Warning.ToBitmap();
                    break;
                case MessageBoxIcon.Error:
                    iconPictureBox.Image = SystemIcons.Error.ToBitmap();
                    break;
                case MessageBoxIcon.Information:
                    iconPictureBox.Image = SystemIcons.Information.ToBitmap();
                    break;
                case MessageBoxIcon.Question:
                    iconPictureBox.Image = global::IridiumDialAuxiPaste.Properties.Resources.PumpingElephant;
                    break;
            }
            this.Controls.Add(iconPictureBox);

            // 创建OK按钮
            confermButton = new Button();
            confermButton.Text = "真不错";
            confermButton.DialogResult = DialogResult.OK;
            confermButton.Location = new Point(110, 80);
            this.Controls.Add(confermButton);
            this.AcceptButton = confermButton;
        }

        // 静态方法用于显示消息框
        public static DialogResult Show(Form owner, string message, string title, MessageBoxIcon icon)
        {
            using (AbstractMessageBox msgBox = new AbstractMessageBox(message, title, icon))
            {
                // 计算并设置位置使其在父窗口中居中
                if (owner != null)
                {
                    Point centerPoint = new Point(
                        owner.Location.X + (owner.Width / 2),
                        owner.Location.Y + (owner.Height / 2)
                    );

                    msgBox.Location = new Point(
                        centerPoint.X - (msgBox.Width / 2),
                        centerPoint.Y - (msgBox.Height / 2)
                    );
                }

                return msgBox.ShowDialog(owner);
            }
        }
    }
}
