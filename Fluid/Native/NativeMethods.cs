using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Fluid.Native
{
    public static class NativeMethods
    {
        [DllImport("coredll.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);

        private const int WM_PRINT = 0x317;
        private const int PRF_NON_CLIENT = 0x02;
        private const int PRF_CLIENT = 0x04;
        private const int PRF_ERASEBKGND = 0x08;
        private const int PRF_CHILDREN = 0x10;


        public static void DrawControlToBitmap(Control control, Bitmap bitmap, Rectangle targetBounds)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException("bitmap");
            }
            if (((targetBounds.Width <= 0) || (targetBounds.Height <= 0)) || ((targetBounds.X < 0) || (targetBounds.Y < 0)))
            {
                throw new ArgumentException("targetBounds");
            }


            int width = Math.Min(control.Width, targetBounds.Width);
            int height = Math.Min(control.Height, targetBounds.Height);
            using (Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppRgb))
            {
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    IntPtr hdc = graphics.GetHdc();
                    SendMessage(control.Handle, 0x317, hdc, PRF_CLIENT | PRF_CHILDREN | PRF_ERASEBKGND);
                    graphics.ReleaseHdc(hdc);
                }
                using (Graphics graphics2 = Graphics.FromImage(bitmap))
                {
                    ImageAttributes ia = new ImageAttributes();
                    graphics2.DrawImage(image, targetBounds, 0, 0, width, height, GraphicsUnit.Pixel, ia);
                }
            }
        }

    }
}
