using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Drawing
{
    /// <summary>
    /// Specifies the corner to be rounded.
    /// </summary>
    public enum RoundedCorners
    {
        None = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 4,
        BottomRight = 8,
        Top = 3,
        Bottom = 12,
        Left = 5,
        Right = 10,
        All = 15
    }
}
