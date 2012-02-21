using System;

using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Drawing;

namespace Fluid.Controls
{
    /// <summary>
    /// Manages caching of Pens.
    /// </summary>
    public static class Pens
    {
        private static HybridDictionary pens = new HybridDictionary();

        /// <summary>
        /// Gets or creates a new pen.
        /// </summary>
        /// <param name="color">The color of the pen to retreive from the cache or to create.</param>
        /// <returns>A new or existing Pen.</returns>
        public static Pen GetPen(Color color)
        {
            if (pens.Contains(color)) return pens[color] as Pen;

            Pen pen = new Pen(color);
            pens.Add(color, pen);
            return pen;
        }
    }
}
