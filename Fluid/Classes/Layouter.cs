using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Fluid.Controls
{
    /// <summary>
    /// Performs basic layouting of controls inside a IFluidContainer.
    /// </summary>
    public static class Layouter
    {
        /// <summary>
        /// Layouts all child controls when a IFluidContainer has changed it's size depending on the child's anchor style.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="oldContainerSize"></param>
        /// <param name="newContainerSize"></param>
        /// <param name="scaleFactor"></param>
        public static void Layout(IMultiControlContainer container, Size oldContainerSize, Size newContainerSize, SizeF scaleFactor)
        {
            Rectangle Bounds = container.Bounds;
            Size oldSize = Bounds.Size;
            if (newContainerSize != oldContainerSize)
            {
                foreach (FluidControl c in container.Controls)
                {
                    Adjust(c, oldContainerSize, newContainerSize, container.ScaleFactor);
                }
            }
        }


        private static void Adjust(FluidControl c, Size oldContainerSize, Size newContainerSize, SizeF scaleFactor)
        {
            Rectangle bounds = c.Bounds;
            int left = bounds.Left;
            int right = bounds.Right;
            int top = bounds.Top;
            int bottom = bounds.Bottom;

            AnchorStyles a = c.Anchor;
            if ((a & AnchorStyles.Right) != 0)
            {
                int dw = newContainerSize.Width - oldContainerSize.Width;
                right += dw;
                if ((a & AnchorStyles.Left) == 0)
                {
                    left += dw;
                }
            }

            if ((a & AnchorStyles.Bottom) != 0)
            {
                int dh = newContainerSize.Height - oldContainerSize.Height;
                bottom += dh;
                if ((a & AnchorStyles.Top) == 0)
                {
                    top += dh;
                }
            }
            Rectangle b = new Rectangle(left, top, right - left, bottom - top);
            if (c.Bounds.Size != b.Size)
            {
                c.Scale(scaleFactor);
            }
            c.Bounds = b;
        }

        /// <summary>
        /// Layouts a control when it has changed it's size depending on it's anchor style.
        /// </summary>
        /// <param name="c"></param>
        /// <param name="oldContainerSize"></param>
        /// <param name="newContainerSize"></param>
        /// <param name="scaleFactor"></param>
        public static void AdjustSize(FluidControl c, Size oldContainerSize, Size newContainerSize, SizeF scaleFactor)
        {
            Rectangle bounds = c.Bounds;
            int left = bounds.Left;
            int right = bounds.Right;
            int top = bounds.Top;
            int bottom = bounds.Bottom;

            AnchorStyles a = c.Anchor;
            if ((a & AnchorStyles.Right) != 0 && (a & AnchorStyles.Left) == 0)
            {
                int dw = newContainerSize.Width - oldContainerSize.Width;
                right -= dw;
                left -= dw;
            }

            if ((a & AnchorStyles.Bottom) != 0 && (a & AnchorStyles.Top) == 0)
            {
                int dh = newContainerSize.Height - oldContainerSize.Height;
                bottom -= dh;
                top -= dh;
            }
            Rectangle b = new Rectangle(left, top, right - left, bottom - top);
            c.bounds = b;
        }
    }
}
