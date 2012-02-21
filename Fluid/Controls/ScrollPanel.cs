
using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Fluid.Drawing.GdiPlus;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Fluid.Controls
{
    /// <summary>
    /// A control that contains other fluid controls and supports up-down scrolling.
    /// </summary>
    public partial class ScrollPanel : ScrollContainer, IMultiControlContainer, ICommandContainer
    {
        public ScrollPanel()
            : base()
        {
            Init();
        }

        public ScrollPanel(int x, int y, int width, int height)
            : base()
        {
            Init();
            Bounds = new Rectangle(x, y, width, height);
        }

        public override void Dispose()
        {
            transparentBrush.Dispose();
            base.Dispose();
        }

        private void Init()
        {
            controls = new FluidControlCollection(this);
            BeginInit();
            Initialize();
            EndInit();
        }


        /// <summary>
        /// Used for derived classes to initialize the control.
        /// </summary>
        protected virtual void Initialize()
        { }


        /// <summary>
        /// dictionary of all cached bitmaps for each control.
        /// </summary>
        private HybridDictionary imageBuffer = new HybridDictionary();

        public override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == '\t')
            {
                e.Handled = true;
                SelectNextControl();
            }
        }




        private FluidControlCollection controls;

        /// <summary>
        /// Gets the collection of all TouchControls inside this panel.
        /// </summary>
        /// <value></value>
        public FluidControlCollection Controls { get { return controls; } }

        IEnumerable<FluidControl> IMultiControlContainer.Controls
        {
            get
            {
                return controls;
            }
        }


        public override int DisplayHeight
        {
            get
            {
                return GetVirtualHeight();
            }
        }

        private int GetVirtualHeight()
        {
            int height = 0;
            foreach (FluidControl c in controls)
            {
                height = Math.Max(height, c.Bounds.Bottom);
            }
            return height;
        }




        /// <summary>
        /// Paints this control double buffered.
        /// </summary>
        /// <param name="pe">The  paint event args.</param>
        protected override void PaintContent(FluidPaintEventArgs pe)
        {
            base.PaintContent(pe);
            PaintControls(pe);
        }

        /// <summary>
        /// Paints all  controls.
        /// </summary>
        /// <param name="pe">The PaintEventArgs.</param>
        private void PaintControls(FluidPaintEventArgs pe)
        {
            Rectangle controlBounds = pe.ControlBounds;
            Graphics g = pe.Graphics;
            Region clip = pe.Region;


            foreach (FluidControl c in controls)
            {
                if (!c.Visible) continue;
                Rectangle bounds = c.Bounds;
                bounds.Offset(controlBounds.X, controlBounds.Y);
                if (clip.IsVisible(bounds))
                {
                    if (this.EnableDoubleBuffer)
                    {
                        PaintControlDoubleBuffered(pe, c, bounds);
                    }
                    else
                    {
                        PaintControlUnbuffered(pe, c, bounds);
                    }
                }
            }

        }

        private FluidPaintEventArgs paintEventArgs2 = new FluidPaintEventArgs();

        private void PaintControlUnbuffered(FluidPaintEventArgs pe, FluidControl control, Rectangle bounds)
        {
            //bounds.Offset(Left, Top);
            //Region clp = pe.IntersectClip(bounds);
            FluidPaintEventArgs e = paintEventArgs2;
            e.Graphics = pe.Graphics;
            e.ControlBounds = bounds;
            e.Region = pe.Region;
            e.ScaleFactor = pe.ScaleFactor;
            control.OnPaint(e);
            //pe.ResetClip(clp);
        }

        private Color transparentColor = Color.Fuchsia;

        /// <summary>
        /// Gets or sets the color that is used as transparent color when EnableDoubleBuffer is set to true.
        /// see also <seealso cref="EnabledDoubleBuffer"/>.
        /// </summary>
        public Color TransparentColor
        {
            get { return transparentColor; }
            set { transparentColor = value; }
        }

        private FluidPaintEventArgs paintEventArgs = new FluidPaintEventArgs();
        private SolidBrush transparentBrush = new SolidBrush(Color.Transparent);

        private void PaintControlDoubleBuffered(FluidPaintEventArgs pe, FluidControl control, Rectangle bounds)
        {
            Bitmap buffer;
            if (imageBuffer.Contains(control))
            {
                buffer = imageBuffer[control] as Bitmap;
            }
            else
            {
                buffer = new Bitmap(bounds.Width, bounds.Height);
                imageBuffer.Add(control, buffer);
                using (Graphics bg = Graphics.FromImage(buffer))
                {
                    this.transparentBrush.Color = transparentColor;
                    bg.FillRectangle(transparentBrush, 0, 0, buffer.Width, buffer.Height);
                    Rectangle paintRect = control.ClientRectangle;
                    //bg.Clear(transparentColor);

                    FluidPaintEventArgs e = paintEventArgs;
                    e.Graphics = bg;
                    e.ControlBounds = paintRect;
                    e.Region = bg.Clip;
                    e.ScaleFactor = pe.ScaleFactor;
                    control.OnPaint(e);
                }

            }
            ia.SetColorKey(transparentColor, transparentColor);
            pe.Graphics.DrawImage(buffer, bounds, 0, 0, bounds.Width, bounds.Height, GraphicsUnit.Pixel, ia);
        }

        ImageAttributes ia = new ImageAttributes();

        Rectangle IContainer.GetScreenBounds(Rectangle bounds)
        {
            bounds.Offset(Left, Top - ScaledTopOffset);
            return Container.GetScreenBounds(bounds);
        }

        public override void Invalidate(Rectangle bounds)
        {
            if (Initializing) return;
            if (Container == null) return;
            if (!bounds.IsEmpty)
            {
                bounds.Offset(Left, Top - ScaledTopOffset);
                //// don't let the child control invalidate anything outside the own bounds:
                ClipRectangle(ref bounds);
                if (!bounds.Size.IsEmpty) Container.Invalidate(bounds);
            }
        }

        public override void Invalidate()
        {
            if (Initializing) return;
            base.Invalidate();
            InvalidateChildren();
        }



        private void InvalidateChildren()
        {
            return; // debug==
            //foreach (FluidControl c in controls)
            //{
            //    if (c.IsTransparent) c.Invalidate();
            //}
        }

        private FluidControl downControl = null;


        private PointEventArgs TranslatePoint(FluidControl c, PointEventArgs p)
        {
            p.X -= c.Bounds.X;
            p.Y -= c.Bounds.Y;
            //PointEventArgs e = new PointEventArgs(p.Gesture, p.X - c.Bounds.X, p.Y - c.Bounds.Y);
            return p;
        }

        public override void OnGesture(GestureEventArgs e)
        {
            if (downControl != null) downControl.OnGesture(e);
        }

        public override void OnMove(PointEventArgs p)
        {
            base.OnMove(p);
            if (downControl != null) downControl.OnMove(this.TranslatePoint(downControl, p));
        }

        public override void OnRelease(PointEventArgs p)
        {
            base.OnRelease(p);
            if (downControl != null) downControl.OnRelease(this.TranslatePoint(downControl, p));
        }

        private FluidControl selectedControl;

        public override void OnDown(PointEventArgs p)
        {
            base.OnDown(p);
            FluidControl control = ControlFromPoint(p.X, p.Y);
            downControl = control;
            if (control != null)
            {
                PointEventArgs p2 = TranslatePoint(control, p);
                control.OnDown(p2);
            }
            if (control != null && control.Selectable)
            {
                selectedControl = control;
            }
            else if (control == null)
            {
                selectedControl = null;
            }
        }

        public override void OnUp(PointEventArgs p)
        {
            if (downControl != null) downControl.OnUp(TranslatePoint(downControl, p));
            downControl = null;
        }

        public override bool OnClick(PointEventArgs p)
        {
            base.OnClick(p);
            if (downControl != null) downControl.OnClick(TranslatePoint(downControl, p));
            return true;
        }

        private Point checkPoint = new Point();

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
        public FluidControl ControlFromPoint(int x, int y)
        {
            Point p = checkPoint; 
            p.X = x;
            p.Y = y + TopOffset;
            for (int i = controls.Count - 1; i >= 0; i--)
            {
                FluidControl c = controls[i];
                if (!c.Visible) continue;
                Rectangle r = c.Bounds;
                if (r.Contains(p)) return c;
            }
            return null;
        }

        protected override void BeginMoving()
        {
            base.BeginMoving();
            selectedControl = null;
        }

        public override void OnKeyDown(KeyEventArgs e)
        {          
            StopAutoScroll();
            base.OnKeyDown(e);
        }

        void ControlKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\t')
            {
                e.Handled = true;
                SelectNextControl();
            }
        }

        void ControlKeyDown(object sender, KeyEventArgs e)
        {
            //switch (e.KeyCode)
            //{
            //    case Keys.Escape:
            //        e.Handled = true;
            //        SelectedControl = null;
            //        break;

            //    case Keys.Down:
            //    case Keys.Tab:
            //        e.Handled = true;
            //        SelectNextControl();
            //        break;

            //    case Keys.Up:
            //        e.Handled = true;
            //        SelectPreviousControl();
            //        break;
            //}
        }

        public virtual void SelectNextControl()
        {
            throw new ArgumentException("Not implemententad");
            //int index = SelectedControl != null ? controls.IndexOf(SelectedControl) : -1;
            //int n = controls.Count;
            //for (int i = 1; i <= n; i++)
            //{
            //    int idx = i + index;
            //    if (idx >= n) idx -= n;
            //    FluidControl c = controls[idx];
            //    if (c.Selectable)
            //    {
            //        SelectedControl = c;
            //        return;
            //    }
            //}
        }

        public virtual void SelectPreviousControl()
        {
            throw new ArgumentException("Not implemententad");
            //int index = SelectedControl != null ? controls.IndexOf(SelectedControl) : controls.Count;
            //int n = controls.Count;
            //index = n - index;
            //for (int i = n - 1; i >= 0; i--)
            //{
            //    int idx = i - index;
            //    if (idx < 0) idx += n;
            //    FluidControl c = controls[idx];
            //    if (c.Selectable)
            //    {
            //        SelectedControl = c;
            //        return;
            //    }
            //}
        }


        /// <summary>
        /// Ensure that the specified control is visible, and make it become visible if not.
        /// </summary>
        /// <param name="c">The control that needs to be visible.</param>
        /// <returns>True, if the control was already visible, otherwise false.</returns>
        public bool EnsureVisible(FluidControl c)
        {
            bool result = true;
            Rectangle bounds = c.Bounds;
            bounds.Y -= TopOffset;
            if (!Bounds.Contains(bounds))
            {
                EnsureVisible(bounds);
            }
            return result;
        }

        public IHost Host { get { return this.Container.Host; } }

        public Point PointToHost(int x, int y)
        {
            return Container.PointToHost(x + Left, y + Top);
        }

        #region IControlContainer Members


        #endregion


        protected override void OnSizeChanged(Size oldSize, Size newSize)
        {
            imageBuffer.Clear();
            base.OnSizeChanged(oldSize, newSize);
           // Layout(oldSize, newSize);
        }

        protected override void OnTopOffsetChange(int actualValue, int newValue)
        {
            LeaveDescendant();
            base.OnTopOffsetChange(actualValue, newValue);

        }

        /// <summary>
        /// Removes the focus of the selected control, if this control is a parent of the selected control.
        /// </summary>
        private void LeaveDescendant()
        {
            FluidControl selected = Container.Host.FocusedControl;
            if (selected != null)
            {
                if (selected.IsDescendantOf(this))
                {
                    Container.Host.FocusedControl = this;
                }
            }
        }


        #region ICommandContainer Members

        public virtual void RaiseCommand(CommandEventArgs e)
        {
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
    }
}
