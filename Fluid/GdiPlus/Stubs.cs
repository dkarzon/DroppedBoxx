using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Fluid.Drawing.GdiPlus
{

    [StructLayout(LayoutKind.Sequential)]
    public struct GpGraphics
    {
        public static implicit operator IntPtr(GpGraphics obj) { return obj.ptr; }
        internal IntPtr ptr;
        public IntPtr Handle { get { return ptr; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpMatrix
    {
        IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpPath
    {
        IntPtr ptr;
        public static implicit operator IntPtr(GpPath obj) { return obj.ptr; }
        public static implicit operator GpPath(string n) { if (n == null) return new GpPath(); else throw new ArgumentException(); }
        public IntPtr Handle { get { return ptr; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpFont
    {
        IntPtr ptr;
        public static implicit operator IntPtr(GpFont obj) { return obj.ptr; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpPen
    {
        IntPtr ptr;
        public static implicit operator IntPtr(GpPen obj) { return obj.ptr; }
        public static implicit operator GpPen(string n) { if (n == null) return new GpPen(); else throw new ArgumentException(); }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpBrush
    {
        internal IntPtr ptr;
        public static implicit operator IntPtr(GpBrush obj) { return obj.ptr; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpHatch
    {
        IntPtr ptr;
        public static implicit operator GpBrush(GpHatch me)
        {
            GpBrush br = new GpBrush();
            br.ptr = me.ptr;
            return br;
        }
        public static explicit operator GpHatch(GpBrush br)
        {
            GpHatch n = new GpHatch();
            n.ptr = br.ptr;
            return n;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpTexture
    {
        IntPtr ptr;
        public static implicit operator GpBrush(GpTexture me)
        {
            GpBrush br = new GpBrush();
            br.ptr = me.ptr;
            return br;
        }
        public static explicit operator GpTexture(GpBrush br)
        {
            GpTexture n = new GpTexture();
            n.ptr = br.ptr;
            return n;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpSolidFill
    {
        IntPtr ptr;
        public static implicit operator GpBrush(GpSolidFill me)
        {
            GpBrush br = new GpBrush();
            br.ptr = me.ptr;
            return br;
        }
        public static explicit operator GpSolidFill(GpBrush br)
        {
            GpSolidFill n = new GpSolidFill();
            n.ptr = br.ptr;
            return n;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpPathGradient
    {
        IntPtr ptr;
        public static implicit operator GpBrush(GpPathGradient me)
        {
            GpBrush br = new GpBrush();
            br.ptr = me.ptr;
            return br;
        }
        public static explicit operator GpPathGradient(GpBrush br)
        {
            GpPathGradient n = new GpPathGradient();
            n.ptr = br.ptr;
            return n;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpLineGradient
    {
        IntPtr ptr;
        public static implicit operator GpBrush(GpLineGradient me)
        {
            GpBrush br = new GpBrush();
            br.ptr = me.ptr;
            return br;
        }
        public static explicit operator GpLineGradient(GpBrush br)
        {
            GpLineGradient n = new GpLineGradient();
            n.ptr = br.ptr;
            return n;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpRegion
    {
        public IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpImage
    {
        IntPtr ptr;
        public static implicit operator GpImage(string n) { if (n == null) return new GpImage(); else throw new ArgumentException(); }
        public static implicit operator GpImage(IntPtr p) { GpImage img = new GpImage(); img.ptr = p; return img; }
        public static implicit operator IntPtr(GpImage obj) { return obj.ptr; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpBitmap
    {
        internal IntPtr ptr;
        public static explicit operator IntPtr(GpBitmap bm) { return bm.ptr; }
        public static explicit operator GpImage(GpBitmap bm)
        {
            GpImage img = (GpImage)(IntPtr)bm;
            return img;
        }
        public static implicit operator GpBitmap(IntPtr p) { GpBitmap bm = new GpBitmap(); bm.ptr = p; return bm; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpPathData
    {
        public int Count;
        public PointF[] Points;
        public byte[] Types;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpCachedBitmap
    {
        IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpLineCap
    {
        IntPtr ptr;
        public static implicit operator IntPtr(GpLineCap obj) { return obj.ptr; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpCustomLineCap
    {
        IntPtr ptr;
        public static implicit operator IntPtr(GpCustomLineCap obj) { return obj.ptr; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpImageAttributes
    {
        IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpFontFamily
    {
        IntPtr ptr;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GpStringFormat
    {
        IntPtr ptr;
    }



    public enum GpStatus
    {
        Ok = 0,
        GenericError = 1,
        InvalidParameter = 2,
        OutOfMemory = 3,
        ObjectBusy = 4,
        InsufficientBuffer = 5,
        NotImplemented = 6,
        Win32Error = 7,
        WrongState = 8,
        Aborted = 9,
        FileNotFound = 10,
        ValueOverflow = 11,
        AccessDenied = 12,
        UnknownImageFormat = 13,
        FontFamilyNotFound = 14,
        FontStyleNotFound = 15,
        NotTrueTypeFont = 16,
        UnsupportedGdiplusVersion = 17,
        GdiplusNotInitialized = 18,
        PropertyNotFound = 19,
        PropertyNotSupported = 20,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HDC
    {
        private HDC(IntPtr v) { val = v; }
        public IntPtr val;
        public static implicit operator IntPtr(HDC hdc) { return hdc.val; }
        public static implicit operator HDC(IntPtr hdc) { return new HDC(hdc); }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HANDLE
    {
        private HANDLE(IntPtr v) { val = v; }
        public IntPtr val;
        public static implicit operator IntPtr(HANDLE hdc) { return hdc.val; }
        public static implicit operator HANDLE(IntPtr hdc) { return new HANDLE(hdc); }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HWND
    {
        private HWND(IntPtr v) { val = v; }
        public IntPtr val;
        public static implicit operator IntPtr(HWND hdc) { return hdc.val; }
        public static implicit operator HWND(IntPtr hdc) { return new HWND(hdc); }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HRGN
    {
        private HRGN(IntPtr v) { val = v; }
        public IntPtr val;
        public static implicit operator IntPtr(HRGN hdc) { return hdc.val; }
        public static implicit operator HRGN(IntPtr hdc) { return new HRGN(hdc); }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HFONT
    {
        private HFONT(IntPtr v) { val = v; }
        public IntPtr val;
        public static implicit operator IntPtr(HFONT hdc) { return hdc.val; }
        public static implicit operator HFONT(IntPtr hdc) { return new HFONT(hdc); }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HBITMAP
    {
        private HBITMAP(IntPtr v) { val = v; }
        public IntPtr val;
        public static implicit operator IntPtr(HBITMAP v) { return v.val; }
        public static implicit operator HBITMAP(IntPtr v) { return new HBITMAP(v); }
    }

    public enum PROPID : ushort { }
}
