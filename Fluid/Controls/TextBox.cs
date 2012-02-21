using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Fluid.Native;
using Microsoft.WindowsCE.Forms;
using System.Diagnostics;

namespace Fluid.Controls
{
    public class FluidTextBox : FluidLabel
    {
        public FluidTextBox(string text, int x, int y, int w, int h)
            : base(text, x, y, w, h)
        {
            CanShowInputPanel = true;
            LineAlignment = StringAlignment.Center;
        }

        protected override void InitControl()
        {
            base.InitControl();
            BackColor = Color.White;
        }
        private bool focused = false;

        /// <summary>
        /// Gets or sets whether the TextBox is focused.
        /// </summary>
        public bool Focused
        {
            get { return focused; }
            set
            {
                if (!allowEdit) value = false;
                if (focused != value)
                {
                    focused = value;
                    OnFocusChanged();
                }
            }
        }

        private bool allowEdit = true;

        public bool AllowEdit
        {
            get { return allowEdit; }
            set
            {
                if (allowEdit != value)
                {
                    allowEdit = value;
                    Leave();
                    Invalidate();
                }
            }
        }


        /// <summary>
        /// Occurs when the IsFocused property has changed,
        /// </summary>
        protected virtual void OnFocusChanged()
        {
            if (!focused)
            {
                if (Container != null && Container.Host != null && Container.Host.FocusedControl == this)
                {
                    Container.Host.FocusedControl = null;
                }
                Select(startSel, selCount);
                OnLostFocus();
            }
            else
            {
                Container.Host.FocusedControl = this;
                OnGotFocus();
            }
            if (FocusChanged != null) FocusChanged(this, EventArgs.Empty);
            Invalidate();
        }

        protected virtual void OnLostFocus()
        {
            if (LostFocus != null) LostFocus(this, EventArgs.Empty);
        }


        public event EventHandler FocusChanged;
        public event EventHandler LostFocus;

        static InputPanel inputPanel;

        public static InputPanel InputPanel
        {
            get
            {
                if (inputPanel == null) inputPanel = new InputPanel();
                return inputPanel;
            }
        }

        public bool CanShowInputPanel { get; set; }

        public override void OnEnter(IHost host)
        {
            base.OnEnter(host);
            Focused = true;
            if (CanShowInputPanel)
            {
                if (FluidHost.Instance.IsVertical)
                {
                    InputPanel.Enabled = true;
                }
            }
        }

        public override void OnLeave(IHost host)
        {
            base.OnLeave(host);
            Focused = false;

            //do not enable the following, otherwise in a listbox the input panel would be disabled, when
            //an item with a textbox is clicked again, since the item first sets itself to the selected item:
            InputPanel.Enabled = false;
        }

        private Color borderColor = Color.Black;

        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                if (borderColor != value)
                {
                    borderColor = value;
                    OnBorderColorChanged();
                }
            }
        }

        private bool showBorder = true;
        public bool ShowBorder
        {
            get { return showBorder; }
            set
            {
                if (showBorder != value)
                {
                    showBorder = value;
                    Invalidate();
                }
            }
        }

        protected virtual void OnBorderColorChanged()
        {
            Invalidate();
        }

        protected override void OnPaintBackground(FluidPaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (showBorder && !BorderColor.IsEmpty)
            {
                Rectangle r = e.ControlBounds;
                r.Width--;
                r.Height--;
                e.Graphics.DrawRectangle(Pens.GetPen(BorderColor), r);
            }
        }

        private int startSel;
        private int selCount;
        protected volatile bool showCursor = true;


        private int EnsureStartSel(int value)
        {
            if (value < 0) return 0;
            string text = Text ?? string.Empty;
            if (value > text.Length) return text.Length;
            return value;
        }

        protected virtual void OnBind()
        {
            if (Bind != null) Bind(this, EventArgs.Empty);
        }

        public event EventHandler Bind;

        public override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            OnBind();
            if (!e.Handled)
            {
                e.Handled = true;
                switch (e.KeyCode)
                {
                    case Keys.Left: LeftKey(); break;
                    case Keys.Right: RightKey(); break;
                    case Keys.Home: HomeKey(); break;
                    case Keys.End: EndKey(); break;
                    case Keys.Delete: Delete(); break;
                    default: e.Handled = false; break;
                }
            }
            base.OnKeyDown(e);
        }

        private void EndKey()
        {
            Select(0, Text.Length);
        }

        private void HomeKey()
        {
            Select(0, 0);
        }

        private void RightKey()
        {
            int w = Math.Max(1, selCount);
            Select(startSel + w, 0);
        }

        private void LeftKey()
        {
            Select(startSel - 1, 0);
        }

        private void Delete()
        {
            if (startSel < Text.Length)
            {
                Text = Text.Remove(startSel, selCount > 0 ? selCount : 1);
                Select(startSel, 0);
            }
        }

        public override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (!e.Handled)
            {
                OnBind();
                char c = e.KeyChar;
                switch (c)
                {
                    case '\r': EnterKey(); e.Handled = false; break;
                    case '\x1b': EscapeKey(); break;
                    case '\t': TabKey(); break;
                    case '\b': Backspace(); break;
                    default: InsertChar(c); e.Handled = true; break;
                }
            }
        }


        private void EnterKey()
        {

        }

        private void TabKey()
        {

        }

        private void EscapeKey()
        {
            Leave();
        }

        private void Backspace()
        {
            if (selCount > 0) Delete();
            else
            {
                if (startSel > 0)
                {
                    int start = EnsureStartSel(startSel - 1);
                    Text = Text.Remove(start, selCount + 1);
                    Select(start, 0);
                }
            }
        }

        public void InsertChar(char c)
        {
            if (!Enabled) return;
            string text = Text;
            int startSel = this.startSel;
            if (selCount > 0) text = text.Remove(startSel, selCount);
            Text = text.Insert(startSel, c.ToString());
            Select(startSel + 1, 0);
        }

        protected override void PaintText(FluidPaintEventArgs e)
        {
            if (focused)
            {
                Graphics g = e.Graphics;
                if (SelectBounds.IsEmpty) SelectBounds = GetSelectBounds(startSel, selCount);
                Rectangle bounds = e.ControlBounds;
                Rectangle selRect = SelectBounds;
                selRect.Offset(e.ScaleX(3), 0);
                selRect.Offset(bounds.X, bounds.Y);
                if (selRect.Width > 1)
                {
                    g.FillRectangle(Brushes.GetBrush(Color.SkyBlue), selRect);
                }
                else
                {
                    selRect.Width = 1;
                    g.FillRectangle(Brushes.GetBrush(Color.Black), selRect);
                }
            }

            string text = GetDisplayText();
            if (!string.IsNullOrEmpty(text))
            {
                PaintDefaultText(e, text);
            }
        }

        private Rectangle SelectBounds;

        public void Select(int startSel, int selCount)
        {
            if (!IsAttached) return;
            string text = Text ?? string.Empty;
            startSel = EnsureStartSel(startSel);
            int max = selCount + startSel;
            if (max > text.Length) selCount = max - startSel;
            if (this.startSel != startSel || this.selCount != selCount)
            {
                this.startSel = startSel;
                this.selCount = selCount;
                showCursor = true;

                SelectBounds = GetSelectBounds(startSel, selCount);
                Invalidate();
            }
        }

        private Rectangle GetSelectBounds(int startSel, int selCount)
        {
            string text = GetDisplayText() ?? string.Empty;
            using (Graphics g = CreateGraphics())
            {
                int fontHeight = (int)(g.MeasureString("X", Font).Height);

                string leftStr = text.Substring(0, startSel);

                float f_leftWidth = !string.IsNullOrEmpty(leftStr) ? g.MeasureString(leftStr, Font).Width : 0f;
                float f_allWidth = !string.IsNullOrEmpty(text) ? g.MeasureString(text, Font).Width : 0f;
                int selLeft = (int)f_leftWidth;
                int right = (int)f_allWidth;

                int width = 1;
                if (selCount > 0)
                {
                    int rightSel = startSel + selCount;
                    if (rightSel > text.Length) selCount = text.Length - startSel;
                    string rightStr = text.Substring(startSel, selCount);
                    float f_selWidth = !string.IsNullOrEmpty(rightStr) ? g.MeasureString(rightStr, Font).Width : 0f;
                    width = (int)f_selWidth;
                }
                Rectangle r = new Rectangle(selLeft, 0, width, fontHeight);
                switch (LineAlignment)
                {
                    case StringAlignment.Center:
                        r.Y += (Height - r.Height) / 2;
                        break;

                    case StringAlignment.Far:
                        r.Y += (Height - r.Height);
                        break;
                }
                return r;
            }
        }

        private string GetDisplayText()
        {
            return PasswordChar == '\0' ? Text : new string(PasswordChar, Text.Length);
        }


        public override bool Selectable { get { return allowEdit; } }
        public override bool Active { get { return true; } }

        int downIndex;

        public override void OnDown(PointEventArgs e)
        {
            base.OnDown(e);
            int index = IndexFromPoint(e);
            downIndex = index;
            if (index == startSel)
            {
                Select(0, Text.Length);
            }
            else
            {
                Select(index, 0);
            }
        }

        public override void OnMove(PointEventArgs e)
        {
            base.OnMove(e);
            int index = IndexFromPoint(e);

            if (index != downIndex)
            {
                int idx0 = Math.Min(index, downIndex);
                int idx1 = Math.Max(index, downIndex);
                int selCount = idx1 - idx0;
                int startSel = idx0;
                if (this.selCount != selCount || this.startSel != startSel)
                {
                    Select(startSel, selCount);
                }
            }
        }

        private int IndexFromPoint(PointEventArgs e)
        {
            int index = 0;
            if (Text.Length > 0)
            {
                using (Graphics g = CreateGraphics())
                {
                    if (g != null)
                    {
                        Rectangle r = ClientRectangle;
                        r.Inflate(ScaleX(-3), 0);
                        int left = r.Left;
                        int x = e.X;
                        for (int i = 1; i <= Text.Length; i++)
                        {
                            string s = Text.Substring(0, i);
                            int w = (int)g.MeasureString(s, Font).Width;
                            if ((w + left) >= x) break;
                            index++;
                        }
                    }
                }
            }
            index = EnsureStartSel(index);
            return index;
        }

        /// <summary>
        /// Cancel a left and right gesture to allow selection:
        /// </summary>
        public override void OnGesture(GestureEventArgs e)
        {
            base.OnGesture(e);
            if (Focused)
            {
                //  if (!e.Handled)   // always check for left and right and cancel even if it was already handled.
                {
                    switch (e.Gesture)
                    {
                        case Gesture.Left:
                        case Gesture.Right:
                            e.Handled = true;
                            e.Gesture = Gesture.None;
                            break;
                    }
                }
            }
        }

        public char PasswordChar { get; set; }

        public int SelStart { get { return startSel; } }
        public int SelCount { get { return selCount; } }

        public void SelectAll()
        {
            string text = Text ?? string.Empty;
            Select(0, text.Length);
        }

        protected override void OnTextChanged()
        {
            base.OnTextChanged();
            string text = Text ?? string.Empty;
            Select(text.Length, 0);
        }

    }
}
