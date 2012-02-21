using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Fluid.Controls.Classes;
using Fluid.Drawing;
using Fluid.Classes;

namespace Fluid.Controls
{
    /// <summary>
    /// A control container that contains PageControls and allows forward and backward navigation between  Pages.
    /// This control also has a header with a title bar, a back button and optional buttons.
    /// </summary>
    public class NavigationPanel : ControlContainer
    {
        private FluidHeader header = new FluidHeader();
        private TransitionPanel navigation = new TransitionPanel();

        protected override void InitControl()
        {
            bounds = new Rectangle(0, 0, 240, 300);
            pages = new PageCollection(this);
            base.InitControl();
            header.Anchor = AnchorStyles.None;
            navigation.Anchor = AnchorStyles.None;
            Layout();
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            navigation.InternalEnableDoubleBuffer = false;
            controls.Add(header);
            controls.Add(navigation);


            navigation.SelectedIndexChanging += new EventHandler<ChangedEventArgs<int>>(OnSelectedIndexChanging);
            navigation.TransitionChange += new EventHandler<AnimationEventArgs>(OnTransitionChange);
            navigation.TransitionCompleted += new EventHandler<AnimationEventArgs>(OnTransitionCompleted);
            navigation.BeginTransition += new EventHandler<ChangedEventArgs<int>>(OnBeginTransition);
            header.BackButton.Click += new EventHandler(OnBackButtonClick);
        }

        const int animDuration = 275;

        /// <summary>
        /// Occurs when the back button is clicked,
        /// </summary>
        protected virtual void OnBackButtonClick(object sender, EventArgs e)
        {
            OnNavigateBack();
        }


        /// <summary>
        /// Navigate back to the previous page.
        /// </summary>
        public void GoBack()
        {
            OnNavigateBack();
        }

        protected virtual void OnNavigateBack()
        {
            if (NavigateBack != null) NavigateBack(this, EventArgs.Empty);
            int index = SelectedIndex - 1;
            if (index >= 0)
            {
                StopNestedAnimations();
                back = true;
                BeforeNavigate(index, back);
                navigation.Switch(index, Transition.ShiftRight, animDuration);
            }
        }

        public event EventHandler NavigateBack;

        public void Forward()
        {
            int index = SelectedIndex + 1;
            if (index < pages.Count)
            {
                StopNestedAnimations();
                back = false;
                BeforeNavigate(index, false);
                navigation.Switch(index, Transition.ShiftLeft, animDuration);
            }
        }

        /// <summary>
        /// Occurs before an animated transition starts.
        /// </summary>
        /// <param name="index">The target index to transit to</param>
        /// <param name="back">True, if it is a back navigation, otherwise false.</param>
        protected virtual void BeforeNavigate(int targetIndex, bool back)
        {
        }

        private void Layout()
        {
            int w = Width;
            int h = Height;
            header.Bounds = new Rectangle(0, 0, w, header.Height);
            navigation.Bounds = new Rectangle(0, header.Height, w, h - header.Height);
        }

        /// <summary>
        /// Occurs when the transition begins.
        /// </summary>
        protected virtual void OnBeginTransition(object sender, ChangedEventArgs<int> e)
        {
            inTransition = true;
            int index = e.NewValue;
            transitionTabItem = pages[e.NewValue];
            string title = Title;
            header.AnimLabel.Text = back ? title : transitionTabItem.Title;
            Title = back ? transitionTabItem.Title : title;
        }

#if SUPPORT_ALPHA
            ButtonGroup bg = EnsureAnimButtons();
            bg.BeginInit();
            bg.Buttons.Clear();
            foreach (FluidButton b in tabs[index].Buttons) bg.Buttons.Add(b);
            bg.EndInit();
            FluidButton back = EnsureAnimBackButton();
            back.Alpha = 0;
            back.Visible = index > 0;
            back.Text = index > 0 ? tabs[index - 1].Title : "";
            bg.Visible = bg.Buttons.Count > 0;
            bg.Alpha = 0;


        }

        private ButtonGroup EnsureAnimButtons()
        {
            if (header.AnimButtons == null)
            {
                header.AnimButtons = new ButtonGroup();
            }
            return header.AnimButtons;
        }

        private FluidButton EnsureAnimBackButton()
        {
            if (header.AnimBackButton == null)
            {
                FluidButton back = header.BackButton;
                FluidButton btn = new FluidButton();
                btn.Shape = ButtonShape.Back;
                btn.Visible = false;
                btn.Font = back.Font;
                btn.ForeColor = back.ForeColor;
                btn.BackColor = back.BackColor;
                header.AnimBackButton = btn;
                btn.Bounds = header.BackButton.Bounds;
                animBackButton = btn;
            }
            return animBackButton;
        }

#endif

        private PageControl transitionTabItem;

        /// <summary>
        /// Occurs when the transition has completed.
        /// </summary>
       protected virtual void OnTransitionCompleted(object sender, AnimationEventArgs e)
        {
            header.BeginInit();
#if SUPPORT_ALPHA
            animBackButton.Visible = false;
            header.ButtonsAlpha = 255;
            header.AnimButtons.Visible = false;
            header.BackButton.Alpha = 255;
            header.AnimButtons.Alpha = 0;
#endif
            header.AnimLabel.Visible = false;
            header.AnimOffset = 0;
            header.EndInit();
            header.Refresh();
            inTransition = false;
        }

        /// <summary>
        /// Occurs when the transition changes.
        /// </summary>
        protected virtual void OnTransitionChange(object sender, AnimationEventArgs e)
        {
            header.AnimOffset = back ? e.EndValue - e.Value : e.Value;
            header.AnimLabel.Visible = true;
#if SUPPORT_ALPHA
            int alpha2 = (255 * e.Value / e.EndValue);

            int alpha = 255 - alpha2;
            header.BackButton.Alpha = alpha;
            header.ButtonsAlpha = alpha;
            header.AnimButtons.Alpha = alpha2;
            header.AnimBackButton.Alpha = alpha2;
        private FluidButton animBackButton;
#endif
        }

        private bool back = false;

        private int minBackIndex = 0;

        public int MinBackIndex
        {
            get { return minBackIndex; }
            set
            {
                value = CheckBoundsMinBackIndex(value);
                if (minBackIndex != value)
                {
                    minBackIndex = value;
                    SetHeaderIndex(SelectedIndex);

                }
            }
        }

        private int CheckBoundsMinBackIndex(int value)
        {
            if (value < 0) return 0;
            return value;
        }

        private void OnSelectedIndexChanging(object sender, ChangedEventArgs<int> e)
        {
            SetHeaderIndex(e.NewValue);
            OnSelectedIndexChanging(e);
        }

        /// <summary>
        /// Occurs when the selected index is changing.
        /// </summary>
        protected virtual void OnSelectedIndexChanging(ChangedEventArgs<int> e)
        {
            if (Initializing) return;
            int index = e.NewValue;
            if (e.NewValue >= 0)
            {
                FluidControl c = Pages[index].Control;
                c.Invalidate();
                c.Focus();
            }
        }

        private void SetHeaderIndex(int index)
        {
            if (index < 0 || index >= pages.Count) return;
            PageControl item = pages[index];
            Title = item.Title;

            if (item.BackButton != null)
            {
                header.BackButton = item.BackButton;
            }
            else
            {
                header.SetDefaultBackButton();
            }

            Buttons.RaiseListChangedEvents = false;
            Buttons.Clear();
            foreach (FluidButton b in item.Buttons) Buttons.Add(b);
            Buttons.Notify();
            header.ButtonsWidth = item.Buttons.Width;

            if (header.IsDefaultBackButton)
            {
                if (index > minBackIndex)
                {
                    PageControl parent = pages[index - 1];
                    BackButton.Visible = true;
                    BackButton.Text = parent.Title;

                    using (Graphics g = CreateGraphics())
                    {
                        if (g != null)
                        {
                            SizeF size = g.MeasureString(parent.Title, BackButton.Font);
                            int w = (int)size.Width + ScaleX(8);

                            BackButton.Width = Math.Max(w, ScaleX(48));
                        }
                    }

                }
                else
                {
                    BackButton.Visible = false;
                }
            }
        }

        public override void Dispose()
        {
            header.Dispose();
            navigation.Dispose();
            if (dbuffer != null) dbuffer.Dispose();
            base.Dispose();
        }

        public ButtonCollection Buttons { get { return header.Buttons; } }

        /// <summary>
        /// Gets the current BackButton.
        /// </summary>
        public FluidButton BackButton { get { return header.BackButton; } }

        /// <summary>
        /// Gets the width of the current back button.
        /// </summary>
        public int ButtonsWidth
        {
            get { return header.ButtonsWidth; }
            set { header.ButtonsWidth = value; }
        }

        /// <summary>
        /// Gets the corner radius for  the header.
        /// </summary>
        public RoundedCorners Corners
        {
            get { return header.Corners; }
            set { header.Corners = value; }
        }

        /// <summary>
        /// Gets or sets the Title for the header.
        /// </summary>
        internal string Title
        {
            get { return header.Title; }
            set { header.Title = value; }
        }

        private PageCollection pages;

        /// <summary>
        /// Gets the collection of PageControls.
        /// </summary>
        public PageCollection Pages
        {
            get { return pages; }
        }

        /// <summary>
        /// Gets or sets the index of the selected PageControl
        /// </summary>
        public int SelectedIndex
        {
            get { return navigation.SelectedIndex; }
            set { navigation.SelectedIndex = value; }
        }

        /// <summary>
        /// Occurs when the selected index is changed.
        /// </summary>
        public event EventHandler<ChangedEventArgs<int>> SelectedIndexChanged
        {
            add { navigation.SelectedIndexChanging += value; }
            remove { navigation.SelectedIndexChanging -= value; }
        }

        /// <summary>
        /// Gets or sets the selected  PageControl.
        /// </summary>
        public PageControl SelectedPage
        {
            get { return navigation.SelectedControl as PageControl; }
            set { navigation.SelectedControl = value; }
        }

        internal FluidControlCollection Controls { get { return navigation.Controls; } }


        public void Switch(int index, Transition transition, int duration)
        {
            navigation.Switch(index, transition, duration);
        }

        protected override void OnPaintBackground(FluidPaintEventArgs e)
        {
            //  do nothing
        }

        private DoubleBuffer dbuffer;

        DoubleBuffer EnsureDBuffer()
        {
            if (dbuffer == null) dbuffer = new DoubleBuffer();
            return dbuffer;
        }

        private bool inTransition = false;

        public override void OnPaint(FluidPaintEventArgs e)
        {
            if (inTransition)
            {
                DoubleBuffer buffer = EnsureDBuffer();
                buffer.Paint(e, this, base.OnPaint, 255);

            }
            else
            {
                base.OnPaint(e);
            }
        }

        public override void Invalidate(Rectangle bounds)
        {
            if (dbuffer != null) dbuffer.Invalidate(bounds);
            base.Invalidate(bounds);
        }

        /// <summary>
        /// Occurs when the size has changed.
        /// </summary>
        protected override void OnSizeChanged(Size oldSize, Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);
            Layout();
        }

        /// <summary>
        /// Paints the client controls.
        /// </summary>
        protected override void PaintControls(FluidPaintEventArgs pe)
        {
            PaintControl(pe, header);
            PaintControl(pe, navigation);
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        e.Handled = true;
                        OnNavigateBack();
                        break;

                }
            }
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Handles a gesture.
        /// </summary>
        public override void OnGesture(GestureEventArgs e)
        {
            if (!e.IsPressed)
            {
                switch (e.Gesture)
                {
                    case Gesture.Right:
                        e.Handled = true;
                        e.Gesture = Gesture.None;
                        OnNavigateBack();
                        break;
                }
            }
            base.OnGesture(e);
        }

        public override void Focus()
        {
            SelectedPage.Control.Focus();
        }
    }
}
