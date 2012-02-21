using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Fluid.Controls
{
    /// <summary>
    /// Specifies the interfaces of an IHost.
    /// </summary>
    public interface IHost:IContainer
    {

        /// <summary>
        /// Gets or sets the selected control.
        /// </summary>
        FluidControl FocusedControl { get; set; }

        /// <summary>
        /// Updates the host's graphic canvas.
        /// </summary>
        void Update();

        /// <summary>
        /// Creates the graphics to be used for all FluidControls.
        /// </summary>
        /// <returns>The Graphics for all FluidControls.</returns>
        Graphics CreateGraphics();

        /// <summary>
        /// Gets or sets the cursor image.
        /// </summary>
        Cursor Cursor { get; set; }
    }
}
