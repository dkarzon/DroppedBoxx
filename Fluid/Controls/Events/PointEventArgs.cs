using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Event for point events.
    /// </summary>
    public class PointEventArgs : EventArgs
    {
        internal PointEventArgs()
            : base()
        {
        }

        public PointEventArgs(Gesture gesture, int x, int y)
            : base()
        {
            this.X = x;
            this.Y = y;
            this.Gesture = gesture;
        }

        /// <summary>
        /// Gets the X value of the point.
        /// </summary>
        public int X { get; internal set; }

        /// <summary>
        /// Gets the Y value of the point.
        /// </summary>
        public int Y { get; internal set; }

        /// <summary>
        /// Gets the gesture associated with the event.
        /// </summary>
        public Gesture Gesture { get; internal set; }
    }
}
