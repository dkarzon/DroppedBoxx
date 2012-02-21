using System;

using System.Collections.Generic;
using System.Text;

namespace Fluid.Controls
{
    /// <summary>
    /// A control that represents a tab item for a tab control.
    /// </summary>
    public class PageControl : ControlContainer
    {
        public PageControl(string title)
            : base()
        {
            this.Title = title;
        }

        protected override void InitControl()
        {
            base.InitControl();
            if (Bounds.IsEmpty) bounds = new System.Drawing.Rectangle(0, 0, 240, 300);
            buttons = new ButtonCollection(null);
            buttons.ListChanged += new System.ComponentModel.ListChangedEventHandler(ButtonsListChanged);
        }

        void ButtonsListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {

        }

        private FluidControl control;

        /// <summary>
        /// Gets or sets the child control.
        /// </summary>
        public FluidControl Control
        {
            get { return control; }
            set
            {
                if (control != value)
                {
                    if (control != null) controls.Remove(control);
                    control = value;
                    if (value != null)
                    {
                        controls.Add(value);
                        value.Bounds = ClientRectangle;
                    }
                }
            }
        }

        private string title;

        /// <summary>
        /// Gets or sets the title of this page.
        /// This title becomes visible in the hosting NavigationPanel when this Page is selected.
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    OnTitleChanged();
                }
            }
        }

        private FluidButton backButton;

        /// <summary>
        /// Gets or sets the BackButton for this Page that appears in the hosting NavigationPanel when this Page is selected.
        /// If not button is specified, a default button is used for the NavigationPanel.
        /// </summary>
        public FluidButton BackButton
        {
            get { return backButton; }
            set { backButton = value; }
        }

        private ButtonCollection buttons;

        /// <summary>
        /// Gets or sets the Header Buttons on the right of the NavigationPanel that appear when this Page is selected.
        /// </summary>
        public ButtonCollection Buttons
        {
            get { return buttons; }
        }

        private void OnTitleChanged()
        {
            if (TitleChanged != null) TitleChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the Title is changed.
        /// </summary>
        public event EventHandler TitleChanged;

        /// <summary>
        /// Occurs when the Size is changed..
        /// </summary>
        protected override void OnSizeChanged(System.Drawing.Size oldSize, System.Drawing.Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);
            if (control != null) control.Bounds = ClientRectangle;
        }

        protected override void OnPaintBackground(FluidPaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        public override void Scale(System.Drawing.SizeF scaleFactor)
        {
            base.Scale(scaleFactor);
            Buttons.Width = (int)(Buttons.Width * scaleFactor.Width);
        }

        public override void Focus()
        {
            if (Control != null) Control.Focus();
        }

    }
}
