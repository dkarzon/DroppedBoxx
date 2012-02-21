using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Windows.Forms;
using System.Drawing;
using Fluid.Drawing;

namespace Fluid.Controls
{
    /// <summary>
    ///  A customizable numeric pad.
    /// </summary>
    public class NumericPad : FluidPanel
    {
        public NumericPad()
            : base()
        {
        }

        public NumericPad(int x, int y, int w, int h)
            : base(x, y, w, h)
        { }

        protected override void InitControl()
        {
            base.InitControl();
            EnableDoubleBuffer = false;
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            BackColor = Color.Black;
            ForeColor = Color.White;
            rows = new ButtonGroup[] 
            {
                new ButtonGroup(),
                new ButtonGroup(),
                new ButtonGroup(),
                new ButtonGroup(),
            };
            int index = 0;
            int buttonIndex = 0;
            foreach (ButtonGroup row in rows)
            {
                RoundedCorners corners;
                switch (index++)
                {
                    case 0: corners = RoundedCorners.Top; break;
                    case 3: corners = RoundedCorners.Bottom; break;
                    default: corners = RoundedCorners.None; break;
                }
                row.Anchor = AnchorStyles.None;
                row.ButtonWidth = ScaleX(54);
                //row.Buttons.RaiseListChangedEvents = false;
                for (int i = 0; i < 4; i++)
                {
                    row.Corners = corners;
                    FluidButton btn = new FlatButton(labels[buttonIndex]);
                    btn.Tag = buttonIndex;
                    btn.Command = i.ToString();
                    buttons[buttonIndex] = btn;
                    if (i != 3) btn.PaintButtonText += new EventHandler<FluidPaintEventArgs>(PaintInternalButtonText);
                    btn.Click += new EventHandler(InternalButtonClick);
                    // btn.ForeColor = ForeColor;
                    switch (buttonIndex)
                    {
                        //case 3: btn.BackColor = Color.FromArgb(48, 0, 0); break;
                        case 15: btn.BackColor = Color.FromArgb(0, 40, 0); break;
                        default: btn.BackColor = BackColor; break;
                    }
                    row.Buttons.Add(btn);
                    buttonIndex++;
                }
                //row.Buttons.RaiseListChangedEvents = true;
                //row.Render();
                controls.Add(row);
            }
            Layout();
        }

        enum ButtonType
        {
            Numeric,
            Del,
            Back,
            Special,
            Enter
        };

        ButtonType GetButtonType(int index)
        {
            switch (index)
            {
                case 3: return ButtonType.Del;
                case 7: return ButtonType.Back;
                case 11: return ButtonType.Special;
                case 15: return ButtonType.Enter;
                default: return ButtonType.Numeric;
            }
        }

        void InternalButtonClick(object sender, EventArgs e)
        {
            FluidButton btn = (FluidButton)sender;
            int index = (int)btn.Tag;
            ButtonType type = GetButtonType(index);

            if (OnButtonClick(index)) return;

            switch (type)
            {
                case ButtonType.Numeric:
                    string chr = btn.Text.Substring(0, 1);
                    OnInsertChar(chr);
                    string text = chr;
                    Text += text;
                    break;

                case ButtonType.Back:
                    if (Text.Length > 0) Text = Text.Substring(0, Text.Length - 1);
                    break;

                case ButtonType.Del:
                    Text = "";
                    break;

                case ButtonType.Special:
                    OnSpecialKeyClick();
                    break;

                case ButtonType.Enter:
                    OnEnter();
                    break;
            }
        }

        private void OnInsertChar(string chr)
        {
            if (InsertChar != null)
            {
                KeyPressEventArgs e = new KeyPressEventArgs(chr[0]);
                InsertChar(this, e);
            }
        }

        /// <summary>
        /// Occurs when one of the keys 0...9 was tapped.
        /// </summary>
        public event KeyPressEventHandler InsertChar;

        private NumericPanelEventArgs clickEvent = new NumericPanelEventArgs();

        /// <summary>
        /// Occurs when a button was clicked.
        /// </summary>
        protected virtual bool OnButtonClick(int index)
        {
            if (ButtonClick != null)
            {
                clickEvent.Handled = false;
                clickEvent.ButtonIndex = index;
                ButtonClick(this, clickEvent);
                return clickEvent.Handled;
            }
            return false;
        }

        /// <summary>
        /// Occurs when a button was clicked.
        /// </summary>
        public event EventHandler<NumericPanelEventArgs> ButtonClick;

        private RoundedCorners corners = RoundedCorners.All;

        /// <summary>
        /// Gets or sets the corner radius this control
        /// </summary>
        public RoundedCorners Corners
        {
            get { return corners; }
            set
            {
                if (corners != value)
                {
                    corners = value;
                    Layout();
                }
            }
        }

        /// <summary>
        /// Occurs when the enter button is clicked,
        /// </summary>
        protected virtual void OnEnter()
        {
            if (Enter != null) Enter(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when one of the 4 special button is clicked,
        /// </summary>
        protected virtual void OnSpecialKeyClick()
        {
            if (SpecialKeyClick != null) SpecialKeyClick(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when one of the 4 special buttons is clicked,
        /// </summary>
        public event EventHandler SpecialKeyClick;

        /// <summary>
        /// Occurs when the enter button is clicked,
        /// </summary>
        public event EventHandler Enter;

        private FluidButton[] buttons = new FluidButton[16];
        private int buttonHeight = 50;

        /// <summary>
        /// Gets or sets the height of each buttons.
        /// </summary>
        public int ButtonHeight
        {
            get { return buttonHeight; }
            set
            {
                if (buttonHeight != value)
                {
                    buttonHeight = value;
                    Layout();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets all the buttons.
        /// </summary>
        public FluidButton[] Buttons { get { return buttons; } }

        readonly string[] labels = new string[] {
            "1 ","2abc","3def","Clr",
            "4ghi","5jkl","6mno","Back",
            "7pqrs","8tuv","9wxyz","",
            "* ","0+","#_","Enter"};

        static Font bigFont = new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Bold);
        static Font smallFont = new Font(FontFamily.GenericSansSerif, 7f, FontStyle.Regular);

        static SolidBrush textBrush = new SolidBrush(Color.White);
        static SolidBrush downTextbrush = new SolidBrush(Color.LightSteelBlue);
        static StringFormat stringFormat = new StringFormat(StringFormatFlags.NoWrap);



        void PaintInternalButtonText(object sender, FluidPaintEventArgs e)
        {
            FluidButton btn = (FluidButton)sender;

            string text = btn.Text;
            if (text.Length > 0)
            {
                Brush brush = IsDown ? downTextbrush : textBrush;
                string bigText = text.Substring(0, 1);
                string smallText = text.Substring(1);
                Graphics g = e.Graphics;
                Rectangle r = e.ControlBounds;
                if (IsDown) r.Offset(ScaleX(1), ScaleY(1));
                stringFormat.Alignment = StringAlignment.Near;
                stringFormat.LineAlignment = StringAlignment.Center;
                SizeF bigSize = g.MeasureString(bigText, bigFont);
                float smallWidth = string.IsNullOrEmpty(smallText) ? 0f : e.ScaleFactor.Width * 5f + g.MeasureString(smallText, smallFont).Width;
                int dw = (int)((btn.Width - smallWidth) / 2f);
                //int dh = (int)((btn.Height - bigSize.Height) / 2f);
                int dh = (int)(2f * e.ScaleFactor.Width);
                r.Inflate(-dw, -dh);
                RectangleF rf = RectFFromRect(r);
                g.DrawString(bigText, bigFont, brush, rf, stringFormat);

                if (!string.IsNullOrEmpty(smallText))
                {
                    float width = bigSize.Width + 4f * e.ScaleFactor.Width;
                    rf.X += width;
                    g.DrawString(smallText, smallFont, brush, rf, stringFormat);
                }
            }

        }

        private void Layout()
        {
            int w = this.Width;
            int maxH = this.Height / 4;
            int h = buttonHeight <= 0 || buttonHeight > maxH ? maxH : buttonHeight;
            int top = Height - h * 4;
            int index = 0;
            foreach (ButtonGroup row in rows)
            {
                RoundedCorners c;
                switch (index++)
                {
                    case 0: c = RoundedCorners.Top; break;
                    case 3: c = RoundedCorners.Bottom; break;
                    default: c = RoundedCorners.None; break;
                }
                row.Corners = c & corners;

                row.Bounds = new System.Drawing.Rectangle(0, top, w, h);
                top += h;
            }
        }

        protected override void OnSizeChanged(System.Drawing.Size oldSize, System.Drawing.Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);
            Layout();
        }

        protected override void OnPaintBackground(FluidPaintEventArgs e)
        {
        }

        private ButtonGroup[] rows;

        protected override void PaintControls(FluidPaintEventArgs pe)
        {
            base.PaintControls(pe);
        }

        public override void Scale(SizeF scaleFactor)
        {
            base.Scale(scaleFactor);
            ButtonHeight = (int)(scaleFactor.Height * buttonHeight);
        }

        private string text = "";

        /// <summary>
        /// Gets or sets the text to edit.
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;
                    OnTextChanged();
                }
            }
        }

        /// <summary>
        /// Occurs when the text is changed.
        /// </summary>
        protected virtual void OnTextChanged()
        {
            if (TextChanged != null) TextChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the text is changed.
        /// </summary>
        public event EventHandler TextChanged;

        public override bool Selectable { get { return true; } }

    }
}
