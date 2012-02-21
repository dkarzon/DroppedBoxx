using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Controls;
using System.Drawing;
using System.Windows.Forms;
using Fluid.Classes;
using DroppedBoxx.Properties;
using DroppedBoxx.Code.Responses;
using DroppedBoxx.Code;

namespace DroppedBoxx
{
    public class SettingsPanel : FluidPanel
    {
        private FluidHeader header = new FluidHeader();

        private FluidLabel lblFileSizeVal;
        private FluidLabel lblCameraVal;
        private FluidLabel lblThemeVal;

        protected override void InitControl()
        {
            base.InitControl();
            EnableDoubleBuffer = true;
            Anchor = AnchorAll;
            Bounds = new Rectangle(0, 0, 240, 300);
            BackColor = Theme.Current.PanelBackColor;
            GradientFill = Theme.Current.PanelGradinated;

            header.Title = "Settings";
            header.BackColor = Theme.Current.HeaderBackColor;
            header.ForeColor = Theme.Current.HeaderForeColor;
            header.GradientFill = Theme.Current.HeaderGradianted;
            header.BackButton.Click += new EventHandler(BackButton_Click);
            header.BackButton.BackColor = Theme.Current.HeaderBackButtonBackColor;
            header.BackButton.ForeColor = Theme.Current.HeaderBackButtonForeColor;
            header.BackButton.Visible = true;
            header.BackButton.Shape = ButtonShape.Back;
            header.BackButton.TextOffset = new Point(6, 0);
            header.BackButton.Text = "Back";
            header.BackButton.GradientFill = Theme.Current.ButtonsGradianted;
            Controls.Add(header);

            //Add the controls to the panel here...
            var lblLogin = new FluidLabel("Login", 10, 30, 60, 30);
            lblLogin.Font = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
            Controls.Add(lblLogin);

            var lblForgetMe = new FluidLabel("Clear Login Details: ", 10, 55, 100, 25);
            lblForgetMe.Font = new Font(FontFamily.GenericSerif, 8, FontStyle.Regular);
            Controls.Add(lblForgetMe);

            var btnForgetMe = new FluidButton("Forget Me", 110, 50, 60, 25);
            btnForgetMe.BackColor = Theme.Current.SettingsButtonBackColor;
            btnForgetMe.GradientFill = Theme.Current.ButtonsGradianted;
            btnForgetMe.ForeColor = Theme.Current.SettingsButtonForeColor;
            btnForgetMe.Click += new EventHandler(btnForgetMe_Click);
            btnForgetMe.Font = Theme.Current.ButtonFont;
            Controls.Add(btnForgetMe);

            var line1 = new FluidLine(0, 80, Width);
            line1.Anchor = AnchorLR;
            Controls.Add(line1);

            var lblSync = new FluidLabel("Syncing", 10, 80, 80, 30);
            lblSync.Font = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
            Controls.Add(lblSync);

            //File Size
            var lblFileSize = new FluidLabel("Max File Size: ", 10, 105, 80, 25);
            lblFileSize.Font = new Font(FontFamily.GenericSerif, 8, FontStyle.Regular);
            Controls.Add(lblFileSize);

            lblFileSizeVal = new FluidLabel(string.Empty, 80, 102, 60, 25);
            lblFileSizeVal.Font = new Font("Tahoma", 9, FontStyle.Bold);
            Controls.Add(lblFileSizeVal);
            ResetLabelSize();

            var btnSizeDown = new FluidButton("-", 140, 97, 24, 24);
            btnSizeDown.BackColor = Theme.Current.SettingsButtonBackColor;
            btnSizeDown.ForeColor = Theme.Current.SettingsButtonForeColor;
            btnSizeDown.GradientFill = Theme.Current.ButtonsGradianted;
            btnSizeDown.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnSizeDown.Click += new EventHandler(btnSizeDown_Click);
            Controls.Add(btnSizeDown);

            var btnSizeUp = new FluidButton("+", 170, 97, 24, 24);
            btnSizeUp.BackColor = Theme.Current.SettingsButtonBackColor;
            btnSizeUp.ForeColor = Theme.Current.SettingsButtonForeColor;
            btnSizeUp.GradientFill = Theme.Current.ButtonsGradianted;
            btnSizeUp.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnSizeUp.Click += new EventHandler(btnSizeUp_Click);
            Controls.Add(btnSizeUp);

            var line2 = new FluidLine(0, 125, Width);
            line2.Anchor = AnchorLR;
            Controls.Add(line2);

            var lblMisc = new FluidLabel("Misc", 10, 125, 80, 30);
            lblMisc.Font = new Font(FontFamily.GenericSerif, 12, FontStyle.Bold);
            Controls.Add(lblMisc);

            //Camera Res
            var lblCameraRes = new FluidLabel("Camera Res: ", 10, 145, 80, 25);
            lblCameraRes.Font = new Font(FontFamily.GenericSerif, 8, FontStyle.Regular);
            Controls.Add(lblCameraRes);

            lblCameraVal = new FluidLabel(string.Empty, 80, 142, 60, 25);
            lblCameraVal.Font = new Font("Tahoma", 9, FontStyle.Bold);
            Controls.Add(lblCameraVal);
            ResetLabelCamera();

            var btnCameraDown = new FluidButton("-", 140, 137, 24, 24);
            btnCameraDown.BackColor = Theme.Current.SettingsButtonBackColor;
            btnCameraDown.ForeColor = Theme.Current.SettingsButtonForeColor;
            btnCameraDown.GradientFill = Theme.Current.ButtonsGradianted;
            btnCameraDown.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnCameraDown.Click += new EventHandler(btnCameraDown_Click);
            Controls.Add(btnCameraDown);

            var btnCameraUp = new FluidButton("+", 170, 137, 24, 24);
            btnCameraUp.BackColor = Theme.Current.SettingsButtonBackColor;
            btnCameraUp.ForeColor = Theme.Current.SettingsButtonForeColor;
            btnCameraUp.GradientFill = Theme.Current.ButtonsGradianted;
            btnCameraUp.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnCameraUp.Click += new EventHandler(btnCameraUp_Click);
            Controls.Add(btnCameraUp);

            //Theme
            var lblTheme = new FluidLabel("Theme: ", 10, 175, 80, 25);
            lblTheme.Font = new Font(FontFamily.GenericSerif, 8, FontStyle.Regular);
            Controls.Add(lblTheme);

            lblThemeVal = new FluidLabel(string.Empty, 80, 172, 60, 25);
            lblThemeVal.Font = new Font("Tahoma", 9, FontStyle.Bold);
            Controls.Add(lblThemeVal);
            ResetLabelTheme();

            var btnThemeDown = new FluidButton("-", 140, 167, 24, 24);
            btnThemeDown.BackColor = Theme.Current.SettingsButtonBackColor;
            btnThemeDown.ForeColor = Theme.Current.SettingsButtonForeColor;
            btnThemeDown.GradientFill = Theme.Current.ButtonsGradianted;
            btnThemeDown.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnThemeDown.Click += new EventHandler(btnThemeDown_Click);
            Controls.Add(btnThemeDown);

            var btnThemeUp = new FluidButton("+", 170, 167, 24, 24);
            btnThemeUp.BackColor = Theme.Current.SettingsButtonBackColor;
            btnThemeUp.ForeColor = Theme.Current.SettingsButtonForeColor;
            btnThemeUp.GradientFill = Theme.Current.ButtonsGradianted;
            btnThemeUp.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnThemeUp.Click += new EventHandler(btnThemeUp_Click);
            Controls.Add(btnThemeUp);
        }

        void BackButton_Click(object sender, EventArgs e)
        {
            //Back
            Form1.Instance.OpenFoldersPanel();
            Close(ShowTransition.None);
        }

        void btnForgetMe_Click(object sender, EventArgs e)
        {
            Settings.Instance.RememberMe = false;
            MessageDialog.Show("Your Login has been forgotten.", "OK", null);
        }

        private void ResetLabelSize()
        {
            if (Settings.Instance.MaxSizeMB <= 0)
            {
                lblFileSizeVal.Text = "No Limit";
            }
            else
            {
                lblFileSizeVal.Text = string.Format("{0} MB", Settings.Instance.MaxSizeMB.ToString());
            }
        }

        void btnSizeDown_Click(object sender, EventArgs e)
        {
            if (Settings.Instance.MaxSizeMB > 0)
            {
                Settings.Instance.MaxSizeMB--;
            }

            ResetLabelSize();
        }

        void btnSizeUp_Click(object sender, EventArgs e)
        {
            Settings.Instance.MaxSizeMB++;

            ResetLabelSize();
        }

        private void ResetLabelCamera()
        {
            lblCameraVal.Text = Settings.Instance.CameraResText;
        }

        void btnCameraDown_Click(object sender, EventArgs e)
        {
            Settings.Instance.CameraResDown();

            ResetLabelCamera();
        }

        void btnCameraUp_Click(object sender, EventArgs e)
        {
            Settings.Instance.CameraResUp();

            ResetLabelCamera();
        }

        private void ResetLabelTheme()
        {
            switch (Settings.Instance.Theme)
            {
                case 2:
                    lblThemeVal.Text = "B & W";
                    break;
                case 1:
                    lblThemeVal.Text = "Dropbox";
                    break;
                default:
                    lblThemeVal.Text = "Default";
                    break;
            }
        }

        void btnThemeDown_Click(object sender, EventArgs e)
        {
            Settings.Instance.ThemeDown();

            ResetLabelTheme();
        }

        void btnThemeUp_Click(object sender, EventArgs e)
        {
            Settings.Instance.ThemeUp();

            ResetLabelTheme();
        }

    }
}
