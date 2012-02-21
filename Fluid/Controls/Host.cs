using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.WindowsCE.Forms;
using System.Threading;

namespace Fluid.Controls
{
    /// <summary>
    /// A windows control that represents a host for a fluid control.
    /// The host also provides gesture, mouse and keyboard events it is the interface between a windows form and fluid control.
    /// </summary>
    public partial class FluidHost : Control, IContainer, ICommandContainer, IHost
    {
        /// <summary>
        /// Gets the instance of the host. There can be only one...
        /// </summary>
        public static FluidHost Instance { get; private set; }


        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public FluidHost()
            : base()
        {
            //   Font = new Font(FontFamily.GenericSansSerif, 8f, FontStyle.Regular);
            if (Instance != null) throw new ArgumentException("There can be only one host.");
            region.MakeInfinite();
            CreateKeyInputHelper();
            BackColor = Color.Empty;
            panel = new FluidPanel();
            panel.BackColor = Color.Transparent;
            panel.EnableDoubleBuffer = true;
            panel.Anchor = AnchorStyles.None;
            this.Control = panel;
            panel.Name = "__root";
            Instance = this;
        }


        private FluidPanel panel;

        #region KeyInputHelper
        /// <summary>
        /// A derived control class does not recognize the correct character for the OnKeyPress event if it comes from a physical keyboard (my xperia x1 keyboard. 
        /// For instance, instead of an 'e' it has a KeyChar='&' value, and Shift or Alt Keys do not work neither on derived control.
        /// Therefore, I use a simple Button control that recognizes the correct character and always focus the button instead of the control and redirect the 
        /// Keyboard events from the button to the child controls. Since I cannot derive from Button class as it does not support OnPaint or OnPaintBackground, I need that helper control.
        /// Unless I don't have a solution for the problem, this workaround simply works.
        /// </summary>
        private void CreateKeyInputHelper()
        {
            keyInputHelperControl = new MyButton();
            keyInputHelperControl.Bounds = new Rectangle(0, 0, 0, 0);
            keyInputHelperControl.KeyPress += new KeyPressEventHandler(tb_KeyPress);
            keyInputHelperControl.KeyDown += new KeyEventHandler(tb_KeyDown);
            keyInputHelperControl.KeyUp += new KeyEventHandler(tb_KeyUp);
            Controls.Add(keyInputHelperControl);
            keyInputHelperControl.Focus();
        }

        private Button keyInputHelperControl;

        void tb_KeyUp(object sender, KeyEventArgs e)
        {
            if (FocusedControl != null) FocusedControl.OnKeyUp(e);
        }

        void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (FocusedControl != null) FocusedControl.OnKeyDown(e);
        }

        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (FocusedControl != null) FocusedControl.OnKeyPress(e);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            region.Dispose();
            region = null;
            panel.Dispose();
            base.Dispose(disposing);
        }

        Region region = new Region();

        private SizeF scaleFactor = new SizeF(1f, 1f);
        private FluidControl control;

        private bool isDown;

        /// <summary>
        /// On a OnDown event which is serialized from root control up to the top control, it can happen, that the selected control changes several times.
        /// Therefore, when an OnDown event is starts, the IsDown is set to true, and after the OnDown event returns, it is set to false. 
        /// In this time, when the SelectedControl changes, no OnLeave and OnFocus events are raised, unless the IsDown is set back to false.
        /// </summary>
        /// <example>
        /// A listbox has a template with a textbox.
        /// When the textbox is already focused and the textbox is clicked again,
        /// the listbox is first set as selected control within the OnDown event, and then the textbox is set again as selected control.
        /// This would happen to raise the OnLeave and OnFocus event for the textbox which would provide some sideeffects 
        /// (e.g. the InputPanel is disabled and enabled which would let the listbox eventually change its position to make the selected item visible, and many more...).
        /// To prevent this, this functionlity is provided.
        /// </example>
        private bool IsDown
        {
            get { return isDown; }
            set
            {
                if (isDown != value)
                {
                    if (value) delayedSelectedControl = selectedControl;
                    isDown = value;
                    if (!value) FocusedControl = delayedSelectedControl;
                }
            }
        }

        private FluidControl selectedControl;
        private FluidControl delayedSelectedControl;

        public FluidControl FocusedControl
        {
            get { return isDown ? delayedSelectedControl : selectedControl; }
            set
            {
                if (IsDown)
                {
                    delayedSelectedControl = value;
                }
                else
                {
                    if (selectedControl != value)
                    {
                        FluidControl c = selectedControl;
                        selectedControl = value;
                        if (c != null) c.OnLeave(this);
                        if (selectedControl != null) selectedControl.OnEnter(this);
                        //            keyInputHelperControl.Focus();
                    }
                }
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            keyInputHelperControl.Focus();
        }


        /// <summary>
        /// Gets or sets the FluidControl to be hosted.
        /// </summary>
        protected FluidControl Control
        {
            get { return control; }
            set
            {
                if (control != value)
                {
                    if (control != null)
                    {
                        SizeF scale = new SizeF(1f / scaleFactor.Width, 1f / scaleFactor.Width);
                        control.Scale(scale);
                    }
                    control = value;
                    if (value != null)
                    {
                        control.Container = this;
                        value.Scale(scaleFactor);
                        control.Bounds = Bounds;
                    }
                }
            }
        }

        protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
        {
            scaleFactor = factor;
            //if (control!=null) control.Layout(Bounds.Size, Bounds.Size, scaleFactor);

            return base.GetScaledBounds(bounds, factor, specified);
        }

        protected override void OnPaintBackground(PaintEventArgs pe)
        {
            // prevent flickering
        }

        private FluidPaintEventArgs paintEvents = new FluidPaintEventArgs();

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (control != null)
            {
                Graphics g = pe.Graphics;

                // this is necassary, when the invalidate() was made outside the fluid control (e.g. when a control overlaps this control)::
                region.Union(pe.ClipRectangle);
                //                pe.Graphics.Clear(Color.Yellow);
                FluidPaintEventArgs e = paintEvents;
                e.Graphics = g;
                e.Region = region;
                g.Clip = region;
                e.ControlBounds = ClientRectangle;
                e.ScaleFactor = ScaleFactor;
                control.OnPaint(e);
                region.MakeEmpty();
            }
        }


        int startTick = 0;
        int lastTick = 0;
        private Point movePoint;
        Point lastGesturePoint = new Point();
        Point startPoint = new Point();
        int ppms;
        Gesture lastGesture = Gesture.None;

        // 200 ms as timeout to recognize  a gesture:
        const int gestureTimeout = 180;

        const int gestureRecognizeMinPixel = 32;

        private GestureEventArgs gestureEvent = new GestureEventArgs();

        protected Gesture Gesture
        {
            get { return lastGesture; }
            set
            {
                if (lastGesture != value)
                {
                    lastGesture = value;
                    if (value != Gesture.None && control != null)
                    {
                        GestureEventArgs e = gestureEvent;
                        e.Gesture = value;
                        e.IsPressed = true;
                        e.PixelPerMs = ppms;
                        e.Distance = 0;
                        control.OnGesture(e);
                        if (e.Gesture != Gesture.None)
                        {
                            control.OnRelease(TranslatePoint(startPoint.X, startPoint.Y));
                        }
                        lastGesture = e.Gesture;
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (control == null) return;
            lastTick = startTick = Environment.TickCount;
            lastGesture = Gesture.None;
            Point p = new Point(e.X, e.Y);
            startPoint = lastGesturePoint = p;
            PointEventArgs pe = TranslatePoint(p.X, p.Y);
            IsDown = true;
            control.OnDown(pe);
            IsDown = false;
            keyInputHelperControl.Focus();
        }

        const int ClickThreshold = 1000;

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (control == null) return;
            int duration = Environment.TickCount - lastTick;
            PointEventArgs p = TranslatePoint(e.X, e.Y);
            control.OnUp(p);
            // note that p might change it's value, therefore it is set again:
            p = TranslatePoint(e.X, e.Y);
            bool gestured = PerformUpGesture(e);
            if (!gestured)
            {
                if (duration < ClickThreshold) control.OnClick(p);
            }
        }

        private bool PerformUpGesture(MouseEventArgs e)
        {
            int tick = Environment.TickCount;
            if (lastGesture != Gesture.None)
            {
                if ((tick - lastTick) < gestureTimeout)
                {
                    int distance = CalculateDistance(e.X, e.Y, startPoint);
                    int time = tick - startTick;
                    int ppms = (int)(1000f * distance / time);
                    GestureEventArgs ge = gestureEvent;
                    ge.Gesture = lastGesture;
                    ge.IsPressed = false;
                    ge.Distance = distance;
                    ge.PixelPerMs = ppms;
                    control.OnGesture(ge);
                    return ge.Handled;
                }
            }
            return false;
        }

        private int CalculateDistance(int x, int y, Point startPoint)
        {
            int dx = x - startPoint.X;
            int dy = y - startPoint.Y;

            return (int)(Math.Sqrt(dx * dx + dy * dy));
        }

        private int CalculatePointsPerMs(int x, int y, Point startPoint, int duration)
        {
            int dx = x - startPoint.X;
            int dy = y - startPoint.Y;
            if (duration == 0) duration = 1;
            return (int)(Math.Sqrt(dx * dy + dy * dy) * 1000f / duration);

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (control == null) return;
            movePoint = new Point(e.X, e.Y);
            int tick = Environment.TickCount;
            control.OnMove(TranslatePoint(e.X, e.Y));
            Gesture = RecognizeGesture(e.X, e.Y, tick, lastGesture);
        }

        private PointEventArgs TranslatePoint(int x, int y)
        {
            return new PointEventArgs(lastGesture, x, y);
        }

        private Gesture RecognizeGesture(int x, int y, int tick, Gesture lastGesture)
        {
            Gesture gesture = lastGesture;

            int duration = tick - lastTick;
            if (duration > gestureTimeout && gesture != Gesture.None) gesture = Gesture.Canceled;
            if (gesture != Gesture.Canceled)
            {
                int dx = x - lastGesturePoint.X;
                int dy = y - lastGesturePoint.Y;

                if (dx == 0 && dy == 0) return gesture;

                int ax = Math.Abs(dx);
                int ay = Math.Abs(dy);

                float xy = ay != 0 ? ax / ay : 1000f;
                float yx = ax != 0 ? ay / ax : 1000f;


                int l = (int)Math.Sqrt((double)((ax * ax / ScaleFactor.Width) + (ay * ay / ScaleFactor.Height)));

                if (l < gestureRecognizeMinPixel) return gesture;

                if (duration == 0) duration = 1;
                ppms = (int)(1000f * l / duration);
                lastGesturePoint = new Point(x, y);
                lastTick = tick;

                if (xy > 3)
                {
                    switch (gesture)
                    {
                        case Gesture.None:
                            gesture = dx > 0 ? Gesture.Right : Gesture.Left;
                            break;

                        case Gesture.Left:
                            if (dx > 0) gesture = Gesture.Canceled;
                            break;

                        case Gesture.Right:
                            if (dx < 0) gesture = Gesture.Canceled;
                            break;

                        default:
                            gesture = Gesture.Canceled;
                            break;
                    }
                }
                else if (yx > 3)
                {
                    switch (gesture)
                    {
                        case Gesture.None:
                            gesture = dy > 0 ? Gesture.Down : Gesture.Up;
                            break;

                        case Gesture.Up:
                            if (dy > 0) gesture = Gesture.Canceled;
                            break;

                        case Gesture.Down:
                            if (dy < 0) gesture = Gesture.Canceled;
                            break;

                        case Gesture.Left:
                            gesture = dy > 0 ? Gesture.LeftDown : Gesture.LeftUp;
                            break;

                        case Gesture.Right:
                            gesture = dy > 0 ? Gesture.RightDown : Gesture.LeftDown;
                            break;

                        default:
                            gesture = Gesture.Canceled;
                            break;
                    }
                }
            }
            if (gesture == Gesture.Canceled)
            {
                startPoint = lastGesturePoint;
                startTick = tick;
            }
            lastTick = tick;

            return gesture;
        }

        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    if (SelectedControl != null) SelectedControl.OnKeyDown(e);
        //}

        //protected override void OnKeyUp(KeyEventArgs e)
        //{
        //    if (SelectedControl != null) SelectedControl.OnKeyUp(e);
        //}

        //protected override void OnKeyPress(KeyPressEventArgs e)
        //{
        //    base.OnKeyPress(e);
        //    if (SelectedControl != null) SelectedControl.OnKeyPress(e);
        //}

        #region IFluidContainer Members


        void IContainer.Invalidate(Rectangle bounds)
        {
            //  Debug.WriteLine("Invalidate " + bounds.Left.ToString() + ", " + bounds.Top.ToString() + "; " + bounds.Width.ToString() + ", " + bounds.Height.ToString());
            region.Union(bounds);
            Invalidate(bounds);
        }


        public IHost Host
        {
            get { return this; }
        }

        public SizeF ScaleFactor
        {
            get { return scaleFactor; }
        }

        #endregion

        /// <summary>
        /// Gets wether the display is in vertical or horicontal mode.
        /// </summary>
        public bool IsVertical
        {
            get
            {
                return Width < Height;
            }
        }

        private Size size = Size.Empty;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.Parent != null)
            {
                Rectangle r = ClientRectangle;
                if (control != null && size != r.Size)
                {
                    size = r.Size;
                    control.Bounds = r;
                    control.Invalidate();
                }
            }
        }

        #region IFluidContainer Members


        public Point PointToHost(int x, int y)
        {
            return new Point(x, y);
        }

        Rectangle IContainer.GetScreenBounds(Rectangle bounds)
        {
            Point p = this.PointToScreen(this.Location);
            bounds.Offset(p.X, p.Y);
            return bounds;
        }


        public void Layout(LayoutEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICommandContainer Members

        public virtual void RaiseCommand(CommandEventArgs e)
        {
            if (Command != null)
            {
                Command(this, e);
            }
        }

        /// <summary>
        /// Occurs when a nested control has raised a command.
        /// </summary>
        public event EventHandler<CommandEventArgs> Command;

        #endregion

        #region IHost Members

        //Graphics IHost.CreateGraphics()
        //{
        //    return this.CreateGraphics();
        //}

        #endregion

        public class MyButton : Button
        {
            protected override void OnPaint(PaintEventArgs e)
            {
                //base.OnPaint(e);
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                //base.OnPaintBackground(e);
            }
        }



        public Rectangle ClientBounds { get { return panel.ClientRectangle; } }

        public void Show(FluidControl control)
        {
            panel.Controls.Add(control);
            panel.Invalidate(control.Bounds);
        }

        public void ShowMaximized(FluidControl control)
        {
            panel.BeginInit();
            panel.Controls.Add(control);
            control.Bounds = panel.ClientRectangle;
            panel.EndInit();
            control.Anchor = FluidControl.AnchorAll;
            panel.Invalidate(control.Bounds);
        }

        public void Hide(FluidControl control)
        {
            Rectangle bounds = control.Bounds;
            panel.Controls.Remove(control);
            panel.Invalidate(bounds);
            control.Visible = false;
        }

        public void Add(FluidControl c)
        {
            if (!panel.Controls.Contains(c))
            {
                panel.Controls.Add(c);
            }
        }

        public void Remove(FluidControl c)
        {
            panel.Controls.Remove(c);
        }

        public void Insert(int index, FluidControl c)
        {
            if (!panel.Controls.Contains(c))
            {
                panel.Controls.Insert(index, c);
            }
        }

        public Cursor Cursor
        {
            get { return Cursor.Current; }
            set { Cursor.Current = value; }
        }

    }


}
