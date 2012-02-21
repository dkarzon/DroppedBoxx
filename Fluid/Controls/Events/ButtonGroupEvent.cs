using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Controls.Classes;

namespace Fluid.Controls
{
    /// <summary>
    /// Event that occurs on a ButtonGroup.
    /// </summary>
    public class ButtonGroupEventArgs:EventArgs
    {
        /// <summary>
        /// Gets the index of the affected button in the group.
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// Gets the affected button.
        /// </summary>
        public FluidButton Button { get; internal set; }
    }
}
