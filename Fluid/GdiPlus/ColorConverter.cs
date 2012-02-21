using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Fluid.Drawing.GdiPlus
{
    public static class ColorConverter
    {
        /// <summary>
        /// Gets the Color as a result of a background color overlapped by a color with a transparency.
        /// </summary>
        /// <param name="backColor">The background color.</param>
        /// <param name="blendColor">The overlapped color.</param>
        /// <param name="alpha">The alphablend value between 0 and 255.</param>
        /// <returns>A combinded color.</returns>
        public static Color AlphaBlendColor(Color backColor, Color blendColor, int alpha)
        {
            int r = Blend(blendColor.R, backColor.R, alpha);
            int g = Blend(blendColor.G, backColor.G, alpha);
            int b = Blend(blendColor.B, backColor.B, alpha);

            return Color.FromArgb(r, g, b);
        }


        private static int Blend(int value1, int value2, int alpha)
        {
            int d = value1 - value2;
            return (d * alpha) / 255 + value2;
        }

        /// <summary>
        /// Converts a 24 or less bit color to a 32 bit color with a alpha channel of 255 (opaque).
        /// </summary>
        /// <param name="color">The color without alpha channel.</param>
        /// <returns>The same color, but with alpha channel set to 255.</returns>
        public static Color OpaqueColor(Color color)
        {
            int argb = color.ToArgb() | (0xff << 24);
            return Color.FromArgb(argb);
        }

        /// <summary>
        /// Converts a 24 or less bit color to a 32 bit color with a specific alpha channel.
        /// </summary>
        /// <param name="c">The rgb color to convert.</param>
        /// <param name="alpha">The alpha value for the color.</param>
        /// <returns>The new color with alpha channel.</returns>
        public static Color AlphaColor(Color c, int alpha)
        {
            uint ualpha = (uint)alpha;
            ualpha = ualpha << 24;
            int mask = (int)ualpha;
            int v = c.B | (c.G << 8) | (c.R << 0x10) | mask;

            Color result = Color.FromArgb(v);
            return result;

        }
    }
}
