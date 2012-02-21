using System;

using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using Fluid.Controls.Classes;
using Fluid.Controls.Interfaces;
using Fluid.Classes;
using Microsoft.WindowsCE.Forms;

namespace Fluid.Controls
{

    /// <summary>
    /// A listbox control.
    /// </summary>
    public partial class FluidListBox : ScrollContainer, ICommandContainer, IMultiControlContainer, ITemplateHost, ILayoutPanel
    {
        public FluidListBox()
            : base()
        {
        }

        public FluidListBox(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
        }

        protected override void InitControl()
        {
            templates = new TemplateCollection(this);
            base.InitControl();
            EnableDoubleBuffer = true;
            SelectedBackColor = SystemColors.Highlight;
            SelectedForeColor = SystemColors.HighlightText;
            HoveredBackColor = Color.FromArgb(242, 242, 243);
            HoveredForeColor = Color.Black;
            PressedBackColor = Color.Gray;
            PressedForeColor = Color.White;
            BorderColor = Color.Silver;
            templateEventArgs = new TemplateEventArgs(this);
            FluidTextBox.InputPanel.EnabledChanged += new EventHandler(InputPanel_EnabledChanged);
        }


        /// <summary>
        /// when the InputPanel appears, make sure that the selected item is visible by changing
        /// the MaxValue for the display height:
        /// </summary>
        void InputPanel_EnabledChanged(object sender, EventArgs e)
        {
            if (Host == null) return;
            InputPanel inputPanel = FluidTextBox.InputPanel;
            bool enabled = inputPanel.Enabled;

            Rectangle bounds = inputPanel.Bounds;
            Rectangle hostBounds = this.ScreenBounds;
            int height = hostBounds.Height;

            if (enabled)
            {
                if (hostBounds.Bottom > bounds.Top)
                {
                    height = bounds.Top - hostBounds.Top;
                }
            }
            maxHeight = Math.Max(0, height);
            if (enabled)
            {
                bool isItemVisible = IsItemVisible(SelectedItemIndex);
                if (isItemVisible) EnsureVisible(SelectedItemIndex);
            }
            else EnsureValidPosition();
        }

        /// <summary>
        /// Gets wether the item with the specified index is inside the visible display area.
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <returns></returns>
        public bool IsItemVisible(int itemIndex)
        {
            Rectangle itemBounds = GetItemBounds(itemIndex);
            return itemBounds.IntersectsWith(ClientRectangle);
        }

        int maxHeight = int.MaxValue;

        protected override int GetMaxHeight()
        {
            int max = base.GetMaxHeight();
            return Math.Min(maxHeight, max);
        }


        public override void Dispose()
        {
            LeaveNestedControl();
            if (Host != null && Host.FocusedControl == this) Host.FocusedControl = null;
            if (headerBitmap != null) headerBitmap.Dispose();
            headerBitmap = null;
            base.Dispose();
        }


        private int headerHeight = 15;
        public int HeaderHeight
        {
            get { return headerHeight; }
            set
            {
                if (headerHeight != value)
                {
                    headerHeight = value;
                    RecalulateItemOffsets();
                }
            }
        }

        private List<int> itemOffsets = new List<int>();

        private void RecalulateItemOffsets()
        {
            lock (itemOffsets)
            {
                itemOffsets.Clear();
                if (items != null)
                {
                    int top = 0;
                    foreach (object o in items)
                    {
                        itemOffsets.Add(top);
                        top += (o is IGroupHeader) ? headerHeight : ScaledItemHeight;
                    }
                    HasDynamicHeights = true;
                    itemOffsets.Add(top); /// necassary since the height of an item is calculated by the current and the next value.
                    Invalidate();
                }
            }
        }

        protected bool HasDynamicHeights = false;

        /// <summary>
        /// Gets the index of the item that is under the specified y position relative to the control.
        /// </summary>
        /// <param name="y">The y value of a point relative to the control.</param>
        /// <returns>The index of the item under that control, otherwise -1. </returns>
        public int GetItemIndexUnderDisplayY(int y)
        {
            y += ScaledTopOffset;
            return GetItemIndexUnderAbsY(y);
        }

        public int GetItemIndexUnderAbsY(int y)
        {
            if (HasDynamicHeights)
            {
                return GetItemIndexUnderYBinary(y);
            }
            else
            {
                int index = (int)(y / ScaledItemHeight);
                if (index < 0) index = 0;
                if (index >= ItemCount) index = ItemCount - 1;
                return index;
            }
        }

        /// <summary>
        /// Used within GetItemIndexUnderY. Do not use this directly, use GetItemIndexUnderY instead!
        /// </summary>
        /// <remarks>
        /// doing a binary search. there is a method itemTops.BinarySearch though, but due to overhead this function is
        /// even slower for view items (<100) with a linear search, therefore the direct implemntation is the most performant:
        /// </remarks>
        private int GetItemIndexUnderYBinary(int y)
        {
            int c = ItemCount - 1;
            if (c < 0) return -1;
            if (c > ItemCount) return -1;

            int max = c + 0;   //changed from c+1 to c+0 because overflow occured.
            int min = 0;
            int i;

            if (y <= 0) return 0;
            for (; ; )
            {
                i = (max - min) / 2 + min;
                int v = itemOffsets[i];
                int v2 = itemOffsets[i + 1];
                if (v <= y && v2 >= y) break;
                if (min >= max) return -1;
                if (v > y) max = i - 1; else min = i + 1;
            }
            if (i > c) i = c;
            return i;
        }

        /// <summary>
        /// Gets the height of the virtual.
        /// </summary>
        /// <value>The height of the virtual.</value>
        public override int DisplayHeight
        {
            get
            {
                int count = ItemCount;
                if (count == 0) return 0;
                return (int)(GetItemTopOffset(count) / ScaleFactor.Height);
            }
        }

        bool pressed = false;

        /// <summary>
        /// Gets or sets whether the selected item is pressed.
        /// </summary>
        public bool Pressed
        {
            get { return pressed; }
            set
            {
                if (pressed != value)
                {
                    pressed = value;
                    InvalidateItem(selectedItemIndex);
                }
            }
        }

        private int downIndex = -1;

        public override void OnKeyUp(KeyEventArgs e)
        {
            //  if (selectedItemIndex >= 0) OnTemplateEvent(selectedItemIndex, e, TemplateEvent.KeyUp);
            base.OnKeyUp(e);
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            //if (selectedItemIndex >= 0) OnTemplateEvent(selectedItemIndex, e, TemplateEvent.KeyDown);
            if (!e.Handled)
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        e.Handled = true;
                        SelectNextItem();
                        break;

                    case Keys.Up:
                        e.Handled = true;
                        SelectPreviousItem();
                        break;

                    case Keys.Enter:
                        e.Handled = true;
                        OnItemClick(selectedItemIndex);
                        break;
                }
            }
            base.OnKeyDown(e);
        }

        public override void OnKeyPress(KeyPressEventArgs e)
        {
            //if (selectedItemIndex >= 0) OnTemplateEvent(selectedItemIndex, e, TemplateEvent.KeyPress);
            base.OnKeyPress(e);
        }


        bool canClick = true;

        /// <summary>
        /// Occurs on a touch panel click.
        /// </summary>
        /// <param name="mousePosition"></param>
        public override bool OnClick(PointEventArgs p)
        {
            if (!canClick || !CanClick) return false;
            if (templateIndex >= 0)
            {
                if (OnTemplateEvent(templateIndex, p, TemplateEvent.Click)) return true;
            }

            bool result = false;
            //if (ItemSelect == SelectMode.Up)
            //{
            if (downIndex >= 0) SelectedItemIndex = downIndex;
            bool criteria = !JogSelect;
            if (!criteria) criteria = Pressed && selectedItemIndex >= 0;
            if (criteria && SelectedItemIndex >= 0) OnItemClick(SelectedItemIndex);
            result = true;
            //}
            base.OnClick(p);
            return result;

        }

        /// <summary>
        /// Modes how to select an item.
        /// </summary>
        public enum SelectMode
        {
            /// <summary>
            /// select the item on mouse down.
            /// </summary>
            Down,

            /// <summary>
            /// select the item on mouse up.
            /// </summary>
            Up,

            /// <summary>
            /// Never select an item with a click.
            /// </summary>
            Never
        }

        int templateIndex = -1;

        /// <summary>
        /// Gets or sets how to select an item with mouse, stylus or finger.
        /// </summary>
        public SelectMode ItemSelect { get; set; }

        PointEventArgs downPoint = new PointEventArgs(Gesture.None, 0, 0);

        public override void OnMove(PointEventArgs e)
        {
            base.OnMove(e);
            if (templateIndex >= 0) OnTemplateEvent(templateIndex, e, TemplateEvent.Move);
        }

        public override void OnDown(PointEventArgs e)
        {
            canClick = !this.IsAutoScrolling;
            base.OnDown(e);
            if (!canClick) return;

            templateIndex = -1;
            downPoint.X = e.X;
            downPoint.Y = e.Y;

            int index = GetItemIndexUnderDisplayY(e.Y);
            if (index >= 0) OnTemplateEvent(index, e, TemplateEvent.Down);

            int selectedItemIndex = SelectedItemIndex;
            if (JogSelect)
            {
                selectedItemIndex = CheckJogSelect(e);
            }

            if (pressed) SelectedItemIndex = selectedItemIndex;
            else
            {
                int hoverIndex = index;

                if (hoverIndex == selectedItemIndex) Pressed = true; else HoveredItemIndex = hoverIndex;
            }
            downIndex = hoveredItemIndex;
            if (ItemSelect == SelectMode.Down && hoveredItemIndex >= 0) SelectedItemIndex = hoveredItemIndex;

        }


        public override void OnUp(PointEventArgs e)
        {
            ScrollBarVisible = false;
            base.OnUp(e);
            if (templateIndex >= 0) OnTemplateEvent(templateIndex, e, TemplateEvent.Up);

            Pressed = false;
            HoveredItemIndex = -1;
        }

        public override void OnRelease(PointEventArgs p)
        {
            if (templateIndex >= 0) OnTemplateEvent(templateIndex, p, TemplateEvent.Release);

            Pressed = false;
            HoveredItemIndex = -1;
        }

        private enum TemplateEvent
        {
            Up,
            Down,
            Release,
            Click,
            Move,
            KeyDown,
            KeyUp,
            KeyPress
        }


        PointEventArgs templatePointEventArg = new PointEventArgs();
        FluidTemplate selectedTemplate;
        int selectedTemplateIndex;

        private bool OnTemplateEvent(int index, EventArgs ea, TemplateEvent type)
        {
            bool result = false;
            // if (BindTemplate != null)
            {
                //                Debug.WriteLine("TemplateEvent: " + type.ToString());
                FluidTemplate template = GetTemplate(index);
                PointEventArgs e = ea as PointEventArgs;


                // set the selected template to this template. A template is always reused and the Index and ItemIndex might change
                // when it is not accessed within the OnBind method (e.g. on a TextChanged event of a TextBox in the template).
                // Therefore these two values are stored on any OnTemplate event, assuming that the control that has the focus and might raise
                // an event is always the control that is inside the template that lately received an event:
                selectedTemplate = template;
                selectedTemplateIndex = template != null ? template.ItemIndex : -1;

                if (template != null)
                {
                    PointEventArgs p = templatePointEventArg;
                    if (e != null)
                    {
                        template.BeginInit();
                        p.X = e.X;
                        p.Y = e.Y;
                        p.Gesture = e.Gesture;
                        p = TranslatePoint(template, p);
                        template.EndInit();
                    }
                    switch (type)
                    {
                        case TemplateEvent.Down:
                            templateIndex = index;
                            template.OnDown(p);
                            break;

                        case TemplateEvent.Up:
                            template.OnUp(p);
                            break;

                        case TemplateEvent.Release:
                            template.OnRelease(p);
                            templateIndex = -1;
                            break;

                        case TemplateEvent.Click:
                            result = template.OnClick(p);
                            break;

                        case TemplateEvent.Move:
                            template.OnMove(p);
                            break;

                        case TemplateEvent.KeyDown:
                            template.OnKeyDown(ea as KeyEventArgs);
                            break;

                        case TemplateEvent.KeyUp:
                            template.OnKeyUp(ea as KeyEventArgs);
                            break;

                        case TemplateEvent.KeyPress:
                            template.OnKeyPress(ea as KeyPressEventArgs);
                            break;


                    }

                }
            }
            return result;
        }

        private PointEventArgs TranslatePoint(FluidTemplate c, PointEventArgs p)
        {
            p.X -= c.Bounds.X;
            p.Y -= c.Bounds.Y;
            return p;
        }


        /// <summary>
        /// Invalids the area of the specified item.
        /// </summary>
        /// <param name="itemIndex">The index of the item.</param>
        public void InvalidateItem(int itemIndex)
        {
            Invalidate(GetItemBounds(itemIndex));
        }

        private int CheckJogSelect(PointEventArgs e)
        {
            const int fuzzyHeight = 18;
            int index = SelectedItemIndex;

            if (CanShowScrollbar)
            {
                if (index >= 0)
                {
                    Rectangle itemBounds = GetItemBounds(index);
                    int fuzzy = (int)(Math.Max(fuzzyHeight, itemHeight / 2) * ScaleFactor.Height);
                    int y = e.Y - fuzzy;
                    Rectangle pressedArea = new Rectangle(itemBounds.Left, y, itemBounds.Width, 2 * fuzzy);
                    Pressed = pressedArea.IntersectsWith(itemBounds);
                    if (Pressed)
                    {
                        index = GetItemIndexUnderDisplayY(e.Y);
                        if (index >= 0) SelectedItemIndex = index;
                    }
                }
                else Pressed = false;
            }
            else
            {
                index = GetItemIndexUnderDisplayY(e.Y);
                if (index >= 0) SelectedItemIndex = index;
                Pressed = true;
            }
            return index;
        }

        /// <summary>
        /// Occurs when the the panel starts beeing virtually moved.
        /// </summary>
        protected override void BeginMoving()
        {
            HoveredItemIndex = -1;
            if (!JogSelect) Pressed = false;
            if (templateIndex >= 0) OnTemplateEvent(templateIndex, downPoint, TemplateEvent.Release);
            if (!pressed) base.BeginMoving();
        }

        protected override void Scroll(PointEventArgs p, Point moving)
        {
            //LeaveNestedControl(); <-- not necassary!
            if (pressed && JogSelect)
            {
                SelectInMove(moving);
            }
            else
            {
                base.Scroll(p, moving);
            }
        }

        private void SelectInMove(Point e)
        {
            if ((e.Y < 0) || (e.Y >= (Height - 1)))
            {
                StartTimer(e);
            }
            else
            {
                StopTimer();
                int index = GetItemIndexUnderDisplayY(e.Y);
                if (index >= 0) SelectedItemIndex = index;
            }
        }

        private Timer timer;

        private void StopTimer()
        {
            if (timer != null)
            {
                timer.Enabled = false;
            }
            ScrollBarVisible = false;
        }

        private void StartTimer(Point e)
        {
            if (timer == null)
            {
                timer = new Timer();
                timer.Interval = 50;
                timer.Tick += new EventHandler(timer_Tick);
                timer.Enabled = true;
                int y = e.Y > 0 ? itemHeight / 2 : -itemHeight / 2;
                tickPoint = new Point(e.X, y);
            }
            ScrollBarVisible = true;
            timer.Enabled = true;
        }

        private Point tickPoint;

        void timer_Tick(object sender, EventArgs e)
        {
            if (tickPoint.Y > 0) SelectNextItem(); else SelectPreviousItem();
        }


        /// <summary>
        /// Selects the next item that is not a group header and ensures to be visible.
        /// </summary>
        public void SelectNextItem()
        {
            ScrollBarVisible = true;
            if (HoveredItemIndex >= 0) SelectedItemIndex = HoveredItemIndex;
            HoveredItemIndex = -1;
            int index = SelectedItemIndex + 1;
            int count = ItemCount;
            for (; ; )
            {
                // skip group headers:
                if (index >= count) break;
                IGroupHeader gh = GetItem(index) as IGroupHeader;
                if (gh == null) break;
                index++;
            }
            index = EnsureIndexBounds(index);
            Rectangle bounds = GetItemBounds(index);
            SelectedItemIndex = index;
            EnsureVisible(bounds);
            ScrollBarVisible = false;
        }

        /// <summary>
        /// Selects the previous item that is not a group header and ensures to be visible.
        /// </summary>
        public void SelectPreviousItem()
        {
            ScrollBarVisible = true;
            if (HoveredItemIndex >= 0) SelectedItemIndex = HoveredItemIndex;
            HoveredItemIndex = -1;
            int index = SelectedItemIndex - 1;
            for (; ; )
            {
                // skip group headers:
                if (index < 0) return; // abort, the top item is a header!
                IGroupHeader gh = GetItem(index) as IGroupHeader;
                if (gh == null) break;
                index--;
            }
            index = EnsureIndexBounds(index);
            Rectangle bounds = GetItemBounds(index);
            SelectedItemIndex = index;
            EnsureVisible(bounds);
            ScrollBarVisible = false;
        }

        private int itemHeight = 16;

        /// <summary>
        /// Gets or sets the height of an item in designer mode.
        /// </summary>
        [DefaultValue(16)]
        public int ItemHeight
        {
            get { return itemHeight; }
            set
            {
                if (itemHeight != value)
                {
                    itemHeight = value;
                    DBufferSpace = Math.Min(value, 200);
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets the scaled height of an item.
        /// </summary>
        public int ScaledItemHeight
        {
            get
            {
                return (int)(ScaleFactor.Height * itemHeight);
            }
        }

        private FluidTemplate template;

        /// <summary>
        /// Gets or sets the template for the items.
        /// </summary>
        public FluidTemplate ItemTemplate
        {
            get { return template; }
            set
            {
                if (template != value)
                {
                    if (value != null) value.Container = null;
                    template = value;
                    if (value != null) value.Container = this;
                }
            }
        }

        private IList items = null;

        private object dataSource;

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        public object DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                    groupHeaderIndex = -1;
                    if (dataSource is IList) SetListSource(dataSource as IList);
                    else if (dataSource is DataTable) SetDataTableSource(dataSource as DataTable);
                    else ClearDataSource();

                    Invalidate();
                }
                else
                {
                    ResetBindingList();
                }

            }
        }

        /// <summary>
        /// Clears the data source items.
        /// </summary>
        private void ClearDataSource()
        {
            items = null;
        }

        /// <summary>
        /// Sets the data source from a DataTable.
        /// </summary>
        private void SetDataTableSource(DataTable dataTable)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the data source from an IList.
        /// </summary>
        private void SetListSource(IList iList)
        {
            StopAnimations();
            StopAutoScroll();
            items = iList;

            ConnectBindingList(iList as IBindingList);

            //TODO: if the buffer is not deleted, the list appears not empty, if the new ItemCount is 0 but was >0 before. Fix this without clearing the buffer!
            ClearDBuffer();
            RecalulateItemOffsets();
            topOffset = 0;
            ShowHeader = GetItem(0) is IGroupHeader;
            SelectFirstDataItem();
            Invalidate();
        }


        private int selectedItemIndex;

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        public int SelectedItemIndex
        {
            get
            {
                return selectedItemIndex;
            }
            set
            {
                value = EnsureIndexBounds(value);
                if (selectedItemIndex != value)
                {
                    int old = selectedItemIndex;
                    if (selectedItemIndex >= 0)
                    {
                        InvalidateItem(selectedItemIndex);
                    }
                    if (value >= 0)
                    {
                        InvalidateItem(value);
                    }
                    selectedItemIndex = value;
                    OnSelectedIndexChanged(old, value);
                }
            }
        }

        private int EnsureIndexBounds(int value)
        {
            if (value < 0) value = -1;
            if (value >= ItemCount) value = ItemCount - 1;
            return value;
        }

        private ChangedEventArgs<int> selectedIndexChangedEvent;

        protected virtual void OnSelectedIndexChanged(int oldValue, int newValue)
        {
            FluidTemplate t = GetTemplate(oldValue);
            if (t != null) t.LeaveNestedFocusedControl();
            if (Host == null) return;
            FluidControl sc = Host.FocusedControl;
            if (sc == null) Focus();
            if (SelectedIndexChanged != null)
            {
                if (selectedIndexChangedEvent == null) selectedIndexChangedEvent = new ChangedEventArgs<int>();
                ChangedEventArgs<int> e = selectedIndexChangedEvent;
                e.OldValue = oldValue;
                e.NewValue = newValue;
                SelectedIndexChanged(this, e);
            }
        }


        public event EventHandler<ChangedEventArgs<int>> SelectedIndexChanged;

        private int hoveredItemIndex = -1;

        /// <summary>
        /// Gets or sets the index of the hovered item.
        /// </summary>
        public int HoveredItemIndex
        {
            get
            {
                return hoveredItemIndex;
            }
            set
            {
                if (hoveredItemIndex != value)
                {
                    if (hoveredItemIndex >= 0)
                    {
                        InvalidateItem(hoveredItemIndex);
                    }
                    if (value >= 0)
                    {
                        InvalidateItem(value);
                    }
                    hoveredItemIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets the number of items in the data source.
        /// </summary>
        public int ItemCount
        {
            get
            {
                if (items != null) return items.Count;
                return 0;
            }
        }

        private int GetItemTopOffset(int index)
        {
            if (index < 0) return 0;
            if (index >= itemOffsets.Count) index = itemOffsets.Count - 1;
            if (HasDynamicHeights && this.itemOffsets.Count > 0)
            {
                return itemOffsets[index];
            }
            return index * ScaledItemHeight;
        }

        /// <summary>
        /// Gets the bounds for an item.
        /// </summary>
        /// <param name="index">The index of the item for which to get the bounds.</param>
        /// <returns>The bounds of the item.</returns>
        public Rectangle GetItemBounds(int index)
        {
            int y = GetItemTopOffset(index);
            int h = items != null && index >= 0 && index < items.Count ? GetItemHeight(items[index]) : 0;
            Rectangle r = new Rectangle(0, y - ScaledTopOffset, Width, h);
            return r;
        }

        /// <summary>
        /// Gets the bounds for an item.
        /// </summary>
        /// <param name="index">The index of the item for which to get the bounds.</param>
        /// <returns>The bounds of the item.</returns>
        protected Rectangle GetPaintItemBounds(int index)
        {
            int y = GetItemTopOffset(index);
            int h = items != null && index >= 0 && index < items.Count ? GetItemHeight(items[index]) : 0;
            Rectangle r = new Rectangle(0, y - PaintTop, Width, h);
            return r;
        }

        /// <summary>
        /// Gets or sets the index of the first partially visible item.
        /// </summary>
        public int TopItemIndex
        {
            get
            {
                int item = (TopOffset / itemHeight);
                if (item < 0) item = 0;
                return item;
            }
            set
            {
                int pos = value * itemHeight;
                if (pos < 0) pos = 0;
                TopOffset = pos;
            }
        }

        /// <summary>
        /// Gets or sets the index of the last partially visible item.
        /// </summary>
        public int BottomItem
        {
            get
            {
                int maxItems = Height / ScaledItemHeight;
                int bottom = maxItems + TopItemIndex;
                if (items != null)
                {
                    if (bottom > items.Count) bottom = items.Count;
                }
                else bottom = 0;
                return bottom;
            }
            set
            {
                int maxItems = Height / ScaledItemHeight;
                int top = value - maxItems;
                if (top < 0) top = 0;
                TopItemIndex = top;
            }
        }


        /// <summary>
        /// Paints the content of this control.
        /// </summary>
        /// <param name="pe">The  paint event args.</param>
        protected override void PaintContent(FluidPaintEventArgs pe)
        {
            if (AlwaysPaintBackground)
            {
                base.PaintContent(pe);
            }
            PaintItems(pe);
        }

        /// <summary>
        /// Gets or sets the background color for the hovered item.
        /// </summary>       
        public Color HoveredBackColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for the hovered item.
        /// </summary>
        public Color HoveredForeColor { get; set; }

        /// <summary>
        /// Gets or sets the background color for the pressed item.
        /// </summary>
        public Color PressedBackColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground color for the pressed item.
        /// </summary>
        public Color PressedForeColor { get; set; }

        /// <summary>
        /// Gets or sets the border color for an item.
        /// </summary>
        public Color BorderColor { get; set; }


        /// <summary>
        /// Gets or sets the background color for a selected item.
        /// </summary>
        public Color SelectedBackColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground (pen) color for a selected item.
        /// </summary>
        public Color SelectedForeColor { get; set; }


        /// <summary>
        /// Gets the background color for a specific item.
        /// </summary>
        /// <param name="index">The index of the item for which to get the background color.</param>
        /// <returns>A color.</returns>
        protected Color GetBackColorForItem(int index)
        {
            if (index == SelectedItemIndex)
            {
                return pressed ? PressedBackColor : SelectedBackColor;
            }
            if (index == HoveredItemIndex) return HoveredBackColor;
            return BackColor;
        }

        /// <summary>
        /// Gets the foreground color for a specific item.
        /// </summary>
        /// <param name="index">The index of the item for which to get the foreground color.</param>
        /// <returns>A color.</returns>
        protected Color GetForeColorForItem(int index)
        {
            if (index == SelectedItemIndex)
            {
                return pressed ? PressedForeColor : SelectedForeColor;
            }
            if (index == HoveredItemIndex) return HoveredForeColor;
            return ForeColor;
        }

        /// <summary>
        /// Gets or sets whether to always paint the background or to paint only the necassary bounds with the background color.
        /// The latter (default) value is faster but does not enable to paint more than a solid color.
        /// </summary>
        public bool AlwaysPaintBackground { get; set; }

        private ListBoxItemPaintEventArgs paintEventArgs = new ListBoxItemPaintEventArgs();

        /// <summary>
        /// Paints all items visible in the control.
        /// </summary>
        /// <param name="pe">PaintEventArgs.</param>
        protected virtual void PaintItems(FluidPaintEventArgs pe)
        {
            if (items != null)
            {
                Graphics g = pe.Graphics;

                Rectangle clipBounds = pe.ControlBounds;

                int first = GetItemIndexUnderDisplayY(clipBounds.Top + PaintTop - ScaledTopOffset);
                if (first < 0) first = 0;

                int count = ItemCount;
                int max = clipBounds.Bottom;

                ListBoxItemPaintEventArgs args = paintEventArgs;
                args.Graphics = g;
                args.Region = pe.Region;
                args.ScaleFactor = ScaleFactor;
                Region clip = pe.Region;
                for (int index = first; count > index; index++)
                {
                    Rectangle itemBounds = GetPaintItemBounds(index);
                    if (itemBounds.Top >= max) break;

                    if (itemBounds.IntersectsWith(clipBounds) && clip.IsVisible(itemBounds))
                    {
                        object item = GetItem(index);
                        if (!(item is IGroupHeader))
                        {
                            args.Template = GetTemplate(index);
                            //                            OnDataBound(args.Template);
                        }
                        args.Item = item;
                        args.ItemIndex = index;
                        args.ForeColor = GetForeColorForItem(index);
                        args.BackColor = GetBackColorForItem(index);
                        args.BorderColor = BorderColor;
                        args.Text = item.ToString();
                        args.Handled = false;
                        args.Font = Font;
                        args.ClientBounds = itemBounds;

                        if (index == SelectedItemIndex)
                        {
                            args.State = pressed ? ListBoxItemState.Selected | ListBoxItemState.Pressed : ListBoxItemState.Selected;
                        }
                        else
                        {
                            args.State = ListBoxItemState.Default;
                        }
                        if (index == hoveredItemIndex)
                        {
                            args.State |= ListBoxItemState.Hovered;
                        }

                        OnPaintItem(args);
                    }
                }

                if (!AlwaysPaintBackground)
                {
                    FillSpace(g, clip, clipBounds);
                }
            }
        }


        /// <summary>
        /// Fill the area that does not contain any item with the background color:
        /// </summary>
        /// <param name="bottom">The bottom offset to paint.</param>
        private void FillSpace(Graphics g, Region clip, Rectangle bounds)
        {
            int top = GetItemTopOffset(ItemCount) - PaintTop;
            int height = bounds.Height;
            int h = height - top;
            if (h > 0)
            {
                Rectangle bottomBounds = new Rectangle(0, top, Width, h);
                if (clip.IsVisible(bottomBounds))
                {
                    backgroundBrush.Color = BackColor;
                    g.FillRectangle(backgroundBrush, bottomBounds);
                }
            }
            if (DBufferTop < 0)
            {
                Rectangle topBounds = new Rectangle(0, 0, Width, 0 - DBufferTop);
                if (clip.IsVisible(topBounds))
                {
                    backgroundBrush.Color = BackColor;
                    g.FillRectangle(backgroundBrush, topBounds);
                }
            }
        }


        /// <summary>
        /// Gets the template for the specified item.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The template for the item.</returns>
        public FluidTemplate GetTemplate(int index)
        {
            object item = GetItem(index);
            FluidTemplate template = OnBindTemplate(index, item, this.template);
            if (template != null)
            {
                template.BeginInit();
                template.ItemIndex = index;
                template.Item = item;
                template.Container = this;
                template.Scale(ScaleFactor);
                template.Bounds = GetItemBounds(index);
                template.EndInit();
            }
            return template;
        }

        private TemplateEventArgs templateEventArgs;

        /// <summary>
        /// Occurs when to get and/or bind a template.
        /// The event is used for two reasons:
        /// a) To apply a custom template to the event other than the default template.
        /// b) assign the properties of all child controls of the template to the data specified as Item and/or ItemIndex. 
        /// </summary>
        /// <example>
        ///     for instance, if there is a Label control in the template you could assign the Text like followed: 
        ///     label.Text = (e.Item as MyValue).FirstName;
        /// </example>
        protected virtual FluidTemplate OnBindTemplate(int index, object item, FluidTemplate template)
        {
            if (item is IGroupHeader) return null;
            if (BindTemplate != null)
            {
                TemplateEventArgs e = templateEventArgs;
                e.ItemIndex = index;
                e.Item = item;
                e.Template = template;
                template.BeginInit();
                BindTemplate(this, e);
                template.EndInit();
                template = e.Template;
            }
            return template;
        }



        /// <summary>
        /// Occurs after the Template was data bound an the EndInit state is completed for the template.
        /// </summary>
        /// <example>
        /// You can use this events to perform some actions that require to raise an event, for instance to set the focus to a control like tbFirstName.Focus().
        /// </example>        
        [Obsolete("seems to make no practical sense...")]
        protected virtual void OnDataBound(FluidTemplate fluidTemplate)
        {
            if (DataBound != null)
            {
                TemplateEventArgs e = templateEventArgs;
                DataBound(this, e);
            }
        }

        /// <summary>
        /// Occurs after the Template was data bound an the EndInit state is completed for the template.
        /// </summary>
        /// <example>
        /// You can use this events to perform some actions that require to raise an event, for instance to set the focus to a control like tbFirstName.Focus().
        /// </example>        
        public event EventHandler<TemplateEventArgs> DataBound;

        /// <summary>
        /// Occurs when to get and/or bind a template.
        /// The event is used for two reasons:
        /// a) To apply a custom template to the event other than the default template.
        /// b) assign the properties of all child controls of the template to the data specified as Item and/or ItemIndex. 
        ///     Note, that template is in BeginInit state so no events are raised within this event.
        /// </summary>
        /// <example>
        ///     for instance, if there is a Label control in the template you could assign the Text like followed: 
        ///     label.Text = (e.Item as MyValue).FirstName;
        /// </example>
        public event EventHandler<TemplateEventArgs> BindTemplate;


        private ListBoxItemPaintEventArgs itemEventArgs = new ListBoxItemPaintEventArgs();

        /// <summary>
        /// Occurs when an item was clicked.
        /// </summary>
        protected virtual void OnItemClick(int index)
        {
            if (ItemClick != null)
            {
                ListBoxItemEventArgs e = itemEventArgs;
                e.Item = GetItem(index);
                e.ItemIndex = index;
                ItemClick(this, e);
            }

        }

        /// <summary>
        /// Occurs when an item was clicked.
        /// </summary>
        public event EventHandler<ListBoxItemEventArgs> ItemClick;

        protected virtual void OnPaintItem(ListBoxItemPaintEventArgs e)
        {
            if (e.Item is IGroupHeader)
            {
                IColorGroupHeader colorGroup = e.Item as IColorGroupHeader;
                e.BackColor = colorGroup != null ? colorGroup.BackColor : ListBoxItemPaintEventArgs.DefaultHeaderColor;
                if (e.BorderColor.IsEmpty) e.BorderColor = ListBoxItemPaintEventArgs.DefaultHeaderColor;
                e.BorderColor = e.BorderColor;
                if (PaintGroupHeader != null)
                {
                    PaintGroupHeader(this, e);
                }
            }
            else
            {
                if (PaintItem != null)
                {
                    PaintItem(this, e);
                }
            }
            if (!e.Handled) e.PaintDefault();
        }

        /// <summary>
        /// Occurs when an item is painted.
        /// </summary>
        public event EventHandler<ListBoxItemPaintEventArgs> PaintGroupHeader;

        /// <summary>
        /// Occurs when an item is painted.
        /// </summary>
        public event EventHandler<ListBoxItemPaintEventArgs> PaintItem;

        /// <summary>
        /// Gets or sets whether to enable the selected item to move up or down when it is pressed.
        /// </summary>
        public bool JogSelect { get; set; }

        /// <summary>
        /// ListBox can receive keyboard focus for up and down:
        /// </summary>
        public override bool Selectable { get { return true; } }

        private SizeF scaleFactor = new SizeF(1f, 1f);

        //TODO: implement this to replace all Scaled... properties
        public override void Scale(SizeF scaleFactor)
        {
            if (this.scaleFactor.Height != scaleFactor.Height)
            {
                headerHeight = (int)(headerHeight * scaleFactor.Height);
                RecalulateItemOffsets();
            }
            this.scaleFactor = scaleFactor;
            Size size = this.Bounds.Size;
            base.Scale(scaleFactor);
            if (template != null) template.Scale(scaleFactor);
        }

        #region ICommandContainer Members

        public virtual void RaiseCommand(CommandEventArgs e)
        {
            if (Command != null)
            {
                Command(this, e);
            }
            if (!e.Handled)
            {
                ICommandContainer container = this.Container as ICommandContainer;
                if (container != null)
                {
                    container.RaiseCommand(e);
                }
            }
        }

        public event EventHandler<CommandEventArgs> Command;

        #endregion

        /// <summary>
        /// Gets the item index of the IGroupHeader that comes directly before  the specified item index.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <returns>The index of the previous IGroupHeader, otherwise -1.</returns>
        public int GetPreviousGroupHeader(int index)
        {
            for (int i = index; i >= 0; i--)
            {
                IGroupHeader gh = items[i] as IGroupHeader;
                if (gh != null) return i;
            }
            return -1;
        }

        bool showHeader = true;

        /// <summary>
        /// Gets whether the listbox has a header.
        /// this is automatically set to true, if the first item is of IGroupHeader.
        /// </summary>
        public bool ShowHeader
        {
            get { return showHeader; }
            private set
            {
                if (showHeader != value)
                {
                    showHeader = value;
                    Invalidate();
                }
            }
        }

        public override bool Active { get { return true; } }


        protected override Rectangle GetListBounds()
        {
            Rectangle r = base.GetListBounds();
            if (showHeader)
            {
                r.Y += headerHeight;
                r.Height -= headerHeight;
            }
            return r;
        }

        /// <summary>
        /// Gets the bounds for the header.
        /// </summary>
        /// <returns>A rectangle with the bounds, otherwise Rectangle.Empty.</returns>
        public Rectangle GetHeaderBounds()
        {
            if (showHeader)
            {
                return new Rectangle(0, 0, Width, headerHeight);
            }
            else return Rectangle.Empty;
        }

        //protected override int GetMinTop()
        //{
        //    return showHeader ? headerHeight : 0;
        //}

        private int groupHeaderIndex = -1;

        public int GroupHeaderIndex
        {
            get { return groupHeaderIndex; }
            set
            {
                if (groupHeaderIndex != value)
                {
                    OnGroupHeaderIndexChange(groupHeaderIndex, value);
                    groupHeaderIndex = value;
                    Invalidate(GetHeaderBounds());
                }
            }
        }

        protected virtual void OnGroupHeaderIndexChange(int actualValue, int newValue)
        {
        }

        private int GetTopGroupHeaderIndex(int dy)
        {
            int index = GetItemIndexUnderDisplayY((int)(dy * ScaleFactor.Height));
            return GetPreviousGroupHeader(index);
        }


        protected override void OnTopOffsetChange(int actualValue, int newValue)
        {
            //LeaveNestedControl();
            base.OnTopOffsetChange(actualValue, newValue);
            if (EnableTransparentHeader || TopOffset < 0) Invalidate(GetHeaderBounds());
            else
            {
                CheckScrollbarButtonOverlapsInHeader(actualValue, newValue);
            }

            int index = GetTopGroupHeaderIndex(newValue - actualValue);
            if (index != groupHeaderIndex)
            {
                groupHeaderIndex = index;
                Invalidate(GetHeaderBounds());
            }
            else
            {
                if (!EnableTransparentHeader)
                {
                    int overlapped = GetOverlappedGroupHeader();
                    if (overlapped >= 0) Invalidate(GetHeaderBounds());
                }
            }

        }

        private void LeaveNestedControl()
        {
            if (Container == null || Container.Host == null) return;
            FluidControl selected = Container.Host.FocusedControl;
            if (selected != null)
            {
                if (selectedTemplate != null && selected.IsDescendantOf(selectedTemplate))
                {
                    Container.Host.FocusedControl = this;
                }
            }
        }


        /// <summary>
        /// Checks wether either to previous or the new value of TopOffset causes the scrollbar button to overlap into
        /// the header canvas and invalidates the header canvas if positive.
        /// </summary>
        /// <param name="actualvalue">The old value of TopOffset.</param>
        /// <param name="newvalue">The new value of TopOffset</param>
        private void CheckScrollbarButtonOverlapsInHeader(int actualvalue, int newvalue)
        {
            if (ShowHeader)
            {
                int value = Math.Min(actualvalue, newvalue);
                Rectangle bounds = GetScrollbarButtonBounds((int)(value * ScaleFactor.Height));

                if (bounds.Top < headerHeight) Invalidate(GetHeaderBounds());
            }
        }

        public override void OnPaint(FluidPaintEventArgs pe)
        {
            PaintHeader(pe);
            base.OnPaint(pe);
        }

        private Bitmap headerBitmap;


        bool enableTransparentHeader = false;


        protected override int GetMinTop()
        {
            return headerHeight;
        }



        /// <summary>
        /// Gets or sets wether a transparent header is enabled.
        /// Note,that also <see>EnableDoubleBuffer</see> is required to be set to true.
        /// Note, that TransparentHeaders might scrolling scrolling slower if hardware acceleration is not supported (or featured in gdi++)!!
        /// </summary>
        /// 
        [DefaultValue(false)]
        public bool EnableTransparentHeader
        {
            get { return enableTransparentHeader && EnableDoubleBuffer; }
            set
            {
                if (enableTransparentHeader != value)
                {
                    enableTransparentHeader = value;

                    // the following check makes sense, since EnableTransparentHeader is not always the same as enableTransparentHeader:
                    if (EnableTransparentHeader != value)
                    {
                        Invalidate(GetHeaderBounds());
                    }
                }
            }
        }

        private FluidPaintEventArgs headerEvents;

        private void PaintHeader(FluidPaintEventArgs pe)
        {
            Rectangle bounds = GetHeaderBounds();
            Rectangle clip = pe.ControlBounds;
            bounds.Offset(clip.X, clip.Y);
            if (pe.Region.IsVisible(bounds))
            {
                if (!EnableDoubleBuffer)
                {
                    PaintHeaderUnbuffered(pe);
                }
                else
                {
                    PaintHeaderContentDBuffered(pe);
                }
            }
        }

        private void PaintHeaderUnbuffered(FluidPaintEventArgs pe)
        {
            PaintHeaderContent(pe);
        }

        private void PaintHeaderContentDBuffered(FluidPaintEventArgs pe)
        {
            if (headerEvents == null) headerEvents = new FluidPaintEventArgs();
            Bitmap bm = EnsureHeaderBitmap();
            FluidPaintEventArgs e = headerEvents;
            Rectangle dstRect = GetHeaderBounds();
            Rectangle srcRect = new Rectangle(0, 0, dstRect.Width, dstRect.Height);
            Rectangle b = pe.ControlBounds;
            dstRect.Offset(b.X, b.Y);
            using (Graphics g = Graphics.FromImage(bm))
            {
                e.Graphics = g;
                e.ControlBounds = srcRect;
                e.Region = g.Clip;
                e.DoubleBuffered = true;
                e.ScaleFactor = this.ScaleFactor;

                PaintHeaderContent(e);
                if (ScrollBarVisible) PaintScrollBarButton(g, GetScrollbarButtonBounds());
                pe.Graphics.DrawImage(bm, dstRect, srcRect, GraphicsUnit.Pixel);
            }
        }

        private Bitmap EnsureHeaderBitmap()
        {
            Rectangle rect = GetHeaderBounds();
            if (headerBitmap == null || headerBitmap.Size != rect.Size)
            {
                if (headerBitmap != null) headerBitmap.Dispose();
                int w = Math.Max(1, rect.Width);
                int h = Math.Max(1, rect.Height);
                headerBitmap = new Bitmap(w, h);
            }
            return headerBitmap;
        }

        private int PaintHeaderContent(FluidPaintEventArgs pe)
        {
            Rectangle rect = GetHeaderBounds();
            int deltaY = 0;
            if (rect.IsEmpty) return 0;
            rect.Offset(pe.ControlBounds.X, pe.ControlBounds.Y);
            if (rect.IntersectsWith(pe.ControlBounds))
            {
                pe.Graphics.FillRectangle(backgroundBrush, rect);
                if (groupHeaderIndex < 0)
                {
                    groupHeaderIndex = GetTopGroupHeaderIndex(0);
                }
                int gi = EnsureIndexBounds(groupHeaderIndex);
                int top = GetItemTopOffset(gi) - ScaledTopOffset;
                if (gi == 0 && top > 0) deltaY = top;
                if (gi >= 0)
                {
                    rect.Offset(0, deltaY);
                    IGroupHeader gh = items[gi] as IGroupHeader;
                    if (gh != null)
                    {
                        ListBoxItemPaintEventArgs e = paintEventArgs;
                        e.Graphics = pe.Graphics;
                        e.Region = pe.Region;
                        e.Font = Font;
                        e.BackColor = BackColor;
                        e.BorderColor = BorderColor;
                        e.ForeColor = ForeColor;
                        e.Handled = false;
                        e.ClientBounds = rect;
                        e.Item = gh;
                        e.ItemIndex = groupHeaderIndex;
                        e.State = ListBoxItemState.Default;
                        e.Text = gh.Title;

                        int overlapped = GetOverlappedGroupHeader();

                        if (EnableTransparentHeader)
                        {
                            if (top < 0)
                            {
                                e.State |= ListBoxItemState.Transparent;
                                Point p = Container.PointToHost(0, 0);
                                PaintDBuffer(pe, p.X, p.Y, GetHeaderBounds());
                            }
                        }

                        OnPaintItem(e);
                        if (overlapped > gi)
                        {
                            int delta = GetItemTopOffset(overlapped) - ScaledTopOffset;
                            rect.Y += delta;
                            e.ClientBounds = rect;
                            gh = items[overlapped] as IGroupHeader;
                            e.Item = gh;
                            e.Text = gh.Title;
                            e.State = ListBoxItemState.Default;
                            e.ItemIndex = overlapped;
                            OnPaintItem(e);
                        }
                    }
                }
                else
                {
                    pe.Graphics.FillRectangle(backgroundBrush, rect);
                }

            }
            return deltaY;
        }

        private int GetOverlappedGroupHeader()
        {
            int index = EnsureIndexBounds(GetItemIndexUnderAbsY(ScaledTopOffset) + 1);
            if (index < ItemCount && index >= 0)
            {
                if (items[index] is IGroupHeader)
                {
                    int top = GetItemTopOffset(index) - ScaledTopOffset;
                    if (top < headerHeight) return index;
                }
            }
            return -1;
        }

        Rectangle IContainer.GetScreenBounds(Rectangle bounds)
        {
            bounds.Offset(Left, Top);
            return Container.GetScreenBounds(bounds);
        }


        #region IControlContainer Members

        IEnumerable<FluidControl> IMultiControlContainer.Controls
        {
            get
            {
                if (ItemTemplate != null)
                {
                    return new FluidControl[] { ItemTemplate };
                }
                else
                {
                    return new FluidControl[] { };
                }
            }
        }

        public IHost Host
        {
            get { return Container.Host; }
        }

        public Point PointToHost(int x, int y)
        {
            return Container.PointToHost(x + Left, y + Top);
        }

        #endregion

        /// <summary>
        /// Gets the item for the specified index.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        /// <returns>The item of the specified item if the index is within the valid range, otherwise null.</returns>
        public object GetItem(int index)
        {
            return (index >= 0 && index < ItemCount) ? items[index] : null;
        }

        #region ITemplateHost Members

        /// <summary>
        /// Binds a template with the data of the currently selected template.
        /// </summary>
        /// <param name="template"></param>
        public void Bind(FluidTemplate template)
        {
            Bind(template, selectedItemIndex);
        }

        /// <summary>
        /// Binds a template with the data of the currently selected template.
        /// </summary>
        /// <param name="template"></param>
        public void Bind(FluidTemplate template, int index)
        {
            object item = GetItem(index);
            template.BeginInit();
            template.ItemIndex = index;
            template.Item = item;
            template.Container = this;
            template.Scale(ScaleFactor);
            template.Bounds = GetItemBounds(index);
            template.EndInit();
        }

        #endregion


        /// <summary>
        /// Ensures that the specified item is completely visible and scrolls the display to a appropriate position if not.
        /// </summary>
        /// <param name="itemIndex">The index of the selected item to be visible.</param>
        public void EnsureVisible(int itemIndex)
        {
            Rectangle bounds = GetItemBounds(itemIndex);
            EnsureVisible(bounds);
        }

        protected override void OnSizeChanged(Size oldSize, Size newSize)
        {
            LeaveNestedControl();
            base.OnSizeChanged(oldSize, newSize);
            UpdateTemplatesSize(oldSize, newSize);
        }

        private void UpdateTemplatesSize(Size oldSize, Size newSize)
        {
            Rectangle bounds = GetItemBounds(0);
            foreach (FluidTemplate t in templates)
            {
                t.Bounds = bounds;
            }
            if (ItemTemplate != null) ItemTemplate.Bounds = bounds;
        }

        public override void OnGesture(GestureEventArgs e)
        {
            base.OnGesture(e);
            if (templateIndex >= 0)
            {
                FluidTemplate template = GetTemplate(templateIndex);
                if (template != null) template.OnGesture(e);
            }
        }

        private TemplateCollection templates;

        public TemplateCollection Templates { get { return templates; } }

    }
}
