using System;
using System.Collections.Generic;
using System.Text;
using Color = System.Drawing.Color;
using System.Drawing;

namespace Fluid.Drawing.GdiPlus
{
    public partial class GraphicsPlus : IDisposable
    {

        public static GraphicsPlus FromHDC(HDC hdc)
        {
            return new GraphicsPlus(hdc);
        }

        public static GraphicsPlus FromHDC(HDC hdc, HANDLE hdevice)
        {
            return new GraphicsPlus(hdc, hdevice);
        }

        public static GraphicsPlus FromHWND(HWND hwnd, bool icm)
        {
            return new GraphicsPlus(hwnd, icm);
        }

        public static GraphicsPlus FromImage(ImagePlus image)
        {
            return new GraphicsPlus(image);
        }

        IntPtr hdc = IntPtr.Zero;
        Graphics g;

        public GraphicsPlus(Graphics g)
        {
            this.g = g;
            hdc = g.GetHdc();
            GpGraphics Graphics = new GpGraphics();

            lastResult = GdiPlus.GdipCreateFromHDC(hdc, out Graphics);

            SetNativeGraphics(Graphics);

        }

        public GraphicsPlus(HDC hdc)
        {
            GpGraphics Graphics = new GpGraphics();

            lastResult = GdiPlus.GdipCreateFromHDC(hdc, out Graphics);

            SetNativeGraphics(Graphics);
        }

        public GraphicsPlus(HDC hdc, HANDLE hdevice)
        {
            GpGraphics Graphics = new GpGraphics();

            lastResult = GdiPlus.GdipCreateFromHDC2(hdc, hdevice, out Graphics);

            SetNativeGraphics(Graphics);
        }

        public GraphicsPlus(HWND hwnd, bool icm)
        {
            GpGraphics Graphics = new GpGraphics();

            if (icm)
            {
                lastResult = GdiPlus.GdipCreateFromHWNDICM(hwnd, out Graphics);
            }
            else
            {
                lastResult = GdiPlus.GdipCreateFromHWND(hwnd, out Graphics);
            }

            SetNativeGraphics(Graphics);
        }

        public GraphicsPlus(ImagePlus image)
        {
            GpGraphics Graphics = new GpGraphics();

            if (image != null)
            {
                lastResult = GdiPlus.GdipGetImageGraphicsContext(image.nativeImage, out Graphics);
            }
            SetNativeGraphics(Graphics);
        }

        ~GraphicsPlus()
        {
            Dispose(false);
        }

        public void Flush(FlushIntention intention)
        {
            GdiPlus.GdipFlush(nativeGraphics, intention);
        }


        public HDC GetHDC()
        {
            HDC hdc;

            SetStatus(GdiPlus.GdipGetDC(nativeGraphics, out hdc));

            return hdc;
        }

        private void SetStatus(GpStatus gpStatus)
        {
            if (gpStatus != GpStatus.Ok) throw GdiPlusStatusException.Exception(gpStatus);
        }




        public void ReleaseHDC(HDC hdc)
        {
            SetStatus(GdiPlus.GdipReleaseDC(nativeGraphics, hdc));
        }

        public Point RenderingOrigin
        {
            get
            {
                int x, y;
                GetRenderingOrigin(out x, out y);
                return new Point(x, y);
            }
            set
            {
                SetRenderingOrigin(value.X, value.Y);
            }
        }

        private void SetRenderingOrigin(int x, int y)
        {
            SetStatus(
                GdiPlus.GdipSetRenderingOrigin(
                    nativeGraphics, x, y
                )
            );
        }

        private void GetRenderingOrigin(out int x, out int y)
        {
            SetStatus(
                GdiPlus.GdipGetRenderingOrigin(
                    nativeGraphics, out x, out y
                )
            );
        }

        //public void SetCompositingMode(CompositingMode compositingMode)
        //{
        //    SetStatus(NativeMethods.GdipSetCompositingMode(nativeGraphics,
        //                                                        compositingMode));
        //}

        //public CompositingMode GetCompositingMode()
        //{
        //    CompositingMode mode;

        //    SetStatus(NativeMethods.GdipGetCompositingMode(nativeGraphics,
        //                                                 out mode));

        //    return mode;
        //}

        //public void SetCompositingQuality(CompositingQuality compositingQuality)
        //{
        //    SetStatus(NativeMethods.GdipSetCompositingQuality(
        //        nativeGraphics,
        //        compositingQuality));
        //}

        //public CompositingQuality GetCompositingQuality()
        //{
        //    CompositingQuality quality;

        //    SetStatus(NativeMethods.GdipGetCompositingQuality(
        //        nativeGraphics,
        //        out quality));

        //    return quality;
        //}

        //public void SetTextRenderingHint(TextRenderingHint newMode)
        //{
        //    SetStatus(NativeMethods.GdipSetTextRenderingHint(nativeGraphics,
        //                                                      newMode));
        //}

        //public TextRenderingHint GetTextRenderingHint()
        //{
        //    TextRenderingHint hint;

        //    SetStatus(NativeMethods.GdipGetTextRenderingHint(nativeGraphics,
        //                                               out hint));

        //    return hint;
        //}

        //public void SetTextContrast(uint contrast)
        //{
        //    SetStatus(NativeMethods.GdipSetTextContrast(nativeGraphics,
        //                                                      contrast));
        //}

        //public uint GetTextContrast()
        //{
        //    uint contrast;

        //    SetStatus(NativeMethods.GdipGetTextContrast(nativeGraphics,
        //                                                out contrast));

        //    return contrast;
        //}

        //public InterpolationMode GetInterpolationMode()
        //{
        //    InterpolationMode mode = InterpolationMode.InterpolationModeInvalid;

        //    SetStatus(NativeMethods.GdipGetInterpolationMode(nativeGraphics,
        //                                                       out mode));

        //    return mode;
        //}

        //public void SetInterpolationMode(InterpolationMode interpolationMode)
        //{
        //    SetStatus(NativeMethods.GdipSetInterpolationMode(nativeGraphics,
        //                                                       interpolationMode));
        //}

        public SmoothingMode SmoothingMode
        {
            get
            {
                return GetSmoothingMode();
            }
            set
            {
                SetSmoothingMode(value);
            }
        }

        private SmoothingMode GetSmoothingMode()
        {
            SmoothingMode smoothingMode = SmoothingMode.Invalid;

            SetStatus(GdiPlus.GdipGetSmoothingMode(nativeGraphics,
                                                       out smoothingMode));

            return smoothingMode;
        }

        private void SetSmoothingMode(SmoothingMode smoothingMode)
        {
            SetStatus(GdiPlus.GdipSetSmoothingMode(nativeGraphics,
                                                              smoothingMode));
        }

        //public PixelOffsetMode GetPixelOffsetMode()
        //{
        //    PixelOffsetMode pixelOffsetMode = PixelOffsetMode.PixelOffsetModeInvalid;

        //    SetStatus(NativeMethods.GdipGetPixelOffsetMode(nativeGraphics,
        //                                                 out pixelOffsetMode));

        //    return pixelOffsetMode;
        //}

        //public void SetPixelOffsetMode(PixelOffsetMode pixelOffsetMode)
        //{
        //    SetStatus(NativeMethods.GdipSetPixelOffsetMode(nativeGraphics,
        //                                                        pixelOffsetMode));
        //}

        //public void SetPageUnit(Unit unit)
        //{
        //    SetStatus(NativeMethods.GdipSetPageUnit(nativeGraphics,
        //                                                 unit));
        //}

        //public Unit GetPageUnit()
        //{
        //    Unit unit;

        //    SetStatus(NativeMethods.GdipGetPageUnit(nativeGraphics, out unit));

        //    return unit;
        //}

        //public float GetPageScale()
        //{
        //    float scale;

        //    SetStatus(NativeMethods.GdipGetPageScale(nativeGraphics, out scale));

        //    return scale;
        //}

        //public float GetDpiX()
        //{
        //    float dpi;

        //    SetStatus(NativeMethods.GdipGetDpiX(nativeGraphics, out dpi));

        //    return dpi;
        //}

        //public float GetDpiY()
        //{
        //    float dpi;

        //    SetStatus(NativeMethods.GdipGetDpiY(nativeGraphics, out dpi));

        //    return dpi;
        //}



        public void DrawLine(PenPlus pen, float x1, float y1, float x2, float y2)
        {
            SetStatus(GdiPlus.GdipDrawLine(nativeGraphics, pen.nativePen, x1, y1, x2, y2));
        }

        public void DrawLine(PenPlus pen, PointF pt1, PointF pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        public void DrawLines(PenPlus pen, PointF[] points)
        {
            SetStatus(GdiPlus.GdipDrawLines(nativeGraphics,
                                                       pen.nativePen,
                                                       points, points.Length));
        }

        public void DrawLine(PenPlus pen, int x1, int y1, int x2, int y2)
        {
            SetStatus(GdiPlus.GdipDrawLineI(nativeGraphics, pen.nativePen, x1, y1, x2, y2));
        }

        public void DrawLine(PenPlus pen, Point pt1, Point pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        public void DrawLines(PenPlus pen, Point[] points)
        {
            SetStatus(GdiPlus.GdipDrawLinesI(nativeGraphics,
                                                        pen.nativePen,
                                                        points,
                                                        points.Length));
        }



        public void DrawPie(PenPlus pen, RectangleF rect, float startAngle, float sweepAngle)
        {
            DrawPie(pen,
                           rect.X,
                           rect.Y,
                           rect.Width,
                           rect.Height,
                           startAngle,
                           sweepAngle);
        }

        public void DrawPie(PenPlus pen,
                       float x,
                       float y,
                       float width,
                       float height,
                       float startAngle,
                       float sweepAngle)
        {
            SetStatus(GdiPlus.GdipDrawPie(nativeGraphics,
                                                     pen.nativePen,
                                                     x,
                                                     y,
                                                     width,
                                                     height,
                                                     startAngle,
                                                     sweepAngle));
        }

        public void DrawPie(PenPlus pen, Rectangle rect,  float startAngle, float sweepAngle)
        {
            DrawPie(pen,
                           rect.X,
                           rect.Y,
                           rect.Width,
                           rect.Height,
                           startAngle,
                           sweepAngle);
        }

        public void DrawPie(PenPlus pen,
                       int x,
                       int y,
                       int width,
                       int height,
                       float startAngle,
                       float sweepAngle)
        {
            SetStatus(GdiPlus.GdipDrawPieI(nativeGraphics,
                                                      pen.nativePen,
                                                      x,
                                                      y,
                                                      width,
                                                      height,
                                                      startAngle,
                                                      sweepAngle));
        }


        public void DrawPath(PenPlus pen,
                         GraphicsPath path)
        {
            SetStatus(GdiPlus.GdipDrawPath(nativeGraphics,
                                                      pen != null ? pen.nativePen : null,
                                                      path != null ? path.nativePath : null));
        }


        //TODO: Does this work?
        void Clear(Color color)
        {
            SetStatus(GdiPlus.GdipGraphicsPlusClear(nativeGraphics,color.ToArgb()));
        }

        public void FillRectangle(BrushPlus brush, RectangleF rect)
        {
            FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillRectangle(BrushPlus brush,
                             float x,
                             float y,
                             float width,
                             float height)
        {
            SetStatus(GdiPlus.GdipFillRectangle(nativeGraphics,
                                                           brush.nativeBrush, x, y,
                                                           width, height));
        }

        public void FillRectangles(BrushPlus brush,
                               RectangleF[] rects)
        {
            SetStatus(GdiPlus.GdipFillRectangles(nativeGraphics,
                                                            brush.nativeBrush,
                                                            rects, rects.Length));
        }

        public void FillRectangle(BrushPlus brush,
                              Rectangle rect)
        {
            FillRectangle(brush,
                                 rect.X,
                                 rect.Y,
                                 rect.Width,
                                 rect.Height);
        }

        public void FillRectangle(BrushPlus brush,
                             int x,
                             int y,
                             int width,
                             int height)
        {
            SetStatus(GdiPlus.GdipFillRectangleI(nativeGraphics,
                                                            brush.nativeBrush,
                                                            x,
                                                            y,
                                                            width,
                                                            height));
        }

        public void FillRectangles(BrushPlus brush,
                               Rectangle[] rects)
        {
            SetStatus(GdiPlus.GdipFillRectanglesI(nativeGraphics,
                                                             brush.nativeBrush,
                                                             rects,
                                                             rects.Length));
        }


        public void FillPie(BrushPlus brush,
                        RectangleF rect,
                       float startAngle,
                       float sweepAngle)
        {
            FillPie(brush, rect.X, rect.Y, rect.Width, rect.Height,
                           startAngle, sweepAngle);
        }

        public void FillPie(BrushPlus brush,
                       float x,
                       float y,
                       float width,
                       float height,
                       float startAngle,
                       float sweepAngle)
        {
            SetStatus(GdiPlus.GdipFillPie(nativeGraphics,
                                                     brush.nativeBrush, x, y,
                                                     width, height, startAngle,
                                                     sweepAngle));
        }

        public void FillPie(BrushPlus brush,
                        Rectangle rect,
                       float startAngle,
                       float sweepAngle)
        {
            FillPie(brush, rect.X, rect.Y, rect.Width, rect.Height,
                           startAngle, sweepAngle);
        }

        public void FillPie(BrushPlus brush,
                       int x,
                       int y,
                       int width,
                       int height,
                       float startAngle,
                       float sweepAngle)
        {
            SetStatus(GdiPlus.GdipFillPieI(nativeGraphics,
                                                      brush.nativeBrush,
                                                      x,
                                                      y,
                                                      width,
                                                      height,
                                                      startAngle,
                                                      sweepAngle));
        }

        public void FillPath(BrushPlus brush, GraphicsPath path)
        {
            SetStatus(GdiPlus.GdipFillPath(nativeGraphics, brush.nativeBrush, path.nativePath));
        }


        public void DrawImage(ImagePlus image, PointF point)
        {
             DrawImage(image, point.X, point.Y);
        }

        public void DrawImage(ImagePlus image, float x, float y)
        {
            SetStatus(GdiPlus.GdipDrawImage(nativeGraphics,
                                                       image != null ? image.nativeImage : null, x,y));
        }

        public void DrawImage(ImagePlus image, RectangleF rect)
        {
            Unit unit;
            RectangleF bounds = image.GetBounds(out unit);
             DrawImage(image, rect, 0, 0, bounds.Width, bounds.Height, unit, null);
        }

        public void DrawImage(ImagePlus image,
                         float x,
                         float y,
                         float width,
                         float height)
        {
            Unit unit;
            RectangleF bounds = image.GetBounds(out unit);
            DrawImage(image, new RectangleF(x, y, width, height), 0, 0, bounds.Width, bounds.Height, unit, null);
        }

        public void DrawImage(ImagePlus image,  Point point)
        {
             DrawImage(image, point.X, point.Y);
        }

        public void DrawImage(ImagePlus image, int x,   int y)
        {
            Unit unit;
            RectangleF bounds = image.GetBounds(out unit);
            bounds.X += x;
            bounds.Y += y;
            DrawImage(image, bounds, 0, 0, bounds.Width, bounds.Height, unit, null);
        }

        public void DrawImage(ImagePlus image,  Rectangle rect)
        {
            DrawImage(image,
                             rect.X,
                             rect.Y,
                             rect.Width,
                             rect.Height);
        }

        public void DrawImage(ImagePlus image,
                         int x,
                         int y,
                         int width,
                         int height)
        {
            DrawImage(image, new RectangleF(x, y, width, height));
        }


        public void DrawImage(ImagePlus image,  PointF[] destPoints)
        {
            int count = destPoints.Length;

            if (count != 3 && count != 4)
                SetStatus(GpStatus.InvalidParameter);

            SetStatus(GdiPlus.GdipDrawImagePoints(nativeGraphics,
                                                             image != null ? image.nativeImage
                                                                   : new GpImage(),
                                                             destPoints, count));
        }

        public void DrawImage(ImagePlus image,  Point[] destPoints)
        {
            int count = destPoints.Length;
            if (count != 3 && count != 4)
                SetStatus(GpStatus.InvalidParameter);

            SetStatus(GdiPlus.GdipDrawImagePointsI(nativeGraphics,
                                                              image != null ? image.nativeImage
                                                                    : null,
                                                              destPoints,
                                                              count));
        }

        public void DrawImage(ImagePlus image,
                         float x,
                         float y,
                         float srcx,
                         float srcy,
                         float srcwidth,
                         float srcheight,
                         Unit srcUnit)
        {
            Unit unit;
            RectangleF bounds = image.GetBounds(out unit);
            DrawImage(image, new RectangleF(x, y, srcwidth, srcheight), srcx, srcy, srcwidth, srcheight, srcUnit, null);
        }

        public void DrawImage(ImagePlus image,
                          RectangleF destRect,
                         float srcx,
                         float srcy,
                         float srcwidth,
                         float srcheight,
                         Unit srcUnit,
                          ImageAttributesPlus imageAttributes)
        {
            SetStatus(GdiPlus.GdipDrawImageRectRect(nativeGraphics,
                                                               image != null ? image.nativeImage
                                                                     : null,
                                                               destRect.X,
                                                               destRect.Y,
                                                               destRect.Width,
                                                               destRect.Height,
                                                               srcx, srcy,
                                                               srcwidth, srcheight,
                                                               srcUnit,
                                                               imageAttributes != null
                                                                ? imageAttributes.nativeImageAttr
                                                                : new GpImageAttributes(),
                                                               IntPtr.Zero,
                                                               IntPtr.Zero));
        }

        //public void DrawImage(ImagePlus image,
        //                  PointF[] destPoints,
        //                 float srcx,
        //                 float srcy,
        //                 float srcwidth,
        //                 float srcheight,
        //                 Unit srcUnit,
        //                  ImageAttributesPlus imageAttributes)
        //{
        //    SetStatus(GdiPlus.GdipDrawImagePointsRect(nativeGraphics,
        //                                                         image != null ? image.nativeImage
        //                                                               : null,
        //                                                         destPoints, destPoints.Length,
        //                                                         srcx, srcy,
        //                                                         srcwidth,
        //                                                         srcheight,
        //                                                         srcUnit,
        //                                                         imageAttributes != null
        //                                                          ? imageAttributes.nativeImageAttr
        //                                                          : new GpImageAttributes(),
        //                                                         IntPtr.Zero,
        //                                                         IntPtr.Zero));
        //}

        //public void DrawImage(ImagePlus image,
        //                 int x,
        //                 int y,
        //                 int srcx,
        //                 int srcy,
        //                 int srcwidth,
        //                 int srcheight,
        //                 Unit srcUnit)
        //{
        //    SetStatus(GdiPlus.GdipDrawImagePointRectI(nativeGraphics,
        //                                                         image != null ? image.nativeImage
        //                                                               : null,
        //                                                         x,
        //                                                         y,
        //                                                         srcx,
        //                                                         srcy,
        //                                                         srcwidth,
        //                                                         srcheight,
        //                                                         srcUnit));
        //}

        //public void DrawImage(ImagePlus image,
        //                  Rectangle destRect,
        //                 int srcx,
        //                 int srcy,
        //                 int srcwidth,
        //                 int srcheight,
        //                 Unit srcUnit,
        //                  ImageAttributesPlus imageAttributes)
        //{
        //    SetStatus(GdiPlus.GdipDrawImageRectRectI(nativeGraphics,
        //                                                        image != null ? image.nativeImage
        //                                                              : null,
        //                                                        destRect.X,
        //                                                        destRect.Y,
        //                                                        destRect.Width,
        //                                                        destRect.Height,
        //                                                        srcx,
        //                                                        srcy,
        //                                                        srcwidth,
        //                                                        srcheight,
        //                                                        srcUnit,
        //                                                        imageAttributes != null
        //                                                        ? imageAttributes.nativeImageAttr
        //                                                        : new GpImageAttributes(),
        //                                                        IntPtr.Zero,
        //                                                        IntPtr.Zero));
        //}

        public void DrawImage(ImagePlus image,
                          Point[] destPoints,
                         int srcx,
                         int srcy,
                         int srcwidth,
                         int srcheight,
                         Unit srcUnit,
                          ImageAttributesPlus imageAttributes)
        {
            SetStatus(GdiPlus.GdipDrawImagePointsRectI(nativeGraphics,
                                                                  image != null ? image.nativeImage
                                                                        : null,
                                                                  destPoints,
                                                                  destPoints.Length,
                                                                  srcx,
                                                                  srcy,
                                                                  srcwidth,
                                                                  srcheight,
                                                                  srcUnit,
                                                                  imageAttributes != null
                                                                   ? imageAttributes.nativeImageAttr
                                                                   : new GpImageAttributes(),
                                                                  IntPtr.Zero,
                                                                  IntPtr.Zero));
        }



        //GpStatus SetClip(HRGN hRgn, CombineMode combineMode)
        //{
        //    SetStatus(GdiPlus.GdipSetClipHrgn(nativeGraphics, hRgn, combineMode));
        //}


        bool IsVisible(int x, int y)
        {
            return IsVisible(new Point(x, y));
        }

        bool IsVisible(Point point)
        {
            bool booln = false;

            SetStatus(GdiPlus.GdipIsVisiblePathPointI(new GpPath(),
                                                      point.X,
                                                      point.Y,
                                                      nativeGraphics,
                                                      out booln));

            return booln;
        }


        bool IsVisible(float x,  float y)
        {
            return IsVisible(new PointF(x, y));
        }

        bool IsVisible(PointF point)
        {
            bool booln = false;

            SetStatus(GdiPlus.GdipIsVisiblePathPoint(new GpPath(),
                                                     point.X,
                                                     point.Y,
                                                     nativeGraphics,
                                                     out booln));

            return booln;
        }


        public GraphicsState Save()
        {
            GraphicsState gstate;

            SetStatus(GdiPlus.GdipSaveGraphics(nativeGraphics, out gstate));

            return gstate;
        }

       public  void Restore(GraphicsState gstate)
        {
            SetStatus(GdiPlus.GdipRestoreGraphics(nativeGraphics, gstate));
        }


        public GpStatus GetLastStatus()
        {
            GpStatus lastStatus = lastResult;
            lastResult = GpStatus.Ok;

            return lastStatus;
        }


        protected GraphicsPlus(GpGraphics Graphics)
        {
            lastResult = GpStatus.Ok;
            SetNativeGraphics(Graphics);
        }

        protected void SetNativeGraphics(GpGraphics Graphics)
        {
            this.nativeGraphics = Graphics;
        }

        internal GpGraphics GetNativeGraphics()
        {
            return this.nativeGraphics;
        }

        internal GpPen GetNativePen(PenPlus pen)
        {
            return pen.nativePen;
        }

        internal GpGraphics nativeGraphics;
        protected GpStatus lastResult;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (g != null && hdc != null)
                {
                    g.ReleaseHdc(hdc);
                }
            }
            if ((IntPtr)nativeGraphics != IntPtr.Zero)
            {
                GdiPlus.GdipDeleteGraphics(nativeGraphics);
                nativeGraphics = new GpGraphics();
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        internal IntPtr GetGraphicHdc()
        {
            return hdc;
        }

        public void AlphaBlendImage(Image image, int x, int y, int alpha, bool srcAlpha)
        {
            IntPtr hdc = (IntPtr)this.GetHDC();
            GdiExt.AlphaBlendImage(hdc, image, x, y, alpha, srcAlpha);

            this.ReleaseHDC(hdc);
        }
    };

}

