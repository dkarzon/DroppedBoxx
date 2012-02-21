using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Event that occurs on changes to a property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChangedEventArgs<T> : EventArgs
    {
        public ChangedEventArgs()
            : base()
        {
        }

        public ChangedEventArgs(T old, T newValue)
            : base()
        {
            OldValue = old;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        public T OldValue { get; internal set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public T NewValue { get; internal set; }
    }
}
