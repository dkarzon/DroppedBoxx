using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using Fluid.Drawing.GdiPlus;
using Fluid.GdiPlus;

namespace Fluid.Controls
{
    /// <summary>
    /// Specifies the state of the ListBoxItem.
    /// </summary>
    public enum ListBoxItemState
    {
        Default = 0,
        Selected = 1,
        Pressed = 2,
        Hovered = 4,
        Transparent = 8
    }


    /// <summary>
    /// Event that occurs when to paint a ListBoxItem.
    /// </summary>
    public class ListBoxItemPaintEventArgs : ListBoxItemEventArgs
    {

        public ListBoxItemPaintEventArgs()
            : base()
        {
            StringFormat = new StringFormat();
            StringFormat.LineAlignment = StringAlignment.Center;
            StringFormat.Alignment = StringAlignment.Near;
        }

        /// <summary>
        /// Gets the FluidTemplate of the item.
        /// </summary>
        public FluidTemplate Template { get; internal set; }

        /// <summary>
        /// Gets whether the item is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return (State & ListBoxItemState.Selected) != 0; }
        }

        /// <summary>
        /// Gets whether the item is transparent.
        /// </summary>
        public bool IsTransparent
        {
            get { return (State & ListBoxItemState.Transparent) != 0; }
        }

        /// <summary>
        /// Gets whether the item is hovered.
        /// </summary>
        public bool IsHovered
        {
            get { return (State & ListBoxItemState.Hovered) != 0; }
        }

        /// <summary>
        /// Gets whether the item is pressed.
        /// </summary>
        public bool IsPressed
        {
            get { return (State & ListBoxItemState.Pressed) != 0; }
        }


        /// <summary>
        /// Gets the State of the item.,
        /// </summary>
        public ListBoxItemState State { get; internal set; }

        /// <summary>
        /// Gets the ScaleFactor for the item.
        /// </summary>
        public SizeF ScaleFactor { get; internal set; }

        /// <summary>
        /// Gets or sets the foreground color for the item.
        /// </summary>
        public Color ForeColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for the item.
        /// </summary>
        public Color BackColor { get; set; }

        /// <summary>
        /// Gets or sets the border color for the item.
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// Gets the graphics to paint the item.
        /// </summary>
        public Graphics Graphics { get; internal set; }

        public Region Region { get; internal set; }


        /// <summary>
        /// Get the bounds where to paint the item.
        /// </summary>
        public Rectangle ClientBounds { get; internal set; }

        /// <summary>
        /// Gets or sets the font for the item text.
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// Gets or sets the text for the item.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets whether the event was handled, thus no default paint will occur.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets the StringFormat for use with the text.
        /// </summary>
        public StringFormat StringFormat { get; internal set; }

        private FluidPaintEventArgs paintEvent = new FluidPaintEventArgs();

        public void PaintTemplate()
        {

            if (Template != null)
            {
                Template.BackColor = this.BackColor;
                Template.ForeColor = ForeColor;
                FluidPaintEventArgs e = paintEvent;
                e.Graphics = Graphics;
                e.Region = Region;
                e.ControlBounds = ClientBounds;
                e.ScaleFactor = ScaleFactor;
                Template.OnPaint(e);
            }
        }


        /// <summary>
        /// Paints the default background of the item.
        /// </summary>
        public void PaintDefaultBackground()
        {

            using (Brush backGround = new SolidBrush(BackColor))
            {
                Graphics g = Graphics;
                Rectangle r = ClientBounds;
                g.FillRectangle(backGround, r);
            }
            //g.Clear(BackColor);

            PaintDefaultBorder();
        }

        /// <summary>
        /// Paints the default border for the item.
        /// </summary>
        public void PaintDefaultBorder()
        {
            Graphics g = Graphics;
            Rectangle r = ClientBounds;
            r.Width--;
            if (!BorderColor.IsEmpty)
            {
                using (Pen borderPen = new Pen(BorderColor))
                {
                    g.DrawRectangle(borderPen, r);
                }
            }
        }

        /// <summary>
        /// Paints the default content (text) for the item.
        /// </summary>
        public void PaintDefaultContent()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                Graphics g = Graphics;
                Rectangle r = ClientBounds;

                r.Height--;
                r.Width--;
                r.Inflate((int)(-5 * ScaleFactor.Width), -1);
                RectangleF tr = new RectangleF((float)r.Left, (float)r.Top, (float)r.Width, (float)r.Height);
                using (Brush pen = new SolidBrush(ForeColor))
                {
                    g.DrawString(Text, Font, pen, tr, StringFormat);
                }
            }
        }

        /// <summary>
        /// Paints the default item.
        /// </summary>
        public void PaintDefault()
        {
            if (Item is IGroupHeader)
            {
                PaintGroupHeader();
            }
            else
            {
                if (Template != null)
                {
                    PaintTemplateBackground();
                    PaintTemplate();
                    PaintDefaultBorder();
                }
                else
                {
                    PaintDefaultBackground();
                    PaintDefaultContent();
                    PaintDefaultBorder();
                }
            }
        }

        internal static Color DefaultHeaderColor = Color.LightSkyBlue;

        /// <summary>
        /// Paints the header background for the item.
        /// </summary>
        public void PaintHeaderBackground()
        {
            Graphics g = Graphics;
            Rectangle r = ClientBounds;

            Color color = BackColor;
            Color startColor = ColorConverter.AlphaBlendColor(Color.White, color, 100);
            if (IsTransparent)
            {
                color = ColorConverter.AlphaColor(color, 200);
                startColor = ColorConverter.AlphaColor(startColor, 150);
                using (GraphicsPlus gp = new GraphicsPlus(g))
                {
                    PointF p1 = new PointF(r.X, r.Y);
                    PointF p2 = new PointF(r.X, r.Bottom);
                    using (LinearGradientBrush backGround = new LinearGradientBrush(p1, p2, startColor, color))
                    {
                        gp.FillRectangle(backGround, r);
                    }
                }
            }
            else
            {
                GdiExt.GradientFill(g, r, startColor, color, GdiExt.FillDirection.TopToBottom);
            }
        }

        private void PaintTemplateBackground()
        {
            Graphics g = Graphics;
            Rectangle r = ClientBounds;

            Color color = BackColor;
            if (IsTransparent)
            {
                color = ColorConverter.AlphaColor(color, 220);
                using (GraphicsPlus gp = new GraphicsPlus(g))
                {
                    using (SolidBrushPlus backGround = new SolidBrushPlus(color))
                    {
                        gp.FillRectangle(backGround, r);
                    }
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillRectangle(brush, r);
                }
            }
        }

        /// <summary>
        /// Paints a group header item.
        /// </summary>
        public void PaintGroupHeader()
        {
            IGroupHeader header = Item as IGroupHeader;
            Graphics g = Graphics;
            Rectangle r = ClientBounds;

            PaintHeaderBackground();
            Text = header.Title;
            PaintTemplateContent();
            BorderColor = BackColor;
            r.Height--;
            ClientBounds = r;
            PaintDefaultBorder();
        }

        static Font templateFont = new Font(FontFamily.GenericSansSerif, 9f, System.Drawing.FontStyle.Bold);

        private void PaintTemplateContent()
        {
            if (!string.IsNullOrEmpty(Text))
            {
                Graphics g = Graphics;
                Rectangle r = ClientBounds;

                r.Height--;
                r.Width--;
                r.Inflate((int)(-5 * ScaleFactor.Width), -1);
                RectangleF tr = new RectangleF((float)r.Left, (float)r.Top, (float)r.Width, (float)r.Height);
                tr.X += 1f;
                tr.Y += 1f;
                using (Brush pen = new SolidBrush(Color.Black))
                {
                    g.DrawString(Text, templateFont, pen, tr, StringFormat);
                }
                tr.X -= 1f;
                tr.Y -= 1f;
                using (Brush pen = new SolidBrush(Color.White))
                {
                    g.DrawString(Text, templateFont, pen, tr, StringFormat);
                }
            }
        }
    }
}
