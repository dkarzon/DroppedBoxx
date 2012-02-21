using System;

using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Fluid.Classes;

namespace Fluid.Controls
{
    public partial class FluidListBox
    {
        private bool animateInsertDelete = false;

        /// <summary>
        /// Gets or sets whether an animation is performed if an item is inserted or removed from the datasource if it is a IBindingList.
        /// </summary>
        public bool EnableInsertDeleteAnimation { get { return animateInsertDelete; } set { animateInsertDelete = value; } }

        private void ConnectBindingList(IBindingList list)
        {
            if (list != null)
            {
                list.ListChanged += new ListChangedEventHandler(BindingListChanged);
            }
        }

        void BindingListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    ShowHeader = GetItem(0) is IGroupHeader;
                    AddBindingItem(this.items[e.NewIndex], e.NewIndex);
                    break;

                case ListChangedType.ItemDeleted:
                    //                    ResetBindingList();
                    ShowHeader = GetItem(0) is IGroupHeader;
                    RemoveBindingItem(e.NewIndex);
                    break;

                case ListChangedType.ItemMoved:
                case ListChangedType.Reset:
                    ResetBindingList();
                    break;
            }
        }

        private void ResetBindingList()
        {
            StopAnimations();
            StopAutoScroll();

            ShowHeader = GetItem(0) is IGroupHeader;
            groupHeaderIndex = -1;
            RecalulateItemOffsets();
            TopOffset = 0;


            SelectFirstDataItem();
            Invalidate();
        }

        /// <summary>
        /// Selects the first item that is no IGroupHeader.
        /// </summary>
        public void SelectFirstDataItem()
        {
            if (items == null) return;

            for (int i = 0; i < items.Count; i++)
            {
                object o = items[i];
                if (!(o is IGroupHeader))
                {
                    SelectedItemIndex = i;
                    return;
                }
            }
            SelectedItemIndex = 0;
        }

        private List<Animation> bindingAnimations = new List<Animation>();

        private void AddBindingItem(object item, int index)
        {
            int offset = itemOffsets[index];
            itemOffsets.Insert(index + 1, offset);
            int h = GetItemHeight(item);
            if (EnableInsertDeleteAnimation)
            {
                //ModifyAnimations(index + 1, 1);
                animIndex = index + 1;
                Animation.LogAnimation(0, h, Animation.DefaultDuration, BindingScene);
            }
            else
            {
                OffseItemTops(index + 1, h);
            }
        }

        /// <summary>
        /// Performs an animation when an item was removed from the datasource as IBIndingList.
        /// </summary>
        /// <param name="index"></param>
        private void RemoveBindingItem(int index)
        {
            if (index < items.Count)
            {

                if (EnableInsertDeleteAnimation)
                {
                    // ModifyAnimations(index, -1);
                    int h0 = GetItemHeight(index);
                    itemOffsets.RemoveAt(index);
                    int h1 = index > 0 ? GetItemHeight(index - 1) : h0;
                    int h2 = h1 - h0;
                    animIndex = index;
                    Animation.LogAnimation(h1, h2, 150, BindingScene);
                }
                else
                {
                    int h = GetItemHeight(index);
                    itemOffsets.RemoveAt(index);
                    OffseItemTops(index, -h);
                    Invalidate();
                }
            }
            else
            {
                itemOffsets.RemoveAt(index + 1);
                Invalidate();
            }
        }

        private int animIndex;

        private bool IsItemAnimating(int index)
        {
            foreach (Animation a in bindingAnimations)
            {
                int idx = (int)a.Data;
                if (idx == index) return true;
            }
            return false;
        }

        private void ModifyAnimations(int index, int offset)
        {
            Animation[] array = bindingAnimations.ToArray();
            foreach (Animation a in array)
            {
                int idx = (int)a.Data;

                if (idx > index) a.Data = idx + offset;
                else if (idx == index)
                {
                    if (offset < 0) a.Stop(); else a.Data = idx + offset;
                }
            }
        }

        private void OffseItemTops(int index, int delta)
        {
            if (delta == 0) return;
            for (int i = index; i < itemOffsets.Count; i++)
            {
                itemOffsets[i] += delta;
            }
        }

        private void OffseDeltaItemTops(int index, int h)
        {
            int h0 = index > 0 ? GetItemHeight(index - 1) : itemOffsets[0];
            int delta = h - h0;
            OffseItemTops(index, delta);
        }


        void BindingScene(object sender, AnimationEventArgs e)
        {
            Animation a = (Animation)sender;
            int index = animIndex;
            OffseDeltaItemTops(index, e.Value);

            Invalidate();
            Update();
        }

    }
}
