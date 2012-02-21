using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Fluid.Drawing.GdiPlus
{
    internal partial class GdiPlus
    {
        [DllImport(dllName)]
        internal static extern GpStatus GdipCreateCustomLineCap(GpPath fillPath, GpPath strokePath, LineCap baseCap, float baseInset, out GpCustomLineCap customCap);

        [DllImport(dllName)]
        internal static extern GpStatus GdipDeleteCustomLineCap(GpCustomLineCap customCap);

        [DllImport(dllName)]
        internal static extern GpStatus GdipSetCustomLineCapStrokeCaps(GpCustomLineCap customCap, GpLineCap startCap, GpLineCap endCap);


    }
}
