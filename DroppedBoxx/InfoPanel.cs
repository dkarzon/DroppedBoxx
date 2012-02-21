using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;
using System.Windows.Forms;
using Fluid.Classes;
using DroppedBoxx.Properties;
using DroppedBoxx.Code.Responses;

namespace DroppedBoxx
{
    public class InfoPanel : FluidPanel
    {
        private AccountInfo _accountInfo;

        private FluidHeader header = new FluidHeader();

        protected override void InitControl()
        {
            base.InitControl();
            EnableDoubleBuffer = true;
            Anchor = AnchorAll;
            Bounds = new Rectangle(0, 0, 240, 300);
            BackColor = Theme.Current.PanelBackColor;
            GradientFill = Theme.Current.PanelGradinated;

            _accountInfo = Form1.Instance.DropBox.GetAccountInfo();

            header.Title = "Info";
            header.BackColor = Theme.Current.HeaderBackColor;
            header.ForeColor = Theme.Current.HeaderForeColor;
            header.GradientFill = Theme.Current.HeaderGradianted;
            header.BackButton.Click += new EventHandler(BackButton_Click);
            header.BackButton.BackColor = Theme.Current.HeaderBackButtonBackColor;
            header.BackButton.Visible = true;
            header.BackButton.Shape = ButtonShape.Back;
            header.BackButton.TextOffset = new Point(6, 0);
            header.BackButton.Text = "Back";
            header.BackButton.GradientFill = Theme.Current.ButtonsGradianted;
            Controls.Add(header);

            //Add the controls to the panel here...
            var lblHeader = new FluidLabel();
            lblHeader.Bounds = new Rectangle(10, 30, 150, 25);
            lblHeader.Font = new Font(FontFamily.GenericSerif, 14, FontStyle.Bold);
            lblHeader.Text = "DroppedBoxx";
            Controls.Add(lblHeader);

            var lblVersion = new FluidLabel();
            lblVersion.Bounds = new Rectangle(10, 60, 150, 25);
            lblVersion.Font = new Font(FontFamily.GenericSerif, 8, FontStyle.Regular);
            lblVersion.Text = "Version: " + Form1.Version;
            Controls.Add(lblVersion);

            var lblSent = new FluidLabel();
            lblSent.Bounds = new Rectangle(10, 75, 150, 25);
            lblSent.Font = new Font(FontFamily.GenericSerif, 8, FontStyle.Regular);
            lblSent.Text = string.Format("Sent: {0:#,##0} KB", (Form1.Instance.DropBox.BytesSent / 1024));
            Controls.Add(lblSent);

            var lblRecieved = new FluidLabel();
            lblRecieved.Bounds = new Rectangle(10, 90, 150, 25);
            lblRecieved.Font = new Font(FontFamily.GenericSerif, 8, FontStyle.Regular);
            lblRecieved.Text = string.Format("Recieved: {0:#,##0} KB", (Form1.Instance.DropBox.BytesRecieved / 1024));
            Controls.Add(lblRecieved);

            var line1 = new FluidLine(0, 115, Width);
            line1.Anchor = AnchorLR;
            Controls.Add(line1);

            var lblAcHeader = new FluidLabel();
            lblAcHeader.Bounds = new Rectangle(10, 120, 150, 25);
            lblAcHeader.Font = new Font(FontFamily.GenericSerif, 14, FontStyle.Bold);
            lblAcHeader.Text = "Account";
            Controls.Add(lblAcHeader);

            if (_accountInfo != null)
            {
                var lblAcQ = new FluidLabel();
                lblAcQ.Bounds = new Rectangle(10, 145, 150, 25);
                lblAcQ.Font = new Font(FontFamily.GenericSerif, 8, FontStyle.Regular);
                lblAcQ.Text = string.Format("Quota: {0:#,##0} MB", (_accountInfo.Quota_Info.Quota / 1024 / 1024));
                Controls.Add(lblAcQ);

                var lblAcU = new FluidLabel();
                lblAcU.Bounds = new Rectangle(10, 160, 150, 25);
                lblAcU.Font = new Font(FontFamily.GenericSerif, 8, FontStyle.Regular);
                lblAcU.Text = string.Format("Used: {0:#,##0} MB", (_accountInfo.Quota_Info.Normal / 1024 / 1024));
                Controls.Add(lblAcU);

                var lblAcS = new FluidLabel();
                lblAcS.Bounds = new Rectangle(10, 175, 150, 25);
                lblAcS.Font = new Font(FontFamily.GenericSerif, 8, FontStyle.Regular);
                lblAcS.Text = string.Format("Shared: {0:#,##0} MB", (_accountInfo.Quota_Info.Shared / 1024 / 1024));
                Controls.Add(lblAcS);
            }
            else
            {
                var lblNoInfo = new FluidLabel();
                lblNoInfo.Bounds = new Rectangle(10, 145, 150, 45);
                lblNoInfo.Font = new Font(FontFamily.GenericSerif, 8, FontStyle.Regular);
                lblNoInfo.Text = string.Format("Failed to get Account Info{0}Check Data Connection", Environment.NewLine);
                Controls.Add(lblNoInfo);
            }
        }

        void BackButton_Click(object sender, EventArgs e)
        {
            //Back
            Form1.Instance.OpenFoldersPanel();
            Close(ShowTransition.None);
        }

    }
}
