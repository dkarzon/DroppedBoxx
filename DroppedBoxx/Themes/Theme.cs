using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace DroppedBoxx
{
    /// <summary>
    ///  A theme for Password Safe.
    /// </summary>
    public class Theme
    {
        //Login...?

        //Lists
        public Color ListBackColor { get; set; }
        public Color ListSelectedBackColor { get; set; }
        public Color TabHeaderBackColor { get; set; }
        public Color ListHeaderBackColor { get; set; }
        public Color ListForeColor { get; set; }
        public Color ListSecondaryForeColor { get; set; }
        public Color ListAlternateForeColor { get; set; }
        public Color ListSelectedForeColor { get; set; }
        public Color ListSecondarySelectedForeColor { get; set; }
        public Color ListAlternateSelectedForeColor { get; set; }
        public Font ListPrimaryFont { get; set; }
        public Font ListSecondaryFont { get; set; }

        public Color ButtonsBackColor { get; set; }
        public Color ButtonsForeColor { get; set; }
        public bool ButtonsGradianted { get; set; }
        public Color TabHeaderButtonBackColor {get;set;}
        public Color TabHeaderButtonForeColor { get; set; }
        public Color ButtonsButtonBackColor { get; set; }
        public Color ListBorderColor { get; set; }
        public Color ScrollbarColor { get; set; }
        public Color ScrollbarBorderColor { get; set; }
        public Color GrayTextColor { get; set; }
        public Color ItemButtonColor { get; set; }

        //header
        public Color HeaderBackColor { get; set; }
        public Color HeaderForeColor { get; set; }
        public Color HeaderSecondaryButtonBackColor { get; set; }
        public Font HeaderFont { get; set; }
        public bool HeaderGradianted { get; set; }
        public Color HeaderBackButtonBackColor { get; set; }
        public Color HeaderBackButtonForeColor { get; set; }

        public Color LogoutButtonBackColor { get; set; }

        public Font ButtonFont { get; set; }

        //Modals
        public Font ModalFont { get; set; }

        //Panels
        public Color PanelBackColor { get; set; }
        public bool PanelGradinated { get; set; }

        //Settings
        public Color SettingsButtonBackColor { get; set; }
        public Color SettingsButtonForeColor { get; set; }


        public static Theme Current { get; set; }
    }
}
