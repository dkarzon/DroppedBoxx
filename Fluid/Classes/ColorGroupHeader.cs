using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Fluid.Controls
{
    /// <summary>
    /// Implements the IColorGroupHeader interface:
    /// A Group Header that can specify a color for it's background.
    /// If set to Color.Empty, the default header color is used.
    /// </summary>
    public class ColorGroupHeader : GroupHeader, IColorGroupHeader
    {
        public ColorGroupHeader(string title, Color color)
            : base(title)
        {
            this.BackColor = color;
        }

        public ColorGroupHeader()
            : base()
        {
        }

        #region IColorGroupHeader Members

        private Color backColor = Color.Empty;

        public System.Drawing.Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
            }
        }

        #endregion
    }
}
