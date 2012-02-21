using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using DroppedBoxx.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace DroppedBoxx
{
    public class ButtonPanel : FluidPanel
    {
        //TODO - Make this generic to allow for different numbers of buttons and dynamically create them
        //also the ability to set the images

        public int ButtonWidth = 40;
        public int ButtonHeight = 35;
        const int ButtonTop = 0;
        const int Space = 7;
        public int LabelHeight = 12;

        FluidButton[] buttons = new FluidButton[5];

        public FluidButton[] Buttons { get { return buttons; } }

        FluidButton btn1;
        FluidButton btn2;
        FluidButton btn3;
        FluidButton btn4;
        FluidButton btn5;

        FluidLabel label1;
        FluidLabel label2;
        FluidLabel label3;
        FluidLabel label4;
        FluidLabel label5;

        protected override void InitControl()
        {
            Theme theme = Theme.Current;
            base.InitControl();
            Bounds = new System.Drawing.Rectangle(0, 0, 240, ButtonHeight + LabelHeight);
            BackColor = theme.ButtonsBackColor;
            GradientFill = false;

            buttons[0] = btn1 = new FluidButton("", Space, ButtonTop, ButtonWidth, ButtonHeight);
            buttons[1] = btn2 = new FluidButton("", Space + (Space + ButtonWidth) * 1, ButtonTop, ButtonWidth, ButtonHeight);
            buttons[2] = btn3 = new FluidButton("", Space + (Space + ButtonWidth) * 2, ButtonTop, ButtonWidth, ButtonHeight);
            buttons[3] = btn4 = new FluidButton("", Space + (Space + ButtonWidth) * 3, ButtonTop, ButtonWidth, ButtonHeight);
            buttons[4] = btn5 = new FluidButton("", Space + (Space + ButtonWidth) * 4, ButtonTop, ButtonWidth, ButtonHeight);

            //Sync Button
            buttons[0].Image = Resources.arrow_refresh;
            //Browse Dropbox Button
            buttons[1].Image = Resources.brick_go;
            //Settings Button
            buttons[2].Image = Resources.cog;
            //Info Button
            buttons[3].Image = Resources.information;
            //Info Button
            buttons[4].Image = Resources.door_in;

            btn1.Command = "A";
            btn2.Command = "B";
            btn3.Command = "C";
            btn4.Command = "D";
            btn5.Command = "E";

            foreach (FluidButton b in buttons)
            {
                b.Anchor = AnchorStyles.None;
                b.Shape = ButtonShape.Flat;
                b.BackColor = theme.ButtonsButtonBackColor;
                b.ForeColor = Color.White;
                b.PressedBackColor = Color.LightGray;
            }

            Controls.Add(btn1);
            Controls.Add(btn2);
            Controls.Add(btn3);
            Controls.Add(btn4);
            Controls.Add(btn5);

            //now add the labels
            var labelFont = new Font(FontFamily.GenericSerif, 7, FontStyle.Regular);

            label1 = new FluidLabel("Sync", Space + 8, ButtonHeight - 2, ButtonWidth - 6, LabelHeight);
            label1.Font = labelFont;

            label2 = new FluidLabel("Dropbox", Space + (Space + ButtonWidth) * 1 - 1, ButtonHeight - 2, ButtonWidth + 2, LabelHeight + 1);
            label2.Font = labelFont;

            label3 = new FluidLabel("Settings", Space + (Space + ButtonWidth) * 2, ButtonHeight - 2, ButtonWidth, LabelHeight + 1);
            label3.Font = labelFont;

            label4 = new FluidLabel("Info", Space + (Space + ButtonWidth) * 3 + 8, ButtonHeight - 2, ButtonWidth - 6, LabelHeight + 1);
            label4.Font = labelFont;

            label5 = new FluidLabel("Exit", Space + (Space + ButtonWidth) * 4 + 8, ButtonHeight - 2, ButtonWidth - 6, LabelHeight + 1);
            label5.Font = labelFont;

            Controls.Add(label1);
            Controls.Add(label2);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(label5);
        }

        protected override void OnSizeChanged(System.Drawing.Size oldSize, System.Drawing.Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);
            if (btn1 == null) return;

            int buttonWidth = ScaleX(ButtonWidth);
            int buttonHeight = ScaleY(ButtonHeight);
            int ButtonTop = ScaleY(ButtonPanel.ButtonTop);

            if (Width > Height)
            {
                int Space = ScaleX(ButtonPanel.Space);
                btn1.Bounds = new Rectangle(Space, ButtonTop, buttonWidth, buttonHeight);
                btn2.Bounds = new Rectangle(Space + (Space + buttonWidth) * 1, ButtonTop, buttonWidth, buttonHeight);
                btn3.Bounds = new Rectangle(Space + (Space + buttonWidth) * 2, ButtonTop, buttonWidth, buttonHeight);
                btn4.Bounds = new Rectangle(Space + (Space + buttonWidth) * 3, ButtonTop, buttonWidth, buttonHeight);
                btn5.Bounds = new Rectangle(Space + (Space + buttonWidth) * 4, ButtonTop, buttonWidth, buttonHeight);
            }
            else
            {
                int Space = ScaleY(ButtonPanel.Space);
                btn1.Bounds = new Rectangle(Space, ButtonTop, buttonWidth, buttonHeight);
                btn2.Bounds = new Rectangle(Space, ButtonTop + (ButtonTop + buttonHeight) * 1, buttonWidth, buttonHeight);
                btn3.Bounds = new Rectangle(Space, ButtonTop + (ButtonTop + buttonHeight) * 2, buttonWidth, buttonHeight);
                btn4.Bounds = new Rectangle(Space, ButtonTop + (ButtonTop + buttonHeight) * 3, buttonWidth, buttonHeight);
                btn5.Bounds = new Rectangle(Space, ButtonTop + (ButtonTop + buttonHeight) * 4, buttonWidth, buttonHeight);
            }
        }

    }
}
