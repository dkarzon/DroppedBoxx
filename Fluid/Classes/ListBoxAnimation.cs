using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Classes
{
    /// <summary>
    /// A specialized Animation class used for ListBox animation.
    /// </summary>
    internal class ListBoxAnimation:Animation
    {
        /// <summary>
        /// Gets or sets the Index of the ListBoxItem to animate.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the ListBoxItem to animate.
        /// </summary>
        public object Item { get; set; }

        /// <summary>
        /// Performs a modal animation of a listbox item.
        /// </summary>
        /// <param name="index">The index of the listbox item.</param>
        /// <param name="beginValue">The intial animation value.</param>
        /// <param name="endValue">The final animation value.</param>
        /// <param name="duration">The duration of the animation.</param>
        /// <param name="func">The AnimationFunc.</param>
        public static void AnimateModal(int index, int beginValue, int endValue, int duration, AnimationFunc func)
        {
            using (ListBoxAnimation a = new ListBoxAnimation())
            {
                a.Index = index;
                a.BeginValue = beginValue;
                a.EndValue = endValue;
                a.Duration = duration;
                a.Scene +=new EventHandler<AnimationEventArgs>(func);
                a.Mode = AnimationMode.Log;
                a.Acceleration = 0.05f;
                a.StartModal();
            }
        }
    }
}
