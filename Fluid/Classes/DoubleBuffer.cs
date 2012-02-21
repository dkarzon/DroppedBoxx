using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Fluid.Drawing.GdiPlus;
using System.Diagnostics;

namespace Fluid.Controls
{
    /// <summary>
    /// A class that manages double buffering of a graphics.
    /// </summary>
    public class DoubleBuffer : IDisposable
    {
        public DoubleBuffer()
            : base()
        {
        }

        /// <summary>
        /// Creates a new double buffer.
        /// </summary>
        /// <param name="useRegions">Set to true for using regions to limit the need for painting, otherwise false.</param>
        public DoubleBuffer(bool useRegions)
            : base()
        {
            this.useRegions = useRegions;
        }

        private bool useRegions = true;

        public DoubleBuffer(int width, int height)
            : base()
        {
            EnsureDBuffer(width, height);
            graphics = EnsureGraphics();
        }

        public void Dispose()
        {
            if (region != null) region.Dispose();
            if (graphics != null) graphics.Dispose();
            if (dbuffer != null) dbuffer.Dispose();
        }

        private Graphics graphics;

        public Graphics Graphics
        {
            get
            {
                EnsureGraphics();
                graphics.ResetClip();
                return graphics;
            }
        }

        private FluidPaintEventArgs paintEventArgs;
        private Bitmap dbuffer;

        private FluidPaintEventArgs EnsurePaintEventArgs()
        {
            if (paintEventArgs == null) paintEventArgs = new FluidPaintEventArgs();
            return paintEventArgs;
        }

        public Bitmap EnsureDBuffer(int width, int height)
        {
            width = Math.Max(width, 1);
            height = Math.Max(height, 1);
            if (dbuffer == null || dbuffer.Width != width || dbuffer.Height != height)
            {
                if (graphics != null) graphics.Dispose();
                graphics = null;
                if (dbuffer != null) dbuffer.Dispose();
                dbuffer = new Bitmap(width, height);
                if (region!=null) region.MakeInfinite();
            }
            return dbuffer;
        }

        private Region region;

        private Graphics EnsureGraphics()
        {
            if (useRegions && region == null) region = new Region();
            if (graphics == null)
            {
                Bitmap bm = dbuffer;
                graphics = Graphics.FromImage(bm);
            }
            return graphics;
        }


        /// <summary>
        /// Paints double buffered.
        /// </summary>
        /// <param name="e">The FluidPaintEventArgs</param>
        /// <param name="control">The control for which to paint double buffered.</param>
        /// <param name="paintFunc">The function that performs actual painting.</param>
        /// <param name="alpha">The Alpha value for transparency. Set to 255 to paint opaque.</param>
        public void Paint(FluidPaintEventArgs e, FluidControl control, PaintFunc paintFunc, int alpha)
        {
            Rectangle dst = e.ControlBounds;
            //            Rectangle dst = control.Bounds;
            EnsureDBuffer(dst.Width, dst.Height);
            PaintBuffer(paintFunc, e.ScaleFactor);
            if (alpha >= 255)
            {
                e.Graphics.DrawImage(dbuffer, dst.X, dst.Y);
            }
            else
            {
                GdiExt.AlphaBlendImage(e.Graphics, dbuffer, dst.X, dst.Y, alpha, false);
            }
        }



        /// <summary>
        /// Paints double buffered.
        /// </summary>
        /// <param name="e">The FluidPaintEventArgs</param>
        /// <param name="control">The control for which to paint double buffered.</param>
        /// <param name="paintFunc">The function that performs actual painting.</param>
        public void Paint(FluidPaintEventArgs e, FluidControl control, PaintFunc paintFunc)
        {
            Paint(e, control, paintFunc, 255);
        }


        /// <summary>
        /// Invalidates a rectangle in the double buffer.
        /// </summary>
        /// <param name="bounds">The bounds of the double buffer to invalidate.</param>
        public void Invalidate(Rectangle bounds)
        {
            if (region != null) region.Union(bounds);
        }

        /// <summary>
        /// Invalidates the double buffer.
        /// </summary>
        public void Invalidate()
        {
            if (region != null) region.MakeInfinite();
        }


        /// <summary>
        /// Gets the width of the double buffer image,
        /// </summary>
        public int Width { get { return dbuffer.Width; } }

        /// <summary>
        /// Gets the height of the double buffer image,
        /// </summary>
        public int Height { get { return dbuffer.Height; } }

        /// <summary>
        /// Gets the double buffer image,
        /// </summary>
        public Bitmap Image { get { return dbuffer; } }

        /// <summary>
        /// Scrolls the canvas of the double buffer image down.
        /// </summary>
        /// <param name="dy">The number of pixels to scroll.</param>
        public void ScrollDown(int dy)
        {
            graphics.ResetClip();
            GdiExt.ScrollDown(graphics, dy, dbuffer.Width, dbuffer.Height);
            Rectangle invalidBounds = new Rectangle(0, 0, Width, dy);
            Invalidate(invalidBounds);
        }


        /// <summary>
        /// Scrolls the canvas of the double buffer image up.
        /// </summary>
        /// <param name="dy">The number of pixels to scroll.</param>
        public void ScrollUp(int dy)
        {
            graphics.ResetClip();
            GdiExt.ScrollUp(graphics, dy, dbuffer.Width, dbuffer.Height);
            Rectangle invalidBounds = new Rectangle(0, dbuffer.Height - dy, Width, dy);
            Invalidate(invalidBounds);
        }


        /// <summary>
        /// Paints the double buffer image with the specified paint function.
        /// </summary>
        /// <param name="paintFunc">A PaintFunc that performs painting on the buffer.</param>
        /// <param name="scaleFactor">The ScaleFactor for painting.</param>
        public void PaintBuffer(PaintFunc paintFunc, SizeF scaleFactor)
        {
            Graphics g = EnsureGraphics();
            {
                if (region == null || !region.IsEmpty(g))
                {
                    //using (SolidBrush b = new SolidBrush(Color.Transparent))
                    //{
                    //    bg.FillRectangle(b, clientRectangle);
                    //}
                    FluidPaintEventArgs ea = EnsurePaintEventArgs();
                    ea.Graphics = g;
                    ea.Region = region;
                    g.Clip = region;
                    ea.ScaleFactor = scaleFactor;
                    ea.ControlBounds = new Rectangle(0, 0, Width, Height);
                    ea.DoubleBuffered = false;
                    paintFunc(ea);
                    if (region != null) region.MakeEmpty();
                }
            }

        }

    }

    /// <summary>
    /// Specifies a Function that performs painting.
    /// </summary>
    /// <param name="e">FluidPaintEventArgs for information about the canvas.</param>
    public delegate void PaintFunc(FluidPaintEventArgs e);
}
