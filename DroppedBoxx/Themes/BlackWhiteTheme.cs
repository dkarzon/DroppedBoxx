using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Fluid.Drawing.GdiPlus;

namespace DroppedBoxx.Themes
{
    public class BlackWhiteTheme : Theme
    {
        public BlackWhiteTheme()
        {
            //Buttons
            this.ButtonsGradianted = false;
            this.ButtonsBackColor = Color.SlateGray;
            this.ButtonsForeColor = Color.White;
            this.ButtonsBackColor = Color.White;
            this.ButtonsButtonBackColor = Color.Black;
            this.ItemButtonColor = Color.SlateGray;
            this.SettingsButtonBackColor = Color.Black;
            this.SettingsButtonForeColor = Color.White;

            //List
            this.ListBackColor = Color.White;
            this.ListBorderColor = Color.FromArgb(233, 244, 255);
            this.ListForeColor = Color.FromArgb(32, 32, 32);
            this.ListHeaderBackColor = Color.Gray;
            this.ListSecondaryForeColor = Color.FromArgb(100, 100, 100);
            this.ListSecondarySelectedForeColor = ColorConverter.AlphaBlendColor(Color.Black, Color.LightGoldenrodYellow, 160);
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
            this.HeaderBackColor = Color.FromArgb(20, 20, 20);
            this.HeaderForeColor = Color.White;
            this.HeaderBackButtonBackColor = Color.FromArgb(210, 210, 210);
            this.HeaderBackButtonForeColor = Color.Black;
            this.HeaderSecondaryButtonBackColor = Color.FromArgb(230, 230, 230);
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
