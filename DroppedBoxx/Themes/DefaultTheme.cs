using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Fluid.Drawing.GdiPlus;

namespace DroppedBoxx.Themes
{
    public class DefaultTheme : Theme
    {
        public DefaultTheme()
        {
            //Buttons
            this.ButtonsGradianted = true;
            this.ButtonsBackColor = Color.SlateGray;
            this.ButtonsForeColor = Color.White;
            this.ButtonsBackColor = Color.White;
            this.ButtonsButtonBackColor = Color.Black;
            this.ItemButtonColor = Color.SlateGray;
            this.SettingsButtonBackColor = Color.Orange;
            this.SettingsButtonForeColor = Color.Black;

            //List
            this.ListBackColor = Color.White;
            this.ListBorderColor = Color.Silver;
            this.ListForeColor = Color.Black;
            this.ListHeaderBackColor = Color.LightSteelBlue;
            this.ListSecondaryForeColor = Color.FromArgb(100, 100, 100);
            this.ListSecondarySelectedForeColor = ColorConverter.AlphaBlendColor(Color.Black, Color.DarkOrange, 160);
            this.ListSelectedBackColor = Color.Orange;
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
            this.HeaderBackColor = Color.Black;
            this.HeaderForeColor = Color.White;
            this.HeaderBackButtonBackColor = Color.Gray;
            this.HeaderBackButtonForeColor = Color.White;
            this.HeaderSecondaryButtonBackColor = Color.FromArgb(32, 32, 32);
            this.HeaderGradianted = true;

            this.LogoutButtonBackColor = Color.Maroon;

            //Fonts
            this.HeaderFont = new Font("Tahoma", 8, FontStyle.Bold);
            this.ButtonFont = new Font("Tahoma", 8, FontStyle.Bold);
            this.ListPrimaryFont = new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Bold);
            this.ListSecondaryFont = new Font(FontFamily.GenericSansSerif, 7f, FontStyle.Regular);
            this.ModalFont = new Font("Tahoma", 8, FontStyle.Bold);

            //Panels
            this.PanelBackColor = Color.FromArgb(33, 124, 197);
            this.PanelGradinated = true;
        }
    }
}
