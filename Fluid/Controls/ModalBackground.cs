using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Fluid.Drawing.GdiPlus;

namespace Fluid.Controls
{
    public class ModalBackground : FluidPanel
    {
        protected override void InitControl()
        {
            base.InitControl();
            EnableDoubleBuffer = true;
            BackColor = Color.Black;
            Alpha = 170;
            bounds = new Rectangle(0, 0, 240, 320);
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
        }

        internal protected override void OnControlAdded(IContainer container)
        {
            FluidControl parent = container as FluidControl;
            Bounds = parent.ClientRectangle;
        }
    }
}
