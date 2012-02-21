using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;
using System.Windows.Forms;
using DroppedBoxx.ListBoxes;
using System.IO;
using DroppedBoxx.Properties;

namespace DroppedBoxx.Templates
{
    public class AddFolderTemplate : FluidTemplate
    {
        private FluidLabel lblPrimary;
        private FluidLabel lblSecondary;
        private FluidButton image;

        protected override void InitControl()
        {
            base.InitControl();

            this.Bounds = new Rectangle(0, 0, 240, 26);

            lblPrimary = new FluidLabel("", 20, 0, 220, 20);
            lblPrimary.LineAlignment = StringAlignment.Near;
            lblPrimary.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            lblPrimary.Font = Theme.Current.ListPrimaryFont;
            Controls.Add(lblPrimary);

            lblSecondary = new FluidLabel("", 20, 14, 220, 14);
            lblSecondary.LineAlignment = StringAlignment.Near;
            lblSecondary.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            lblSecondary.Font = Theme.Current.ListSecondaryFont;
            lblSecondary.ForeColor = Theme.Current.ListSecondaryForeColor;
            Controls.Add(lblSecondary);

            image = new FluidButton("", 2, 3, 16, 16);
            image.Anchor = AnchorStyles.None;
            image.BackColor = Color.Transparent;
            image.Shape = ButtonShape.Flat;
            image.Enabled = false;
            image.Image = Resources.Folder;
            Controls.Add(image);
        }

        protected override void OnBindValue(object value)
        {
            var folder = value as DirectoryInfo;
            if (folder != null)
            {
                lblPrimary.Text = folder.Name;
                lblSecondary.Text = folder.FullName;
            }
        }
    }
}
