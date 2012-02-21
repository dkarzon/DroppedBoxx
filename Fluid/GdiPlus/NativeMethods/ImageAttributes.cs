using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Fluid.Drawing.GdiPlus
{
    internal partial class GdiPlus
    {

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipCreateImageAttributes(out GpImageAttributes imageattr);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipCloneImageAttributes(GpImageAttributes imageattr,
            out GpImageAttributes cloneImageattr);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipDisposeImageAttributes(GpImageAttributes imageattr);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetImageAttributesToIdentity(GpImageAttributes imageattr,
                    ColorAdjustType type);
        [DllImport(dllName)]
        internal static extern GpStatus
            GdipResetImageAttributes(GpImageAttributes imageattr,
            ColorAdjustType type);


        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetImageAttributesThreshold(GpImageAttributes imageattr,
                   ColorAdjustType type,
                   bool enableFlag,
                   float threshold);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetImageAttributesGamma(GpImageAttributes imageattr,
               ColorAdjustType type,
               bool enableFlag,
               float gamma);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetImageAttributesNoOp(GpImageAttributes imageattr,
              ColorAdjustType type,
              bool enableFlag);

        [DllImport(dllName)]
        internal static extern GpStatus
            GdipSetImageAttributesColorKeys(GpImageAttributes imageattr,
                   ColorAdjustType type,
                   bool enableFlag,
                   int colorLow,
                   int colorHigh);
    }
}
