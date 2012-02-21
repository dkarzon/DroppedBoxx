using System;
using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Interface for a control that hosts multiple child controls.
    /// </summary>
    public interface IMultiControlContainer:IContainer
    {
        /// <summary>
        /// Gets the collection of all TouchControls inside this control.
        /// </summary>
        IEnumerable<FluidControl> Controls { get; }

    }

    /// <summary>
    /// Interface for a control that hosts one single child control.
    /// </summary>
    public interface ISingleControlContainer : IContainer
    {
        /// <summary>
        /// Gets the control that this control is hosting.
        /// </summary>
        FluidControl Control { get; }
    }
}
