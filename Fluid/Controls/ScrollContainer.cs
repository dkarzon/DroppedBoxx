using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.Diagnostics;
using Fluid.Native;
using Fluid.Drawing.GdiPlus;
using Fluid.Classes;

namespace Fluid.Controls
{
    /// TODO: Jog select after gesure left+right and then move up or down.

    /// <summary>
    /// A base class for vertical scrollable controls that implements standard scroll behavior.
    /// </summary>
    public partial class ScrollContainer : FluidControl, IVirtualContainer
    {
        #region const values
        /// <summary>
        /// The minimum pixels distance to scroll up or down within OnMove events.
        /// </summary>
        const int MinMovePixelDistance = 12;

        /// <summary>
        /// The minimum pixel distance to recognize up or down gestures for auto scroll.
        /// </summary>
        const int MinAutoScrollPixelDistance = 10;
        #endregion

        public ScrollContainer()
            : base()
        {
        }

        public ScrollContainer(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
        }


        protected override void InitControl()
        {
            base.InitControl();
            EnableDoubleBuffer = true;
            ShowScrollBar = true;
            ScrollBarButtonBorderColor = Color.Silver;
            ScrollBarButtonColor = Color.Black;
            EnableAnimation = false;
            EnableAutoScroll = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            ClearDBuffer();
            if (this.scrollBarButton != null)
            {
                scrollBarButton.Dispose();
                scrollBarButton = null;
            }
            scrollAnimation.Dispose();
            this.backgroundBrush.Dispose();

            if (scrollBarButtonBuffer != null) scrollBarButtonBuffer.Dispose();
            scrollBarButtonBuffer = null;
        }

        protected void ClearDBuffer()
        {
            if (doubleBuffer != null) doubleBuffer.Dispose();
            doubleBuffer = null;
        }


        protected int topOffset = 0;

        private int EnsureTopOffset(int value)
        {
            int min = 0;
            if (value < min) value = min;
            else
            {
                int h = Math.Max(DisplayHeight - (int)(Height / this.ScaleFactor.Height), 0);
                if (value > h) value = h;
            }
            return value;
        }

        /// <summary>
        /// Gets or sets the top position of the content.
        /// </summary>
        public int TopOffset
        {
            get { return topOffset; }
            set
            {
                if (!enableScroll) value = 0;
                value = EnsureTopOffset(value);
                if (topOffset != value)
                {
                    SetTopOffset(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the scaled value for TopOffset
        /// </summary>
        public int ScaledTopOffset
        {
            get { return (int)Math.Round(TopOffset * ScaleFactor.Height); }
            set
            {
                TopOffset = (int)(value / ScaleFactor.Height);
            }
        }

        /// <summary>
        /// Gets the scaled value for of the DisplayHeight property.
        /// </summary>
        public int ScaledDisplayHeight
        {
            get { return (int)Math.Round(DisplayHeight * ScaleFactor.Height); }
        }

        /// <summary>
        /// Gets the display height 
        /// </summary>
        public virtual int DisplayHeight
        {
            get { return Height; }
        }

        /// <summary>
        /// Gets the bottom offset that is displayed.
        /// </summary>
        public int BottomOffset
        {
            get
            {
                return (TopOffset + Height);
            }
        }

        /// <summary>
        /// Gets the scaled value of DisplayBottom.
        /// </summary>
        public int ScaledBottomOffset
        {
            get
            {
                return ScaledTopOffset + Height;
            }
        }

        //TODO: Remove
        /// <summary>
        /// Gets the virtual bottom of the container.
        /// </summary>        
        public int TotalBottom
        {
            get
            {
                return TopOffset + DisplayHeight;
            }
        }

        /// TODO: Remove
        /// <summary>
        /// Gets the scaled value of TotalBottom
        /// </summary>
        [Obsolete]
        public int ScaledTotalBottom
        {
            get
            {
                return (int)Math.Round(TotalBottom * ScaleFactor.Height);
            }
        }

        protected PointEventArgs touchdPosition = new PointEventArgs(Gesture.None, 0, 0);
        int downTop = 0;
        bool canScroll = true;

        /// <summary>
        /// Gets or sets wether the control can scroll.
        /// </summary>
        internal bool CanScroll
        {
            get { return canScroll && enableScroll; }
            set
            {
                if (canScroll != value)
                {
                    canScroll = value;
                    // IsDown = false;
                }
            }
        }
        protected override void OnDownChanged(bool value)
        {
            base.OnDownChanged(value);
        }
        private Gesture pressedGesture;

        public override void OnGesture(GestureEventArgs e)
        {
            if (e.IsPressed)
            {
                pressedGesture = e.Gesture;
                switch (e.Gesture)
                {
                    case Gesture.Up:
                    case Gesture.Down:
                        e.Handled = true;
                        break;

                    case Gesture.Canceled:
                        e.Gesture = Gesture.None;
                        e.Handled = true;
                        break;

                    default:
                        IsDown = false;
                        CanScroll = false;
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                switch (e.Gesture)
                {
                    case Gesture.Down:
                        if (e.Distance > MinAutoScrollPixelDistance)
                        {
                            e.Handled = true;
                            int speed = e.PixelPerMs;
                            if (speed > 0) BeginAutoScroll(-speed);
                        }
                        break;

                    case Gesture.Up:
                        if (e.Distance > MinAutoScrollPixelDistance)
                        {
                            e.Handled = true;
                            int speed = e.PixelPerMs;
                            if (speed > 0) BeginAutoScroll(speed);
                        }
                        break;
                }
            }
            if (MouseGesture != null) MouseGesture(this, e);
        }

        private bool enableScroll = true;

        /// <summary>
        /// Gets or sets whether scrolling is enabled.
        /// </summary>
        [DefaultValue(true)]
        public bool EnableScroll
        {
            get { return enableScroll; }
            set { enableScroll = value; }
        }

        public override void OnDown(PointEventArgs e)
        {
            ///stop the count down to hide the scrollbar, but do not necassarily show the scrollbar:
            StopFadeTimer();

            touchdPosition.X = e.X;
            touchdPosition.Y = e.Y;
            if (enableScroll)
            {
                pressedGesture = Gesture.None;
                bool isAutoScrolling = IsAutoScrolling;
                StopAutoScroll();
                lastDeltaY = 0;
                lastDeltaX = 0;
                lastTick = Environment.TickCount;
                downTop = TopOffset;

                CanScroll = true;
                IsMoving = false;
                IsDown = !isAutoScrolling;
            }
        }

        public override void OnUp(PointEventArgs e)
        {
            base.OnUp(e);

            int tickDelta = Environment.TickCount - lastTick;
            IsMoving = false;

            EnsureValidPosition();
            if (IsDown)
            {
                OnClick(touchdPosition);
            }
            IsDown = false;
        }

        /// <summary>
        /// Gets whether the position is valid
        /// </summary>
        /// <returns></returns>
        public bool IsPositionValid()
        {
            int top = ScaledTopOffset;
            if (top < 0) return false;
            int h = ScaledDisplayHeight - ScaledTopOffset;
            return h >= Height;

        }

        /// <summary>
        /// checks whether to scroll to a valid position, e.g. when VirtualTop > 0.
        /// </summary>
        protected bool EnsureValidPosition()
        {
            if (TopOffset < 0)
            {
                AutoScrollToDefault();
                return true;
            }
            else
            {
                if (TopOffset > 0)
                {
                    int h = ScaledDisplayHeight - ScaledTopOffset + GetMaxHeight() - Height;
                    if (h < Height)
                    {
                        AutoScrollToDefault();
                        return true;
                    }
                }
            }
            return false;
        }

        private bool BeginAutoScroll(int dy)
        {
            if (EnableAutoScroll)
            {
                if (TopOffset < 0 && dy < 0) return false;
                if (BottomOffset > TotalBottom && dy > 0) return false;

                int v = Math.Abs(dy);
                AutoScrollUpDown(dy);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets or sets whether AutoScroll is enabled.
        /// </summary>
        [DefaultValue(true)]
        public bool EnableAutoScroll { get; set; }

        /// <summary>
        /// Specifies wheter the thresold was reached and the content of the control is virtually moved.
        /// </summary>
        private bool isMoving = false;

        /// <summary>
        /// Gets whether the content of the control is currently moving.
        /// </summary>
        public bool IsMoving
        {
            get { return isMoving; }
            protected set
            {
                if (isMoving != value)
                {
                    isMoving = value;
                    if (value) BeginMoving(); else EndMoving();
                }
            }
        }

        private bool scrollBarVisible = false;
        protected bool ScrollBarVisible
        {
            get { return scrollBarVisible; }
            set
            {
                if (!CanShowScrollbar) value = false;
                if (scrollBarVisible != value)
                {
                    scrollBarVisible = value;
                    if (!value)
                    {
                        FadeOutScrollBar();
                    }
                }
                if (value)
                {
                    // always set scrollbarVisibleCountDown to the start value to increase the countdown.
                    scrollbarButtonAlpha = 257;

                    // don't invalidate the bitmap since it is not modified:                    
                    BaseInvalidate(GetListBounds());
                }
            }
        }
        private int lastDeltaY;
        private int lastDeltaX;
        private int lastTick;

        public override void OnMove(PointEventArgs e)
        {
            SizeF factor = ScaleFactor;
            int deltaY = e.Y - touchdPosition.Y;
            int deltaX = e.X - touchdPosition.X;

            lastDeltaX = deltaX;
            lastTick = Environment.TickCount;
            int threshold = (int)(ScrollContainer.MinMovePixelDistance * factor.Height);

            if ((isMoving || Math.Abs(deltaY) >= threshold) && CanScroll)
            {
                if (!isMoving) touchdPosition = e;
                IsMoving = true;
                Scroll(touchdPosition, new Point(e.X, e.Y));
            }
        }



        /// <summary>
        /// Scroll the container with finger, stylus or mouse.
        /// </summary>
        /// <param name="pressed">The position when the container was first touched.</param>
        /// <param name="moving">The position while moving.</param>
        protected virtual void Scroll(PointEventArgs pressed, Point moving)
        {
            if (CanShowScrollbar)
            {
                StopFadeTimer();
                ShowScrollBar = true;
                int deltaY = (int)((moving.Y - pressed.Y));
                int deltaX = (int)((moving.X - pressed.X));
                lastDeltaY = downTop - deltaY - TopOffset;
                int vt = downTop - (int)(deltaY / ScaleFactor.Height);
                SetTopOffset(vt);
            }
        }

        private int dBufferTop = 0;

        /// <summary>
        /// Gets the top offset of the double buffer.
        /// </summary>
        protected int DBufferTop
        {
            get { return dBufferTop; }
            set { dBufferTop = value; }
        }

        /// <summary>
        /// Gets the top offset for painting.
        /// </summary>
        protected int PaintTop
        {
            get { return EnableDoubleBuffer ? DBufferTop : ScaledTopOffset; }
        }

        protected int DBufferBottom
        {
            get { return DBufferTop + DBufferHeight; }
        }

        private int dBufferSpace = 60;
        protected int DBufferSpace
        {
            get { return dBufferSpace; }
            set { dBufferSpace = value; }
        }

        protected virtual int DBufferHeight
        {
            get
            {
                return Height + dBufferSpace;
            }
        }

        /// <summary>
        /// Sets the TopOffset value without checking for valid limits.
        /// </summary>
        /// <param name="top">The value of TopOffset that can be ANY value, even values less than zero.</param>
        /// <remarks>
        /// This method is actually used to enable the display to be moved outside the range manually and then flipping to the legal
        /// position when the display is untapped.
        /// </remarks>
        protected void SetTopOffset(int top)
        {
            if (top != topOffset)
            {
                OnTopOffsetChange(TopOffset, top);
                int dy = top - TopOffset;
                int ay = Math.Abs(dy);
                if (ay < 5 * Height)
                {
                    if (dy > 0) MoveUp(ay);
                    if (dy < 0) MoveDown(ay);
                }
                else
                {
                    //base.Invalidate();
                    BaseInvalidate(GetListBounds());
                }
            }
        }


        /// <summary>
        /// Occurs when the TopOffset value is about to be changed.
        /// </summary>
        /// <param name="actualValue">The actual value of TopOffset.</param>
        /// <param name="newValue">The new value of TopOffset.</param>
        protected virtual void OnTopOffsetChange(int actualValue, int newValue)
        {
        }

        private void MoveDown(int dy)
        {
            if (IsDoubleBuffered)
            {
                PaintDoubleBuffer();
                topOffset -= dy;
                InternalRefresh();
            }
            else
            {
                BaseInvalidate(GetListBounds());
                //                Container.Invalidate(GetListBounds());
                //                base.Invalidate();
            }
        }


        public override bool IsDoubleBuffered { get { return doubleBuffer != null; } }

        private void MoveUp(int dy)
        {
            if (IsDoubleBuffered)
            {
                PaintDoubleBuffer();
                topOffset += dy;
                InternalRefresh();
            }
            else
            {
                topOffset += dy;
                BaseInvalidate(GetListBounds());
                //                Container.Invalidate(GetListBounds());
                //                base.Invalidate(GetListBounds());
            }
        }

        void InternalRefresh()
        {
            // dont' call the derived Invalidate, since it would unecassarily also update the bitmap cache, which would make the cache obsolete:
            Rectangle r = GetListBounds();
            r.Offset(Left, Top);
            Container.Invalidate(r);
            Update();
        }

        /// <summary>
        /// Gets whether the scrollbar can be shown.
        /// </summary>
        protected bool CanShowScrollbar
        {
            get
            {
                return ScaledDisplayHeight > Height;
            }
        }

        /// <summary>
        /// Occurs when the the panel starts beeing virtually moved.
        /// </summary>
        protected virtual void BeginMoving()
        {
            ScrollBarVisible = true;
            IsDown = false;
        }

        /// <summary>
        /// Occurs when teh panel end beeing virtually moved.
        /// </summary>
        protected virtual void EndMoving()
        {
            ScrollBarVisible = false;
        }

        private int alpha = 255;

        [DefaultValue(255)]
        public int Alpha
        {
            get { return alpha; }
            set
            {
                if (value < 0) value = 0;
                if (value > 255) value = 255;
                if (alpha != value)
                {
                    alpha = value;
                    BaseInvalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether double buffering is enabled.
        /// see also <seealso cref="TransparentColor"/>.
        /// </summary>
        public bool EnableDoubleBuffer { get; set; }

        bool IsScrollBarVisible { get { return scrollbarButtonAlpha > 0; } }

        /// <summary>
        /// Paints the control.
        /// Note: Dot derive this method, since it handles double buffering. Derive from PaintBuffered instead.
        /// </summary>
        /// <param name="pe">The FluidPaintEventArgs.</param>
        public override void OnPaint(FluidPaintEventArgs pe)
        {
            if (EnableDoubleBuffer)
            {
                PaintDoubleBuffered(pe);
            }
            else
            {
                PaintUnbuffered(pe);
            }
        }

        private void PaintUnbuffered(FluidPaintEventArgs pe)
        {
            PaintContent(pe);
            if (IsScrollBarVisible)
            {
                Rectangle clip = pe.ControlBounds;
                Rectangle bounds = GetScrollbarButtonBounds();
                bounds.Offset(clip.X, clip.Y);
                PaintScrollBarButton(pe.Graphics, bounds);
            }
        }

        private void PaintDoubleBuffered(FluidPaintEventArgs pe)
        {
            PaintDoubleBuffer();
            if (!DBufferContainsDisplay())
            {
                EnsureDisplayInDBuffer();
            }
            if (IsScrollBarVisible)
            {
                PaintDbWithScrollButtonBitmap(pe);
            }
            else
            {
                Point p = Container.PointToHost(Left, Top);
                PaintDBuffer(pe, p.X, p.Y);
            }
        }

        private void EnsureDoubleBuffer()
        {
            if (doubleBuffer == null) doubleBuffer = new DoubleBuffer(Width, Height + DBufferSpace);
        }

        protected void PaintDBuffer(FluidPaintEventArgs pe, int x, int y)
        {
            int top = ScaledTopOffset;
            int dy = DBufferTop - top;
            Rectangle dstRect = GetListBounds();
            Rectangle srcRect = new Rectangle(0, -dy + dstRect.Y, dstRect.Width, dstRect.Height);
            dstRect.Offset(x, y);
            EnsureDoubleBuffer();

            pe.Graphics.DrawImage(doubleBuffer.Image, dstRect, srcRect, GraphicsUnit.Pixel);
        }


        protected void PaintDBuffer(FluidPaintEventArgs pe, int x, int y, Rectangle dstRect)
        {
            int top = ScaledTopOffset;
            int dy = DBufferTop - top;
            Rectangle srcRect = new Rectangle(0, -dy + dstRect.Y, dstRect.Width, dstRect.Height);
            dstRect.Offset(x, y);
            EnsureDoubleBuffer();
            pe.Graphics.DrawImage(doubleBuffer.Image, dstRect, srcRect, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Gets the bounds of the canvas where to paint the list, where point(0,0) represent the top left corner of this control.
        /// </summary>
        /// <returns>The rectangle with the bounds.</returns>
        protected virtual Rectangle GetListBounds()
        {
            return new Rectangle(0, 0, Width, Height);
        }


        /// <summary>
        /// Gets the bounds for the canvas.
        /// </summary>
        /// <returns>A rectangle that specifies the bounds where painting is allowed.</returns>
        protected Rectangle GetPaintBounds()
        {
            if (EnableDoubleBuffer) return new Rectangle(0, 0, Width, DBufferHeight); else return new Rectangle(0, 0, Width, Height);
        }



        private DoubleBuffer scrollBarButtonBuffer;

        private DoubleBuffer EnsureScrollbarButtonBuffer(int width, int height)
        {
            if (scrollBarButtonBuffer == null)
            {
                scrollBarButtonBuffer = new DoubleBuffer(false);
            }
            scrollBarButtonBuffer.EnsureDBuffer(width, height);
            return scrollBarButtonBuffer;
        }

        /// <summary>
        /// Paints the bitmap  with the scrollbar button which is temporarily added to the buffer.
        /// </summary>
        /// <param name="pe"></param>
        private void PaintDbWithScrollButtonBitmap(FluidPaintEventArgs pe)
        {
            Rectangle bounds = GetBufferedScrollbarButtonBounds();

            DoubleBuffer buffer = EnsureScrollbarButtonBuffer(bounds.Width, bounds.Height);
            Graphics g = doubleBuffer.Graphics;
            Graphics bufferG = buffer.Graphics;
            Rectangle clip = new Rectangle(0, 0, bounds.Width, bounds.Height);

            GdiExt.Copy(g, bounds, bufferG, 0, 0);
            PaintScrollBarButton(g, bounds);
            Rectangle r = pe.ControlBounds;
            PaintDBuffer(pe, r.X, r.Y);

            GdiExt.Copy(bufferG, clip, g, bounds.X, bounds.Y);
        }


        private DoubleBuffer doubleBuffer;

        /// <summary>
        /// Determines wether the display is completely inside the double buffer image.
        /// </summary>
        /// <returns>Ture, whether the display is completely inside the dbuffer, otherwise false.</returns>
        protected bool DBufferContainsDisplay()
        {
            int top = ScaledTopOffset;
            if (top < DBufferTop) return false;
            if (ScaledBottomOffset > DBufferBottom) return false;
            return true;
        }

        /// <summary>
        /// Ensures that the Display is completely inside the DBuffer.
        /// </summary>
        protected void EnsureDisplayInDBuffer()
        {
            //PaintDoubleBuffer();  // not necassary, since already down immediately before calling EnsureDisplayInDBuffer.

            int dy = DBufferTop - ScaledTopOffset;

            if (dy > 0)
            {
                dy = Math.Max(dy, DBufferSpace);
                DBufferTop -= dy;
                EnsureDoubleBuffer();
                doubleBuffer.ScrollDown(dy);
                PaintDoubleBuffer();
            }
            if (dy < 0)
            {
                dy = -dy;
                dy = Math.Max(dy, DBufferSpace);
                DBufferTop += dy;
                EnsureDoubleBuffer();
                doubleBuffer.ScrollUp(dy);
                PaintDoubleBuffer();
            }
        }

        //   FluidPaintEventArgs paintEventArgs = new FluidPaintEventArgs();

        /// <summary>
        /// Paints the invalid region of the image buffer.
        /// </summary>
        /// <param name="pe"></param>
        private void PaintDoubleBuffer()
        {
            EnsureDoubleBuffer();
            doubleBuffer.EnsureDBuffer(Width, Height + DBufferSpace);
            doubleBuffer.PaintBuffer(PaintContent, ScaleFactor);
        }

        /// <summary>
        /// A helper variable a timer counts down to detect when to hide the scrollbar.
        /// If the scrollbar should remain visible, this value simply needs to be set to a value higher 255.
        /// When the value is equal or less than 255, the scrollbar will become invisible (or transparent if allowed).
        /// </summary>
        int scrollbarButtonAlpha;

        protected SolidBrush backgroundBrush = new SolidBrush(Color.Empty);

        /// <summary>
        /// Paints the content of this control.
        /// </summary>
        /// <param name="pe">The  paint event args.</param>
        protected virtual void PaintContent(FluidPaintEventArgs pe)
        {
            if (BackColor != Color.Transparent)
            {
                Rectangle bounds = pe.ControlBounds;
                backgroundBrush.Color = BackColor;
                pe.Graphics.FillRectangle(backgroundBrush, bounds);
            }
        }

        /// <summary>
        /// Paints the scrollbar.
        /// </summary>
        /// <param name="e">The PaintEventArgs</param>
        protected void PaintScrollBarButton(Graphics g, Rectangle bounds)
        {
            if (scrollbarButtonAlpha > 0)
            {
                EnsureScrollButtonBitmap(bounds);
                if (alpha >= 255)
                {
                    PaintScrollBarSolid(g, bounds);
                }
                else
                {
                    PaintScrollBarAlphaBlend(g, bounds, alpha);
                }
            }
        }


        private void PaintScrollBarSolid(Graphics g, Rectangle bounds)
        {
            ImageAttributes ia = new ImageAttributes();
            ia.SetColorKey(Color.Fuchsia, Color.Fuchsia);
            g.DrawImage(scrollBarButton, bounds, 0, 0, bounds.Width, bounds.Height, GraphicsUnit.Pixel, ia);
        }

        private void PaintScrollBarAlphaBlend(Graphics g, Rectangle bounds, int opacity)
        {
            Fluid.Drawing.GdiPlus.GdiExt.AlphaBlendImage(g, scrollBarButton, bounds.X, bounds.Y, opacity, true);
        }

        /// <summary>
        /// The button of the scrollbar.
        /// </summary>
        private Image scrollBarButton;

        private void EnsureScrollButtonBitmap(Rectangle bounds)
        {
            if (scrollBarButton == null || scrollBarButton.Height != bounds.Height || scrollBarButton.Height != bounds.Height)
            {
                scrollBarButton = CreateScrollButtonBitmap(bounds.Width, bounds.Height);
            };
        }

        /// <summary>
        /// Creates and paints the button bitmap of the virtual scrollbar.
        /// </summary>
        /// <param name="width">The width of the button.</param>
        /// <param name="height">The height of the button.</param>
        /// <returns>The button image.</returns>
        protected virtual Image CreateScrollButtonBitmap(int width, int height)
        {

            Bitmap bm = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bm))
            {
                int r = Math.Min(width, height / 2) - 1;
                if (r < 1) r = 1;

                backgroundBrush.Color = Color.Fuchsia;
                //g.Clear(Color.Fuchsia);
                g.FillRectangle(backgroundBrush, 0, 0, width, height);

                using (GraphicsPlus gp = new GraphicsPlus(g))
                {
                    Rectangle bounds = new Rectangle(0, 0, width - 1, height - 1);
                    gp.SmoothingMode = SmoothingMode.None;
                    Color color = scrollbarButtonColor;
                    using (SolidBrushPlus brush = new SolidBrushPlus(color, true))
                    {
                        gp.FillRoundRectangle(bounds, r, brush);
                    }
                    using (PenPlus pen = new PenPlus(ScrollBarButtonBorderColor, 1, true))
                    {
                        gp.DrawRoundRectangle(bounds, r, pen);
                    }
                }
            }

            return bm;
        }


        private Rectangle GetBufferedScrollbarButtonBounds()
        {
            Rectangle rect = GetScrollbarButtonBounds();
            rect.Offset(0, ScaledTopOffset - DBufferTop);
            return rect;

        }

        /// <summary>
        /// Gets the bounds for the scrollbar button.
        /// </summary>
        /// <returns>The bounds of the scrollbar button where point(0,0) is the top.</returns>
        protected Rectangle GetScrollbarButtonBounds()
        {
            return GetScrollbarButtonBounds(ScaledTopOffset);
        }

        /// <summary>
        /// Gets the bounds for the scrollbar button.
        /// </summary>
        /// <returns>The bounds of the scrollbar button where point(0,0) is the top.</returns>
        protected virtual Rectangle GetScrollbarButtonBounds(int topOffset)
        {
            int width = (int)(ScaleFactor.Width * 6f);

            Rectangle clientRect = ClientRectangle;
            Rectangle bounds = new Rectangle(clientRect.Width - width, 0, width, Height);

            Rectangle button = bounds;

            float max = ScaledDisplayHeight;
            float scale = ((float)bounds.Height) / max;
            button.Y += (int)(topOffset * scale);
            button.Height = (int)Math.Round(ClientRectangle.Height * scale);
            button.Height = Math.Min(button.Height, bounds.Height);
            button.X++;

            return button;
        }

        private Color scrollbarButtonBorder;

        /// <summary>
        /// Gets or sets the color for the scrollbar border and the scrollbar button.
        /// </summary>
        public Color ScrollBarButtonBorderColor
        {
            get { return scrollbarButtonBorder; }
            set
            {
                if (scrollbarButtonBorder != value)
                {
                    scrollbarButtonBorder = value;
                    UpdateScrollbar();
                }
            }
        }

        private Color scrollbarButtonColor;

        /// <summary>
        /// Gets or sets the color for the scrollbar background.
        /// </summary>
        public Color ScrollBarButtonColor
        {
            get { return scrollbarButtonColor; }
            set
            {
                if (scrollbarButtonColor != value)
                {
                    scrollbarButtonColor = value;
                    UpdateScrollbar();
                }
            }
        }

        /// <summary>
        /// Ensures that the scrollbar image is actual with the current settings.
        /// </summary>
        private void UpdateScrollbar()
        {
            if (scrollBarButton != null)
            {
                scrollBarButton.Dispose();
                scrollBarButton = null;
            }
            if (IsScrollBarVisible) Invalidate(GetBufferedScrollbarButtonBounds());
        }

        /// <summary>
        /// Gets or sets whether to show the virtual scrollbar while scrolling.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowScrollBar { get; protected set; }

        private int smallScrollHeight = 14;
        public int SmallScrollHeigt
        {
            get { return smallScrollHeight; }
            set
            {
                if (value < 1) value = 1;
                smallScrollHeight = value;
            }
        }

        public override void OnKeyUp(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                    ScrollBarVisible = false;
                    break;
            }
            base.OnKeyUp(e);
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            StopAutoScroll();
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        e.Handled = true;
                        TopOffset += (int)(smallScrollHeight * ScaleFactor.Height);
                        ScrollBarVisible = true;
                        break;

                    case Keys.Up:
                        e.Handled = true;
                        TopOffset -= (int)(smallScrollHeight * ScaleFactor.Height);
                        ScrollBarVisible = true;
                        break;

                    default:
                        base.OnKeyDown(e);
                        //  ScrollBarVisible = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to enable animation (e.g. scrollbar fadout animation).
        /// </summary>
        [DefaultValue(true)]
        public bool EnableAnimation { get; set; }

        /// <summary>
        /// used for fading effects of the scrollbar.
        /// </summary>
        private System.Windows.Forms.Timer fadeScrollBarTimer;

        /// <summary>
        /// Fades out the  scroll bar.
        /// </summary>
        private void FadeOutScrollBar()
        {
            if (fadeScrollBarTimer == null)
            {
                fadeScrollBarTimer = new System.Windows.Forms.Timer();
                fadeScrollBarTimer.Interval = 750;
                fadeScrollBarTimer.Tick += new EventHandler(OnFadeScrollBarTick);
            }
            fadeScrollBarTimer.Enabled = true;
        }

        /// <summary>
        /// Decrease the scrollbarAlpha value to trigger when to hide the scrollbar.
        /// </summary>
        void OnFadeScrollBarTick(object sender, EventArgs e)
        {
            scrollbarButtonAlpha--;

            if (scrollbarButtonAlpha <= 255)
            {
                StopFadeTimer();
                scrollbarButtonAlpha = 0;
                BaseInvalidate();
            }
        }

        private void StopFadeTimer()
        {
            if (fadeScrollBarTimer != null)
            {
                fadeScrollBarTimer.Enabled = false;
            }
        }

        protected void StopAutoScroll()
        {
            if (scrollAnimation != null) scrollAnimation.Stop();
        }

        private void AutoScrollUpDown(int dy)
        {
            int ay = Math.Abs(dy);
            int sgn = Math.Sign(dy);
            int pixels = TopOffset + (dy * 2) / 3;
            int target = EnsureTopOffset(pixels);
            int delta = Math.Abs(pixels - target);
            delta = delta * 3 / 2;
            if (delta >= 2000) delta = 1999;
            int duration = 2000 - delta;
            if (target != TopOffset)
            {
                AnimateTopOffset(target, duration, AnimationMode.Accelerated);
            }
        }



        /// <summary>
        /// Depending on the  current value of TopOffset, the value is changed to ensure that the display is in a valid state,
        /// not negative, and not bigger than the maximum available size to display.
        /// </summary>
        private void AutoScrollToDefault()
        {
            int t = TopOffset;
            int min = 0;
            if (t < min) AnimateTopOffset(min, 250, AnimationMode.Log);
            else
            {
                int h = DisplayHeight - (int)(GetMaxHeight() / this.ScaleFactor.Height);
                if (t > h) AnimateTopOffset(h, 250, AnimationMode.Log);
            }
        }

        /// <summary>
        /// Animates TopOFfsewt to the specified position.
        /// </summary>
        /// <param name="pos"></param>
        private void AnimateTopOffset(int pos, int duration, AnimationMode mode)
        {
            if (scrollAnimation == null)
            {
                scrollAnimation = new Animation(250, TopOffset, 0);
                scrollAnimation.Scene += new EventHandler<AnimationEventArgs>(ScrollAnimationScene);
                scrollAnimation.Interval = 40;  // set to 25 pictures per seconds.
            }
            StopAutoScroll();
            scrollAnimation.Mode = mode;
            switch (mode)
            {
                case AnimationMode.Log:
                    scrollAnimation.Acceleration = 0.05f;
                    break;
                case AnimationMode.Accelerated:
                    scrollAnimation.Acceleration = -5f;
                    break;
            }
            scrollAnimation.Duration = duration;
            scrollAnimation.BeginValue = TopOffset;
            scrollAnimation.EndValue = pos;
            scrollAnimation.Start();
        }


        void ScrollAnimationScene(object sender, AnimationEventArgs e)
        {
            ScrollBarVisible = true;
            SetTopOffset(e.Value);
        }

        private Animation scrollAnimation;

        /// <summary>
        /// Gets whether the control performs auto scrolling.
        /// </summary>
        protected bool IsAutoScrolling
        {
            get
            {
                return scrollAnimation != null && !scrollAnimation.IsCompleted;
            }
        }

        /// <summary>
        /// Occurs when a gesture appeared.
        /// </summary>
        public event EventHandler<GestureEventArgs> MouseGesture;

        /// <summary>
        /// Invalids the specified bounds.
        /// </summary>
        public virtual void Invalidate(Rectangle bounds)
        {
            if (Container == null) return;

            int delta = DBufferTop - ScaledTopOffset;
            Rectangle r = new Rectangle(bounds.X, bounds.Y - delta, bounds.Width, bounds.Height);
            if (doubleBuffer != null) doubleBuffer.Invalidate(r);
            bounds.Offset(Left, Top);
            // don't let the child control invalidate anything outside the own bounds:
            ClipRectangle(ref bounds);
            Container.Invalidate(bounds);
        }

        protected void BaseInvalidate(Rectangle bounds)
        {
            if (Container == null) return;

            int delta = DBufferTop - ScaledTopOffset;
            Rectangle r = new Rectangle(bounds.X, bounds.Y - delta, bounds.Width, bounds.Height);
            bounds.Offset(Left, Top);
            // don't let the child control invalidate anything outside the own bounds:
            ClipRectangle(ref bounds);
            Container.Invalidate(bounds);
        }

        protected void BaseInvalidate()
        {
            BaseInvalidate(ClientRectangle);
        }

        public override void Invalidate()
        {
            if (doubleBuffer != null) doubleBuffer.Invalidate();
            base.Invalidate();
        }

        /// <summary>
        /// Ensures that the specified rectangle is completely visible and scrolls the display to a appropriate position if not.
        /// </summary>
        /// <param name="bounds">The rectangle to be visible.</param>
        /// <returns>True if the rectangle was already visible, otherwise false.</returns>
        public bool EnsureVisible(Rectangle bounds)
        {
            int top = GetMinTop();
            int height = GetMaxHeight();
            if (bounds.Top < top)
            {
                int dy = bounds.Top + ScaledTopOffset - top;
                ScaledTopOffset = dy;
                return false;
            }
            else if (bounds.Bottom > height)
            {
                int dy = bounds.Bottom - height + ScaledTopOffset;
                SetTopOffset((int)(dy / ScaleFactor.Height));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the top that is at least visible. This method is used for moving up/down on keyboard events.
        /// </summary>
        /// <returns>0 or any positive value.</returns>
        /// <example>ListBox overrides this to return the HeaderHeight to ensure that the header is always visible.</example>
        protected virtual int GetMinTop()
        {
            return 0;
        }

        private int maxHeightOffset = 0;

        /// <summary>
        /// Gets or sets the offset for the maximum allowed height before scrolling.
        /// </summary>
        public int MaxHeightOffset
        {
            get { return maxHeightOffset; }
            set
            {
                if (value < 0) value = 0;
                if (maxHeightOffset != value)
                {
                    this.maxHeightOffset = value;
                    EnsureDisplayInDBuffer();
                    EnsureValidPosition();
                }
            }
        }

        /// <summary>
        /// Gets the maximum height that ensures to be visible.
        /// Primarily, this method is used to ensure that item in a listbox are also visible when the input box is visible, by modyifing this value.
        /// </summary>
        /// <returns>the maximum height</returns>
        protected virtual int GetMaxHeight()
        {
            return Height - MaxHeightOffset;
        }


        private FluidPaintEventArgs paintEventArgs = new FluidPaintEventArgs();

        /// <summary>
        /// Performs the OnPaint event.
        /// </summary>
        public void PerformPaint(FluidPaintEventArgs e)
        {
            OnPaint(e);
        }

        /// <summary>
        /// Occurs when the size has changed.
        /// </summary>
        protected override void OnSizeChanged(Size oldSize, Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);
            EnsureValidPosition();
        }


        /// <summary>
        /// Stops any animation.
        /// </summary>
        public override void StopAnimations()
        {
            StopAutoScroll();
            StopFadeTimer();
            scrollbarButtonAlpha = 0;
            EnsureValidPosition();
            //            StopFadeTimer();
            //            EnsureSelectedControlRemoved();
        }
    }
}
