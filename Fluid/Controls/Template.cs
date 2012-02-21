using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Fluid.Controls
{
    /// <summary>
    /// A control  that is a container for other controls and supports data binding from an ITemplateHost.
    /// </summary>
    public class FluidTemplate : ControlContainer, ILayoutPanel
    {
        public FluidTemplate()
            : base()
        {
        }

        public FluidTemplate(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            BackColor = Color.Empty;
        }

        /// <summary>
        /// Gets or sets the index, for instance the item index if this is a template for a listbox.
        /// </summary>
        public int ItemIndex { get; set; }

        private object item;

        /// <summary>
        /// Gets or sets the item for the template.
        /// </summary>
        public object Item
        {
            get { return item; }
            set
            {
                if (item != value)
                {
                    item = value;
                    OnBindValue(value);
                }
                OnItemUpdate(value);
            }
        }

        /// <summary>
        /// Occurs when the template needs to be refreshed.
        /// This event occurs whenever the Item property is reset, even if the current property value is the new value to be set.
        /// </summary>
        public event EventHandler UpdateValue;


        /// <summary>
        /// Occurs whenever the Item object is set even if the actual Item property value is the new value to set.
        /// This is different to the OnBindValue event that occurs only when the new Item value is different to the actual value.
        /// </summary>
        /// <param name="value">The new Item value to be refreshed.</param>
        protected virtual void OnItemUpdate(object value)
        {
            if (UpdateValue != null) UpdateValue(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the template must bind data.
        /// </summary>
        protected virtual void OnBindValue(object value)
        {
            if (BindValue != null) BindValue(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the template must be data bound.
        /// </summary>
        public event EventHandler BindValue;

        public override void RaiseCommand(CommandEventArgs e)
        {
            e.Index = ItemIndex;
            if (e.Data == null) e.Data = Item;
            base.RaiseCommand(e);
        }

        /// <summary>
        /// Binds the data for the template. Before accessing data outside a OnBind event, you must call this method, otherwise ItemIndex and Item might
        /// return false data since the template is reused and bound on occasion.
        /// </summary>
        public void Bind()
        {
            ITemplateHost host = Container as ITemplateHost;
            if (host != null) host.Bind(this);
        }

        /// <summary>
        /// Binds this template with a specified item.
        /// </summary>
        /// <param name="itemIndex">The index of the item for which to bind.</param>
        public void Bind(int itemIndex)
        {
            ITemplateHost host = Container as ITemplateHost;
            if (host != null) host.Bind(this, itemIndex);
        }

        /// <summary>
        /// Get the collection of controls for this template.
        /// </summary>
        public FluidControlCollection Controls
        {
            get { return controls; }
        }
    }
}
