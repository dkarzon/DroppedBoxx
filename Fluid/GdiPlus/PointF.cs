using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Fluid.Drawing.GdiPlus
{
    public struct PointF
    {
        public float X;
        public float Y;

        public PointF(PointF point)
        {
            X = point.X;
            Y = point.Y;
        }

        public PointF(SizeF size)
        {
            this.X = size.Width;
            this.Y = size.Height;
        }

        public PointF(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public static PointF operator +(PointF point1, PointF point2)
        {
            return new PointF(point1.X + point2.X,  point1.Y + point2.Y);
        }

        public static PointF operator -(PointF point1, PointF point2)
        {
            return new PointF(point1.X - point2.X, point1.Y - point2.Y);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PointF)) return false;
            return Equals((PointF)obj);
        }

        public bool Equals(PointF point)
        {
            return (X == point.X) && (Y == point.Y);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() | Y.GetHashCode();
        }
    }
}


