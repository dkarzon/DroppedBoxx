using System;
using System.Collections.Generic;
using System.Text;
using Fluid.Drawing.Runtime.InteropServices.ComTypes;
using System.Drawing;

namespace Fluid.Drawing.GdiPlus
{
    public class ImagePlus : IDisposable
    {

        public ImagePlus(string filename, bool useEmbeddedColorManagement)
        {
        }

        public ImagePlus(IStream stream, bool useEmbeddedColorManagement )
        {
            GdiPlus.GdipLoadImageFromStream(stream, out nativeImage);
        }


        static public ImagePlus FromStream( IStream stream, bool useEmbeddedColorManagement )
        {
            return new ImagePlus(stream, useEmbeddedColorManagement);
        }


        public void GetPhysicalDimension(out SizeF size)
        {

            float width, height;

            SetStatus(GdiPlus.GdipGetImageDimension(nativeImage, out width, out height));

            size = new SizeF(width, height);
        }

        public RectangleF GetBounds(out Unit srcUnit)
        {
            RectangleF srcRect;
            SetStatus(GdiPlus.GdipGetImageBounds(nativeImage, out srcRect, out srcUnit));
            return srcRect;
        }

        public uint GetWidth()
        {
            Unit unit;
            RectangleF rc = GetBounds(out unit);
            
            return (uint)rc.Width;
        }

        public uint GetHeight()
        {
            Unit unit;
            RectangleF rc = GetBounds(out unit);
            return (uint)rc.Height;
        }

        public void GetRawFormat(out Guid format)
        {
            SetStatus(GdiPlus.GdipGetImageRawFormat(nativeImage, out format));
        }

        public System.Drawing.Imaging.PixelFormat GetPixelFormat()
        {

            System.Drawing.Imaging.PixelFormat format;

            SetStatus(GdiPlus.GdipGetImagePixelFormat(nativeImage, out format));

            return format;

        }

        internal ImagePlus() { }

        internal ImagePlus(GpImage nativeImage, GpStatus status)
        {
            SetNativeImage(nativeImage);
        }

        internal void SetNativeImage(GpImage nativeImage)
        {
            this.nativeImage = nativeImage;
        }

        protected void SetStatus(GpStatus gpStatus)
        {
            if (gpStatus != GpStatus.Ok) throw GdiPlusStatusException.Exception(gpStatus);
        }

        internal GpImage nativeImage;
        protected GpStatus lastResult;
        protected GpStatus loadStatus;


        private ImagePlus(ImagePlus C)
        {
            GdiPlus.GdipCloneImage(C.nativeImage, out this.nativeImage);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (nativeImage != IntPtr.Zero)
            {
                GdiPlus.GdipDisposeImage(nativeImage);
                nativeImage = null;
            }
        }

        #endregion
    }
}
