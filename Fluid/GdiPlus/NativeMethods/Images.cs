using System;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Text;
using Fluid.Drawing.Runtime.InteropServices.ComTypes;
using System.Drawing;

namespace Fluid.Drawing.GdiPlus
{
    internal partial class GdiPlus
    {

        [DllImport(dllName)]
        internal static extern GpStatus GdipLoadImageFromStream(IStream stream, out GpImage image);

        [DllImport(dllName)]
        internal static extern GpStatus GdipLoadImageFromFile([MarshalAs(UnmanagedType.BStr)]string filename, out GpImage image);

        [DllImport(dllName)]
        internal static extern GpStatus GdipCloneImage(GpImage image, out GpImage cloneImage);

        [DllImport(dllName)]
        internal static extern GpStatus GdipDisposeImage(GpImage image);

        [DllImport(dllName)]
        internal static extern GpStatus GdipSaveImageToFile(GpImage image, string filename,
        ref Guid clsidEncoder,
        EncoderParameters encoderParams);


        [DllImport(dllName)]
        internal static extern GpStatus
        GdipSaveAdd(GpImage image, EncoderParameters encoderParams);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipSaveAddImage(GpImage image, GpImage newImage,
        EncoderParameters encoderParams);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageGraphicsContext(GpImage image, out GpGraphics graphics);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageBounds(GpImage image, out RectangleF srcRect, out Unit srcUnit);
        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageBounds(GpImage image, float[] srcRect, Unit srcUnit);
        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageBounds(GpImage image, byte[] srcRect, Unit srcUnit);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageDimension(GpImage image, out float width, out float height);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageType(GpImage image, out ImageType type);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageWidth(GpImage image, out uint width);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageHeight(GpImage image, out uint height);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageHorizontalResolution(GpImage image, out float resolution);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageVerticalResolution(GpImage image, out float resolution);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageFlags(GpImage image, out uint flags);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageRawFormat(GpImage image, out Guid format);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImagePixelFormat(GpImage image, out PixelFormat format);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetImageThumbnail(GpImage image, uint thumbWidth, uint thumbHeight,
        out GpImage thumbImage,
        IntPtr /*GetThumbnailImageAbort*/ callback, IntPtr callbackData);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetEncoderParameterListSize(GpImage image, ref Guid clsidEncoder,
         out uint size);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetEncoderParameterList(GpImage image, ref Guid clsidEncoder,
        uint size, EncoderParameters buffer);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipImageGetFrameDimensionsCount(GpImage image, out uint count);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipImageGetFrameDimensionsList(GpImage image, Guid[] dimensionIDs,
         uint count);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipImageGetFrameCount(GpImage image, ref Guid dimensionID,
        out uint count);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipImageSelectActiveFrame(GpImage image, ref Guid dimensionID,
        uint frameIndex);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipImageRotateFlip(GpImage image, RotateFlipType rfType);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetPropertyCount(GpImage image, out uint numOfProperty);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetPropertyIdList(GpImage image, uint numOfProperty, PROPID[] list);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipGetPropertyItemSize(GpImage image, PROPID propId, out uint size);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipDrawImage(GpGraphics graphics, GpImage image, float x, float y);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipDrawImageI(GpGraphics graphics, GpImage image, int x, int y);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipDrawImageRect(GpGraphics graphics, GpImage image, float x, float y,
        float width, float height);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipDrawImageRectI(GpGraphics graphics, GpImage image, int x, int y,
        int width, int height);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipDrawImagePoints(GpGraphics graphics, GpImage image,
        PointF[] dstpoints, int count);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipDrawImagePointsI(GpGraphics graphics, GpImage image,
        Point[] dstpoints, int count);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipDrawImagePointRect(GpGraphics graphics, GpImage image, float x,
           float y, float srcx, float srcy, float srcwidth,
           float srcheight, Unit srcUnit);

        //[DllImport(dllName)]
        //internal static extern GpStatus
        //GdipDrawImagePointRectI(GpGraphics graphics, GpImage image, int x,
        //   int y, int srcx, int srcy, int srcwidth,
        //   int srcheight, Unit srcUnit);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipDrawImageRectRect(GpGraphics graphics, GpImage image, float dstx,
        float dsty, float dstwidth, float dstheight,
        float srcx, float srcy, float srcwidth, float srcheight,
        Unit srcUnit,
        GpImageAttributes imageAttributes,
        IntPtr callback, IntPtr callbackData);

        //[DllImport(dllName)]
        //internal static extern GpStatus
        //GdipDrawImageRectRectI(GpGraphics graphics, GpImage image, int dstx,
        //int dsty, int dstwidth, int dstheight,
        //int srcx, int srcy, int srcwidth, int srcheight,
        //Unit srcUnit,
        //GpImageAttributes imageAttributes,
        //IntPtr callback, IntPtr callbackData);

        //[DllImport(dllName)]
        //internal static extern GpStatus
        //GdipDrawImagePointsRect(GpGraphics graphics, GpImage image,
        //PointF[] points, int count, float srcx,
        //float srcy, float srcwidth, float srcheight,
        //Unit srcUnit,
        //GpImageAttributes imageAttributes,
        //IntPtr callback, IntPtr callbackData);

        [DllImport(dllName)]
        internal static extern GpStatus
        GdipDrawImagePointsRectI(GpGraphics graphics, GpImage image,
        Point[] points, int count, int srcx,
        int srcy, int srcwidth, int srcheight,
        Unit srcUnit,
        GpImageAttributes imageAttributes,
        IntPtr callback, IntPtr callbackData);

    }
}
