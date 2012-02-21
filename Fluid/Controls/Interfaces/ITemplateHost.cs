using System;
using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Specifies a control that provides data for IFluidTemplates.
    /// </summary>
    public interface ITemplateHost
    {
        /// <summary>
        /// Bind a template's data with the information of the currently selected index of the host.
        /// </summary>
        /// <param name="template"></param>
        void Bind(FluidTemplate template);

        /// <summary>
        /// Bind a template's data with the information of the data item with the specified index.
        /// </summary>
        /// <param name="template"></param>
        void Bind(FluidTemplate template, int itemIndex);
    }
}
