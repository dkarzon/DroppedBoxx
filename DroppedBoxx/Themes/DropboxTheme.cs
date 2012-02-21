using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Fluid.Drawing.GdiPlus;

namespace DroppedBoxx.Themes
{
    public class DropboxTheme : Theme
    {
        public DropboxTheme()
        {
            //Buttons
            this.ButtonsGradianted = false;
            this.ButtonsBackColor = Color.SlateGray;
            this.ButtonsForeColor = Color.White;
            this.ButtonsBackColor = Color.White;
            this.ButtonsButtonBackColor = Color.Black;
            this.ItemButtonColor = Color.SlateGray;
            this.SettingsButtonBackColor = Color.FromArgb(192, 222, 237);
            this.SettingsButtonForeColor = Color.Black;

            //List
            this.ListBackColor = Color.White;
            this.ListBorderColor = Color.FromArgb(233, 244, 255);
            this.ListForeColor = Color.FromArgb(32, 32, 32);
            this.ListHeaderBackColor = Color.LightSteelBlue;
            this.ListSecondaryForeColor = Color.FromArgb(100, 100, 100);
            this.ListSecondarySelectedForeColor = ColorConverter.AlphaBlendColor(Color.Black, Color.DarkOrange, 160);
            this.ListSelectedBackColor = Color.FromArgb(200, 200, 200);
            this.ListSelectedForeColor = Color.Black;

            //Tabs
            this.TabHeaderButtonForeColor = Color.White;
            this.TabHeaderBackColor = Color.SlateGray;

            //Scrolls
            this.ScrollbarBorderColor = Color.Silver;
            this.ScrollbarColor = Color.Black;

            //Other
            this.GrayTextColor = Color.Gray;

            //Header
            this.HeaderBackColor = Color.FromArgb(41, 111, 165);
            this.HeaderForeColor = Color.White;
            this.HeaderBackButtonBackColor = Color.FromArgb(50, 50, 50);
            this.HeaderBackButtonForeColor = Color.White;
            this.HeaderSecondaryButtonBackColor = Color.FromArgb(17, 60, 94);
            this.HeaderGradianted = false;

            this.LogoutButtonBackColor = Color.FromArgb(130, 0, 0);

            //Fonts
            this.HeaderFont = new Font("Tahoma", 8, FontStyle.Bold);
            this.ButtonFont = new Font("Tahoma", 8, FontStyle.Bold);
            this.ListPrimaryFont = new Font(FontFamily.GenericSansSerif, 9f, FontStyle.Bold);
            this.ListSecondaryFont = new Font(FontFamily.GenericSansSerif, 7f, FontStyle.Regular);
            this.ModalFont = new Font("Tahoma", 8, FontStyle.Bold);

            //Panels
            this.PanelBackColor = Color.FromArgb(233, 244, 255);
            this.PanelGradinated = true;
        }
    }
}
