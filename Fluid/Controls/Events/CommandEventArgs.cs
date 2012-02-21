using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Event that specifies a command.
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(string command, FluidControl control, object data)
            : base()
        {
            this.Command = command;
            this.OriginalSource = control;
            this.Data = data;
        }

        /// <summary>
        /// Gets the command that occured.
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// Gets or set optional data for the command.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the optional index for the command. e.g. if raised within a listbox template, this specifies the item index.
        /// </summary>
        public int Index { get; set; }
        
        
        /// <summary>
        /// Gets the control that initialized the command.
        /// </summary>
        public FluidControl OriginalSource { get; private set; }

        /// <summary>
        /// Gets or sets whether the command is handled.
        /// </summary>
        public bool Handled { get; set; }
    }
}
