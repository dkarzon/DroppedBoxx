using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Fluid.Drawing.GdiPlus;
using Fluid.GdiPlus;
using System.Drawing.Imaging;
using System.Collections.Specialized;
using System.Threading;
using Fluid.Drawing;

namespace Fluid.Controls
{
    public partial class FluidButton : FluidLabel
    {
        /// <summary>
        /// Gets or sets wheter to use gradient fill (true) or solid color fill (false) for the background.
        /// If set to true, the background is painted with a gradient with the the specified <see>GradientFillOffset</see>.
        /// </summary>
        [DefaultValue(true)]
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

        public FluidButton()
            : base()
        {
        }
        public FluidButton(string text)
            : base()
        {
            this.Text = text;
        }
        public FluidButton(Image image)
            : base()
        {
            this.Image = image;
        }

        public FluidButton(string text, Color backColor)
        {
            this.Text = text;
            this.BackColor = backColor;
        }

        public FluidButton(string text, int x, int y, int width, int height)
            : base(text, x, y, width, height)
        {
        }

        protected override void InitControl()
        {
            base.InitControl();
            Alignment = StringAlignment.Center;
            LineAlignment = StringAlignment.Center;
            EnableCache = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            transparentBrush.Dispose();
            if (cachedBitmaps != null)
            {
                cachedBitmaps.Clear();
                cachedBitmaps = null;
            }
        }

        private Color pressedBackColor = Color.Empty;

        public Color PressedBackColor
        {
            get { return pressedBackColor; }
            set
            {
                if (pressedBackColor != value)
                {
                    pressedBackColor = value;
                    if (IsDown) InvalidateCache();
                }
            }
        }

        private Color pressedForeColor = Color.Empty;

        public Color PressedForeColor
        {
            get { return pressedForeColor; }
            set
            {
                if (pressedForeColor != value)
                {
                    pressedForeColor = value;
                    if (IsDown) InvalidateCache();
                }
            }
        }

        public override Color BackColor
        {
            get
            {
                return IsDown && !pressedBackColor.IsEmpty ? pressedBackColor : base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        private int alpha = 255;
        public int Alpha
        {
            get { return alpha; }
            set
            {
                value = CheckAlphaBounds(value);
                if (value != alpha)
                {
                    alpha = value;
                    InvalidateCache();
                    Invalidate();
                }
            }
        }

        private int CheckAlphaBounds(int value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;
            return value;
        }


        /// <summary>
        /// Gets or sets wether to enable a bitmap cache for the button for better performance.
        /// </summary>
        [DefaultValue(true)]
        public bool EnableCache { get; set; }

        private Image image;
        /// <summary>
        /// Gets or sets an image for the button. Note that Color.Transparent is the transparent color.
        /// </summary>
        public Image Image
        {
            get { return image; }
            set
            {
                if (image != value)
                {
                    image = value;
                    OnImageChanged();
                }
            }
        }

        protected virtual void OnImageChanged()
        {
            InvalidateCache();
            Invalidate();
        }


        public override void PerformClick()
        {
            base.PerformClick();
            RaiseCommand();
        }


        private ButtonShape shape = ButtonShape.Rounded;
        public ButtonShape Shape
        {
            get { return shape; }
            set
            {
                if (shape != value)
                {
                    shape = value;
                    InvalidateCache();
                    RemoveCache();
                    Invalidate();
                }
            }
        }

        private void RemoveCache()
        {
            // don't remove, other controls might also use the shape.
            //string key = GetKey();
            //cachedBitmaps.Remove(key);
        }


        private static HybridDictionary cachedBitmaps;


        protected override void OnPaintBackground(FluidPaintEventArgs e)
        {
            Size size = bounds.Size;
            if (size.Width <= 0 || size.Height <= 0) return;
            if (EnableCache)
            {
                PaintCached(e);
            }
            else
            {
                PaintButtonBackground(e);
            }
        }

        protected void PerformPaintButtonContent(FluidPaintEventArgs e)
        {
            if (OnPaintContent(e) == false)
            {
                PaintDefaultContent(e);
            }
        }

        public Point TextOffset { get; set; }

        protected void PaintDefaultContent(FluidPaintEventArgs e)
        {
            Rectangle rect = ButtonRectangle;
            Color endColor = this.BackColor;
            Graphics g = e.Graphics;
            rect.Offset(e.ControlBounds.X, e.ControlBounds.Y);
            Color textColor = ForeColor;
            if (IsDown)
            {
                textColor = pressedForeColor.IsEmpty ? ColorConverter.AlphaBlendColor(endColor, textColor, 200) : pressedForeColor;
            }
            if (!Enabled)
            {
                textColor = ColorConverter.AlphaBlendColor(endColor, textColor, 32);
            }
            Brush foreBrush = Brushes.GetBrush(textColor);
            StringFormat sf = this.stringFormat;
            Rectangle r = rect;
            r.Inflate(e.ScaleX(-3), e.ScaleY(2));
            RectangleF rf = new RectangleF(r.Left, r.Top, r.Width, r.Height);
            if (IsDown)
            {
                rf.X++;
                rf.Y++;
            }

            rf.X += (float)ScaleX(TextOffset.X);
            rf.Y += (float)ScaleY(TextOffset.Y);

            g.DrawString(Text, Font, foreBrush, rf, sf);

            if (Image != null)
            {
                int imW = ScaleX(Image.Width);
                int imH = ScaleY(Image.Height);

                int w = Math.Min(imW, rect.Width);
                int h = Math.Min(imH, rect.Height);

                rect.Y += (rect.Height - h) / 2;
                rect.X += (rect.Width - h) / 2;
                rect.Width = w;
                rect.Height = h;
                ImageAttributes ia = new ImageAttributes();
                ia.SetColorKey(Color.Transparent, Color.Transparent);
                if (rect.Width > w)
                {
                    rect.X += (rect.Width - w) / 2;
                    rect.Width = w;
                }
                if (rect.Height > h)
                {
                    rect.X += (rect.Height-h) / 2;
                    rect.Height = h;
                }
                if (IsDown)
                {
                    rect.X++;
                    rect.Y++;
                }
                g.DrawImage(Image, rect, 0, 0, Image.Width,Image.Height, GraphicsUnit.Pixel, ia);
                //                g.DrawImage(Image, rect, new Rectangle(0, 0, imW, imH), GraphicsUnit.Pixel);
            }
        }

        protected virtual bool OnPaintContent(FluidPaintEventArgs e)
        {
            if (PaintButtonText != null)
            {
                PaintButtonText(this, e);
                return true;
            }
            else return false;
        }

        public event EventHandler<FluidPaintEventArgs> PaintButtonText;

        protected override void PaintText(FluidPaintEventArgs e)
        {
            //base.PaintText(e);
        }


        /// <summary>
        /// Gets the key the identifies the button with its current properties and is unique.
        /// </summary>
        /// <returns>A string that represents the key.</returns>
        private string GetKey()
        {
            Size size = GetButtonSize();
            string imgKey = Image != null ? Image.GetHashCode().ToString() : string.Empty;
            return string.Format("{0},{1},{2},{3},{4},{5}{6}_{7}_{8}{9}",
                size.Width, size.Height,
                BackColor.ToArgb(),
                (int)shape, Text,
                IsDown ? 1 : 0,
                (int)corners,
                GetType().Name,
                Enabled ? "" : "0",
                imgKey);
        }

        protected override void OnTextChanged()
        {
            base.OnTextChanged();
            InvalidateCache();
        }



        private Bitmap cachedBitmap;

        private void InvalidateCache()
        {
            cachedBitmap = null;
            if (EnableCache && !Initializing)
            {
                CreateChachedBitmap();
            }
        }

        protected override void OnSizeChanged(Size oldSize, Size newSize)
        {
            base.OnSizeChanged(oldSize, newSize);
            InvalidateCache();
        }

        private void PaintCached(FluidPaintEventArgs pe)
        {
            Bitmap bm = CreateChachedBitmap();
            Rectangle rect = pe.ControlBounds;
            ia.SetColorKey(transparentColor, transparentColor);
            if (alpha < 255)
            {
                GdiExt.AlphaBlendImage(pe.Graphics, bm, rect.X, rect.Y, alpha, true);
            }
            else
            {
                pe.Graphics.DrawImage(bm, rect, 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel, ia);
            }
        }

        private Size buttonSize = Size.Empty;

        /// <summary>
        /// Gets or sets the size of the button.
        /// If set to Size.Empty, the size of the bounds is used.
        /// This property enables to scale the button and only works with double buffering.
        /// </summary>
        public Size ButtonSize
        {
            get { return buttonSize; }
            set
            {
                if (buttonSize != value)
                {
                    buttonSize = value;
                    InvalidateCache();
                    Invalidate();
                }
            }
        }

        private Bitmap CreateChachedBitmap()
        {
            Bitmap bm = cachedBitmap;
            if (bm == null)
            {
                if (cachedBitmaps == null) cachedBitmaps = new HybridDictionary();
                Rectangle r = ClientRectangle;
                string key = GetKey();
                if (cachedBitmaps.Contains(key))
                {
                    bm = cachedBitmaps[key] as Bitmap;
                }
                else
                {
                    bm = CreateBitmap(key);
                }
                cachedBitmap = bm;
            }
            return bm;
        }

        ImageAttributes ia = new ImageAttributes();

        private static Color transparentColor = Color.Fuchsia;
        private SolidBrush transparentBrush = new SolidBrush(transparentColor);

        private Size GetButtonSize()
        {
            return buttonSize.IsEmpty ? bounds.Size : buttonSize;
        }

        private Bitmap CreateBitmap(string key)
        {
            Size size = GetButtonSize();
            int w = Math.Max(size.Width, 2);
            int h = Math.Max(size.Height, 2);
            Bitmap bm = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bm))
            {
                transparentBrush.Color = transparentColor;
                g.FillRectangle(transparentBrush, 0, 0, bm.Width, bm.Height);
                //g.Clear(transparentColor);
                FluidPaintEventArgs pe = new FluidPaintEventArgs(g, ClientRectangle, ScaleFactor);
                PaintButtonBackground(pe);
            }
            cachedBitmaps.Add(key, bm);
            return bm;
        }

        private GraphicShape ButtonShapeToGraphic(ButtonShape shape)
        {
            switch (shape)
            {
                case ButtonShape.Back: return GraphicShape.Back;
                case ButtonShape.Rounded: return GraphicShape.Rounded;
                case ButtonShape.Rectangle: return GraphicShape.Rectangle;
                case ButtonShape.Next: return GraphicShape.Next;
                case ButtonShape.Flat: return GraphicShape.Rounded;
                case ButtonShape.Ellipse: return GraphicShape.Ellipse;
                default: return GraphicShape.Rounded;
            }
        }

        Rectangle ButtonRectangle
        {
            get
            {
                Size size = GetButtonSize();
                return new Rectangle(0, 0, size.Width, size.Height);
            }
        }

        protected virtual void PaintButtonBackground(FluidPaintEventArgs e)
        {
            const int const_radius = 8;

            if (this.shape != ButtonShape.Flat || IsDown)
            {
                Rectangle rect = ButtonRectangle;
                rect.Width--;
                rect.Height--;
                if (rect.Width < 1 || rect.Height < 1)
                {
                    return;
                }

                rect.Offset(e.ControlBounds.X, e.ControlBounds.Y);

                Graphics g = e.Graphics;

                int radius = e.ScaleX(const_radius);
                if (radius > rect.Width / 2) radius = rect.Width / 2;

                Color endColor = this.BackColor;
                if (!Enabled)
                {
                    endColor = ColorConverter.AlphaBlendColor(Color.Black, endColor, 128);
                }
                int alpha = Enabled ? 127 : 32;
                Color startColor = ColorConverter.AlphaBlendColor(endColor, Color.White, alpha);  //Color.FromArgb(0x7fffffff);
                Color borderColor = ColorConverter.AlphaBlendColor(endColor, Color.White, 100);

                GraphicShape shape = ButtonShapeToGraphic(this.shape);
                using (GraphicsPlus gp = new GraphicsPlus(g))
                {
                    if (GradientFill)
                    {
                        GraphicsPlus.GradientMode mode = IsDown ? GraphicsPlus.GradientMode.Bottom : GraphicsPlus.GradientMode.Top;
                        gp.GradientFillShape(rect, radius, startColor, endColor, mode, shape, corners);
                        using (PenPlus pen = new PenPlus(borderColor, (float)1))
                        {
                            gp.DrawShape(rect, radius, pen, shape, corners);
                        }
                    }
                    else
                    {
                        if (IsDown)
                        {
                            endColor = ColorConverter.AlphaBlendColor(endColor, endColor, 128);
                        }
                        gp.GradientFillShape(rect, radius, endColor, endColor, GraphicsPlus.GradientMode.Bottom, shape, corners);
                        using (PenPlus pen = new PenPlus(borderColor, (float)1))
                        {
                            gp.DrawShape(rect, radius, pen, shape, corners);
                        }
                    }
                }
            }
            PerformPaintButtonContent(e);
        }

        public override void OnDown(PointEventArgs p)
        {
            base.OnDown(p);
            Invalidate();
        }

        bool gestureCanceled = false;

        public override void OnGesture(GestureEventArgs e)
        {
            base.OnGesture(e);
            if (e.Gesture != Gesture.Canceled)
            {
                IsDown = false;
                gestureCanceled = true;
            }
        }

        protected override void OnDownChanged(bool value)
        {
            InvalidateCache();
            gestureCanceled = false;
            base.OnDownChanged(value);
            Invalidate();
        }

        public override void OnMove(PointEventArgs e)
        {
            if (!gestureCanceled && Enabled) IsDown = ClientRectangle.Contains(e.X, e.Y);
        }

        public override bool OnClick(PointEventArgs p)
        {

            base.OnClick(p);
            return true;
        }



        public void RaiseCommand()
        {
            if (!string.IsNullOrEmpty(Command))
            {
                ICommandContainer container = Container as ICommandContainer;
                if (container != null)
                {
                    CommandEventArgs e = new CommandEventArgs(Command, this, null);
                    container.RaiseCommand(e);
                }
            }
        }

        private Bitmap doubleBuffer;

        public override void Invalidate()
        {
            //       FreeDoubleBuffer();
            base.Invalidate();
        }

        private void FreeDoubleBuffer()
        {
            if (doubleBuffer != null) doubleBuffer.Dispose();
            doubleBuffer = null;
        }



        private RoundedCorners corners = RoundedCorners.All;

        public RoundedCorners Corners
        {
            get
            {
                return corners;
            }
            set
            {
                if (corners != value)
                {
                    corners = value;
                    InvalidateCache();
                    Invalidate();
                }
            }
        }

        protected override void OnEnabledChanged()
        {
            base.OnEnabledChanged();
            //Alpha = Enabled ? 255 : 200;
            InvalidateCache();
            Invalidate();
        }

        /// <summary>
        /// Gets or sets the command to raise on a parent ICommandContainer
        /// </summary>
        public string Command { get; set; }

        public override bool Active { get { return true; } }

        public override bool IsDoubleBuffered { get { return EnableCache; } }

        private SizeF sf = new SizeF(1f, 1f);
        public override void Scale(SizeF scaleFactor)
        {
            base.Scale(scaleFactor);
            if (!buttonSize.IsEmpty)
            {
                buttonSize = new Size((int)((scaleFactor.Width * buttonSize.Width / sf.Width)), (int)((scaleFactor.Height * buttonSize.Height / sf.Height)));
                sf = scaleFactor;
            }
        }
    }
}
