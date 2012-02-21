using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;

namespace DroppedBoxx
{
    public class ListBoxBase : FluidListBox
    {
        protected override void InitControl()
        {
            theme = Theme.Current;
            base.InitControl();
            this.EnableDoubleBuffer = true;

            BackColor = theme.ListBackColor;
            ForeColor = theme.ListForeColor;
            BorderColor = theme.ListBorderColor;
            ScrollBarButtonColor = theme.ScrollbarColor;
            ScrollBarButtonBorderColor = theme.ScrollbarBorderColor;

        }

        Theme theme;

        public Theme Theme { get { return theme; } }

        protected override void OnPaintItem(ListBoxItemPaintEventArgs e)
        {
            if (e.IsSelected)
            {
                e.BackColor = theme.ListSelectedBackColor;
                e.ForeColor = theme.ListSelectedForeColor;
            }
            else
            {
                e.BackColor = theme.ListBackColor;
                e.ForeColor = theme.ListForeColor;
            }

            if (e.Item is GroupHeader)
            {
                e.BackColor = theme.ListHeaderBackColor;
                e.PaintDefault();
                e.Handled = true;
            }
            else if (e.IsSelected)
            {
                e.BorderColor = e.BorderColor;
                e.PaintHeaderBackground();
                e.PaintDefaultBorder();
                e.BackColor = Color.Transparent;
                e.PaintTemplate();
                e.Handled = true;
            }
            base.OnPaintItem(e);
        }

        public virtual void AddItem()
        {
        }

        public override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                switch (e.KeyCode)
                {
                    case System.Windows.Forms.Keys.Right:
                        e.Handled = NavigateForward();
                        break;
                    case System.Windows.Forms.Keys.Left:
                        e.Handled = NavigateBackward();
                        break;
                }
            }
        }


        public virtual bool NavigateForward()
        {
            return false;
        }

        public virtual bool NavigateBackward()
        {
            return false;
        }


    }
}
