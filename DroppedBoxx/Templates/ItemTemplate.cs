using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;
using System.Windows.Forms;
using DroppedBoxx.Models;
using DroppedBoxx.ListBoxes;
using DroppedBoxx.Properties;
using DroppedBoxx.Code;
using DroppedBoxx.ViewModels;

namespace DroppedBoxx.Templates
{
    public class ItemTemplate : FluidTemplate
    {
        private FluidLabel lblPrimary;
        private FluidLabel lblSecondary;
        private FluidButton image;

        public bool ShowPath { get; set; }

        public bool IsSyncing { get; set; }

        protected override void InitControl()
        {
            base.InitControl();

            this.Bounds = new Rectangle(0, 0, 240, 26);

            if (IsSyncing) BackColor = Color.LightGray;

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
            Controls.Add(image);
        }

        protected override void OnBindValue(object value)
        {
            if (value == null) return;

            var item = value as ItemViewModel;

            if (item == null) return;

            if (item.IsFolder)
            {
                lblPrimary.Text = item.Name;
                if (ShowPath) lblSecondary.Text = item.Path;
                image.Image = Resources.Folder_sync;
            }
            else
            {
                lblPrimary.Text = item.Name;
                if (ShowPath) lblSecondary.Text = item.Path;
                image.Image = FileHelpers.GetIcon(item.Extension);
            }
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    //Maybe do something here on enter press
                    break;
            }
        }

    }
}
