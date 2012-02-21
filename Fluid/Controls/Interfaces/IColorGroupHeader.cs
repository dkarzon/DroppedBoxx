using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Fluid.Controls
{
    /// <summary>
    /// A Group Header that can specify a color for it's background.
    /// If set to Color.Empty, the default header color is used.
    /// </summary>
    public interface IColorGroupHeader:IGroupHeader
    {
        /// <summary>
        /// Gets or set the color for the group header.
        /// </summary>
        Color BackColor { get; set; }
    }
}
