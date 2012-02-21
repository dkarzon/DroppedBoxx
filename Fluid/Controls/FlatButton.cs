using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Drawing.GdiPlus;
using System.Drawing;

namespace Fluid.Controls
{
    /// <summary>
    /// A Derived FluidButton that renders a flat background.
    /// </summary>
    public class FlatButton : FluidButton
    {
        public FlatButton(string text)
            : base(text)
        {
        }

        protected override void PaintButtonBackground(FluidPaintEventArgs e)
        {
            Rectangle r = e.ControlBounds;
            r.Width--;
            r.Height--;
            Graphics g = e.Graphics;
            Color darkColor = BackColor;
            Color lightColor = ColorConverter.AlphaBlendColor(darkColor, Color.White, 48);

            if (!Enabled)
            {
                darkColor = ColorConverter.AlphaBlendColor(Color.Black, darkColor, 128);
                lightColor = ColorConverter.AlphaBlendColor(Color.Black, lightColor, 128);
            }

            Color beginColor = IsDown ? darkColor : lightColor;
            Color endColor = IsDown ? lightColor : darkColor;



            GdiExt.GradientFill(e.Graphics, r, beginColor, endColor, Fluid.Drawing.GdiPlus.GdiExt.FillDirection.TopToBottom);
            Pen pen = Pens.GetPen(lightColor);
            e.Graphics.DrawRectangle(pen, r);
            PerformPaintButtonContent(e);
        }
    }
}
