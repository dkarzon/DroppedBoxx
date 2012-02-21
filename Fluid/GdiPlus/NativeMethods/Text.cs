using System;

using System.Collections.Generic;
using System.Text;
using Fluid.Drawing.GdiPlus;

namespace Fluid.GdiPlus.NativeMethods
{
    internal partial class GdiPlus
    {
        //GpStatus
        //DrawString(
        //     string text,
        //     int           length,
        //     FontPlus         font,
        //     RectangleF        layoutRect,
        //     StringFormatPlus stringFormat,
        //     BrushPlus        brush
        //)
        //{
        //    return SetStatus(NativeMethods.GdipDrawString(
        //        nativeGraphics,
        //        text,
        //        length,
        //        font != null? font.nativeFont : null,
        //        layoutRect,
        //        stringFormat != null ? stringFormat.nativeFormat : null,
        //        brush != null? brush.nativeBrush : null
        //    ));
        //}

        //GpStatus
        //DrawString(
        //     string text,
        //    int                 length,
        //     FontPlus         font,
        //     PointF       origin,
        //     BrushPlus        brush
        //)
        //{
        //    RectangleF rect = new RectangleF(origin.X, origin.Y, 0.0f, 0.0f);

        //    return SetStatus(NativeMethods.GdipDrawString(
        //        nativeGraphics,
        //        text,
        //        length,
        //        font != null? font.nativeFont : null,
        //        rect,
        //        null,
        //        brush != null? brush.nativeBrush : null
        //    ));
        //}

        //GpStatus
        //DrawString(
        //     string text,
        //     int                 length,
        //     FontPlus         font,
        //     PointF       origin,
        //     StringFormatPlus stringFormat,
        //     BrushPlus        brush
        //)
        //{
        //    RectangleF rect = new RectangleF(origin.X, origin.Y, 0.0f, 0.0f);

        //    return SetStatus(NativeMethods.GdipDrawString(
        //        nativeGraphics,
        //        text,
        //        length,
        //        font != null? font.nativeFont : null,
        //        &rect,
        //        stringFormat != null? stringFormat.nativeFormat : null,
        //        brush != null? brush.nativeBrush : null
        //    ));
        //}

        //GpStatus
        //MeasureString(
        //     string text,
        //     int    length,
        //     FontPlus         font,
        //     RectangleF        layoutRect,
        //     StringFormatPlus stringFormat,
        //    out RectangleF             boundingBox,
        //    out int               codepointsFitted,
        //    out int               linesFilled
        //)
        //{
        //    return SetStatus(NativeMethods.GdipMeasureString(
        //        nativeGraphics,
        //        text,
        //        length,
        //        font != null? font.nativeFont : null,
        //        layoutRect,
        //        stringFormat != null? stringFormat.nativeFormat : null,
        //        boundingBox,
        //        codepointsFitted,
        //        linesFilled
        //    ));
        //}

        //GpStatus
        //MeasureString(
        //     string text,
        //    int                 length,
        //     FontPlus         font,
        //     SizeF        layoutRectSize,
        //     StringFormatPlus stringFormat,
        //    out SizeF             size,
        //    out int               codepointsFitted,
        //    out int               linesFilled
        //)
        //{
        //    RectangleF   layoutRect= new RectangleF(0, 0, layoutRectSize.Width, layoutRectSize.Height);
        //    RectangleF   boundingBox;
        //    GpStatus  status;

        //    if (size == null)
        //    {
        //        return SetStatus(InvalidParameter);
        //    }

        //    status = SetStatus(NativeMethods.GdipMeasureString(
        //        nativeGraphics,
        //        text,
        //        length,
        //        font != null? font.nativeFont : null,
        //        layoutRect,
        //        stringFormat != null? stringFormat.nativeFormat : null,
        //        size != null? boundingBox : null,
        //        codepointsFitted,
        //        linesFilled
        //    ));

        //    if (size != null && status == GpStatus.Ok)
        //    {
        //        size.Width  = boundingBox.Width;
        //        size.Height = boundingBox.Height;
        //    }

        //    return status;
        //}

        //GpStatus
        //MeasureString(
        //     string text,
        //    int                 length,
        //     FontPlus         font,
        //     PointF       origin,
        //     StringFormatPlus stringFormat,
        //    out RectangleF             boundingBox
        //)
        //{
        //    RectangleF rect = new RectangleF(origin.X, origin.Y, 0.0f, 0.0f);

        //    return SetStatus(NativeMethods.GdipMeasureString(
        //        nativeGraphics,
        //        text,
        //        length,
        //        font != null? font.nativeFont : null,
        //        out rect,
        //        stringFormat != null? stringFormat.nativeFormat : null,
        //        boundingBox,
        //        null,
        //        null
        //    ));
        //}


        //GpStatus
        //MeasureCharacterRanges(
        //    string text,
        //    int                 length,
        //     FontPlus         font,
        //     RectangleF        layoutRect,
        //     StringFormatPlus stringFormat,
        //    int                 regionCount,
        //    RegionPlus[]            regions
        //)
        //{
        //    if (!regions || regionCount <= 0)
        //    {
        //        return InvalidParameter;
        //    }

        //    GpRegion[] nativeRegions = new GpRegion [regionCount];


        //    for (int i = 0; i < regionCount; i++)
        //    {
        //        nativeRegions[i] = regions[i].nativeRegion;
        //    }

        //    GpStatus status = SetStatus(NativeMethods.GdipMeasureCharacterRanges(
        //        nativeGraphics,
        //        text,
        //        length,
        //        font != null? font.nativeFont : null,
        //        layoutRect,
        //        stringFormat != null? stringFormat.nativeFormat : null,
        //        regionCount,
        //        nativeRegions
        //    ));


        //    return status;
        //}


    }
}
