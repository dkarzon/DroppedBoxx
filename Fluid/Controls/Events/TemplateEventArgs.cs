using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Event for templates.
    /// </summary>
    public class TemplateEventArgs : ListBoxItemEventArgs
    {
        public TemplateEventArgs(IContainer control)
            : base()
        {
            this.container = control;
        }

        private IContainer container;
        private FluidTemplate template;

        /// <summary>
        /// Gets or sets the template
        /// </summary>
        public FluidTemplate Template
        {
            get { return template; }
            set
            {
                template = value;
                if (template != null)
                {
                    template.BeginInit();
                    template.Container = container;
                }
            }
        }
    }
}
