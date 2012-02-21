using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Fluid.Native;
using Fluid.Drawing.GdiPlus;
using System.Diagnostics;

namespace Fluid.Controls
{
    /// <summary>
    /// Default class for IControlContainer controls.
    /// </summary>
    public class ControlContainer : FluidControl, IMultiControlContainer, ICommandContainer
    {
        public ControlContainer()
            : base()
        { }

        public ControlContainer(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }


        protected override void InitControl()
        {
            base.InitControl();
            Init();
        }

        private void Init()
        {
            controls = new FluidControlCollection(this);
            ForeColor = Color.Empty;
            Initialize();
        }

        protected virtual void Initialize()
        {
        }

        #region IControlContainer Members

        internal FluidControlCollection controls;

        protected virtual IEnumerable<FluidControl> GetControls()
        {
            return controls;
        }

        IEnumerable<FluidControl> IMultiControlContainer.Controls
        {
            get { return GetControls(); }
        }

        public virtual void Invalidate(Rectangle bounds)
        {
            if (!Visible) return;
            if (Initializing) return;
            if (Container == null) return;

            bounds.Offset(Left, Top);
            // don't let the child control invalidate anything outside the own bounds:
            ClipRectangle(ref bounds);
            Container.Invalidate(bounds);
        }

        public IHost Host
        {
            get { return Container != null ? this.Container.Host : null; }
        }

        public System.Drawing.Point PointToHost(int x, int y)
        {
            return Container.PointToHost(x + Left, y + Top);
        }


        #endregion

        private SizeF defaultSize = new SizeF(1f, 1f);
        protected override void OnSizeChanged(Size oldSize, Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);
        }


        public override void OnPaint(FluidPaintEventArgs e)
        {
            base.OnPaint(e);
            PaintControls(e);
        }

        private bool AreAllControOpaque(FluidControlCollection controls)
        {
            bool result = true;
            foreach (FluidControl c in controls)
            {
                if (!c.Visible) continue;
                Rectangle r = c.Bounds;
                if (c.Width <= 0 || c.Height <= 0) continue;
                if (c.IsTransparent)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }


        /// <summary>
        /// Paints all  controls.
        /// The controls are painted in an optimized way:
        /// The controls are rendered from the last to the first, and after painting each control, the bounds of the control are excluded from the region, so this region is not painted twice.
        /// If a there is a transparent control, the remaining controls are painted from the first to this control, while the region is not excluded.
        /// </summary>
        /// <param name="pe">The PaintEventArgs.</param>
        protected virtual void PaintControls(FluidPaintEventArgs pe)
        {
#if true
            Region region = null;


            Region clip = pe.Region;
            Graphics g = pe.Graphics;
            int index = 0;


            for (int i = controls.Count - 1; i >= 0; i--)
            {
                FluidControl c = controls[i];
                if (c.Visible && c.IsTransparent)
                {
                    region = pe.Graphics.Clip;
                    index = i + 1;
                    break;
                }
                Rectangle bounds = PaintControl(pe, c);
                if (!bounds.IsEmpty)
                {
                    clip.Exclude(bounds);
                    g.Clip = clip;
                }
            }
            for (int i = 0; i < index; i++)
            {
                FluidControl c = controls[i];
                if (c.IsTransparent)
                {
                    pe.Graphics.Clip = region;
                }
                PaintControl(pe, c);
            }
            if (region != null) region.Dispose();

#else
            foreach (FluidControl c in controls)
            {
                PaintControl(pe, c);
            }
#endif
        }

        /// <summary>
        /// Paints the control. If the control was painted, the bounds of the rectangle in the canvas are returned, otherwise Rectangle.Empty is returned.
        /// </summary>
        /// <param name="pe">The PaintEventArgs</param>
        /// <param name="control">The control</param>
        /// <returns>If the control was painted, the bounds of the rectangle in the canvas are returned, otherwise Rectangle.Empty is returned.</returns>
        protected virtual Rectangle PaintControl(FluidPaintEventArgs pe, FluidControl control)
        {
            if (!control.Visible) return Rectangle.Empty;
            Rectangle bounds = control.Bounds;
            if (bounds.Width <= 0 || bounds.Height <= 0) return Rectangle.Empty;

            Rectangle controlBounds = pe.ControlBounds;
            Region clip = pe.Region;
            bounds.Offset(controlBounds.X, controlBounds.Y);
            if (clip.IsVisible(bounds))
            {
                PaintControlUnbuffered(pe, control, bounds);
                return bounds;
            }
            return Rectangle.Empty;
        }

        private FluidPaintEventArgs paintEventArgs = new FluidPaintEventArgs();

        protected void PaintControlUnbuffered(FluidPaintEventArgs pe, FluidControl control, Rectangle bounds)
        {
            FluidPaintEventArgs e = paintEventArgs;
            e.Graphics = pe.Graphics;
            e.ControlBounds = bounds;
            e.Region = pe.Region;
            e.ScaleFactor = pe.ScaleFactor;
            control.OnPaint(e);
        }

        private FluidControl downControl = null;

        private PointEventArgs TranslatePoint(FluidControl c, PointEventArgs p)
        {
            p.X -= c.Bounds.X;
            p.Y -= c.Bounds.Y;
            return p;
        }

        public override void OnGesture(GestureEventArgs e)
        {
            if (!Enabled) return;
            if (downControl != null) downControl.OnGesture(e);

        }

        public override void OnMove(PointEventArgs p)
        {
            if (downControl != null) downControl.OnMove(this.TranslatePoint(downControl, p));
        }


        private FluidControl selectedControl;

        public override void OnDown(PointEventArgs p)
        {
            if (!Enabled) return;
            base.OnDown(p);
            FluidControl control = ControlFromPoint(p.X, p.Y);
            if (control != null && !control.Active) control = null;
            downControl = control;
            selectedControl = control;
            if (control != null)
            {
                PointEventArgs pointEventArgs = TranslatePoint(control, p);
                if (control.Active) control.OnDown(pointEventArgs);
            }
        }

        public override void PerformClick()
        {
            base.PerformClick();
            FluidControl control = selectedControl;
            if (control != null)
            {
                if (control.Selectable) Host.FocusedControl = control;
            }
        }

        public override void OnUp(PointEventArgs p)
        {
            base.OnUp(p);
            if (downControl != null) downControl.OnUp(TranslatePoint(downControl, p));
        }

        public override void OnRelease(PointEventArgs p)
        {
            base.OnRelease(p);
            if (downControl != null) downControl.OnRelease(TranslatePoint(downControl, p));
        }

        public override bool OnClick(PointEventArgs p)
        {
            if (!Enabled) return false;
            if (downControl != null) return downControl.OnClick(TranslatePoint(downControl, p));
            base.OnClick(p);
            return false;
        }

        Point p = new Point();

        /// <summary>
        /// Gets the first control that is under the specified point.
        /// </summary>
        /// <param name="x">The x value of the point.</param>
        /// <param name="y">The y value of the point.</param>
        /// <returns>An ITouchControl, otherwise null.</returns>
        /// <remarks>
        /// The enumeration starts from the last to the first control, because the controls appear in this
        /// order in case they are overlapping.
        /// </remarks>
        public virtual FluidControl ControlFromPoint(int x, int y)
        {
            p.X = x;
            p.Y = y;
            for (int i = controls.Count - 1; i >= 0; i--)
            {
                FluidControl c = controls[i];
                if (!c.Visible) continue;
                Rectangle r = c.Bounds;
                if (r.Contains(p)) return c;
            }
            return null;
        }

        Rectangle IContainer.GetScreenBounds(Rectangle bounds)
        {
            bounds.Offset(Left, Top);
            return Container.GetScreenBounds(bounds);
        }

        public override bool Active { get { return true; } }

        #region ICommandContainer Members


        public virtual void RaiseCommand(CommandEventArgs e)
        {
            IsDown = false;
            if (Command != null)
            {
                Command(this, e);
            }
            if (!e.Handled)
            {
                ICommandContainer container = this.Container as ICommandContainer;
                if (container != null)
                {
                    container.RaiseCommand(e);
                }
            }
        }

        public event EventHandler<CommandEventArgs> Command;

        #endregion

        /// <summary>
        /// Paints a background with a gradient.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="alpha">the alpha value between white and the background color for the starting gradiant color.</param>
        /// <param name="gradientHeight">the height of the gradient until solid color.</param>
        public void PaintGradientBackground(FluidPaintEventArgs e, int alpha, int gradientHeight)
        {
            Graphics g = e.Graphics;
            Rectangle r = e.ControlBounds;
            Color color = BackColor;
            Color startColor = ColorConverter.AlphaBlendColor(Color.White, color, alpha);

            Rectangle gr = r;
            gr.Height = gradientHeight;
            GdiExt.GradientFill(g, gr, startColor, color, GdiExt.FillDirection.TopToBottom);
            if (gr.Height < r.Height)
            {
                gr.Y = r.Top + gradientHeight;
                gr.Height = r.Height - gradientHeight;
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    g.FillRectangle(brush, gr);
                }
            }

            r.Width--;
            r.Height--;
            using (Pen pen = new Pen(color))
            {
                g.DrawRectangle(pen, r);
            }
        }

        /// <summary>
        /// Paints a background with a gradient.
        /// </summary>
        /// <param name="e"></param>
        public void PaintGradientBackground(FluidPaintEventArgs e)
        {
            PaintGradientBackground(e, 50, Height);
        }

        private SizeF scaleFactor = new SizeF(1f, 1f);

        public override void Scale(SizeF scaleFactor)
        {
            if (this.scaleFactor != scaleFactor)
            {
                this.scaleFactor = scaleFactor;
                base.Scale(scaleFactor);
            }
        }

    }
}
