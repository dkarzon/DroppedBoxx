using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;

namespace Fluid.Drawing.GdiPlus
{
    public static class GdiExt
    {
        const UInt32 SRCCOPY = 13369376;

        public struct BlendFunction
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        public enum BlendOperation : byte
        {
            AC_SRC_OVER = 0x00
        }

        public enum BlendFlags : byte
        {
            Zero = 0x00
        }


        public enum AlphaFormat : byte
        {
            AC_SRC_ALPHA = 0x01
        }
        [DllImport("coredll.dll", SetLastError = true)]
        public static extern int SetTextColor(IntPtr hDC, int cColor);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern int DrawText(IntPtr hDC, string Text, int nLen, ref Rectangle rect, uint uFormat);


        [DllImport("coredll.dll")]
        extern private static Boolean BitBlt(
            IntPtr hdcDest,
            Int32 nXDest,
            Int32 nYDest,
            Int32 nWidth,
            Int32 nHeight,
            IntPtr hdcSrc,
            Int32 nXSrc,
            Int32 nYSrc,
            UInt32 dwRop);


        [DllImport("coredll.dll")]
        extern private static Int32 AlphaBlend(
            IntPtr hdcDest,
            Int32 xDest,
            Int32 yDest,
            Int32 cxDest,
            Int32 cyDest,
            IntPtr hdcSrc,
            Int32 xSrc,
            Int32 ySrc,
            Int32 cxSrc,
            Int32 cySrc,
            BlendFunction blendFunction);

        [StructLayout(LayoutKind.Sequential)]
        private struct TRIVERTEX
        {
            public int X;
            public int Y;
            public ushort Red;
            public ushort Green;
            public ushort Blue;
            public ushort Alpha;

            public TRIVERTEX(int x, int y, Color color)
            {
                this.X = x;
                this.Y = y;
                this.Red = (ushort)(color.R << 8);
                this.Green = (ushort)(color.G << 8);
                this.Blue = (ushort)(color.B << 8);
                this.Alpha = (ushort)(color.A << 8);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GRADIENT_RECT
        {
            public uint UpperLeft;
            public uint LowerRight;

            public GRADIENT_RECT(uint upperLeft, uint lowerRight)
            {
                this.UpperLeft = upperLeft;
                this.LowerRight = lowerRight;
            }
        }

        [DllImport("coredll.dll")]
        extern private static Int32 GradientFill(
            IntPtr hdc, TRIVERTEX[] pVertex,
            uint dwNumVertex,
            GRADIENT_RECT[] pMesh,
            uint dwNumMesh,
            uint dwMode);

        const int GRADIENT_FILL_RECT_H = 0x00000000;
        const int GRADIENT_FILL_RECT_V = 0x00000001;

        // The direction to the GradientFill will follow
        public enum FillDirection
        {
            LeftToRight = GRADIENT_FILL_RECT_H,
            TopToBottom = GRADIENT_FILL_RECT_V
        }

        public static void GradientFill(Graphics g, Rectangle rect, Color startColor, Color endColor, FillDirection fillDir)
        {
            TRIVERTEX[] vertex = new TRIVERTEX[]
            {
                new TRIVERTEX(rect.X,rect.Y,startColor),
                new TRIVERTEX(rect.Right,rect.Bottom,endColor)
            };
            GRADIENT_RECT[] grect = new GRADIENT_RECT[] {
                new GRADIENT_RECT(0,1)
            };

            IntPtr hdc = g.GetHdc();
            GradientFill(hdc, vertex, (uint)vertex.Length, grect, (uint)grect.Length, (uint)fillDir);
            g.ReleaseHdc(hdc);
        }

        public static void AlphaBlendImage(Graphics g, Image image, int x, int y, int alpha, bool srcAlpha)
        {
            IntPtr hdcDst = g.GetHdc();
            AlphaBlendImage(hdcDst, image, x, y, alpha, srcAlpha);
            g.ReleaseHdc(hdcDst);
        }

        internal static void AlphaBlendImage(IntPtr hdcDst, Image image, int x, int y, int alpha, bool srcAlpha)
        {
            int width = image.Width;
            int weight = image.Height;
            using (Graphics imageG = Graphics.FromImage(image))
            {
                IntPtr hdcSrc = imageG.GetHdc();
                GdiExt.BlendFunction blendFunction = new GdiExt.BlendFunction();
                blendFunction.BlendOp = (byte)GdiExt.BlendOperation.AC_SRC_OVER;
                blendFunction.BlendFlags = (byte)GdiExt.BlendFlags.Zero;
                blendFunction.SourceConstantAlpha = (byte)alpha;
                blendFunction.AlphaFormat = srcAlpha ? (byte)1 : (byte)0;
                AlphaBlend(hdcDst, x, y, width, weight, hdcSrc, 0, 0, width, weight, blendFunction);
                imageG.ReleaseHdc(hdcSrc);
            }
        }

        public static void AlphaBlendImage(Graphics g, Image image, Rectangle bounds, int alpha, bool srcAlpha)
        {
            IntPtr hdcDst = g.GetHdc();
            AlphaBlendImage(hdcDst, image, bounds, alpha, srcAlpha);
            g.ReleaseHdc(hdcDst);
        }

        internal static void AlphaBlendImage(IntPtr hdcDst, Image image, Rectangle bounds, int alpha, bool srcAlpha)
        {
            int width = image.Width;
            int weight = image.Height;
            using (Graphics imageG = Graphics.FromImage(image))
            {
                IntPtr hdcSrc = imageG.GetHdc();
                GdiExt.BlendFunction blendFunction = new GdiExt.BlendFunction();
                blendFunction.BlendOp = (byte)GdiExt.BlendOperation.AC_SRC_OVER;
                blendFunction.BlendFlags = (byte)GdiExt.BlendFlags.Zero;
                blendFunction.SourceConstantAlpha = (byte)alpha;
                blendFunction.AlphaFormat = srcAlpha ? (byte)1 : (byte)0;
                AlphaBlend(hdcDst, bounds.X, bounds.Y, bounds.Width, bounds.Height, hdcSrc, 0, 0, width, weight, blendFunction);
                imageG.ReleaseHdc(hdcSrc);
            }
        }

        public static void ScrollDown(Image image, int dy)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                IntPtr hdc = g.GetHdc();
                try
                {
                    BitBlt(hdc, 0, dy, image.Width, image.Height - dy, hdc, 0, 0, SRCCOPY);
                }
                finally
                {
                    g.ReleaseHdc(hdc);
                }
            }
        }


        public static void ScrollDown(Graphics g, int dy, int width, int height)
        {
            IntPtr hdc = g.GetHdc();
            try
            {
                BitBlt(hdc, 0, dy, width, height - dy, hdc, 0, 0, SRCCOPY);
            }
            finally
            {
                g.ReleaseHdc(hdc);
            }
        }

        public static void ScrollUp(Image image, int dy)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                IntPtr hdc = g.GetHdc();
                try
                {
                    BitBlt(hdc, 0, 0, image.Width, image.Height - dy, hdc, 0, dy, SRCCOPY);
                }
                finally
                {
                    g.ReleaseHdc(hdc);
                }
            }
        }

        public static void ScrollUp(Graphics g, int dy, int width, int height)
        {
            IntPtr hdc = g.GetHdc();
            try
            {
                BitBlt(hdc, 0, 0, width, height - dy, hdc, 0, dy, SRCCOPY);
            }
            finally
            {
                g.ReleaseHdc(hdc);
            }
        }



        //[DllImport("coredll.dll", SetLastError = true)]
        //public static extern int ExtTextOut(IntPtr hdc, int X, int Y, uint fuOptions, ref RECT lprc, string lpString, int cbCount, int[] lpDx);

        //[DllImport("coredll", SetLastError = true)]
        //public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        //[DllImport("coredll.dll", SetLastError = true)]
        //public static extern int SetBkColor(IntPtr hDC, int cColor);

        //[DllImport("coredll.dll", SetLastError = true)]
        //public static extern int SetBkMode(IntPtr hDC, int nMode);

        //[StructLayout(LayoutKind.Sequential)]
        //public struct RECT
        //{
        //    public int left;
        //    public int top;
        //    public int right;
        //    public int bottom;
        //}



        //public static int DrawText(IntPtr hDC, IntPtr hFont, string sText, int nLen, ref Rectangle rect, uint uFormat, Color foreColor, Color backColor)
        //{
        //    IntPtr hObject = SelectObject(hDC, hFont);
        //    uint x = 0xff000000;
        //    int i = (int)x;
        //    i = 0;
        //    SetTextColor(hDC, foreColor.ToArgb() | i);
        //    SetBkColor(hDC, backColor.ToArgb() | i);
        //    if (backColor == Color.Empty)
        //    {
        //        SetBkMode(hDC, 1);
        //    }
        //    else
        //    {
        //        SetBkMode(hDC, 2);
        //    }
        //    RECT lprc = new RECT();
        //    lprc.left = rect.Left;
        //    lprc.top = rect.Top;
        //    lprc.right = rect.Right;
        //    lprc.bottom = rect.Bottom;
        //    int num = ExtTextOut(hDC, lprc.left, lprc.top, uFormat, ref lprc, sText, sText.Length, null);
        //    SelectObject(hDC, hObject);
        //    return num;
        //}


        public static void Copy(Graphics src, Rectangle srcBounds, Graphics dst, int dstX, int dstY)
        {
            IntPtr hdcSrc = src.GetHdc();
            IntPtr hdcDst = dst.GetHdc();
            try
            {
                BitBlt(hdcDst, dstX, dstY, srcBounds.Width, srcBounds.Height, hdcSrc, srcBounds.X, srcBounds.Y, SRCCOPY);
            }
            finally
            {
                dst.ReleaseHdc(hdcDst);
                src.ReleaseHdc(hdcSrc);
            }
        }

        public static void DrawStringShadow(Graphics g, string text, Font font, Color textColor, Color shadowColor, Rectangle bounds, StringFormat stringFormat)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Rectangle r = bounds;
                r.Height--;
                r.Width--;
                RectangleF tr = new RectangleF((float)r.Left, (float)r.Top, (float)r.Width, (float)r.Height);
                tr.X += 1f;
                tr.Y += 1f;
                using (Brush pen = new SolidBrush(shadowColor))
                {
                    g.DrawString(text, font, pen, tr, stringFormat);
                }
                tr.X -= 1f;
                tr.Y -= 1f;
                using (Brush pen = new SolidBrush(textColor))
                {
                    g.DrawString(text, font, pen, tr, stringFormat);
                }
            }
        }

    }
}
