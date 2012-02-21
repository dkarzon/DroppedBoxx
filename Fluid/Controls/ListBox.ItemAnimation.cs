using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Classes;
using Fluid.Controls.Interfaces;
using System.Drawing;

namespace Fluid.Controls
{
    public partial class FluidListBox
    {
        #region Insert/Remove Animations

        /// <summary>
        /// Removes an item visually animated from the list.
        /// </summary>
        /// <param name="index"></param>
        [Obsolete]
        private void RemoveItem(int index)
        {
            if (index == EnsureIndexBounds(index))
            {
                bool selected = SelectedItemIndex == index;
                if (selected)
                {
                    SelectedItemIndex = -1;
                }

                ListBoxAnimation.AnimateModal(index, GetItemHeight(index), 0, Animation.DefaultDuration, changeItemHeightScene);
                if (selected) SelectedItemIndex = index;

                DeleteItem(index);
            }
        }

        /// <summary>
        /// Inserts an item visually to the list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Obsolete]
        private void InsertItem(int index, object item)
        {
            ListBoxAnimation.AnimateModal(index, 0, GetItemHeight(index), Animation.DefaultDuration, changeItemHeightScene);
        }



        /// <summary>
        /// Gets the height of an item in the list by index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The height of this item.</returns>
        public int GetItemHeight(int index)
        {
            int h = itemOffsets[index + 1] - itemOffsets[index];
            return h;
        }

        /// <summary>
        /// Gets the height of an item that is currently not in the list.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The height of the item.</returns>
        public int GetItemHeight(object item)
        {
            IItemHeight itemHeight = item as IItemHeight;
            if (itemHeight != null) return itemHeight.Height;
            return item is IGroupHeader ? headerHeight : ScaledItemHeight;
        }



        private void DeleteItem(int index)
        {
            int h = GetItemHeight(index);
            lock (itemOffsets)
            {
                items.RemoveAt(index);
                itemOffsets.RemoveAt(index);
                for (int i = index; i < itemOffsets.Count; i++)
                {
                    itemOffsets[i] -= h;
                }
            }
            Invalidate();

        }


        void changeItemHeightScene(object sender, AnimationEventArgs e)
        {
            int index = ((ListBoxAnimation)sender).Index;
            lock (itemOffsets)
            {

                int h0 = itemOffsets[index + 1] - itemOffsets[index];
                int h = e.Value;
                int delta = h - h0;
                for (int i = index + 1; i < itemOffsets.Count; i++)
                {
                    itemOffsets[i] += delta;
                }
            }
            Rectangle bounds = GetItemBounds(index);
            bounds.Height = this.Height - bounds.Top;
            Invalidate(bounds);
        }

        #endregion
    }
}
