using System;
using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Specifies a control to be a group header for a listbox.
    /// </summary>
    public interface IGroupHeader
    {
        /// <summary>
        /// Gets the titleof the group header.
        /// </summary>
        string Title { get; }
    }
}
