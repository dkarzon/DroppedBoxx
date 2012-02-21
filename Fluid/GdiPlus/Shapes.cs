using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Fluid.Drawing.GdiPlus
{
    public enum GraphicShape
    {
        Rounded,
        Ellipse,
        Rectangle,
        Back,
        Next
    }

    public partial class GraphicsPlus
    {
        enum PathMode
        {
            All,
            Top,
            Bottom
        }


        private static GraphicsPath GetEllipsePath(Rectangle rect, int r, int reflectionHeight, PathMode mode, RoundedCorners corners)
        {
            GraphicsPath path = new GraphicsPath();
            switch (mode)
            {
                case PathMode.All:
                    path.AddEllipse(rect);
                    break;

                case PathMode.Top:
                    path.AddArc(rect, 180, 180);
                    break;

                case PathMode.Bottom:
                    path.AddArc(rect, 0, 180);
                    break;
            }

            path.CloseFigure();

            return path;

        }

        private static GraphicsPath GetRectPath(Rectangle rect, int r, int reflectionHeight, PathMode mode, RoundedCorners corners)
        {
            switch (mode)
            {
                case PathMode.Top:
                    rect.Height = reflectionHeight;
                    break;

                case PathMode.Bottom:
                    rect.Height = rect.Height - reflectionHeight;
                    rect.Y += reflectionHeight;
                    break;

            }

            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(rect);
            //            path.CloseFigure();
            return path;
        }

        private static GraphicsPath GetRoundPath(Rectangle rect, int r, int reflectionHeight, PathMode mode, RoundedCorners corners)
        {
            GraphicsPath path = new GraphicsPath();
            int x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height;

            if (mode != PathMode.Bottom)
            {
                if ((corners & RoundedCorners.TopLeft) != 0) path.AddArc(x, y, r, r, 180, 90); else path.AddLine(x, y, x + r, y);
                if ((corners & RoundedCorners.TopRight) != 0) path.AddArc(x + w - r, y, r, r, 270, 90); else path.AddLine(x + w - r, y, x + w, y);
            }
            RectangleF gr = new RectangleF(x, y + r, w, h - r);

            if (mode != PathMode.All)
            {
                Point pt1 = new Point(x + w, y + reflectionHeight);
                Point pt2 = new Point(x, y + reflectionHeight);
                if (mode == PathMode.Top)
                {
                    path.AddLine(pt1, pt2);
                }
                else
                {
                    //pt1.Y--;
                    //pt2.Y--;
                    path.AddLine(pt2, pt1);
                }
            }
            if (mode != PathMode.Top)
            {
                if ((corners & RoundedCorners.BottomRight) != 0) path.AddArc(x + w - r, y + h - r, r, r, 0, 90); else path.AddLine(x + w, y + h, x + w - r, y + h);
                if ((corners & RoundedCorners.BottomLeft) != 0) path.AddArc(x, y + h - r, r, r, 90, 90); else path.AddLine(x + r, y + h, x, y + h);
            }

            path.CloseFigure();
            return path;
        }

        private static GraphicsPath GetBackPath(Rectangle rect, int r, int reflectionHeight, PathMode mode, RoundedCorners corners)
        {
            GraphicsPath path = new GraphicsPath();
            int x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height;
            int h2 = rect.Height / 2;
            int offset = (h - h2 * 2);
            int r2 = r / 2;

            if (mode != PathMode.Bottom)
            {
                path.AddArc(x, y + h2 - r2, r, r, 180, 45);
                path.AddArc(x + h2 - r2, y, r, r, 180 + 45, 45);
                path.AddArc(x + w - r, y, r, r, 270, 90);
            }
            RectangleF gr = new RectangleF(x, y + r, w, h - r);

            if (mode != PathMode.All)
            {
                Point pt1 = new Point(x + w, y + h2);
                Point pt2 = new Point(x, y + h2);
                if (mode == PathMode.Top)
                {
                    path.AddLine(pt1, pt2);
                }
                else
                {
                    path.AddLine(pt2, pt1);
                }
            }
            if (mode != PathMode.Top)
            {
                path.AddArc(x + w - r, y + h - r, r, r, 0, 90);
                path.AddArc(x + h2 - r2 + offset, y + h - r, r, r, 90, 45);
                path.AddArc(x, y + h2 - r2, r, r, 90 + 45, 45);
            }

            path.CloseFigure();
            return path;
        }

        private static GraphicsPath GetNextPath(Rectangle rect, int r, int reflectionHeight, PathMode mode, RoundedCorners corners)
        {
            GraphicsPath path = new GraphicsPath();
            int x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height;
            int h2 = rect.Height / 2;
            int offset = (h - h2 * 2);
            int r2 = r / 2;

            if (mode != PathMode.Bottom)
            {
                path.AddArc(x, y, r, r, 180, 90);
                path.AddArc(x + w - h2 - r2, y, r, r, 270, 45);
                path.AddArc(x + w - r, y + h2 - r2, r, r, 270 + 45, 45);
            }
            RectangleF gr = new RectangleF(x, y + r, w, h - r);

            if (mode != PathMode.All)
            {
                Point pt1 = new Point(x + w, y + h2);
                Point pt2 = new Point(x, y + h2);
                if (mode == PathMode.Top)
                {
                    path.AddLine(pt1, pt2);
                }
                else
                {
                    path.AddLine(pt2, pt1);
                }
            }
            if (mode != PathMode.Top)
            {
                path.AddArc(x + w - r, y + h2 - r2, r, r, 0, 45);
                path.AddArc(x + w - h2 - r2 + offset, y + h - r, r, r, 45, 45);
                path.AddArc(x, y + h - r, r, r, 90, 90);
            }

            path.CloseFigure();
            return path;
        }

        public void DrawRoundRectangle(Rectangle rect, int radius, PenPlus pen)
        {
            if (rect.Width < 1 || rect.Height < 1) return;
            GraphicsPath path = GetRoundPath(rect, radius, 0, PathMode.All, RoundedCorners.All);
            this.DrawPath(pen, path);
        }

        public void DrawShape(Rectangle rect, int radius, PenPlus pen, GraphicShape shape)
        {
            if (rect.Width < 1 || rect.Height < 1) return;
            PathFunc func = GetShapeFunc(shape);
            GraphicsPath path = func(rect, radius, 0, PathMode.All, RoundedCorners.All);
            this.DrawPath(pen, path);
        }

        public void DrawShape(Rectangle rect, int radius, PenPlus pen, GraphicShape shape, RoundedCorners corners)
        {
            if (rect.Width < 1 || rect.Height < 1) return;
            PathFunc func = GetShapeFunc(shape);
            GraphicsPath path = func(rect, radius, 0, PathMode.All, corners);
            this.DrawPath(pen, path);
        }

        public void FillRoundRectangle(Rectangle rect, int radius, BrushPlus brush)
        {
            if (rect.Width < 1 || rect.Height < 1) return;
            GraphicsPath path = GetRoundPath(rect, radius, 0, PathMode.All, RoundedCorners.All);
            this.FillPath(brush, path);
        }

        public void FillRoundRectangle(Rectangle rect, int radius, Color color)
        {
            if (rect.Width < 1 || rect.Height < 1) return;
            GraphicsPath path = GetRoundPath(rect, radius, 0, PathMode.All, RoundedCorners.All);
            using (BrushPlus brush = new SolidBrushPlus(color))
            {
                this.FillPath(brush, path);
            }
        }

        public enum GradientMode
        {
            Top,
            Bottom
        }


        /// <summary>
        /// Paints a rounded rectangle with a gradient.
        /// </summary>
        /// <param name="rect">The rectangle to paint.</param>
        /// <param name="radius">The radius of the corner.</param>
        /// <param name="startColor">The start color of the gradiant.</param>
        /// <param name="endColor">The end color of the gradiant.</param>
        /// <param name="gradientMode">The gradiant mode.</param>
        public void GradientFillRoundRectangle(Rectangle rect, int radius, Color startColor, Color endColor, GradientMode gradientMode)
        {
            GradientFillShape(rect, radius, startColor, endColor, gradientMode, GraphicShape.Rounded, RoundedCorners.All);
        }

        /// <summary>
        /// Paints a rounded rectangle with a gradient.
        /// </summary>
        /// <param name="rect">The rectangle to paint.</param>
        /// <param name="radius">The radius of the corner.</param>
        /// <param name="startColor">The start color of the gradiant.</param>
        /// <param name="endColor">The end color of the gradiant.</param>
        /// <param name="gradientMode">The gradiant mode.</param>
        /// <param name="corners">Specifies which corners to be rounded.</param>
        public void GradientFillRoundRectangle(Rectangle rect, int radius, Color startColor, Color endColor, GradientMode gradientMode, RoundedCorners corners)
        {
            GradientFillShape(rect, radius, startColor, endColor, gradientMode, GraphicShape.Rounded, corners);
        }

        delegate GraphicsPath PathFunc(Rectangle rect, int r, int reflectionHeight, PathMode mode, RoundedCorners corners);

        public void GradientFillShape(Rectangle rect, int radius, Color startColor, Color endColor, GradientMode gradientMode, GraphicShape shape, RoundedCorners corners)
        {
            if (rect.Width < 1 || rect.Height < 1) return;
            PathFunc func = GetShapeFunc(shape);
            SmoothingMode mode = SmoothingMode;
            try
            {
                SmoothingMode = SmoothingMode.None;
                int x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height;
                int reflectionHeight = h / 2;


                PointF p1 = new PointF(x, y);
                PointF p2 = new PointF(x, y + reflectionHeight);

                if (gradientMode == GradientMode.Top)
                {
                    PathMode pathMode = endColor.A == 255 ? PathMode.Bottom : PathMode.All;
                    using (GraphicsPath path = func(rect, radius, reflectionHeight, pathMode, corners))
                    {
                        using (SolidBrushPlus brush = new SolidBrushPlus(endColor))
                        {
                            this.FillPath(brush, path);
                        }
                    }
                    using (GraphicsPath path = func(rect, radius, reflectionHeight, PathMode.Top, corners))
                    {
                        using (LinearGradientBrush brush = new LinearGradientBrush(p1, p2, startColor, endColor))
                        {
                            this.FillPath(brush, path);
                        }
                    }
                }
                else
                {
                    PathMode pathMode = endColor.A == 255 ? PathMode.Top : PathMode.All;
                    using (GraphicsPath path = func(rect, radius, reflectionHeight, PathMode.All, corners))
                    {
                        using (SolidBrushPlus brush = new SolidBrushPlus(endColor))
                        {
                            this.FillPath(brush, path);
                        }
                    }
                    using (GraphicsPath path = func(rect, radius, reflectionHeight, PathMode.Bottom, corners))
                    {
                        p1.Y = y + reflectionHeight - 1;
                        p2.Y = y + h;
                        using (LinearGradientBrush brush = new LinearGradientBrush(p1, p2, endColor, startColor))
                        {
                            this.FillPath(brush, path);
                        }
                    }
                }
            }
            finally
            {
                SmoothingMode = mode;
            }
        }

        private static PathFunc GetShapeFunc(GraphicShape shape)
        {
            switch (shape)
            {
                case GraphicShape.Ellipse: return GetEllipsePath;
                case GraphicShape.Back: return GetBackPath;
                case GraphicShape.Next: return GetNextPath;
                case GraphicShape.Rectangle: return GetRectPath;
                default: return GetRoundPath;
            }
        }
    }
}
