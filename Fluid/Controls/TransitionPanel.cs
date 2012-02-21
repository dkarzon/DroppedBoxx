using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using Fluid.Classes;

namespace Fluid.Controls
{
    /// <summary>
    /// Shows only one control sized with it's own size and supports animated navigation from one
    /// control to another. This control is part of the <see>NavigationPanel</see>.
    /// </summary>
    /// <example>
    /// This control usually contains ListBoxes and/or panels to enable an animted switching to another control.
    /// </example>
    public class TransitionPanel : ControlContainer, INotifyControlAdded
    {
        public FluidControlCollection Controls { get { return controls; } }

        /// <summary>
        /// the control that is currently transitioning.
        /// </summary>
        private FluidControl transitionControl;

        protected override void InitControl()
        {
            base.InitControl();
            BackColor = Color.Empty;
        }

        public override void Dispose()
        {
            if (dbuffer != null) dbuffer.Dispose();
            base.Dispose();
        }

        #region INotifyControlAdded Members

        public void ControlAdded(FluidControl control)
        {
            control.Visible = SelectedControl == control;
            control.Bounds = this.ClientRectangle;
        }

        #endregion

        private int selectedIndex = -1;
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                value = EnsureIndexRange(value);
                if (selectedIndex != value)
                {
                    OnSelectedIndexChanging(selectedIndex, value);
                    FluidControl old = SelectedControl;
                    selectedIndex = value;
                    FluidControl c = SelectedControl;
                    BeginInit();
                    if (selectedIndex >= 0) SelectedControl.Bounds = ClientRectangle;
                    if (c != null)
                    {
                        c.Bounds = ClientRectangle;
                        c.Visible = true;
                    }

                    if (old != null)
                    {
                        old.Visible = false;
                    }
                    EndInit();
                    Invalidate();

                }
            }
        }


        private ChangedEventArgs<int> indexChangeEventArgs = new ChangedEventArgs<int>();

        private void OnSelectedIndexChanging(int currentIndex, int newIndex)
        {
            if (SelectedIndexChanging != null)
            {
                indexChangeEventArgs.OldValue = currentIndex;
                indexChangeEventArgs.NewValue = newIndex;
                SelectedIndexChanging(this, indexChangeEventArgs);
            }
        }

        public event EventHandler<ChangedEventArgs<int>> SelectedIndexChanging;

        private DoubleBuffer dbuffer;

        public override void Invalidate(Rectangle bounds)
        {
            if (dbuffer != null) dbuffer.Invalidate(bounds);
            base.Invalidate(bounds);
        }


        /// <summary>
        /// Gets the selected control otherwhise null.
        /// </summary>
        public FluidControl SelectedControl
        {
            get
            {
                return selectedIndex >= 0 ? controls[selectedIndex] : null;
            }
            set
            {
                if (value != null)
                {
                    int index = controls.IndexOf(value);
                    SelectedIndex = index;
                }
                else SelectedIndex = -1;
            }
        }

        private int EnsureIndexRange(int value)
        {
            if (value >= controls.Count) return controls.Count - 1;
            if (value < -1) value = -1;
            return value;
        }

        protected override void OnSizeChanged(System.Drawing.Size oldSize, System.Drawing.Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);
            if (transitionControl != null) transitionControl.Bounds = ClientRectangle;
            if (selectedIndex >= 0) SelectedControl.Bounds = ClientRectangle;
        }

        protected override void OnPaintBackground(FluidPaintEventArgs e)
        {
            // dont use background painting
        }

        //public override void OnPaint(FluidPaintEventArgs e)
        //{
        //    if (!IsInTransiton)
        //    {
        //        FluidControl c = SelectedControl;
        //        if (c != null) c.OnPaint(e);
        //    }
        //    else base.OnPaint(e);
        //}

        private Animation animation;

        Animation EnsureAnimation()
        {
            if (animation == null)
            {
                animation = new Animation();
                animation.Scene += new EventHandler<AnimationEventArgs>(animation_Scene);
                animation.Started += new EventHandler(animation_Started);
                //animation.Mode = AnimationMode.Log;
                //animation.Acceleration = 0.05f;
                animation.Mode = AnimationMode.Accelerated;
                animation.Acceleration = -3.3f;
                animation.Completed += new EventHandler(animation_Completed);
            }
            return animation;
        }

        /// <summary>
        /// Update after the animation has completed the first scene, to ensure no delay while rendering and caching the new tab 
        /// for the very first time:
        /// </summary>
        void animation_Started(object sender, EventArgs e)
        {
            Update();
        }

        void animation_Completed(object sender, EventArgs e)
        {
            transitionControl.Bounds = ClientRectangle;
            SelectedControl = transitionControl;
            transitionControl = null;
            currentTransition = Transition.None;
            dBufferRequired = false;
            OnTransitionCompleted(sender as Animation);
        }

        private int alpha;

        #region Transitions
        private TranistionFunc transitionFunc;

        delegate void TranistionFunc(int value);

        void Fade(int value)
        {
            alpha = 2 * 255 * value / animation.EndValue;
            Rectangle r1 = ClientRectangle;
            SelectedControl.Bounds = r1;
            transitionControl.Bounds = r1;
            Invalidate();
        }

        void MoveLeft(int value)
        {
            BeginInit();
            Rectangle r1 = ClientRectangle;
            Rectangle r2 = ClientRectangle;

            r2.X = r1.Right - value;
            SelectedControl.Bounds = r1;
            transitionControl.Bounds = r2;
            transitionControl.Visible = true;
            EndInit();
            Invalidate();
        }

        void MoveRight(int value)
        {
            BeginInit();
            Rectangle r1 = ClientRectangle;
            Rectangle r2 = ClientRectangle;

            r2.X = value - r2.Width;
            SelectedControl.Bounds = r1;
            transitionControl.Bounds = r2;
            transitionControl.Visible = true;
            EndInit();
            Invalidate();
        }


        DoubleBuffer ensureDBuffer()
        {
            if (dbuffer == null) dbuffer = new DoubleBuffer();
            return dbuffer;
        }


        /// <summary>
        /// TabPanel uses this flag to switch of double buffering, since TabPanel implements it's own buffering.
        /// </summary>
        internal bool InternalEnableDoubleBuffer = true;

        public override void OnPaint(FluidPaintEventArgs e)
        {
            if (InternalEnableDoubleBuffer && dBufferRequired)
            {
                DoubleBuffer buffer = ensureDBuffer();
                buffer.Paint(e, this, base.OnPaint, alpha);
            }
            else base.OnPaint(e);
        }

        protected override void PaintControls(FluidPaintEventArgs pe)
        {
            FluidControl c = SelectedControl;
            FluidControl tc = transitionControl;

            if (tc == null && (c is PageControl))
            {
                /// build a short cut for performance issues:
                c = ((PageControl)c).Control;
            }
            if (c != null) PaintControl(pe, c);

            if (tc != null) PaintControl(pe, tc);
        }

        protected override Rectangle PaintControl(FluidPaintEventArgs pe, FluidControl c)
        {
            Rectangle controlBounds = pe.ControlBounds;
            Region clip = pe.Region;
            Rectangle bounds = c.Bounds;
            bounds.Offset(controlBounds.X, controlBounds.Y);
            if (clip.IsVisible(bounds))
            {
                PaintControlUnbuffered(pe, c, bounds);
            }
            return bounds;
        }

        void ShiftLeft(int value)
        {
            BeginInit();
            Rectangle r1 = ClientRectangle;
            Rectangle r2 = ClientRectangle;

            r1.X -= value;
            r2.X = r1.Right;
            SelectedControl.Bounds = r1;
            if (transitionControl != null)
            {
                transitionControl.Bounds = r2;
                transitionControl.Visible = true;
            }
            EndInit();
            Invalidate();
        }

        void ShiftRight(int value)
        {
            BeginInit();
            Rectangle r1 = ClientRectangle;
            Rectangle r2 = ClientRectangle;

            r1.X += value;
            r2.X = r1.Left - r2.Width;
            SelectedControl.Bounds = r1;
            transitionControl.Bounds = r2;
            transitionControl.Visible = true;
            EndInit();
            Invalidate();
        }

        void None(int value) { }

        TranistionFunc GetTransitionFunc(Transition transition)
        {
            switch (transition)
            {
                case Transition.ShiftLeft: return ShiftLeft;
                case Transition.ShiftRight: return ShiftRight;
                case Transition.MoveLeft: return MoveLeft;
                case Transition.MoveRight: return MoveRight;
                case Transition.Fade: return Fade;

                default: throw new NotSupportedException(transition.ToString());
            }
        }

        public bool RequiresDBuffer(Transition transaction)
        {
            switch (currentTransition)
            {

                case Transition.ShiftRight:
                case Transition.ShiftLeft:
                case Transition.ShiftTop:
                case Transition.ShiftBottom:
                case Transition.None:
                    return false;
                default:
                    return true;
            }
        }

        bool dBufferRequired = false;

        public bool IsInTransiton { get { return transitionControl != null; } }

        private Transition currentTransition = Transition.None;


        void animation_Scene(object sender, AnimationEventArgs e)
        {
            int value = Width * e.Value / e.EndValue;
            transitionFunc(value);
            OnTransitionChange(e);
        }

        protected virtual void OnTransitionChange(AnimationEventArgs e)
        {
            if (TransitionChange != null) TransitionChange(this, e);
        }

        protected virtual void OnTransitionCompleted(Animation a)
        {
            if (TransitionCompleted != null)
            {
                TransitionCompleted(this, a.eventArgs);
            }
        }

        public event EventHandler<AnimationEventArgs> TransitionChange;
        public event EventHandler<AnimationEventArgs> TransitionCompleted;

        public void Switch(int index, Transition transition, int duration)
        {
            StopTransition();
            if (index != selectedIndex)
            {
                alpha = 255;
                currentTransition = transition;
                dBufferRequired = RequiresDBuffer(transition);
                transitionFunc = GetTransitionFunc(transition);
                transitionControl = Controls[index];
                Animation animation = EnsureAnimation();
             
                animation.BeginValue = 0;
                animation.EndValue = Width;
                animation.Duration = duration;
                OnBeginTransition(index);
                animation.Start();
            }
        }

        private ChangedEventArgs<int> beginTransitionEventArgs = new ChangedEventArgs<int>();

        private void OnBeginTransition(int index)
        {
            if (BeginTransition != null)
            {
                beginTransitionEventArgs.NewValue = index;
                BeginTransition(this, beginTransitionEventArgs);
            }

        }

        public event EventHandler<ChangedEventArgs<int>> BeginTransition;

        private void StopTransition()
        {
            Animation animation = EnsureAnimation();
            animation.Wait();
            //animation.Stop();
            currentTransition = Transition.None;
            dBufferRequired = false;
        }

        #endregion

        public override bool IsDoubleBuffered
        {
            get { return IsInTransiton ? false : SelectedControl != null ? SelectedControl.IsDoubleBuffered : false; }
        }


    }
}
