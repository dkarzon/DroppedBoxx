using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Event that occurs for a ListBoxItem.
    /// </summary>
    public class ListBoxItemEventArgs:EventArgs
    {
        /// <summary>
        /// Gets the item index associated with this event.
        /// </summary>
        public int ItemIndex { get;  set; }


        /// <summary>
        /// Gets the item  associated with this event.
        /// </summary>
        public object Item { get; internal set; }

        public ListBoxItemEventArgs(int index, object item)
            : base()
        {
            this.ItemIndex = index;
            this.Item = item;
        }

        internal ListBoxItemEventArgs()
            : base()
        {
        }
    }
}
