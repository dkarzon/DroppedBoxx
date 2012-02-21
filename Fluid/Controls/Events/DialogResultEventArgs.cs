using System;

using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Fluid.Classes
{
    /// <summary>
    /// Event that occurs when a button is pressed on a dialog.
    /// </summary>
    public class DialogEventArgs:EventArgs
    {
        /// <summary>
        /// Gets the result that has been chosen for the dialog.
        /// </summary>
        public DialogResult Result { get; set; }
    }
}
