using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;

namespace Fluid.Drawing.GdiPlus
{
    internal partial class GdiPlus
    {
        [DllImport(dllName)]
        internal static extern GpStatus GdipCreatePath(FillMode brushMode, out GpPath path);

        [DllImport(dllName)]
        internal static extern GpStatus GdipClonePath(GpPath path, out GpPath clonePath);

        [DllImport(dllName)]
        internal static extern GpStatus GdipDeletePath(GpPath path);

        [DllImport(dllName)]
        internal static extern GpStatus GdipResetPath(GpPath path);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetPointCount(GpPath path, out int count);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetPathTypes(GpPath path, byte[] types, int count);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetPathPoints(GpPath path, PointF[] points, int count);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetPathPointsI(GpPath path, Point[] points, int count);

        [DllImport(dllName)]
        internal static extern GpStatus GdipGetPathFillMode(GpPath path, out FillMode fillmode);

        [DllImport(dllName)]
        internal static extern GpStatus GdipSetPathFillMode(GpPath path, FillMode fillmode);

        [DllImport(dllName)]
        internal static extern GpStatus GdipStartPathFigure(GpPath path);

        [DllImport(dllName)]
        internal static extern GpStatus GdipClosePathFigure(GpPath path);

        [DllImport(dllName)]
        internal static extern GpStatus GdipClosePathFigures(GpPath path);


        [DllImport(dllName)]
        internal static extern GpStatus GdipGetPathLastPoint(GpPath path, out PointF lastPoint);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathLine(GpPath path, float x1, float y1, float x2, float y2);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathLine2(GpPath path, PointF[] points, int count);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathArc(GpPath path, float x, float y, float width, float height, float startAngle, float sweepAngle);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathBeziers(GpPath path, PointF[] points, int count);


        [DllImport(dllName)]
        internal static extern GpStatus
        GdipAddPathRectangle(GpPath path, float x, float y, float width, float height);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathRectangles(GpPath path, RectangleF[] rects, int count);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathEllipse(GpPath path, float x, float y, float width, float height);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathPie(GpPath path, float x, float y, float width, float height, float startAngle, float sweepAngle);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathPolygon(GpPath path, PointF[] points, int count);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathPath(GpPath path, GpPath addingPath, bool connect);


        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathLine2I(GpPath path, Point[] points, int count);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathArcI(GpPath path, int x, int y, int width, int height, float startAngle, float sweepAngle);


        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathBeziersI(GpPath path, Point[] points, int count);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathRectangleI(GpPath path, int x, int y, int width, int height);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathRectanglesI(GpPath path, Rectangle[] rects, int count);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathEllipseI(GpPath path, int x, int y, int width, int height);

        [DllImport(dllName)]
        internal static extern GpStatus GdipAddPathPieI(GpPath path, int x, int y, int width, int height, float startAngle, float sweepAngle);

        [DllImport(dllName)]
        internal static extern GpStatus GdipIsVisiblePathPoint(GpPath path, float x, float y, GpGraphics graphics, out bool result);

        [DllImport(dllName)]
        internal static extern GpStatus GdipIsVisiblePathPointI(GpPath path, int x, int y, GpGraphics graphics, out bool result);

        [DllImport(dllName)]
        internal static extern GpStatus GdipIsOutlineVisiblePathPoint(GpPath path, float x, float y, GpPen pen, GpGraphics graphics, out bool result);

    }
}
