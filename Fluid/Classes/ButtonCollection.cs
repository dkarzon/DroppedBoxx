using System;

using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Fluid.Controls
{
    /// <summary>
    /// A collection of FluidButtons.
    /// </summary>
    public class ButtonCollection:BindingList<FluidButton>
    {
        public ButtonCollection(ButtonGroup group)
            : base()
        {
            this.group = group;
        }

        private ButtonGroup group;

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
            if (group!=null) group.Render();
        }

        public void Notify()
        {
            RaiseListChangedEvents = true;
            this.ResetBindings();
        }


        private int width = 32;
        public int Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    if (group != null) group.Buttons.Width = width;
                }
            }
        }
    }
}
