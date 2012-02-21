using System;
using System.Collections.Generic;
using System.Text;
using Color = System.Drawing.Color;

namespace Fluid.Drawing.GdiPlus
{
    public class PenPlus: IDisposable
    {
        public PenPlus(Color color, float width)
        {
            Unit unit = Unit. UnitWorld;
            nativePen = null;
            lastResult = GdiPlus.GdipCreatePen1(color.ToArgb(), width, unit, out nativePen);
        }

        public PenPlus(BrushPlus brush, float width)
        {
            Unit unit = Unit.UnitWorld;
            nativePen = null;
            lastResult = GdiPlus.GdipCreatePen2(brush.nativeBrush,  width, unit, out nativePen);
        }
        public PenPlus(Color color, float width, bool opaque)
        {
            int c = color.ToArgb();
            if (opaque) c |= (0xff << 24);
            Unit unit = Unit.UnitWorld;
            nativePen = null;
            lastResult = GdiPlus.GdipCreatePen1(c, width, unit, out nativePen);
        }

        ~PenPlus()
        {
            Dispose(true);
        }

        public PenPlus Clone()
        {
            GpPen clonePen = null;

            lastResult = GdiPlus.GdipClonePen(nativePen, out clonePen);

            return new PenPlus(clonePen, lastResult);
        }

        public void SetWidth(float width)
        {
            SetStatus(GdiPlus.GdipSetPenWidth(nativePen, width));
        }

        private void SetStatus(GpStatus gpStatus)
        {
            if (gpStatus != GpStatus.Ok) throw GdiPlusStatusException.Exception(gpStatus);
        }

        public float GetWidth()
        {
            float width;

            SetStatus(GdiPlus.GdipGetPenWidth(nativePen, out width));

            return width;
        }

        // Set/get line caps: start, end, and dash

        // Line cap and join APIs by using LineCap and LineJoin enums.

        public void SetLineCap(LineCap startCap,
                          LineCap endCap,
                          DashCap dashCap)
        {
            SetStatus(GdiPlus.GdipSetPenLineCap197819(nativePen,
                                       startCap, endCap, dashCap));
        }

        public void SetStartCap(LineCap startCap)
        {
            SetStatus(GdiPlus.GdipSetPenStartCap(nativePen, startCap));
        }

        public void SetEndCap(LineCap endCap)
        {
            SetStatus(GdiPlus.GdipSetPenEndCap(nativePen, endCap));
        }

        //public void SetDashCap(DashCap dashCap)
        //{
        //    SetStatus(NativeMethods.GdipSetPenDashCap197819(nativePen,
        //                               dashCap));
        //}

        public LineCap GetStartCap()
        {
            LineCap startCap;

            SetStatus(GdiPlus.GdipGetPenStartCap(nativePen, out startCap));

            return startCap;
        }

        public LineCap GetEndCap()
        {
            LineCap endCap;

            SetStatus(GdiPlus.GdipGetPenEndCap(nativePen, out endCap));

            return endCap;
        }

        public DashCap GetDashCap()
        {
            DashCap dashCap;

            SetStatus(GdiPlus.GdipGetPenDashCap197819(nativePen,
                                out dashCap));

            return dashCap;
        }

        public void SetLineJoin(LineJoin lineJoin)
        {
            SetStatus(GdiPlus.GdipSetPenLineJoin(nativePen, lineJoin));
        }

        public LineJoin GetLineJoin()
        {
            LineJoin lineJoin;

            SetStatus(GdiPlus.GdipGetPenLineJoin(nativePen, out lineJoin));

            return lineJoin;
        }

        public void SetCustomStartCap(CustomLineCap customCap)
        {
            GpCustomLineCap nativeCap = new GpCustomLineCap();
            if (customCap != null)
                nativeCap = customCap.nativeCap;

            SetStatus(GdiPlus.GdipSetPenCustomStartCap(nativePen,
                                                                  nativeCap));
        }

        public void GetCustomStartCap(out CustomLineCap customCap)
        {
            customCap = new CustomLineCap();
            SetStatus(GdiPlus.GdipGetPenCustomStartCap(nativePen,
                                                        out customCap.nativeCap));
        }

        public void SetCustomEndCap(CustomLineCap customCap)
        {
            GpCustomLineCap nativeCap = new GpCustomLineCap();
            if (customCap != null)
                nativeCap = customCap.nativeCap;

            SetStatus(GdiPlus.GdipSetPenCustomEndCap(nativePen,
                                                                nativeCap));
        }

        public void GetCustomEndCap(out CustomLineCap customCap)
        {
            customCap = new CustomLineCap();
            SetStatus(GdiPlus.GdipGetPenCustomEndCap(nativePen,
                                                        out customCap.nativeCap));
        }

        public void SetMiterLimit(float miterLimit)
        {
            SetStatus(GdiPlus.GdipSetPenMiterLimit(nativePen,
                                                        miterLimit));
        }

        public float GetMiterLimit()
        {
            float miterLimit;

            SetStatus(GdiPlus.GdipGetPenMiterLimit(nativePen, out miterLimit));

            return miterLimit;
        }

        public void SetAlignment(PenAlignment penAlignment)
        {
            SetStatus(GdiPlus.GdipSetPenMode(nativePen, penAlignment));
        }

        public PenAlignment GetAlignment()
        {
            PenAlignment penAlignment;

            SetStatus(GdiPlus.GdipGetPenMode(nativePen, out penAlignment));

            return penAlignment;
        }

        //PenType GetPenType()
        //{
        //    PenType type;
        //    SetStatus(NativeMethods.GdipGetPenFillType(nativePen, out type));

        //    return type;
        //}

        public void SetColor(Color color)
        {
            SetStatus(GdiPlus.GdipSetPenColor(nativePen,
                                                         color.ToArgb()));
        }

        public void SetBrush(BrushPlus brush)
        {
            SetStatus(GdiPlus.GdipSetPenBrushFill(nativePen,
                                           brush.nativeBrush));
        }

        public Color GetColor()
        {

            int argb;
            SetStatus(GdiPlus.GdipGetPenColor(nativePen,  out argb));
            return Color.FromArgb(argb);
        }

        public DashStyle GetDashStyle()
        {
            DashStyle dashStyle;

            SetStatus(GdiPlus.GdipGetPenDashStyle(nativePen, out dashStyle));

            return dashStyle;
        }

        public void SetDashStyle(DashStyle dashStyle)
        {
            SetStatus(GdiPlus.GdipSetPenDashStyle(nativePen,
                                                             dashStyle));
        }

        public float GetDashOffset()
        {
            float dashOffset;

            SetStatus(GdiPlus.GdipGetPenDashOffset(nativePen, out dashOffset));

            return dashOffset;
        }

        public void SetDashOffset(float dashOffset)
        {
            SetStatus(GdiPlus.GdipSetPenDashOffset(nativePen,
                                                              dashOffset));
        }

        public void SetDashPattern(float[] dashArray)
        {
            SetStatus(GdiPlus.GdipSetPenDashArray(nativePen,
                                                             dashArray,
                                                             dashArray.Length));
        }

        public int GetDashPatternCount()
        {
            int count = 0;

            SetStatus(GdiPlus.GdipGetPenDashCount(nativePen, out count));

            return count;
        }

        public void GetDashPattern(float[] dashArray)
        {
            if (dashArray == null || dashArray.Length == 0)
                SetStatus(GpStatus.InvalidParameter);

            SetStatus(GdiPlus.GdipGetPenDashArray(nativePen,
                                                             dashArray,
                                                             dashArray.Length));
        }


        public GpStatus GetLastStatus()
        {
            GpStatus lastStatus = lastResult;
            lastResult = GpStatus.Ok;

            return lastStatus;
        }




        protected PenPlus(GpPen nativePen, GpStatus status)
        {
            lastResult = status;
            SetNativePen(nativePen);
        }

        void SetNativePen(GpPen nativePen)
        {
            this.nativePen = nativePen;
        }


        internal GpPen nativePen;
        protected GpStatus lastResult;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            // free native resources if there are any.
            if ((IntPtr)nativePen!= IntPtr.Zero)
            {
                GdiPlus.GdipDeletePen(nativePen);
                nativePen = new GpPen();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
