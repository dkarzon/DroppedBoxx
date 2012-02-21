using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Event that occured for a FluidControl. 
    /// </summary>
    public class ControlEventArgs:EventArgs
    {
        public ControlEventArgs(FluidControl c)
        {
            Control = c;
        }

        /// <summary>
        /// Get control for which this event occured.
        /// </summary>
        public FluidControl Control { get; private set; }
    }
}
