using System;
using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls.Interfaces
{
    /// <summary>
    /// Extends a listbox item to contain it's custom height:
    /// </summary>
    public interface IItemHeight
    {
        /// <summary>
        /// Gets the height of the ListBox Item.
        /// </summary>
        int Height { get; }
    }
}
