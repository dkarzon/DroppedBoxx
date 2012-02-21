using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using DroppedBoxx.Properties;
using System.Drawing;
using System.Windows.Forms;
using Fluid.Classes;

namespace DroppedBoxx
{
    public class TextInputPanel : FluidPanel
    {
        private FluidLabel lblTitle;
        private FluidTextBox txtData;

        private FluidButton btnOK;
        private FluidButton btnCancel;
        private FluidButton btnKeyboard;

        private ModalBackground modalBackground;

        public event EventHandler<DialogEventArgs> Result;
        private DialogEventArgs dialogEventArgs = new DialogEventArgs();

        public string Data
        {
            get
            {
                return txtData.Text;
            }
        }

        public TextInputPanel(string labelText, int x, int y, int w, int h) : base(x, y, w, h)
        {
            Bounds = new System.Drawing.Rectangle(x, y, w, h);

            lblTitle = new FluidLabel(labelText, 10, 5, w - 20, 30);
            lblTitle.ForeColor = Color.White;
            lblTitle.Anchor = AnchorBL;
            lblTitle.Font = Theme.Current.ModalFont;
            Controls.Add(lblTitle);

            txtData = new FluidTextBox("", 10, 25, w - 20, 30);
            txtData.ForeColor = Color.Black;
            txtData.Anchor = AnchorBL;
            txtData.Font = Theme.Current.ModalFont;
            Controls.Add(txtData);

            btnOK = new FluidButton("OK", 10, 65, 80, 32);
            btnOK.BackColor = Color.Green;
            btnOK.GradientFill = Theme.Current.ButtonsGradianted;
            btnOK.Font = Theme.Current.ButtonFont;
            btnOK.Anchor = AnchorBL;
            btnOK.Click += new EventHandler(btnOK_click);
            Controls.Add(btnOK);

            btnKeyboard = new FluidButton("", 100, 65, 40, 32);
            btnKeyboard.Image = Resources.keyboard;
            btnKeyboard.BackColor = Color.White;
            btnKeyboard.Click += new EventHandler(btnKeyboard_Click);
            btnKeyboard.Anchor = AnchorBL;
            Controls.Add(btnKeyboard);

            btnCancel = new FluidButton("Cancel", 150, 65, 80, 32);
            btnCancel.BackColor = Color.Red;
            btnCancel.GradientFill = Theme.Current.ButtonsGradianted;
            btnCancel.Anchor = AnchorBL;
            btnCancel.Click += new EventHandler(btnCancel_click);
            Controls.Add(btnCancel);

            modalBackground = new ModalBackground();
            modalBackground.ShowMaximized();
            Anchor = AnchorAll;

            Show();
        }

        protected override void InitControl()
        {
            base.InitControl();
            BackColor = Color.Black;
            GradientFill = true;
            GradientFillOffset = 30;
        }

        void btnKeyboard_Click(object sender, EventArgs e)
        {
            Form1.Instance.InputPanel.Enabled = !Form1.Instance.InputPanel.Enabled;
        }

        protected override void OnSizeChanged(System.Drawing.Size oldSize, System.Drawing.Size newSize)
        {
            //base.OnSizeChanged(oldSize, newSize);

            Bounds = new Rectangle(0, 40, newSize.Width, 100);

            if (txtData != null) txtData.Bounds = new Rectangle(10, 25, newSize.Width - 20, 30);
        }

        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            switch (e.KeyChar)
            {

                case '\r':
                    e.Handled = true;
                    OnResult(DialogResult.OK);
                    break;
            }
        }

        protected void btnOK_click(object sender, EventArgs e)
        {
            Close();
            OnResult(DialogResult.OK);
        }

        protected void btnCancel_click(object sender, EventArgs e)
        {
            Close();
            OnResult(DialogResult.Cancel);
        }

        protected virtual void OnResult(DialogResult result)
        {
            if (Result != null)
            {
                dialogEventArgs.Result = result;
                Result(this, dialogEventArgs);
            }
        }

        protected override void OnClosing()
        {
            modalBackground.Close();
            base.OnClosing();
        }


    }
}
