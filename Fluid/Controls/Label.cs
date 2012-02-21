using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Fluid.Native;

namespace Fluid.Controls
{
    public class FluidLabel : FluidControl
    {
        public FluidLabel()
            : base()
        {
        }

        protected override void InitControl()
        {
            stringFormat.FormatFlags = StringFormatFlags.NoWrap;
            BackColor = Color.Transparent;
        }

        public FluidLabel(string text, int x, int y, int width, int height)
            : base()
        {
            this.text = text;
            this.Bounds = new Rectangle(x, y, width, height);
        }

        private string text = "";

        /// <summary>
        /// Gets or sets the text of the control.
        /// </summary>
        public string Text
        {
            get { return text ?? string.Empty; }
            set
            {
                if (text != value)
                {
                    text = value;
                    OnTextChanged();
                }
            }
        }

        protected StringFormat stringFormat = new StringFormat(StringFormatFlags.NoWrap);

        public StringAlignment Alignment
        {
            get { return stringFormat.Alignment; }
            set
            {
                if (Alignment != value)
                {
                    stringFormat.Alignment = value;
                    Invalidate();
                }
            }
        }

        public StringAlignment LineAlignment
        {
            get { return stringFormat.LineAlignment; }
            set
            {
                if (LineAlignment != value)
                {
                    stringFormat.LineAlignment = value;
                    Invalidate();
                }
            }
        }



        /// <summary>
        /// Occurs when the text property is changed.
        /// </summary>
        protected virtual void OnTextChanged()
        {
            Invalidate();
            if (TextChanged != null) TextChanged(this, EventArgs.Empty);
        }

        public event EventHandler TextChanged;



        public override void OnPaint(FluidPaintEventArgs e)
        {
            base.OnPaint(e);
            PaintText(e);
        }

        public Color ShadowColor { get; set; }

        private SolidBrush brush = new SolidBrush(Color.Black);

        public StringFormat Format { get { return stringFormat; } }

        protected void PaintDefaultText(FluidPaintEventArgs e, string text)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = e.ControlBounds;
            bounds.Inflate(e.ScaleX(-3), e.ScaleY(-1));
            //bounds.Y += e.ScaleY(-1);
            if (bounds.Height > 0 && bounds.Width > 0)
            {
                if (ShadowColor.IsEmpty)
                {
                    brush.Color = Enabled ? ForeColor : Color.Silver;
                    g.DrawString(text, Font, brush, RectFFromRect(bounds), stringFormat);
                }
                else
                {
                    Fluid.Drawing.GdiPlus.GdiExt.DrawStringShadow(g, text, Font, ForeColor, ShadowColor, bounds, stringFormat);
                }
            }
        }


        protected virtual void PaintText(FluidPaintEventArgs e)
        {
            if (!string.IsNullOrEmpty(Text))
            {
                PaintDefaultText(e, Text);
            }
        }
    }
}
