using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Fluid.Drawing.GdiPlus;

namespace Fluid.Controls
{
    /// <summary>
    /// EventArgs used for control painting.
    /// </summary>
    /// <remarks>
    /// FluidPaintEventArgs contains a Rectangle named ControlBounds. this Rectangle specifies the range where the control is allowed to 
    /// paint in the Graphics canvas. It is important to know that due to performance issues, the canvas region is not necassarily limited to that
    /// bounds, so the any control that paint using FluidPaintEventArgs must not use graphic operations that are not limited to the bounds, like
    /// Graphics.Clear(Color) which is very popularily used for windows controls to clear the backgound. Instead you must use 
    /// Graphics.FillRectangle(backgroundbrush, bounds).
    /// Also note, that you should rarily use temporarily created classed, even within "using..." block. Otherwhise the garbage collector would
    /// have to cleanup on occasion and this would slow down performance and animations. However, for usage of derived structs like Point or Rectangle,
    /// there is no restriction.
    /// </remarks>
    public class FluidPaintEventArgs : EventArgs
    {
        internal FluidPaintEventArgs()
            : base()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="g">The graphics.</param>
        /// <param name="clip">The clip bounds for the control.</param>
        /// <param name="scaleFactor">The scale factor of the form.</param>
        public FluidPaintEventArgs(Graphics g, Rectangle bounds, SizeF scaleFactor)
            : base()
        {
            this.Graphics = g;
            this.ControlBounds = bounds;
            this.ScaleFactor = scaleFactor;
            Region = g.Clip;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="g">The graphics.</param>
        /// <param name="clip">The clip bounds for the control.</param>
        /// <param name="scaleFactor">The scale factor of the form.</param>
        /// <param name="doubleBuffered">Specifies wether the parent control uses double buffering.</param>
        public FluidPaintEventArgs(Graphics g, Rectangle clip, SizeF scaleFactor, bool doubleBuffered)
            : base()
        {
            this.Graphics = g;
            this.ControlBounds = clip;
            this.DoubleBuffered = true;
            this.ScaleFactor = scaleFactor;
            Region = g.Clip;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="g">The graphics.</param>
        /// <param name="clip">The clip bounds for the control.</param>
        /// <param name="scaleFactor">The scale factor of the form.</param>
        /// <param name="doubleBuffered">Specifies wether the parent control uses double buffering.</param>
        /// <param name="gp">The gdi plus class for painting.</param>
        public FluidPaintEventArgs(Graphics g, Rectangle clip, SizeF scaleFactor, bool doubleBuffered, GraphicsPlus gp)
            : base()
        {
            this.Graphics = g;
            this.ControlBounds = clip;
            this.DoubleBuffered = true;
            this.GdiPlus = gp;
            this.ScaleFactor = scaleFactor;
            Region = g.Clip;
        }

        /// <summary>
        /// Gets the region for checking the requirements to paint.
        /// Note, that each access of Graphic.Clip whould indeed create a new cloned instance, so g.Clip==g.Clip would always be false.
        /// This would cause a lot of classes being temporarily created. So this property can be used to get the Region without
        /// creating an instance at each access. 
        /// And it is urgently recommended for performance issues to always use this property instead!
        /// </summary>
        public Region Region { get; internal set; }

        /// <summary>
        /// Gets the graphics canvas.
        /// </summary>
        public Graphics Graphics { get; internal set; }

        /// <summary>
        /// Gets the GraphicsPlus class for painting if available.
        /// </summary>
        public GraphicsPlus GdiPlus { get; set; }

        /// <summary>
        /// Gets the bounds where the control should be painted in the canvas.
        /// </summary>
        /// <remarks>
        /// This is different to windows PaintEventArgs since fluid controls do not use a windows sub system where
        /// each control is a window and therefore has it's own graphic canvas. 
        /// </remarks>
        /// <example>
        /// use e.Graphics.FillRectangle(brush, e.ControlBounds) instead of e.Graphics.FillRectange(brush, ClientRectangle).
        /// </example>
        public Rectangle ControlBounds { get; internal set; }

        /// <summary>
        /// Gets whether the canvas uses double buffering.
        /// This flag can be used to determine wether to implement own double buffering for a control or to rely on
        /// the double buffering of the parent IFluidContainer control that raised the event.
        /// </summary>
        public bool DoubleBuffered { get; internal set; }

        /// <summary>
        /// Gets the scale factor of the form.
        /// </summary>
        /// <remarks>
        /// Windows mobile scales windows controls depending on the system settings so they have another size
        /// than the size specified in design mode.  This value can be used to paint the control depending on the scale factor.
        /// </remarks>
        public SizeF ScaleFactor { get; internal set; }

        /// <summary>
        /// Scales a value for the x axies depending on the ScaleFactor.
        /// </summary>
        /// <param name="size">The size to scale</param>
        /// <returns>The scaled value.</returns>
        public int ScaleX(int size)
        {
            return (int)(ScaleFactor.Width * size);
        }

        /// <summary>
        /// Scales a value for the y axies depending on the ScaleFactor.
        /// </summary>
        /// <param name="size">The size to scale</param>
        /// <returns>The scaled value.</returns>
        public int ScaleY(int size)
        {
            return (int)(ScaleFactor.Height * size);
        }

        /// <summary>
        /// Scales a size depending on the ScaleFactor.
        /// </summary>
        /// <param name="size">The size to scale</param>
        /// <returns>The scaled value.</returns>
        public Size Scale(Size size)
        {
            return new Size((int)(ScaleFactor.Width * size.Width), (int)(ScaleFactor.Height * size.Height));
        }

        /// <summary>
        /// Scales a Rectangle  depending on the ScaleFactor.
        /// </summary>
        /// <param name="size">The rectangle to scale</param>
        /// <returns>The scaled value.</returns>
        public Rectangle Scale(Rectangle r)
        {
            float w = ScaleFactor.Width;
            float h = ScaleFactor.Height;
            return new Rectangle(
                (int)(w * r.X),
                (int)(h * r.Y),
                (int)(w * r.Width),
                (int)(h * r.Height));
        }

    }
}
