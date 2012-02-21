using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Classes
{
    public partial class Animation
    {
        public static void LogAnimation(int beginValue, int endValue, int duration, AnimationFunc scene)
        {
            using (Animation a = new Animation())
            {
                a.Mode = AnimationMode.Log;
                a.Acceleration = 0.05f;
                a.BeginValue = beginValue;
                a.EndValue = endValue;
                a.Duration = duration;
                a.Scene += new EventHandler<AnimationEventArgs>(scene);
                a.StartModal();
            }
        }

        public static Animation AsyncLogAnimation(int beginValue, int endValue, int duration, AnimationFunc scene, CompletedFunc completed, object data)
        {
            Animation a = new Animation();
            a.Mode = AnimationMode.Log;
            a.Acceleration = 0.05f;
            a.BeginValue = beginValue;
            a.EndValue = endValue;
            a.Duration = duration;
            a.Data = data;
            if (completed != null) a.Completed += new EventHandler(completed);
            a.Scene += new EventHandler<AnimationEventArgs>(scene);
            a.Start();
            return a;

        }

        public static void AccelerationAnimation(int beginValue, int endValue, int duration, AnimationFunc scene)
        {
            using (Animation a = new Animation())
            {
                a.Mode = AnimationMode.Accelerated;
                a.Acceleration = 4f;
                a.BeginValue = beginValue;
                a.EndValue = endValue;
                a.Duration = duration;
                a.Scene += new EventHandler<AnimationEventArgs>(scene);
                a.StartModal();
            }
        }

        public static void DecelerationAnimation(int beginValue, int endValue, int duration, AnimationFunc scene)
        {
            using (Animation a = new Animation())
            {
                a.Mode = AnimationMode.Accelerated;
                a.Acceleration = -4f;
                a.BeginValue = beginValue;
                a.EndValue = endValue;
                a.Duration = duration;
                a.Scene += new EventHandler<AnimationEventArgs>(scene);
                a.StartModal();
            }
        }

    }
}
