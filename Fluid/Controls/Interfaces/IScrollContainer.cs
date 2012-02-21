using System;
using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Specifies a control to be a vertical scrollable container.
    /// </summary>
    public interface IVirtualContainer
    {
        /// <summary>
        /// Gets the Virtual height of this panel as none scaled value.
        /// </summary>
        int DisplayHeight { get; }

        /// <summary>
        /// Gets the virtual top offset of this panel as none scaled value.
        /// A value less than 0 means that the physical top corner is above the visible top, thus VirtualTop has usually values zero or less than 0.
        /// </summary>
        int TopOffset { get; set; }

    }
}
