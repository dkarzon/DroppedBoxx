using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Specifies a recognized gesture.
    /// </summary>
    public enum Gesture
    {
        /// <summary>
        /// No gesture is recognized.
        /// </summary>
        None,

        /// <summary>
        /// A gesture was recognized but has been canceled.
        /// This property can be used on a (On)Gesture event to cancel a recognized gesture.
        /// </summary>
        Canceled,

        /// <summary>
        /// A left gesture is recognized.
        /// </summary>
        Left,

        /// <summary>
        /// A right gesture is regognized.
        /// </summary>
        Right,

        LeftUp,
        LeftDown,
        RightUp,
        RightDown,

        /// <summary>
        /// A up gesture is recognized.
        /// </summary>
        Up,

        /// <summary>
        /// A down gesutre is recognized.
        /// </summary>
        Down
    }
}
