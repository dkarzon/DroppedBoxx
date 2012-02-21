using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Drawing.GdiPlus;
using System.Drawing;
using Fluid.GdiPlus;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using Fluid.Classes;

namespace Fluid.Controls
{
    /// <summary>
    /// A panel that contains other controls, 
    /// supports transparency and animation.
    /// </summary>
    public class FluidPanel : ControlContainer, ILayoutPanel
    {
        public FluidPanel()
            : base()
        {
        }

        public FluidPanel(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
        }
        public FluidControlCollection Controls { get { return controls; } }

        public override void Dispose()
        {
            base.Dispose();
            ClearDBInfo();
        }

        private void ClearDBInfo()
        {
            if (dbuffer != null) dbuffer.Dispose();
            dbuffer = null;
            if (invalidRegion != null) invalidRegion.Dispose();
            invalidRegion = null;
        }

        private int alpha = 255;

        /// <summary>
        /// Gets or sets the alpha value for the panel.
        /// if DoubleBuffer is set to true, the Alpha value specifies the transparency of the panel between 0 (transparent) and 255 (opaque).
        /// </summary>
        [DefaultValue(255)]
        public int Alpha
        {
            get { return alpha; }
            set
            {
                value = EnsureAlphaBounds(value);
                if (alpha != value)
                {
                    alpha = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets whether the panel is opque.
        /// </summary>
        public bool IsOpaque { get { return alpha == 255; } }

        /// <summary>
        /// Gets wether the panel is transparent.
        /// </summary>
        public bool IsAlpha { get { return alpha < 255; } }

        private Color transparentBgColor;

        private void CreateTransparentBGColor()
        {
            transparentBgColor = ColorConverter.AlphaColor(BackColor, alpha);
        }

        private FluidPaintEventArgs paintEvents;

        public override void OnPaint(FluidPaintEventArgs e)
        {
            if (IsDoubleBuffered) base.OnPaint(e);
            else
            {
                EnsureDBuffer();
                EnsureRegion();
                EnsurePaintEvents();
                if (!invalidRegion.IsEmpty(e.Graphics))
                {
                    using (Graphics g = Graphics.FromImage(dbuffer))
                    {
                        g.Clip = invalidRegion;
                        paintEvents.Graphics = g;
                        paintEvents.ScaleFactor = e.ScaleFactor;
                        paintEvents.ControlBounds = ClientRectangle;
                        paintEvents.Region = invalidRegion;
                        paintEvents.DoubleBuffered = true;
                        base.OnPaint(paintEvents);
                    }
                    invalidRegion.MakeEmpty();
                }
                Rectangle bounds = e.ControlBounds;
                if (IsOpaque)
                {
                    Rectangle srcRect = ClientRectangle;
                    //e.Graphics.DrawImage(dbuffer, bounds.X, bounds.Y);
                    e.Graphics.DrawImage(dbuffer, bounds, srcRect, GraphicsUnit.Pixel);
                }
                else
                {
                    GdiExt.AlphaBlendImage(e.Graphics, dbuffer, bounds, alpha, false);
                    //                    e.Graphics.DrawImage(dbuffer, bounds, 0, 0, Width, Height, GraphicsUnit.Pixel, imageAttributes);
                }
            }
        }

        private void EnsurePaintEvents()
        {
            if (paintEvents == null) paintEvents = new FluidPaintEventArgs();
        }

        /// <summary>
        /// Gets or sets wheter to use gradient fill (true) or solid color fill (false) for the background.
        /// If set to true, the background is painted with a gradient with the the specified <see>GradientFillOffset</see>.
        /// </summary>
        [DefaultValue(false)]
        public bool GradientFill { get; set; }

        private int gradientFillOffset;

        /// <summary>
        /// Gets or sets the fill offset in conjunction with <see>GradientFill</see> set to true.
        /// GradientFillOffset specifies the size of the gradient part until the solid background color part begin.
        /// If set to 0, the offset is the same as the height.
        /// </summary>
        [DefaultValue(0)]
        public int GradientFillOffset
        {
            get { return gradientFillOffset; }
            set
            {
                if (gradientFillOffset != value)
                {
                    gradientFillOffset = value;
                    Invalidate();
                }
            }
        }

        protected override void OnPaintBackground(FluidPaintEventArgs e)
        {
            if (GradientFill)
            {
                this.PaintGradientBackground(e, 50, gradientFillOffset > 0 ? gradientFillOffset : Height);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }


        private int EnsureAlphaBounds(int value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;
            return value;
        }

        private Region invalidRegion;
        private Bitmap dbuffer;

        private void EnsureDBuffer()
        {
            if (dbuffer == null || dbuffer.Width != Width || dbuffer.Height != Height)
            {
                if (dbuffer != null) dbuffer.Dispose();
                int w = Math.Max(1, Width);
                int h = Math.Max(1, Height);
                dbuffer = new Bitmap(w, h);
                EnsureRegion();
                invalidRegion.MakeInfinite();
            }
        }

        private void EnsureRegion()
        {
            if (invalidRegion == null)
            {
                invalidRegion = new Region();
                invalidRegion.MakeInfinite();
            }
        }

        private bool doubleBuffered = false;

        /// <summary>
        /// Gets or sets wether to use a double buffered rendering.
        /// DoubleBuffer enables transparency using the Alpha property and flicker free drawing. 
        /// DoubleBuffer also enables faster rendering, but on the other hand, it requires a back buffer bitmap with the same size of the panel.
        /// </summary>
        [DefaultValue(true)]
        public bool EnableDoubleBuffer
        {
            get
            {
                return doubleBuffered;
            }
            set
            {
                if (doubleBuffered != value)
                {
                    doubleBuffered = value;
                    if (!value)
                    {
                        ClearDBInfo();
                    }
                }
            }
        }


        /// <summary>
        /// Invalidates the specified bounds.
        /// </summary>
        public override void Invalidate(Rectangle bounds)
        {
            if (Initializing) return;
            if (doubleBuffered)
            {
                EnsureDBuffer();
                invalidRegion.Union(bounds);
            }
            base.Invalidate(bounds);
        }

        private Animation topAnimation;
        private Animation alphaAnimation;

        private void StopTopAnimation()
        {
            if (topAnimation != null) topAnimation.Stop();
        }

        private void StopAlphaAnimation()
        {
            if (alphaAnimation != null) alphaAnimation.Stop();
        }

        /// <summary>
        /// Animates the Top of the panel.
        /// </summary>
        /// <param name="targetTop">The top value to target.</param>
        /// <param name="duration">The duration of the animation in milliseconds.</param>
        /// <param name="wait">true, for waiting until the animation has finished, otherwise false.</param>
        public void AnimateTop(int targetTop, int duration, bool wait)
        {
            StopTopAnimation();
            if (targetTop != Top)
            {
                Visible = true;
                Animation animation = EnsureTopAnimation();
                animation.Duration = duration;
                animation.BeginValue = Top;
                animation.EndValue = targetTop;

                if (wait) animation.StartModal(); else animation.Start();
            }
        }

        /// <summary>
        /// Animates the Top of the panel.
        /// </summary>
        /// <param name="targetAlpha">The alpha value to target.</param>
        /// <param name="duration">The duration of the animation in milliseconds.</param>
        /// <param name="wait">true, for waiting until the animation has finished, otherwise false.</param>
        public void AnimateAlpha(int targetAlpha, int duration, bool wait)
        {
            StopAlphaAnimation();
            if (targetAlpha != alpha)
            {
                Animation animation = EnsureAlphaAnimation();
                animation.Duration = duration;
                animation.BeginValue = alpha;
                animation.EndValue = targetAlpha;
                if (wait) animation.StartModal(); else animation.Start();
            }
        }

        private Animation EnsureTopAnimation()
        {
            if (topAnimation == null)
            {
                topAnimation = new Animation();
                topAnimation.Mode = AnimationMode.Log;
                topAnimation.Acceleration = 0.05f;
                topAnimation.Completed += new EventHandler(OnAnimationCompleted);
                topAnimation.Scene += new EventHandler<AnimationEventArgs>(OnAnimationScene);
                topAnimation.Started += new EventHandler(OnAnimationStarted);
            }
            return topAnimation;
        }

        void OnAnimationStarted(object sender, EventArgs e)
        {
            Update();
        }

        private Animation EnsureAlphaAnimation()
        {
            if (alphaAnimation == null)
            {
                alphaAnimation = new Animation();
                alphaAnimation.Mode = AnimationMode.Log;
                alphaAnimation.Acceleration = 0.05f;
                alphaAnimation.Completed += new EventHandler(OnAnimationCompleted);
                alphaAnimation.Scene += new EventHandler<AnimationEventArgs>(AlphaAnimationScene);
                alphaAnimation.Started += new EventHandler(OnAnimationStarted);
            }
            return alphaAnimation;
        }

        void AlphaAnimationScene(object sender, AnimationEventArgs e)
        {
            Alpha = e.Value;
        }

        void OnAnimationCompleted(object sender, EventArgs e)
        {
            OnAnimationCompleted();
        }

        /// <summary>
        /// Occurs when the animation is completed.
        /// </summary>
        protected virtual void OnAnimationCompleted()
        {
            if (AnimationCompleted != null) AnimationCompleted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the animation is completed.
        /// </summary>
        public event EventHandler AnimationCompleted;

        void OnAnimationScene(object sender, AnimationEventArgs e)
        {
            AnchorStyles anchor = Anchor;
            try
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Top;
                Top = e.Value;
            }
            finally
            {
                Anchor = anchor;
            }
        }

        public override void Scale(SizeF scaleFactor)
        {
            base.Scale(scaleFactor);
            if (scaleFactor.Height != ScaleFactor.Height)
            {
                gradientFillOffset = (int)(gradientFillOffset * scaleFactor.Height / ScaleFactor.Height);
            }
        }

        public override bool IsDoubleBuffered { get { return isDoubleBuffered(); } }

        public IEnumerable<FluidControl> VisibleControls
        {
            get
            {
                foreach (FluidControl c in controls)
                {
                    if (c.Visible) yield return c;
                }
            }
        }

        private bool isDoubleBuffered()
        {
            if (!doubleBuffered) return true;

            FluidControl first = null;
            int cnt = 0;
            foreach (FluidControl c in VisibleControls)
            {
                if (cnt++ > 1) break;
                first = c;
            }
            if (cnt == 1 && first != null) return first.IsDoubleBuffered;
            return false;
        }

        /// <summary>
        /// Shows the Panel.
        /// </summary>
        public virtual void Show()
        {
            FluidHost.Instance.Show(this);
            OnShowing();
        }

        /// <summary>
        /// Hides the Panel.
        /// </summary>
        public virtual void Hide()
        {
            FluidHost.Instance.Hide(this);
            OnClosing();
        }

        public virtual void ShowMaximized()
        {
            FluidHost.Instance.ShowMaximized(this);
        }


        /// <summary>
        /// Shows the panel maximized in the host.
        /// </summary>
        /// <param name="transition">The transition to perform while the panel is shown.</param>
        public virtual void ShowMaximized(ShowTransition transition)
        {
            Visible = false;
            FluidHost host = FluidHost.Instance;
            host.Add(this);
            Bounds = host.ClientBounds;
            Show(transition);
        }

        private ShowTransition showTransition = ShowTransition.None;

        /// <summary>
        /// Opens the panel with the specified transition.
        /// </summary>
        /// <param name="transition">The transition to perform while the panel is shown.</param>
        public virtual void Show(ShowTransition transition)
        {
            previousFocusedControl = Host != null ? Host.FocusedControl : null;
            showTransition = transition;
            FluidTextBox.InputPanel.Enabled = false;
            Visible = false;
            FluidHost host = FluidHost.Instance;
            Rectangle client = host.ClientBounds;
            int h = client.Height;
            int w = client.Width;

            host.Add(this);
            bool modal = false;

            Bounds = GetShowStartBounds(transition);
            Visible = true;

            switch (transition)
            {
                case ShowTransition.None:
                    Show();
                    break;

                case ShowTransition.FromBottom:
                    AnimateTop(h - Bounds.Height, Animation.DefaultDuration, modal);
                    break;

                case ShowTransition.FromTop:
                    AnimateTop(Bounds.Height, Animation.DefaultDuration, modal);
                    break;

                default:
                    throw new NotImplementedException();
            }
            Focus();
            OnShowing();
        }

        /// <summary>
        /// Opens the panel modal with the specified transition.
        /// </summary>
        /// <param name="transition">The transition to perform.</param>
        public void ShowModal(ShowTransition transition)
        {
            modalBackground = new ModalBackground();
            modalBackground.ShowMaximized();
            Show(transition);
            Anchor = AnchorAll;

        }

        private ModalBackground modalBackground;

        /// <summary>
        /// Closes the panel using the transition specified with Show, otherwhise without transition.
        /// </summary>
        public  void Close()
        {
            Close(showTransition);
        }

        /// <summary>
        /// The control that was focused before this panel opened.
        /// </summary>
        private FluidControl previousFocusedControl;

        /// <summary>
        /// Closes the panel with the specified transition.
        /// </summary>
        /// <param name="transition">The transition to perform.</param>
        public virtual void Close(ShowTransition transition)
        {
            if (!Visible) return;

            RestorePreviousFocusedControl();

            EnsureTopAnimation();

            FluidHost host = FluidHost.Instance;
            Rectangle client = host.ClientBounds;
            int h = client.Height;
            int w = client.Width;

            bool modal = true;

            switch (transition)
            {
                case ShowTransition.None:
                    Hide();
                    HideModalBackground();
                    break;

                case ShowTransition.FromBottom:
                    topAnimation.Completed += new EventHandler(OnTopAnimationCompleted);
                    AnimateTop(h, Animation.DefaultDuration, modal);
                    break;

                case ShowTransition.FromTop:
                    topAnimation.Completed += new EventHandler(OnTopAnimationCompleted);
                    AnimateTop(0 - Bounds.Height, Animation.DefaultDuration, modal);
                    break;

                default:
                    throw new NotImplementedException();
            }
            OnClosing();
        }

        /// <summary>
        /// Give back the focus to the control that had the focus before this panel appeared and grabbed the focus,
        /// but only if the current focused control belong to a control nested inside this panel.
        /// </summary>
        private void RestorePreviousFocusedControl()
        {
            if (LeaveNestedFocusedControl())
            {
                // check if the previous control is still focusable and alive:
                if (previousFocusedControl != null && (!previousFocusedControl.Enabled || !previousFocusedControl.IsAvailable)) previousFocusedControl = null;
                if (Host != null) Host.FocusedControl = previousFocusedControl;
            }
            previousFocusedControl = null;
        }

        private void HideModalBackground()
        {
            if (modalBackground != null)
            {
                modalBackground.Hide();
                modalBackground = null;
            }
        }

        void OnTopAnimationCompleted(object sender, EventArgs e)
        {
            topAnimation.Completed -= OnTopAnimationCompleted;
            Hide();
            HideModalBackground();
        }

        private Rectangle GetShowStartBounds(ShowTransition transition)
        {
            FluidHost host = FluidHost.Instance;
            Rectangle client = host.ClientBounds;
            int h = client.Height;
            int w = client.Width;

            switch (transition)
            {
                case ShowTransition.FromBottom:
                    return new Rectangle(0, h, w, Height);

                case ShowTransition.FromTop:
                    return new Rectangle(0, -Height, w, Height);

                case ShowTransition.FromLeft:
                    return new Rectangle(-Width, 0, Width, h);

                case ShowTransition.FromRight:
                    return new Rectangle(w, 0, Width, h);

                case ShowTransition.Zoom:
                    throw new NotImplementedException();


                default:
                    return Bounds;
            }
        }


        /// <summary>
        /// Gets wether the panel is transparent.
        /// </summary>
        public override bool IsTransparent
        {
            get
            {
                return Alpha < 255 || base.IsTransparent;
            }
        }


        /// <summary>
        /// Occurs when the panel is about to be closed.
        /// </summary>
        protected virtual void OnClosing()
        {
            if (Closing != null) Closing(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the panel is showing  and has received focus but maybe still in animation.
        /// </summary>
        protected virtual void OnShowing()
        {
            if (Showing != null) Showing(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the panel is showing  and has received focus but maybe still in animation.
        /// </summary>
        public event EventHandler Showing;

        /// <summary>
        /// Occurs when the panel is closing.
        /// </summary>
        public event EventHandler Closing;

    }
}
