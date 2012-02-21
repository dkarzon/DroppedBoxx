using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Fluid.Drawing.GdiPlus;
using System.Diagnostics;

namespace Fluid.Controls
{
    /// <summary>
    /// The base class of all fluid controls.
    /// </summary>
    public class FluidControl : IDisposable
    {
        public FluidControl()
            : base()
        {
            init();
        }

        public FluidControl(int x, int y, int w, int h)
            : base()
        {
            bounds = new Rectangle(x, y, w, h);
            init();
        }

        private void init()
        {
            BeginInit();
            InitControl();
            if (ControlInitialize != null) ControlInitialize(this, EventArgs.Empty);
            EndInit();
        }

        /// <summary>
        /// uses this to initialize a derived control. this method is executed within a BeginInit EndInit.
        /// </summary>
        protected virtual void InitControl()
        {
            backColor = Color.Empty;
            foreColor = Color.Empty;
        }


        /// <summary>
        /// Occcurs when the control is initialized.
        /// </summary>
        public event EventHandler ControlInitialize;

        #region constants
        public const AnchorStyles AnchorTL = AnchorStyles.Top | AnchorStyles.Left;
        public const AnchorStyles AnchorTR = AnchorStyles.Top | AnchorStyles.Right;
        public const AnchorStyles AnchorBL = AnchorStyles.Bottom | AnchorStyles.Left;
        public const AnchorStyles AnchorBR = AnchorStyles.Bottom | AnchorStyles.Right;
        public const AnchorStyles AnchorLR = AnchorStyles.Left | AnchorStyles.Right;
        public const AnchorStyles AnchorTB = AnchorStyles.Top | AnchorStyles.Bottom;
        public const AnchorStyles AnchorTLR = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
        public const AnchorStyles AnchorBLR = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        public const AnchorStyles AnchorLTB = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
        public const AnchorStyles AnchorRTB = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
        public const AnchorStyles AnchorAll = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
        #endregion

        private bool visible = true;

        /// <summary>
        /// Gets or sets whether to control is visible.
        /// </summary>
        [DefaultValue(true)]
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    if (!Initializing) Invalidate();
                    visible = value;
                    if (!Initializing) Invalidate();
                    OnVisibleChanged();
                }
            }
        }

        /// <summary>
        /// Occurs when the Visible property is changed.
        /// </summary>
        protected virtual void OnVisibleChanged()
        {
            StopAnimations();
            LeaveNestedFocusedControl();
            if (!this.Initializing)
            {
                if (VisibleChanged != null) VisibleChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs when the Visible property is changed.
        /// </summary>
        public event EventHandler VisibleChanged;

        private AnchorStyles anchor = AnchorStyles.Left | AnchorStyles.Top;

        /// <summary>
        /// Gets or sets the anchor style for the control that specifies layout behaviour.
        /// </summary>
        public AnchorStyles Anchor
        {
            get { return anchor; }
            set { anchor = value; }
        }

        public string Name { get; set; }

        /// <summary>
        /// Occurs when a Key is pressed.
        /// </summary>
        public virtual void OnKeyPress(KeyPressEventArgs e)
        {
            if (KeyPress != null) KeyPress(this, e);
            if (Parent != null) Parent.OnKeyPress(e);
        }

        /// <summary>
        /// Occurs when a Key is pressed.
        /// </summary>
        public event KeyPressEventHandler KeyPress;

        /// <summary>
        /// Occurs when a key is down.
        /// </summary>
        public virtual void OnKeyDown(KeyEventArgs e)
        {
            if (KeyDown != null) KeyDown(this, e);
            if (Parent != null) Parent.OnKeyDown(e);
        }

        /// <summary>
        /// Occurs when a key is down.
        /// </summary>
        public event KeyEventHandler KeyDown;

        /// <summary>
        /// Occurs when a key is up.
        /// </summary>
        public virtual void OnKeyUp(KeyEventArgs e)
        {
            if (KeyUp != null) KeyUp(this, e);
            if (Parent != null) Parent.OnKeyUp(e);
        }


        /// <summary>
        /// Occurs when a key is up.
        /// </summary>
        public event EventHandler KeyUp;


        /// <summary>
        /// Gets wether this control is double bufferd.
        /// This property is used to determine whether the parent container should use an own double buffer or just rely on the double buffering of
        /// this control to prevent unecasssary multibuffering.
        /// </summary>
        /// <example>
        /// This property is used for instance by the NavigationPanel class to determine wether to create a temporary buffer while transioning between
        /// two controls.
        /// </example>
        public virtual bool IsDoubleBuffered { get { return false; } }

        private IContainer container;

        /// <summary>
        /// Gets or sets the parent container that keeps this control.
        /// </summary>
        public IContainer Container
        {
            get { return container; }
            set
            {
                container = value;
                OnContainerChanged();

            }
        }

        /// <summary>
        /// Occurs when the Container has changed.
        /// </summary>
        protected virtual void OnContainerChanged()
        {
        }

        /// <summary>
        /// Occurs on stylus, mouse or finger move events.
        /// </summary>
        public virtual void OnMove(PointEventArgs e) { }

        /// <summary>
        /// Occurs when the control is entered and receives the focus.
        /// </summary>
        /// <param name="host">the window host control.</param>
        /// <param name="bounds">The bounds of the control relative to the windows host</param>
        /// <remarks>
        /// When a input control receives focus, it can add a windows control to the host.
        /// See FluidTextBox as example.
        /// </remarks>
        public virtual void OnEnter(IHost host)
        {
        }

        /// <summary>
        /// Occurs when the control is left and looses the focus.
        /// </summary>
        /// <param name="host">The window host control.</param>
        /// <param name="bounds">The bounds of the control relative to the windows host</param>
        public virtual void OnLeave(IHost host)
        {
        }

        internal Rectangle bounds;

        /// <summary>
        /// Gets or sets the bounds of the control.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                if (bounds != value)
                {
                    Size old = bounds.Size;
                    if (!Initializing && visible)
                    {
                        Invalidate();
                        bounds = value;
                        Invalidate();
                    }
                    else bounds = value;
                    if (old != bounds.Size) OnSizeChanged(old, bounds.Size);
                }
            }
        }


        /// <summary>
        /// Occurs when the size has changed.
        /// </summary>
        /// <param name="oldSize">The size before changing.</param>
        /// <param name="newSize">The size after changing.</param>
        protected virtual void OnSizeChanged(Size oldSize, Size newSize)
        {
            //            EnsureSelectedControlRemoved();
            if (SizeChanged != null)
            {
                ChangedEventArgs<Size> e = new ChangedEventArgs<Size>(oldSize, newSize);
                SizeChanged(this, e);
            }
            if (!oldSize.IsEmpty)
            {
                Layouter.AdjustSize(this, oldSize, newSize, scaleFactor);
                ILayoutPanel c = this as ILayoutPanel;
                if (c != null)
                {
                    Layouter.Layout(c, oldSize, newSize, defaultSize);
                }
            }
        }

        public event EventHandler<ChangedEventArgs<Size>> SizeChanged;

        private Font font;

        /// <summary>
        /// Gets or sets the font for the control. If set to null the font of the parent container is used.
        /// </summary>
        public Font Font
        {
            get
            {
                if (font != null) return font;
                if (Container != null) return Container.Font;
                return null;
            }
            set
            {
                if (font != value)
                {
                    font = value;
                    Invalidate();
                }
            }
        }

        private bool isDown = false;

        /// <summary>
        /// Gets whether the control is currently pressed down.
        /// </summary>
        public bool IsDown
        {
            get { return isDown; }
            protected set
            {
                if (isDown != value)
                {
                    isDown = value;
                    OnDownChanged(value);
                }
            }
        }

        protected virtual void OnDownChanged(bool value)
        {
            if (Selectable && value && IsAttached) Container.Host.FocusedControl = this;
        }

        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnPaint(FluidPaintEventArgs e)
        {
            OnPaintBackground(e);
        }

        protected virtual void OnPaintBackground(FluidPaintEventArgs e)
        {
            if (PaintBackground != null) PaintBackground(this, e);
            else
            {
                if (!BackColor.IsEmpty && BackColor != Color.Transparent)
                {
                    Graphics g = e.Graphics;
                    if (BackColor.A == 255 || BackColor.A == 0)
                    {
                        SolidBrush brush = Brushes.GetBrush(BackColor);
                        g.FillRectangle(brush, e.ControlBounds);
                    }
                    else
                    {
                        using (GraphicsPlus gp = new GraphicsPlus(g))
                        {
                            using (SolidBrushPlus brush = new SolidBrushPlus(BackColor))
                            {
                                gp.FillRectangle(brush, e.ControlBounds);
                            }
                        }
                    }
                }
            }
        }

        protected bool HasCustomPaintBackgound { get { return PaintBackground != null; } }

        public event EventHandler<FluidPaintEventArgs> PaintBackground;

        private bool initializing = false;

        /// <summary>
        /// Gets wether either this control or a parent of this control is in a BeginInit state.       
        /// </summary>
        /// <remarks>
        /// This is important for template. Imagine a Textbox in a Template assoicated with a OnTextChanged.
        /// to ensure that the OnTextChanged is not raised when the template is data bound (and even bound to another data item!) the OnTextChanged
        /// checks the Initializing state wether to throw an event.
        /// 
        /// Ensure to implement Initializing checking before throwing an event for all other overriden controls!
        /// </remarks>
        public bool Initializing
        {
            get
            {
                if (initializing || Container == null) return true;
                FluidControl parent = Parent;
                if (parent != null) return parent.Initializing;
                return false;
            }
        }

        /// <summary>
        /// Begins to initialize the control and ignores (nested) invalidations.
        /// </summary>
        public void BeginInit()
        {
            initializing = true;
        }

        /// <summary>
        /// Finishes to initilalize the control and enables (nested) invalidations.
        /// </summary>
        public void EndInit()
        {
            initializing = false;
        }

        /// <summary>
        /// Clips the specified rectangle to the bounds of this control to avoid overlapping.
        /// </summary>
        /// <param name="rect">The rectangle to clip.</param>
        public void ClipRectangle(ref Rectangle rect)
        {
            Rectangle union = this.Bounds;
            if (rect.X < union.X)
            {
                int right = rect.Right;
                rect.X = union.X;
                rect.Width = right - rect.X;
            }
            if (rect.Right > union.Right)
            {
                rect.Width = union.Right - rect.X;
            }

            if (rect.Y < union.Y)
            {
                int bottom = rect.Bottom;
                rect.Y = union.Y;
                rect.Height = bottom - rect.Y;
            }
            if (rect.Bottom > union.Bottom)
            {
                rect.Height = union.Bottom - rect.Y;
            }
            if (rect.Height < 0) rect.Height = 0;
            if (rect.Width < 0) rect.Width = 0;
        }

        /// <summary>
        /// Invalidates the canvas of the control.
        /// </summary>
        public virtual void Invalidate()
        {
            if (Initializing || !visible) return;
            if (Container != null) Container.Invalidate(this.Bounds);
        }


        protected Color backColor = Color.Empty;
        protected Color foreColor = Color.Empty;

        /// <summary>
        /// Gets or sets the foreground color. if empty, the foreground color fo the container control is used.
        /// </summary>
        public virtual Color ForeColor
        {
            get
            {
                return foreColor.IsEmpty ? Container.ForeColor : foreColor;
            }
            set
            {
                if (foreColor != value)
                {
                    foreColor = value;
                    OnForeColorChanged();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Occurs when the ForeColor property is changed.
        /// </summary>
        protected virtual void OnForeColorChanged()
        {
            if (ForeColorChanged != null) ForeColorChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the ForeColor property is changed.
        /// </summary>
        public event EventHandler ForeColorChanged;

        /// <summary>
        /// Gets or sets the background color. if empty, the background color fo the container control is used.
        /// </summary>
        public virtual Color BackColor
        {
            get
            {
                if (!backColor.IsEmpty) return backColor;
                return Container != null ? Container.BackColor : Color.Empty;
            }
            set
            {
                if (backColor != value)
                {
                    backColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of this control.
        /// </summary>
        public int Height { get { return bounds.Height; } set { Bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, value); } }
        
        /// <summary>
        /// Gets or sets the with of this control.
        /// </summary>
        public int Width
        {
            get { return bounds.Width; }
            set
            {
                Bounds = new Rectangle(bounds.X, bounds.Y, value, bounds.Height);
            }
        }

        /// <summary>
        /// Gets or sets the left offset of this control relative to the parent control.
        /// </summary>
        public int Left { get { return bounds.X; } set { Bounds = new Rectangle(value, bounds.Y, bounds.Width, bounds.Height); } }

        /// <summary>
        /// Gets or sets the top offset of this control relative to the parent control.
        /// </summary>
        public int Top
        {
            get { return bounds.Y; }
            set
            {
                Bounds = new Rectangle(bounds.X, value, bounds.Width, bounds.Height);
            }
        }

        /// <summary>
        /// Gets the right offset of this control relative to the parent control.
        /// </summary>
        public int Right { get { return bounds.Right; } }

        /// <summary>
        /// Gets the bottom offset of this control relative to the parent control.
        /// </summary>
        public int Bottom { get { return bounds.Bottom; } }


        /// <summary>
        /// Gets wether the control is selectable and can get focus.
        /// </summary>
        public virtual bool Selectable { get { return false; } }

        /// <summary>
        /// Gets whether the control can understand up,down,release events.
        /// </summary>
        public virtual bool Active { get { return false; } }


        /// <summary>
        /// Occurs on a styles or mouse down event.
        /// </summary>
        public virtual void OnDown(PointEventArgs e) { if (enabled) IsDown = true; }

        /// <summary>
        /// Occurs on a styles or mouse up event.
        /// </summary>
        public virtual void OnUp(PointEventArgs e)
        {
            canClick = IsDown;
            IsDown = false;
            //if (Parent == null)
            //{
            //    if (isDown && !clicked) OnClick(e);
            //}
        }

        private bool canClick;

        /// <summary>
        /// Gets wether a Click can be performed.
        /// </summary>
        public bool CanClick { get { return canClick; } }

        /// <summary>
        /// Occurs when the control is clicked.
        /// </summary>
        public virtual bool OnClick(PointEventArgs e)
        {
            if (canClick && enabled)
            {
                if (ClientRectangle.Contains(e.X, e.Y))
                {
                    IsDown = false;
                    PerformClick();
                }
            }
            return false;
        }


        /// <summary>
        /// Performs a Click.
        /// </summary>
        public virtual void PerformClick()
        {
            if (Click != null && enabled) Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the control is clicked.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// Occurs when the control is released to be in down mode but the mouse, style or finger still presses on the display.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnRelease(PointEventArgs e)
        {
            IsDown = false;
        }


        //public Rectangle GetScaledBounds(Rectangle bounds)
        //{
        //    float w = ScaleFactor.Width;
        //    float h = ScaleFactor.Height;
        //    return new Rectangle((int)(w * bounds.X), (int)(h * bounds.Y), (int)(w * bounds.Width), (int)(h * bounds.Height));
        //}

        private readonly SizeF defaultSize = new SizeF(1f, 1f);

        /// <summary>
        /// Gets the scale factor for rendering.
        /// </summary>
        public virtual SizeF ScaleFactor
        {
            get
            {
                return Container != null ? Container.ScaleFactor : scaleFactor;
            }
            set
            {
                scaleFactor = value;
            }
        }

        /// <summary>
        /// invalides the control and repaints the control immediately.
        /// </summary>
        public void Refresh()
        {
            Invalidate();
            if (IsAttached) Container.Host.Update();
        }

        /// <summary>
        /// Updates the invalidated region and repaints the control immediately.
        /// </summary>
        public void Update()
        {
            if (IsAttached)  Container.Host.Update();
        }

        /// <summary>
        /// Gets the ClientRectangle of this control.
        /// This a rectangle of (0,0,Width,Height).
        /// </summary>
        public Rectangle ClientRectangle
        {
            get { return new Rectangle(0, 0, Width, Height); }
        }

        /// <summary>
        /// Occurs when a gesture was dedectecd.
        /// </summary>
        /// <param name="e">The GestureEventArgs.</param>
        public virtual void OnGesture(GestureEventArgs e)
        {
            if (e.Gesture != Gesture.None) IsDown = false;
        }

        /// <summary>
        /// Gets whether the control is (partially) transparent.
        /// </summary>
        /// <remarks>
        /// Used to determine wether a child control must be invalidated when the container panel is invaliated.
        /// </remarks>
        public virtual bool IsTransparent
        {
            get
            {
                if (backColor == Color.Empty || backColor == Color.Transparent) return true;
                int alpha = BackColor.A;
                return alpha != 0 && alpha < 255;
            }
        }


        private SizeF scaleFactor = new SizeF(1f, 1f);

        /// <summary>
        /// Scales the control with the specified factor.
        /// </summary>
        /// <param name="scaleFactor"></param>
        public virtual void Scale(SizeF scaleFactor)
        {
            if (scaleFactor != this.scaleFactor)
            {
                this.scaleFactor = scaleFactor;
                BeginInit();
                try
                {
                    Rectangle r = Bounds;
                    // do not use Bounds so no OnSizeChanged is raised!
                    this.bounds = new Rectangle(
                        (int)(r.X * scaleFactor.Width), (int)(r.Y * scaleFactor.Height),
                        (int)(r.Width * scaleFactor.Width), (int)(r.Height * scaleFactor.Height));

                    {
                        IMultiControlContainer container = this as IMultiControlContainer;
                        if (container != null && container.Controls != null)
                        {
                            foreach (FluidControl c in container.Controls)
                            {
                                c.Scale(scaleFactor);
                            }
                        }
                    }
                    {
                        ISingleControlContainer container = this as ISingleControlContainer;
                        if (container != null && container.Control != null) container.Control.Scale(scaleFactor);
                    }
                }
                finally
                {
                    EndInit();
                    Invalidate();
                }
            }
        }

        static readonly SizeF unscaledSize = new SizeF(1f, 1f);

        /// <summary>
        /// Converts a Rectangle into a RectangelF.
        /// </summary>
        /// <param name="rect">The Rectangle class to convert.</param>
        /// <returns>The converted RectangleF class.</returns>
        public static RectangleF RectFFromRect(Rectangle rect)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            // this is necassary to make the IsAvailable to be false, when the control is disposed, but somewhere else is still a pointer on it:
            this.Container = null;
        }

        #endregion

        /// <summary>
        /// Gets wether the control is alive, which means that it belongs to a host and that it is set to be visible.
        /// </summary>
        public bool IsAvailable
        {
            get { return Container != null && Container.Host != null && visible; }
        }

        /// <summary>
        /// Gets whether the control is attached to an IHost.
        /// </summary>
        public bool IsAttached
        {
            get { return Container != null && Container.Host != null; }
        }

        /// <summary>
        /// Gets whether this control is a descendant of the specified container.
        /// </summary>
        /// <param name="container">The container which might be an ancestor.</param>
        /// <returns>True, if this control is a descendant of the container (or the container is an ancestor of the control), otherwhise false.</returns>
        public bool IsDescendantOf(IContainer container)
        {
            FluidControl parent = Parent;

            while (parent != null)
            {
                if (parent == container) return true;
                parent = parent.Parent;
            }
            return false;
        }

        /// <summary>
        /// Sets the focus to this control.
        /// </summary>
        public virtual void Focus()
        {
            if (Selectable)
            {
                if (IsAttached)
                {
                    Container.Host.FocusedControl = this;
                    OnGotFocus();
                }
            }
        }

        /// <summary>
        /// Leaves the focus from this control.
        /// </summary>
        public virtual void Leave()
        {
            if (Selectable && IsAttached)
            {
                FluidControl parent = Parent;
                while (parent != null)
                {
                    if (parent.Selectable)
                    {
                        parent.Focus();
                        return;
                    }
                    parent = parent.Parent;
                }
                OnLeave(Container.Host);
            }
        }


        /// <summary>
        /// Gets the bounds of the control as they appear in the screen.
        /// </summary>
        public Rectangle ScreenBounds
        {
            get { return GetHostBounds(); }
        }


        /// <summary>
        /// Gets the Bounds of this control relative to the IHost.
        /// </summary>
        /// <returns>The bounds of this control relative to the IHost.</returns>
        private Rectangle GetHostBounds()
        {
            Rectangle bounds = this.Bounds;

            if (container != null) bounds = Container.GetScreenBounds(bounds);

            return bounds;
        }

        /// <summary>
        /// Gets the parent control, otherwise null.
        /// </summary>
        public FluidControl Parent
        {
            get { return Container as FluidControl; }
        }

        /// <summary>
        /// Gets the template for this control, otherwise null.
        /// </summary>
        public FluidTemplate Template
        {
            get
            {
                FluidControl parent = Parent;

                while (parent != null)
                {
                    FluidTemplate template = parent as FluidTemplate;
                    if (template != null) return template;
                    parent = parent.Parent;
                }
                return null;
            }
        }

        /// <summary>
        /// Occurs when the control has got the focus.
        /// </summary>
        public event EventHandler GotFocus;

        /// <summary>
        /// Occurs when the control has got the focus.
        /// </summary>
        protected virtual void OnGotFocus()
        {
            if (GotFocus != null) GotFocus(this, EventArgs.Empty);
        }



        /// <summary>
        /// Ensures that the selected control is removed (OnLeave) if it is a descendant of this control.
        /// </summary>
        /// <returns>True, if the focused control was nested in this control, otherwise false.</returns>
        public bool LeaveNestedFocusedControl()
        {
            if (IsAttached)
            {
                IContainer container = this as IContainer;
                if (container != null)
                {
                    if (Container == null || Container.Host == null) return false;
                    FluidControl selected = IsAttached ? Container.Host.FocusedControl : null;
                    if (selected != null)
                    {
                        if (container != null && selected.IsDescendantOf(container))
                        {
                            if (IsAttached) Container.Host.FocusedControl = this.Selectable ? this : null;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets whether any ancestor has set the IsDoubleBufferd property set to true.
        /// </summary>
        public bool IsAncestorDoubleBuffered
        {
            get
            {
                return isAncestorDoubleBuffered();
            }
        }

        private bool isAncestorDoubleBuffered()
        {
            FluidControl parent = this.Parent;
            while (parent != null)
            {
                if (parent.IsDoubleBuffered) return true;
                parent = parent.Parent;
            }
            return false;
        }

        /// <summary>
        /// Used to stop any animation of a control.
        /// </summary>
        public virtual void StopAnimations()
        {
        }

        /// <summary>
        /// Stops all nested animations of this control.
        /// </summary>
        public virtual void StopNestedAnimations()
        {
            StopAnimations();
            IMultiControlContainer mc = this as IMultiControlContainer;
            if (mc != null)
            {
                foreach (FluidControl c in mc.Controls) c.StopNestedAnimations();
            }
        }


        /// <summary>
        /// Converts a scaled X-Value to it's unscaled value depending on the current ScaleFactor.
        /// </summary>
        public int UnscaleX(int value) { return (int)(value / ScaleFactor.Width); }

        /// <summary>
        /// Converts a scaled Y-Value to it's unscaled value depending on the current ScaleFactor.
        /// </summary>
        public int UnscaleY(int value) { return (int)(value / ScaleFactor.Height); }

        /// <summary>
        /// Converts an X-Value to the scaled value depending on the current ScaleFactor.
        /// </summary>
        public int ScaleX(int value) { return (int)(ScaleFactor.Width * value); }

        /// <summary>
        /// Converts an Y-Value to the scaled value depending on the current ScaleFactor.
        /// </summary>
        public int ScaleY(int value) { return (int)(ScaleFactor.Height * value); }

        public object Tag { get; set; }

        /// <summary>
        /// Creates the Graphics for this control from the IHost.
        /// </summary>
        /// <returns></returns>
        public Graphics CreateGraphics()
        {
            return FluidHost.Instance.CreateGraphics();
        }

        private bool enabled = true;

        /// <summary>
        /// Gets or sets whether the control is enabled.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnEnabledChanged();
                }
            }
        }

        /// <summary>
        /// Occurs when the IsEnabled value is changed.
        /// </summary>
        protected virtual void OnEnabledChanged()
        {
        }

        /// <summary>
        /// Occurs when the control has been added to a ControlCollection.
        /// </summary>
        internal protected virtual void OnControlAdded(IContainer container)
        {
        }
    }
}
