using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Classes
{
    /// <summary>
    /// Event that can be marked as beeing handled.
    /// </summary>
    public class HandledEventArgs:EventArgs
    {
        public bool Handled { get; set; }
    }
}
