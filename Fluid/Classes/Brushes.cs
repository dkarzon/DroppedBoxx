using System;

using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Drawing;

namespace Fluid.Controls
{
    /// <summary>
    /// Handles caching of brushes.
    /// </summary>
    public static class Brushes
    {
        private static HybridDictionary brushes = new HybridDictionary();

        /// <summary>
        /// Gets or creates a new brush.
        /// </summary>
        /// <param name="color">The color of the brush to get from the cache or to created.</param>
        /// <returns>A new or existing brush from the cache.</returns>
        public static SolidBrush GetBrush(Color color)
        {
            if (brushes.Contains(color)) return brushes[color] as SolidBrush;

            SolidBrush brush = new SolidBrush(color);
            brushes.Add(color, brush);
            return brush;
        }
    }
}
