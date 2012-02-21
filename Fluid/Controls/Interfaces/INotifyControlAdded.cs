using System;
using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Specifies a control that gets notified when a control is added.
    /// </summary>
    public interface INotifyControlAdded
    {
        /// <summary>
        /// Adds a child control.
        /// </summary>
        /// <param name="control">The child control to be added.</param>
        void ControlAdded(FluidControl control);
    }
}
