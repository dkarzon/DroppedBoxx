using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Classes;

namespace Fluid.Controls
{
    /// <summary>
    /// Event that occurs on a NumericPanel.
    /// </summary>
    public class NumericPanelEventArgs:HandledEventArgs
    {
        /// <summary>
        /// Gets the index of the button that caused the event.
        /// </summary>
        public int ButtonIndex { get; internal set; }
    }
}
