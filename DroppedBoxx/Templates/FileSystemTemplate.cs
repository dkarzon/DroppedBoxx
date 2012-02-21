using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;
using System.Windows.Forms;
using DroppedBoxx.ListBoxes;
using System.IO;
using DroppedBoxx.Properties;
using DroppedBoxx.Code;

namespace DroppedBoxx.Templates
{
    public class FileSystemTemplate : FluidTemplate
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
            //image.Image = Resources.Folder;
            image.Enabled = false;
            Controls.Add(image);
        }

        protected override void OnBindValue(object value)
        {
            if (value is DirectoryInfo)
            {
                var folder = value as DirectoryInfo;
                lblPrimary.Text = folder.Name;
                lblSecondary.Text = folder.FullName;
                image.Image = Resources.Folder;
            }
            else if (value is FileInfo)
            {
                var file = value as FileInfo;
                lblPrimary.Text = file.Name;
                lblSecondary.Text = string.Empty;
                image.Image = FileHelpers.GetIcon(file.Extension);
            }
        }
    }
}
