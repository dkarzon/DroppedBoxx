using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Fluid.Controls
{
    /// <summary>
    /// Event that occurs for Layout.
    /// </summary>
    public class LayoutEventArgs:EventArgs
    {
        public LayoutEventArgs(SizeF scaleFactor)
            : base()
        {
            this.ScaleFactor = scaleFactor;
        }

        /// <summary>
        /// Sets the ScaleFactor.
        /// </summary>
        public SizeF ScaleFactor { private get; set; }
    }
}
