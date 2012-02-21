using System;
using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Specifies a control to be a container for commands.
    /// </summary>
    public interface ICommandContainer
    {
        /// <summary>
        /// Raises a command event.
        /// </summary>
        /// <param name="e">The command event to be raised.</param>
        void RaiseCommand(CommandEventArgs e);

        /// <summary>
        /// Occurs when a command is raised.
        /// </summary>
        event EventHandler<CommandEventArgs> Command;
    }
}
