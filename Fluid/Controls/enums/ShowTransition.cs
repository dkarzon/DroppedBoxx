using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Specifies the transition for Panel.Show and Panel.Hide.
    /// </summary>
    public enum ShowTransition
    {
        /// <summary>
        /// Don't use a transition.
        /// </summary>
        None,

        /// <summary>
        /// Transition from the bottom.
        /// </summary>
        FromBottom,

        /// <summary>
        /// Transition from the top.
        /// </summary>
        FromTop,

        /// <summary>
        /// Transition from the left.
        /// </summary>
        FromLeft,

        /// <summary>
        /// Transition from the right.
        /// </summary>
        FromRight,

        /// <summary>
        /// Zoom transition (currently not supported!)
        /// </summary>
        Zoom
    }
}
