using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;
using System.Windows.Forms;
using Fluid.Classes;
using DroppedBoxx.Properties;
using DroppedBoxx.Code;
using System.Drawing.Imaging;
using Microsoft.WindowsCE.Forms;
using DroppedBoxx.Code.Exceptions;

namespace DroppedBoxx
{
    public class LoginPanel : FluidPanel
    {

        public LoginPanel(int x, int y, int w, int h)
            : base(x, y, w, h)
        {
        }

        public event EventHandler Enter;

        private FluidTextBox txtEmail;
        private FluidLabel lblEmail;
        private FluidTextBox txtPassword;
        private FluidLabel lblPassword;
        private FluidButton btnLogin;
        private FluidButton btnKeyboard;
        private FluidButton btnExit;

        //Remember me
        private FluidButton btnRemember;
        private FluidLabel lblRemember;
        private bool rememberMe;

        protected override void InitControl()
        {
            base.InitControl();
            EnableDoubleBuffer = true;
            Bounds = new Rectangle(0, 0, 240, 300);
            this.BackColor = Color.FromArgb(33, 124, 197);
            this.PaintBackground += new EventHandler<FluidPaintEventArgs>(login_Paint);

            txtEmail = AddTextLabel("Email", 35, ref lblEmail, true);
            txtPassword = AddTextLabel("Password", 90, ref lblPassword, true);
            txtPassword.PasswordChar = '*';

            txtEmail.GotFocus += new EventHandler(passwordTextBox_GotFocus);
            txtPassword.GotFocus += new EventHandler(passwordTextBox_GotFocus);

            btnRemember = new FluidButton("", 12, 150, 20, 20);
            btnRemember.BackColor = Color.White;
            btnRemember.PressedBackColor = Color.White;
            btnRemember.Image = null;
            btnRemember.Click += new EventHandler(btnRemember_Click);
            Controls.Add(btnRemember);

            lblRemember = new FluidLabel("Remember me?", 38, 150, 120, 20);
            lblRemember.Click += new EventHandler(btnRemember_Click);
            lblRemember.ForeColor = Color.Black;
            lblRemember.ShadowColor = Color.White;
            lblRemember.Font = new Font("Arial", 9, FontStyle.Bold);
            Controls.Add(lblRemember);

            btnLogin = new FluidButton("Login", 10, this.Height - 35, 80, 32);
            btnLogin.BackColor = Color.Green;
            btnLogin.Click += new EventHandler(btnLogin_Click);
            btnLogin.ForeColor = Color.Black;
            btnLogin.Anchor = AnchorBL;
            btnLogin.Font = Theme.Current.ButtonFont;
            Controls.Add(btnLogin);

            btnKeyboard = new FluidButton("", 100, this.Height - 35, 40, 32);
            btnKeyboard.Image = Resources.keyboard;
            btnKeyboard.BackColor = Color.White;
            btnKeyboard.Click += new EventHandler(btnKeyboard_Click);
            btnKeyboard.Anchor = AnchorBL;
            btnKeyboard.ShadowColor = Color.Black;
            Controls.Add(btnKeyboard);

            btnExit = new FluidButton("Exit", 150, this.Height - 35, 80, 32);
            btnExit.BackColor = Color.Red;
            btnExit.Click += new EventHandler(btnExit_Click);
            btnExit.ForeColor = Color.Black;
            btnExit.Anchor = AnchorBL;
            btnExit.Font = Theme.Current.ButtonFont;
            Controls.Add(btnExit);

            if (Settings.Instance != null)
            {
                if (Settings.Instance.RememberMe)
                {
                    ////not too sure what to do here...
                    //txtEmail.Text = Settings.Instance.UserEmail;
                    //txtPassword.Text = Settings.Instance.UserPassword;
                    rememberMe = true;
                    btnRemember.Image = Resources.accept;
                }
            }
        }

        void login_Paint(object sender, FluidPaintEventArgs e)
        {
            Image image = Resources.Background;

            var w = this.Width;
            var h = image.Height * w / image.Width;

            Rectangle rect = new Rectangle(0, 0, w, h);
            
            Graphics g = e.Graphics;

            ImageAttributes ia = new ImageAttributes();
            ia.SetColorKey(Color.Fuchsia, Color.Fuchsia);

            var bgBrush = new SolidBrush(BackColor);

            g.FillRectangle(bgBrush, 0, 0, this.Width, this.Height);

            g.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
        }

        void btnLogin_Click(object sender, EventArgs e)
        {
            OnEnter();
        }

        void btnKeyboard_Click(object sender, EventArgs e)
        {
            Form1.Instance.InputPanel.Enabled = !Form1.Instance.InputPanel.Enabled;
        }

        void btnExit_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }

        void btnRemember_Click(object sender, EventArgs e)
        {
            if (rememberMe)
            {
                rememberMe = false;
                btnRemember.Image = null;
            }
            else
            {
                rememberMe = true;
                btnRemember.Image = Resources.accept;
            }
        }

        private FluidTextBox selectedTextBox;

        public FluidTextBox SelectedTextBox
        {
            get { return selectedTextBox; }
            set
            {
                selectedTextBox = value;
            }
        }

        void passwordTextBox_GotFocus(object sender, EventArgs e)
        {
            SelectedTextBox = (FluidTextBox)sender;

            if (SelectedTextBox == txtPassword)
            {
                //Disable T9?
                InputModeEditor.SetInputMode(Form1.Instance, InputMode.AlphaABC);
            }
            else
            {
                InputModeEditor.SetInputMode(Form1.Instance, InputMode.Default);
            }
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        private void ExitApplication()
        {
            this.Parent.BackColor = Color.Black;
            this.Close(ShowTransition.FromBottom);
            Form1.Instance.Close();
        }

        public override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            switch (e.KeyChar)
            {
                case '\t':
                    e.Handled = SelectNextTextBox();
                    break;

                case '\r':
                    e.Handled = true;
                    PerformEnter();
                    break;
            }
        }

        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        e.Handled = PerformKeyDown();
                        break;

                    case Keys.Up:
                        e.Handled = PerformKeyUp();
                        break;
                }
            }
        }

        private bool PerformKeyUp()
        {
            return true;
        }

        private bool PerformKeyDown()
        {
            return true;
        }

        private bool SelectNextTextBox()
        {
            if (selectedTextBox == txtEmail) SelectedTextBox = txtPassword;
            selectedTextBox.Focus();
            return true;
        }

        FluidTextBox AddTextLabel(string title, int top, ref FluidLabel label, bool visible)
        {
            label = new FluidLabel(title, 10, top + 5, Width - 20, 16);
            label.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            label.Font = new Font(FontFamily.GenericSansSerif, 8f, FontStyle.Regular);
            label.ForeColor = Color.White;
            label.ShadowColor = Color.Black;
            label.Visible = visible;
            Controls.Add(label);
            
            FluidTextBox textbox = new FluidTextBox("", 10, top + 19, Width - 20, 24);
            textbox.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            textbox.Visible = visible;
            textbox.CanShowInputPanel = false;
            Controls.Add(textbox);
            return textbox;
        }

        private void PerformEnter()
        {
            OnEnter();
        }

        protected virtual void OnEnter()
        {
            if (Enter != null)
            {
                if (string.IsNullOrEmpty(txtEmail.Text))
                {
                    ShowEmptyPasswordDialog();
                    SelectedTextBox = txtEmail;
                    txtEmail.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    ShowEmptyPasswordDialog();
                    SelectedTextBox = txtPassword;
                    txtPassword.Focus();
                    return;
                }
                Host.Cursor = Cursors.WaitCursor;
                try
                {
                    Form1.Instance.DropBox.Login(txtEmail.Text, txtPassword.Text);
                }
                catch (DropException ex)
                {
                    MessageDialog.Show(ex.Message, "", null);
                    return;
                }
                catch (Exception ex)
                {
                    MessageDialog.Show("Login Failed. \nPlease try again.", "OK", null);
                    return;
                }
                Host.Cursor = Cursors.Default;
                if (Form1.Instance.DropBox.LoggedIn)
                {
                    if (rememberMe)
                    {
                        Settings.Instance.UserToken = Form1.Instance.DropBox.UserLogin.Token;
                        Settings.Instance.UserSecret = Form1.Instance.DropBox.UserLogin.Secret;
                        Settings.Instance.RememberMe = true;
                    }
                    else
                    {
                        Settings.Instance.RememberMe = false;
                    }
                    Enter(this, EventArgs.Empty);
                }
                else
                {
                    ShowIncorrectPasswordDialog();
                }
            }
        }

        private void ShowEmptyPasswordDialog()
        {
            MessageDialog.Show("Please enter your Email and Password.", "OK", null);
        }

        private void ShowIncorrectPasswordDialog()
        {
            txtPassword.Text = "";
            SelectedTextBox = txtPassword;
            MessageDialog.Show("Login Failed. \nPlease try again.", "OK", null);
        }

        void dlg_Result(object sender, DialogEventArgs e)
        {
            if (e.Result == DialogResult.Cancel)
            {
                ExitApplication();
            }
        }

        protected override void OnSizeChanged(System.Drawing.Size oldSize, System.Drawing.Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);

            if (Controls.Count == 0) return;
            foreach (FluidControl c in Controls)
            {
                if (c.Anchor != AnchorStyles.None)
                {
                    c.Width = (int)(((float)c.Width / (float)oldSize.Width) * newSize.Width);
                    c.Left = (int)(((float)c.Left / (float)oldSize.Width) * newSize.Width);
                }
            }

        }

    }
}
