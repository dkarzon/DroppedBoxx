using System;
using System.Collections.Generic;
using System.Text;

namespace Fluid.Drawing.GdiPlus
{
    public class CustomLineCap
    {
        public CustomLineCap(GpCustomLineCap nativeCap, GpStatus status)
        {
            lastResult = status;
            SetNativeCap(nativeCap);
        }
        public CustomLineCap()
        {
            lastResult = GpStatus.Ok;
        }

        void SetNativeCap(GpCustomLineCap nativeCap)
        {
            this.nativeCap = nativeCap;
        }

        GpStatus SetStatus(GpStatus status)
        {
            if (status != GpStatus.Ok)
                return (lastResult = status);
            else
                return status;
        }


        public CustomLineCap(
            GraphicsPath fillPath,
            GraphicsPath strokePath,
            LineCap baseCap,
            float baseInset
            )
        {
            nativeCap = new GpCustomLineCap();
            GpPath nativeFillPath = null;
            GpPath nativeStrokePath = null;

            if (fillPath != null)
                nativeFillPath = fillPath.nativePath;
            if (strokePath != null)
                nativeStrokePath = strokePath.nativePath;

            lastResult = GdiPlus.GdipCreateCustomLineCap(
                            nativeFillPath, nativeStrokePath,
                            baseCap, baseInset, out nativeCap);
        }

        ~CustomLineCap()
        {
            GdiPlus.GdipDeleteCustomLineCap(nativeCap);
        }

        public GpStatus SetStrokeCaps(GpLineCap startCap, GpLineCap endCap)
        {
            return SetStatus(GdiPlus.GdipSetCustomLineCapStrokeCaps(nativeCap,
                        startCap, endCap));
        }

        internal GpCustomLineCap nativeCap;
        internal GpStatus lastResult;

    }
}
