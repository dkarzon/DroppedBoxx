using System;

using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Fluid.Controls
{
    /// <summary>
    /// A collection of FluidTemplates.
    /// </summary>
    public class TemplateCollection : BindingList<FluidTemplate>
    {
        public TemplateCollection(FluidListBox owner)
            : base()
        {
            this.owner = owner;
        }

        private FluidListBox owner;

        /// <summary>
        /// Occurs when the collection has changed.
        /// </summary>
        /// <param name="e">Specifies what has changed.</param>
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    FluidTemplate t = this[e.NewIndex];
                    t.BeginInit();
                    t.Container = owner;
                    t.Scale(owner.ScaleFactor);
                    t.Bounds = owner.GetItemBounds(0);
                    t.EndInit();
                    break;

            }
        }

        /// <summary>
        /// Removes a template from the collection.
        /// </summary>
        /// <param name="index">The index of the template to remove.</param>
        protected override void RemoveItem(int index)
        {
            this[index].Container = null;
            base.RemoveItem(index);
        }
    }
}
