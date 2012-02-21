using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;
using DroppedBoxx.Sync;

namespace DroppedBoxx
{
    public class SyncPanel : FluidPanel
    {
        public EventHandler UpdateStatus;

        private FluidLabel lblFolder;
        private FluidLabel lblFile;
        private FluidLabel lblSent;
        private FluidLabel lblRecieved;

        protected override void InitControl()
        {
            base.InitControl();
            BackColor = Color.White;
            Anchor = System.Windows.Forms.AnchorStyles.None;

            lblFolder = new FluidLabel();
            lblFolder.Bounds = new Rectangle(10, 10, 240, 35);
            lblFolder.Font = new Font(FontFamily.GenericSerif, 9, FontStyle.Regular);
            Controls.Add(lblFolder);

            lblFile = new FluidLabel();
            lblFile.Bounds = new Rectangle(10, 40, 240, 20);
            lblFile.Font = new Font(FontFamily.GenericSerif, 9, FontStyle.Bold);
            Controls.Add(lblFile);

            lblSent = new FluidLabel();
            lblSent.Bounds = new Rectangle(10, 65, 180, 20);
            lblSent.Font = new Font(FontFamily.GenericSerif, 9, FontStyle.Regular);
            Controls.Add(lblSent);

            lblRecieved = new FluidLabel();
            lblRecieved.Bounds = new Rectangle(10, 80, 180, 20);
            lblRecieved.Font = new Font(FontFamily.GenericSerif, 9, FontStyle.Regular);
            Controls.Add(lblRecieved);

            UpdateStatus += new EventHandler(OnStatusUpdate);

            OnStatusUpdate(this, null);
        }

        protected void OnStatusUpdate(object sender, EventArgs e)
        {
            lblFolder.Text = "Current Folder: " + Environment.NewLine + Syncer.Instance.CurrentFolderPath;

            switch (Syncer.Instance.SyncStatus)
            {
                case SyncStatus.Checking:
                    lblFile.Text = "Checking ";
                    lblFile.ForeColor = Color.DarkOrange;
                    break;
                case SyncStatus.Downloading:
                    lblFile.Text = "Downloading ";
                    lblFile.ForeColor = Color.Maroon;
                    break;
                case SyncStatus.Uploading:
                    lblFile.Text = "Uploading ";
                    lblFile.ForeColor = Color.DarkGreen;
                    break;
            }
            lblFile.Text += Syncer.Instance.CurrentFileName;

            if (Syncer.Instance.DropBox != null)
            {
                lblSent.Text = string.Format("Sent: {0:#,##0} KB", (Syncer.Instance.DropBox.BytesSent / 1024));
                lblRecieved.Text = string.Format("Recieved: {0:#,##0} KB", (Syncer.Instance.DropBox.BytesRecieved / 1024));
            }
        }

        protected override void OnSizeChanged(Size oldSize, Size newSize)
        {
            //base.OnSizeChanged(oldSize, newSize);
        }

    }
}
