using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Controls.Classes;
using System.Drawing;
using Fluid.Drawing;

namespace Fluid.Controls
{
    /// <summary>
    /// Groups buttons together.
    /// </summary>
    public class ButtonGroup : ControlContainer, ICommandContainer,ILayoutPanel
    {
        public ButtonGroup()
            : base()
        {
        }

        public ButtonGroup(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        protected override void InitControl()
        {
            buttons = new ButtonCollection(this);
            base.InitControl();
        }


        private RoundedCorners corners = RoundedCorners.All;

        /// <summary>
        /// Gets or sets the corner radius for the group.
        /// </summary>
        public RoundedCorners Corners
        {
            get { return corners; }
            set
            {
                if (corners != value)
                {
                    corners = value;
                    Render();
                }
            }
        }


        private ButtonCollection buttons;

        /// <summary>
        /// Gets the collection of Buttons inside this ButtonGroup.
        /// </summary>
        public ButtonCollection Buttons
        {
            get { return buttons; }
        }

        /// <summary>
        /// Adds a  button to the group.
        /// </summary>
        /// <param name="button">The Button to add.</param>
        public void Add(FluidButton button)
        {
            buttons.Add(button);
        }

        /// <summary>
        /// Removes a button at the specified index.
        /// </summary>
        /// <param name="index">The index of the button to remove.</param>
        public void RemoveAt(int index)
        {
            buttons.RemoveAt(index);
        }

        /// <summary>
        /// Clears all buttons from this group.
        /// </summary>
        public void Clear()
        {
            buttons.Clear();
        }

        private int buttonWidth = 0;

        /// <summary>
        /// Gets or sets the width for all but the last button. if set to 0, the width is determined automatically.
        /// </summary>
        public int ButtonWidth
        {
            get { return buttonWidth; }
            set
            {
                if (buttonWidth != value)
                {
                    buttonWidth = value;
                    Render();
                    Invalidate();
                }
            }
        }

        protected override void OnPaintBackground(FluidPaintEventArgs e)
        {
            // do nothing!
        }

        /// <summary>
        /// Renders the buttons;
        /// </summary>
        public void Render()
        {
            if (buttons == null) return;
            int c = buttons.Count;
            controls.Clear();
            int index = 0;
            int x = 0;
            int y = 0;
            int h = Bounds.Height;
            int w0 = c > 0 ? Bounds.Width / c : 0;

            if (buttonWidth > 0) w0 = Math.Min(buttonWidth, Width);

            c--;
            RoundedCorners corners = c == 0 ? RoundedCorners.All : RoundedCorners.Left;
            foreach (FluidButton btn in buttons)
            {
                btn.BeginInit();
                int w = (index != c) ? w0 : (Bounds.Width - x);
                if (btn.BackColor == Color.Transparent || btn.BackColor.IsEmpty)
                {
                    btn.BackColor = this.BackColor;
                }
                btn.Bounds = new Rectangle(x, y, w + ((index != c) ? (int)ScaleFactor.Width : 0), h);
                btn.ScaleFactor = ScaleFactor;
                btn.Corners = corners & Corners;
                btn.Command = index.ToString();
                //btn.Alpha = alpha;
                btn.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
                btn.EndInit();
                index++;
                controls.Add(btn);
                x += w;
                corners = index == c ? RoundedCorners.Right : RoundedCorners.None;
            }
            Invalidate();
            OnButtonsChanged();
        }

        private int alpha = 255;

        /// <summary>
        /// Gets the alpha factor for the group.
        /// This can be a range between 0 and 255, with 0 as completely transparent and 255 as opaque.
        /// </summary>
        public int Alpha
        {
            get { return alpha; }
            set
            {
                if (alpha != value)
                {
                    alpha = value;
                    ModifyAlpha(alpha);
                }
            }
        }

        private void ModifyAlpha(int alpha)
        {
            foreach (FluidControl c in controls)
            {
                FluidButton btn = (FluidButton)c;
                btn.Alpha = alpha;
            }
        }

        /// <summary>
        /// Occurs when the collection of  buttons has changed.
        /// </summary>
        protected virtual void OnButtonsChanged()
        {
            if (ButtonsChanged != null) ButtonsChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the collection of  buttons has changed.
        /// </summary>
        public event EventHandler ButtonsChanged;


        private ButtonGroupEventArgs clickEvent;
        
        /// <summary>
        /// Raises a command.
        /// </summary>
        public override void RaiseCommand(CommandEventArgs e)
        {
            // base.RaiseCommand(e); // do not fire to parent containers!
            if (ButtonClick != null)
            {
                if (clickEvent == null) clickEvent = new ButtonGroupEventArgs();
                int index = int.Parse(e.Command);
                clickEvent.Index = index;
                clickEvent.Button = buttons[index];
                ButtonClick(this, clickEvent);
            }

        }

        /// <summary>
        /// Occurs when the size has changed,
        /// </summary>
        protected override void OnSizeChanged(Size oldSize, Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);
            if (!oldSize.IsEmpty)
            {
                Render();
            }
        }

        /// <summary>
        /// Occurs when a button of the collection is clicked.
        /// </summary>
        public event EventHandler<ButtonGroupEventArgs> ButtonClick;

        /// <summary>
        /// Scales the ButtonGroup.
        /// </summary>
        public override void Scale(SizeF scaleFactor)
        {
            base.Scale(scaleFactor);
            ButtonWidth = (int)(ButtonWidth * scaleFactor.Width);
        }
    }
}
