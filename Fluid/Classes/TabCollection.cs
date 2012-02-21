using System;

using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Fluid.Controls.Classes
{
    /// <summary>
    /// A collection of PageControls.
    /// </summary>
    public class PageCollection:BindingList<PageControl>
    {
        public PageCollection(NavigationPanel tabPanel)
            : base()
        {
            this.tabPanel = tabPanel;
        }

        private NavigationPanel tabPanel;

        /// <summary>
        /// Occurs when the collection has changed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    tabPanel.Controls.Add(this[e.NewIndex]);
                    break;

                case ListChangedType.ItemDeleted:
                    tabPanel.Controls.RemoveAt(e.OldIndex);
                    break;
            }
        }

        /// <summary>
        /// clears all PageControls.
        /// </summary>
        protected override void ClearItems()
        {
            tabPanel.Controls.Clear();
            base.ClearItems();
        }
    }
}
