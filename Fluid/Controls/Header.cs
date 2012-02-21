using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Fluid.Controls.Classes;
using Fluid.Drawing;
using System.Windows.Forms;
using Fluid.Drawing.GdiPlus;
using System.ComponentModel;

namespace Fluid.Controls
{
    /// <summary>
    /// A Header control that has a title, a Back-Button on the left and optional buttons on the right.
    /// This control is part of the NavigationPanel.
    /// </summary>
    public class FluidHeader : ControlContainer, ILayoutPanel
    {
        protected override void InitControl()
        {
            base.InitControl();
            backButton = defaultBackButton;
            Anchor = AnchorTLR;
            if (Bounds.Size.IsEmpty) bounds = new Rectangle(0, 0, 240, 32);
            ForeColor = Color.White;
            //       Bounds = new Rectangle(0, 0, 240, 28);
            rightButtons.Font = new Font(FontFamily.GenericSansSerif, 6f, FontStyle.Regular);
            Font = new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Bold);
            BackColor = Color.SlateGray;
            int h = Height - 8;

            backButton.Visible = false;
            backButton.Font = new Font(FontFamily.GenericSansSerif, 6f, FontStyle.Regular);
            backButton.Bounds = new Rectangle(2, 4, 48, h);
            backButton.Shape = ButtonShape.Back;
            backButton.Anchor = AnchorLTB;
            backButton.BackColor = ColorConverter.OpaqueColor(Color.SlateGray);

            rightButtons.Bounds = new Rectangle(Width - 60 - 4, 4, 60, h);
            rightButtons.Anchor = AnchorRTB;
            titleLabel.Anchor |= AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top;
            titleLabel.ForeColor = Color.Empty;
            titleLabel.ShadowColor = Color.Black;
            titleLabel.LineAlignment = StringAlignment.Center;
            titleLabel.Alignment = StringAlignment.Center;
            rightButtons.ButtonsChanged += new EventHandler(OnRightButtonsChanged);
            rightButtons.Visible = false;
        //    InitTitleBounds();
            titleLabel.bounds = ClientRectangle;

            controls.Add(titleLabel);
            controls.Add(backButton);
            controls.Add(rightButtons);
        }


        public override void Dispose()
        {
            titleLabel.Dispose();
            backButton.Dispose();
            if (animLabel != null) animLabel.Dispose();
            base.Dispose();
        }

        protected override void OnForeColorChanged()
        {
            base.OnForeColorChanged();
            //    titleLabel.Text = ForeColor;
        }

        /// <summary>
        /// Occurs when the right buttons have changed.
        /// </summary>
        protected virtual void OnRightButtonsChanged(object sender, EventArgs e)
        {
            rightButtons.Visible = rightButtons.Buttons.Count > 0;
        }


        private int animOffset = 0;

        /// <summary>
        /// Gets or sets the animation offset for the transition between two titles.
        /// This property is internally used by the NavigationPanel to sync a transition from one Page to another.
        /// </summary>
        internal int AnimOffset
        {
            get { return animOffset; }
            set
            {
                if (animOffset != value)
                {
                    animOffset = value;
                    InitTitleBounds();
                    Invalidate(titleLabel.Bounds);
                    Invalidate(animLabel.Bounds);
                }
            }
        }

        private FluidLabel animLabel;

        /// <summary>
        /// Gets or sets the animated label that appears when the header is animated by a NavigationPanel.
        /// </summary>
        internal FluidLabel AnimLabel
        {
            get
            {
                EnsureAnimLabel();
                return animLabel;
            }
        }

#if SUPPORT_ALPHA

        /// <summary>
        /// Gets or sets the animated back button that appears when the header is animated by a NavigationPanel.
        /// </summary>
        internal FluidButton AnimBackButton
        {
            get { return animBackButton; }
            set
            {
                if (animBackButton != value)
                {
                    animBackButton = value;
                    if (value != null)
                    {
                        if (animBackButton != null) controls.Remove(animBackButton);
                        animBackButton.Bounds = backButton.Bounds;
                        controls.Add(value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the animated right  button that appear when the header is animated by a NavigationPanel.
        /// </summary>
        internal ButtonGroup AnimButtons
        {
            get { return animButtons; }
            set
            {
                if (animButtons == null)
                {
                    animButtons = value;
                    if (value != null)
                    {
                        controls.Add(animButtons);
                        animButtons.Font = rightButtons.Font;
                        animButtons.Bounds = rightButtons.Bounds;
                    }
                }
            }
        }

        private FluidButton animBackButton;
        private ButtonGroup animButtons;

#endif

        private void EnsureAnimLabel()
        {
            if (animLabel == null)
            {
                FluidLabel l = new FluidLabel("", Width, 0, Width, Height);
                animLabel = l;
                l.ShadowColor = Color.Black;
                l.LineAlignment = StringAlignment.Center;
                l.Alignment = StringAlignment.Center;
                l.Visible = false;
                controls.Insert(0, l);
            }
        }

        private void InitTitleBounds()
        {
            int w = Width;
            int h = Height - 8;
            int l = backButton.Visible ? 0 : backButton.Right;
            int r = w - (rightButtons.Visible ? w : rightButtons.Left);
            l = Math.Min(l, r);
            r = w + l;


            w = Width - l - l;
            int a = (animOffset * w / Width);

            titleLabel.Bounds = new Rectangle(l - a, 4, w, h);
            AnimLabel.Bounds = new Rectangle(l + w - a, 4, w, h);
        }

        public void SetDefaultBackButton()
        {
            BackButton = defaultBackButton;
        }




        private ButtonGroup rightButtons = new ButtonGroup();
        private FluidButton defaultBackButton = new FluidButton();
        private FluidLabel titleLabel = new FluidLabel();


        private FluidButton backButton;

        public bool IsDefaultBackButton { get { return backButton == defaultBackButton; } }

        /// <summary>
        /// Gets or sets the Back Button on the left.
        /// </summary>
        public FluidButton BackButton 
        { 
            get { return backButton; }
            set
            {
                if (backButton != value)
                {
                    if (backButton != null)
                    {
                        controls.Remove(backButton);
                    }
                    backButton = value;
                    if (backButton != null)
                    {
                        if (backButton.Font == null)
                        {
                            backButton.Font = new Font(FontFamily.GenericSansSerif, 6f, FontStyle.Regular);
                        }
                        if (backButton.BackColor.IsEmpty)
                        {
                            backButton.BackColor = defaultBackButton.BackColor;
                        }
                        int h = UnscaleX(Height) - 8;
                        int w = Math.Min(backButton.Width, 32);
                        backButton.Bounds = new Rectangle(2, 4, w, h);
                        backButton.Anchor = AnchorLTB;
                        controls.Insert(2,backButton);
                    }
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the title for this header.
        /// </summary>
        public string Title
        {
            get { return titleLabel.Text; }
            set { titleLabel.Text = value; }
        }

        public ButtonCollection Buttons
        {
            get { return rightButtons.Buttons; }
        }

        public int ButtonsWidth
        {
            get { return rightButtons.Width; }
            set { rightButtons.Width = value; }
        }


        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        public RoundedCorners Corners
        {
            get { return rightButtons.Corners; }
            set { rightButtons.Corners = value; }
        }

        /// <summary>
        /// Gets or sets the shape for the Back Button.
        /// </summary>
        public ButtonShape BackButtonShape
        {
            get { return backButton.Shape; }
            set { backButton.Shape = value; }
        }

        /// <summary>
        /// Gets or sets wheter to use gradient fill (true) or solid color fill (false) for the background.
        /// If set to true, the background is painted with a gradient with the the specified <see>GradientFillOffset</see>.
        /// </summary>
        [DefaultValue(false)]
        public bool GradientFill { get; set; }

        private int gradientFillOffset;

        /// <summary>
        /// Gets or sets the fill offset in conjunction with <see>GradientFill</see> set to true.
        /// GradientFillOffset specifies the size of the gradient part until the solid background color part begin.
        /// If set to 0, the offset is the same as the height.
        /// </summary>
        [DefaultValue(0)]
        public int GradientFillOffset
        {
            get { return gradientFillOffset; }
            set
            {
                if (gradientFillOffset != value)
                {
                    gradientFillOffset = value;
                    Invalidate();
                }
            }
        }

        protected override void OnPaintBackground(FluidPaintEventArgs e)
        {
            if (GradientFill)
            {
                this.PaintGradientBackground(e, 50, gradientFillOffset > 0 ? gradientFillOffset : Height);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        protected override void OnSizeChanged(Size oldSize, Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);
            InitTitleBounds();
        }
    }
}
