using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Fluid.Drawing.GdiPlus
{
    internal partial class GdiPlus
    {
        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawLine(GpGraphics graphics, GpPen pen, float x1, float y1, float x2, float y2);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawLineI(GpGraphics graphics, GpPen pen, int x1, int y1,  int x2, int y2);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawLines(GpGraphics graphics, GpPen pen, PointF[] points, int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawLinesI(GpGraphics graphics, GpPen pen, Point[] points, int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawArc(GpGraphics graphics, GpPen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawArcI(GpGraphics graphics, GpPen pen, int x, int y, int width, int height, float startAngle, float sweepAngle);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawBezier(GpGraphics graphics, GpPen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawBezierI(GpGraphics graphics, GpPen pen, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawBeziers(GpGraphics graphics, GpPen pen, PointF[] points,  int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawBeziersI(GpGraphics graphics, GpPen pen, Point[] points, int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawRectangle(GpGraphics graphics, GpPen pen, float x, float y, float width, float height);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawRectangleI(GpGraphics graphics, GpPen pen, int x, int y, int width, int height);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawRectangles(GpGraphics graphics, GpPen pen, RectangleF[] rects, int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawRectanglesI(GpGraphics graphics, GpPen pen, Rectangle[] rects, int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawEllipse(GpGraphics graphics, GpPen pen, float x, float y, float width, float height);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawEllipseI(GpGraphics graphics, GpPen pen, int x, int y, int width, int height);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawPie(GpGraphics graphics, GpPen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawPieI(GpGraphics graphics, GpPen pen, int x, int y, int width, int height, float startAngle, float sweepAngle);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawPolygon(GpGraphics graphics, GpPen pen, PointF[] points, int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawPolygonI(GpGraphics graphics, GpPen pen, Point[] points, int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawPath(GpGraphics graphics, GpPen pen, GpPath path);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawCurve(GpGraphics graphics, GpPen pen, PointF[] points, int count);

        [DllImport(dllName)]
        extern static internal GpStatus GdipDrawCurveI(GpGraphics graphics, GpPen pen, Point[] points, int count);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipDrawCurve2(GpGraphics graphics, GpPen pen, PointF[] points,
        int count, float tension);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipDrawCurve2I(GpGraphics graphics, GpPen pen, Point[] points,
        int count, float tension);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipDrawCurve3(GpGraphics graphics, GpPen pen, PointF[] points,
int count, int offset, int numberOfSegments, float tension);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipDrawCurve3I(GpGraphics graphics, GpPen pen, Point[] points,
 int count, int offset, int numberOfSegments, float tension);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipDrawClosedCurve(GpGraphics graphics, GpPen pen,
      PointF[] points, int count);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipDrawClosedCurveI(GpGraphics graphics, GpPen pen,
       Point[] points, int count);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipDrawClosedCurve2(GpGraphics graphics, GpPen pen,
       PointF[] points, int count, float tension);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipDrawClosedCurve2I(GpGraphics graphics, GpPen pen,
        Point[] points, int count, float tension);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipGraphicsClear(GpGraphics graphics, int color);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipFillRectangle(GpGraphics graphics, GpBrush brush, float x, float y,
   float width, float height);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipFillRectangleI(GpGraphics graphics, GpBrush brush, int x, int y,
    int width, int height);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipFillRectangles(GpGraphics graphics, GpBrush brush,
     RectangleF[] rects, int count);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipFillRectanglesI(GpGraphics graphics, GpBrush brush,
      Rectangle[] rects, int count);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipFillPolygon(GpGraphics graphics, GpBrush brush,
  PointF[] points, int count, FillMode fillMode);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipFillPolygon(GpGraphics graphics, GpSolidFill brush,
  PointF[] points, int count, FillMode fillMode);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipFillPolygon(GpGraphics graphics, GpHatch brush,
  PointF[] points, int count, FillMode fillMode);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipFillPolygon(GpGraphics graphics, GpTexture brush,
  PointF[] points, int count, FillMode fillMode);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipFillPolygonI(GpGraphics graphics, GpBrush brush,
   Point[] points, int count, FillMode fillMode);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipFillPolygon2(GpGraphics graphics, GpBrush brush,
   PointF[] points, int count);

        [DllImport(dllName)]
        extern static internal GpStatus
        GdipFillPolygon2I(GpGraphics graphics, GpBrush brush,
        Point[] points, int count);

        [DllImport(dllName)]
        extern static internal GpStatus
        GdipFillPolygon2I(GpGraphics graphics, GpSolidFill brush,
        Point[] points, int count);

        [DllImport(dllName)]
        extern static internal GpStatus
        GdipFillEllipse(GpGraphics graphics, GpBrush brush, float x, float y,
                        float width, float height);

        [DllImport(dllName)]
        extern static internal GpStatus
GdipFillEllipseI(GpGraphics graphics, GpBrush brush, int x, int y,
  int width, int height);

        [DllImport(dllName)]
        extern static internal GpStatus GdipFillPie(GpGraphics graphics, GpBrush brush, float x, float y,
float width, float height, float startAngle, float sweepAngle);

        [DllImport(dllName)]
        extern static internal GpStatus GdipFillPieI(GpGraphics graphics, GpBrush brush, int x, int y,
int width, int height, float startAngle, float sweepAngle);

        [DllImport(dllName)]
        extern static internal GpStatus GdipFillPath(GpGraphics graphics, GpBrush brush, GpPath path);

        //[DllImport(dllName)]
        //extern static internal GpStatus GdipFillClosedCurve(GpGraphics graphics, GpBrush brush,
        //        PointF[] points, int count);

        //[DllImport(dllName)]
        //extern static internal GpStatus GdipFillClosedCurveI(GpGraphics graphics, GpBrush brush,
        //        Point[] points, int count);

        //[DllImport(dllName)]
        //extern static internal GpStatus GdipFillClosedCurve2(GpGraphics graphics, GpBrush brush,
        //        PointF[] points, int count,
        //       float tension, FillMode fillMode);

        //[DllImport(dllName)]
        //extern static internal GpStatus GdipFillClosedCurve2I(GpGraphics graphics, GpBrush brush,
        //        Point[] points, int count,
        //       float tension, FillMode fillMode);

        //[DllImport(dllName)]
        //extern static internal GpStatus GdipFillRegion(GpGraphics graphics, GpBrush brush, GpRegion region);
    }
}
