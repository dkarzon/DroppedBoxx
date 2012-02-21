using System;
using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// Specifies that this control automatically uses the default Layouter to layout it's controls depending on their Anchor property
    /// when the size of this control changes.
    /// </summary>
    public interface ILayoutPanel:IMultiControlContainer
    {
    }
}
