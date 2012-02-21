using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Fluid.Native;

namespace Fluid.Controls
{
    public class FluidLine : FluidControl
    {
        private Pen pen = new Pen(Color.Black);

        public FluidLine()
            : base()
        {
        }

        protected override void InitControl()
        {
            BackColor = Color.Transparent;
        }

        public FluidLine(int x, int y, int width)
            : base()
        {
            this.Bounds = new Rectangle(x, y, width, 10);
        }

        public override void OnPaint(FluidPaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Rectangle bounds = e.ControlBounds;
            bounds.Inflate(e.ScaleX(-3), e.ScaleY(-1));
            //bounds.Y += e.ScaleY(-1);
            if (bounds.Height > 0 && bounds.Width > 0)
            {
                pen.Color = Enabled ? ForeColor : Color.Silver;
                g.DrawLine(pen, bounds.X, bounds.Y, bounds.X + bounds.Width, bounds.Y);
            }
        }

    }
}
