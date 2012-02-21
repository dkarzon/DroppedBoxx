using System;

using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Fluid.Controls;
using System.Diagnostics;

namespace Fluid.Classes
{
    public class AnimationEventArgs : EventArgs
    {
        public AnimationEventArgs(Animation storyboard)
            : base()
        {
            this.storyboard = storyboard;
        }

        public int BeginValue { get { return storyboard.BeginValue; } }
        public int EndValue { get { return storyboard.EndValue; } }
        public int Value { get { return storyboard.Value; } }
        public int Duration { get { return storyboard.Duration; } }

        private Animation storyboard;

        /// <summary>
        /// Stops the storyboard.
        /// </summary>
        public void Stop()
        {
            storyboard.Stop();
        }
    }

    public enum AnimationMode
    {
        Linear,
        Accelerated,
        Log
    }

    /// <summary>
    /// Enables  processor speed idendent animation.
    /// </summary>
    public partial class Animation : IDisposable
    {
        /// <summary>
        /// Gets or sets the default duration for animations if not specified otherwise.
        /// </summary>
        public static int DefaultDuration = 250;

        public Animation()
            : base()
        {
            Init();
        }

        private void Init()
        {
            this.Interval = 40;
            Duration = DefaultDuration;
            timer.Tick += new EventHandler(timer_Tick);
            eventArgs = new AnimationEventArgs(this);
        }

        public Animation(int duration, int beginValue, int endValue)
            : base()
        {
            this.Duration = duration;
            this.BeginValue = beginValue;
            this.EndValue = endValue;
            Init();
        }

        internal AnimationEventArgs eventArgs;

        void timer_Tick(object sender, EventArgs e)
        {
            if (!timer.Enabled) return;
            OnScene();
        }

        /// <summary>
        /// Specifies the acceleration. Set 0 for no acceleration, and negative values for decelleration.
        /// </summary>
        public float Acceleration { get; set; }

        private AnimationMode mode = AnimationMode.Accelerated;

        public AnimationMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// The duration in milliseconds.
        /// </summary>
        public int Duration { get; set; }

        public int Interval { get { return timer.Interval; } set { timer.Interval = value; } }

        /// <summary>
        /// Gets the beginning value.
        /// </summary>
        public int BeginValue { get; set; }

        /// <summary>
        /// Gets the end value.
        /// </summary>
        public int EndValue { get; set; }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public int Value { get; set; }

        private int startTick;
        private int range;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public object Data { get; set; }


        float max;

        private bool isCompleted = true;

        public bool IsCompleted
        {
            get { return isCompleted; }
        }

        public void Start(int interval)
        {
            isCompleted = false;
            range = EndValue - BeginValue;
            Value = BeginValue;
            max = 1f;
            if (Acceleration > 0)
            {
                max = 0.5f * Math.Abs((float)Acceleration) * Duration * Duration + range * Duration;
            }
            PerformSzene();
            OnStarted();
            //if (reset != null) reset.Reset();
            this.Interval = interval;
            startTick = Environment.TickCount;
            timer.Enabled = true;
        }

        public void StartModal()
        {
            isCompleted = false;
            range = EndValue - BeginValue;
            Value = BeginValue;
            max = 1f;
            if (Acceleration > 0)
            {
                max = 0.5f * Math.Abs((float)Acceleration) * Duration * Duration + range * Duration;
            }
            PerformSzene();
            OnStarted();
            //if (reset != null) reset.Reset();
            startTick = Environment.TickCount;
            Wait();
        }

        /// <summary>
        /// Waits until the current animation is ready.
        /// </summary>
        public void Wait()
        {
            timer.Enabled = false;
            while (!IsCompleted)
            {
                FluidHost.Instance.Update();
                OnScene();
                if (IsCompleted) break;
                Thread.Sleep(Interval / 2);
            }
            isCompleted = true;
        }

        /// <summary>
        /// Occurs when the animation has started and completed it's first frame.
        /// </summary>
        protected virtual void OnStarted()
        {
            if (Started != null) Started(this, EventArgs.Empty);
        }



        /// <summary>
        /// Occurs when the animation has started and completed it's first frame.
        /// </summary>
        public event EventHandler Started;

        public void Start()
        {
            Start(40);
        }

        public void Finish()
        {
            bool complete = isCompleted;
            InternalStop();
            Value = EndValue;
            PerformSzene();
            if (!complete) OnCompleted();
        }

        protected int GetValue(int time)
        {
            int d = Duration;
            if (time > d) time = d;
            if (Mode == AnimationMode.Log) return GetLogValue(time);
            switch (Mode)
            {
                case AnimationMode.Log:
                    return GetLogValue(time);

                case AnimationMode.Linear:
                    return GetLinearValue(time);

                case AnimationMode.Accelerated:
                    if (Acceleration > 0) return GetAccelValue(time);
                    if (Acceleration < 0) return GetDecelValue(time);
                    break;
            }
            return GetLinearValue(time);
        }

        private int GetLinearValue(int time)
        {
            int range = EndValue - BeginValue;
            int x = time * range / Duration;
            int value = x + BeginValue;
            if (value >= EndValue) value = EndValue;
            return value;
        }

        private int GetAccelValue(int t)
        {
            if (t > Duration) t = Duration;
            int range = EndValue - BeginValue;
            float at = t;
            float s = 0.5f * Acceleration * at * at + range * at;
            int v = (int)(s * range / max) + BeginValue;
            return v;
        }

        private int GetDecelValue(int t)
        {
            if (t > Duration) t = Duration;
            float a = -Acceleration;
            float t2 = Duration - t;

            int range = EndValue - BeginValue;
            float max = a * Duration * Duration;
            float s = -(a * t2 * t2 * range) / max + range;
            int v = (int)(s + BeginValue);
            return v;
        }

        private int GetLogValue(int t)
        {
            int d = EndValue - BeginValue;
            double a = Acceleration / 2f;
            double max = Math.Log(Duration * a + 1d);
            double s = Math.Log(t * a + 1d) * d / max + BeginValue;
            return (int)s;
        }

        private void OnScene()
        {
            if (isCompleted) return;
            int tick = Environment.TickCount;
            int delta = tick - startTick;
            Value = GetValue(delta);
            if (delta >= Duration && !isCompleted) Finish();
            else PerformSzene();
        }

        void PerformSzene()
        {
            if (Scene != null) Scene(this, eventArgs);
        }

        /// <summary>
        /// Occurs on a new scene.
        /// </summary>
        public event EventHandler<AnimationEventArgs> Scene;

        public void Stop()
        {
            bool enabled = timer.Enabled;
            InternalStop();
            if (enabled) OnCompleted();
        }
        private void InternalStop()
        {
            bool completed = isCompleted;
            isCompleted = true;
            timer.Enabled = false;
            //if (reset != null) reset.Set();
        }

        /// <summary>
        /// Occurs when the animation has completed (or stopped).
        /// </summary>
        protected virtual void OnCompleted()
        {
            if (Completed != null) Completed(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the animation has completed (or stopped).
        /// </summary>
        public event EventHandler Completed;

        //private ManualResetEvent reset; // = new ManualResetEvent(false);


        #region IDisposable Members

        public void Dispose()
        {
            timer.Enabled = false;
            timer.Dispose();
        }

        #endregion

    }

    public delegate void AnimationFunc(object sender, AnimationEventArgs e);

    public delegate void CompletedFunc(object sender, EventArgs e);


}
