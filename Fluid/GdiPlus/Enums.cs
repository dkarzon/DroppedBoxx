using System;
using System.Collections.Generic;
using System.Text;

namespace Fluid.Drawing.GdiPlus
{
    //--------------------------------------------------------------------------
    // Fill mode constants
    //--------------------------------------------------------------------------

    public enum FillMode
    {
        Alternate,        // 0
        Winding           // 1
    };

    //--------------------------------------------------------------------------
    // Quality mode constants
    //--------------------------------------------------------------------------

    public enum QualityMode
    {
        Invalid = -1,
        Default = 0,
        Low = 1, // Best performance
        High = 2  // Best rendering quality
    };

    //--------------------------------------------------------------------------
    // Alpha Compositing mode constants
    //--------------------------------------------------------------------------

    public enum CompositingMode
    {
        SourceOver,    // 0
        SourceCopy     // 1
    };

    //--------------------------------------------------------------------------
    // Alpha Compositing quality constants
    //--------------------------------------------------------------------------

    public enum CompositingQuality
    {
        CompositingQualityInvalid = QualityMode.Invalid,
        CompositingQualityDefault = QualityMode.Default,
        CompositingQualityHighSpeed = QualityMode.Low,
        CompositingQualityHighQuality = QualityMode.High,
        CompositingQualityGammaCorrected,
        CompositingQualityAssumeLinear
    };

    //--------------------------------------------------------------------------
    // Unit constants
    //--------------------------------------------------------------------------

    public enum Unit
    {
        UnitWorld,      // 0 -- World coordinate (non-physical unit)
        UnitDisplay,    // 1 -- Variable -- for PageTransform only
        UnitPixel,      // 2 -- Each unit is one device pixel.
        UnitPoint,      // 3 -- Each unit is a printer's point, or 1/72 inch.
        UnitInch,       // 4 -- Each unit is 1 inch.
        UnitDocument,   // 5 -- Each unit is 1/300 inch.
        UnitMillimeter  // 6 -- Each unit is 1 millimeter.
    };

    //--------------------------------------------------------------------------
    // MetafileFrameUnit
    //
    // The frameRect for creating a metafile can be specified in any of these
    // units.  There is an extra frame unit value (MetafileFrameUnitGdi) so
    // that units can be supplied in the same units that GDI expects for
    // frame rects -- these units are in .01 (1/100ths) millimeter units
    // as defined by GDI.
    //--------------------------------------------------------------------------

    public enum MetafileFrameUnit
    {
        MetafileFrameUnitPixel = Unit.UnitPixel,
        MetafileFrameUnitPoint = Unit.UnitPoint,
        MetafileFrameUnitInch = Unit.UnitInch,
        MetafileFrameUnitDocument = Unit.UnitDocument,
        MetafileFrameUnitMillimeter = Unit.UnitMillimeter,
        MetafileFrameUnitGdi                        // GDI compatible .01 MM units
    };

    //--------------------------------------------------------------------------
    // Coordinate space identifiers
    //--------------------------------------------------------------------------

    public enum CoordinateSpace
    {
        CoordinateSpaceWorld,     // 0
        CoordinateSpacePage,      // 1
        CoordinateSpaceDevice     // 2
    };

    //--------------------------------------------------------------------------
    // Various wrap modes for brushes
    //--------------------------------------------------------------------------

    public enum WrapMode
    {
        WrapModeTile,        // 0
        WrapModeTileFlipX,   // 1
        WrapModeTileFlipY,   // 2
        WrapModeTileFlipXY,  // 3
        WrapModeClamp        // 4
    };

    //--------------------------------------------------------------------------
    // Various hatch styles
    //--------------------------------------------------------------------------

    public enum HatchStyle
    {
        HatchStyleHorizontal,                   // 0
        HatchStyleVertical,                     // 1
        HatchStyleForwardDiagonal,              // 2
        HatchStyleBackwardDiagonal,             // 3
        HatchStyleCross,                        // 4
        HatchStyleDiagonalCross,                // 5
        HatchStyle05Percent,                    // 6
        HatchStyle10Percent,                    // 7
        HatchStyle20Percent,                    // 8
        HatchStyle25Percent,                    // 9
        HatchStyle30Percent,                    // 10
        HatchStyle40Percent,                    // 11
        HatchStyle50Percent,                    // 12
        HatchStyle60Percent,                    // 13
        HatchStyle70Percent,                    // 14
        HatchStyle75Percent,                    // 15
        HatchStyle80Percent,                    // 16
        HatchStyle90Percent,                    // 17
        HatchStyleLightDownwardDiagonal,        // 18
        HatchStyleLightUpwardDiagonal,          // 19
        HatchStyleDarkDownwardDiagonal,         // 20
        HatchStyleDarkUpwardDiagonal,           // 21
        HatchStyleWideDownwardDiagonal,         // 22
        HatchStyleWideUpwardDiagonal,           // 23
        HatchStyleLightVertical,                // 24
        HatchStyleLightHorizontal,              // 25
        HatchStyleNarrowVertical,               // 26
        HatchStyleNarrowHorizontal,             // 27
        HatchStyleDarkVertical,                 // 28
        HatchStyleDarkHorizontal,               // 29
        HatchStyleDashedDownwardDiagonal,       // 30
        HatchStyleDashedUpwardDiagonal,         // 31
        HatchStyleDashedHorizontal,             // 32
        HatchStyleDashedVertical,               // 33
        HatchStyleSmallConfetti,                // 34
        HatchStyleLargeConfetti,                // 35
        HatchStyleZigZag,                       // 36
        HatchStyleWave,                         // 37
        HatchStyleDiagonalBrick,                // 38
        HatchStyleHorizontalBrick,              // 39
        HatchStyleWeave,                        // 40
        HatchStylePlaid,                        // 41
        HatchStyleDivot,                        // 42
        HatchStyleDottedGrid,                   // 43
        HatchStyleDottedDiamond,                // 44
        HatchStyleShingle,                      // 45
        HatchStyleTrellis,                      // 46
        HatchStyleSphere,                       // 47
        HatchStyleSmallGrid,                    // 48
        HatchStyleSmallCheckerBoard,            // 49
        HatchStyleLargeCheckerBoard,            // 50
        HatchStyleOutlinedDiamond,              // 51
        HatchStyleSolidDiamond,                 // 52

        HatchStyleTotal,
        HatchStyleLargeGrid = HatchStyleCross,  // 4

        HatchStyleMin = HatchStyleHorizontal,
        HatchStyleMax = HatchStyleTotal - 1,
    };

    //--------------------------------------------------------------------------
    // Dash style constants
    //--------------------------------------------------------------------------

    public enum DashStyle
    {
        DashStyleSolid,          // 0
        DashStyleDash,           // 1
        DashStyleDot,            // 2
        DashStyleDashDot,        // 3
        DashStyleDashDotDot,     // 4
        DashStyleCustom          // 5
    };

    //--------------------------------------------------------------------------
    // Dash cap constants
    //--------------------------------------------------------------------------

    public enum DashCap
    {
        DashCapFlat = 0,
        DashCapRound = 2,
        DashCapTriangle = 3
    };

    //--------------------------------------------------------------------------
    // Line cap constants (only the lowest 8 bits are used).
    //--------------------------------------------------------------------------

    public enum LineCap
    {
        LineCapFlat = 0,
        LineCapSquare = 1,
        LineCapRound = 2,
        LineCapTriangle = 3,

        LineCapNoAnchor = 0x10, // corresponds to flat cap
        LineCapSquareAnchor = 0x11, // corresponds to square cap
        LineCapRoundAnchor = 0x12, // corresponds to round cap
        LineCapDiamondAnchor = 0x13, // corresponds to triangle cap
        LineCapArrowAnchor = 0x14, // no correspondence

        LineCapCustom = 0xff, // custom cap

        LineCapAnchorMask = 0xf0  // mask to check for anchor or not.
    };

    //--------------------------------------------------------------------------
    // Custom Line cap type constants
    //--------------------------------------------------------------------------

    public enum CustomLineCapType
    {
        CustomLineCapTypeDefault = 0,
        CustomLineCapTypeAdjustableArrow = 1
    };

    //--------------------------------------------------------------------------
    // Line join constants
    //--------------------------------------------------------------------------

    public enum LineJoin
    {
        LineJoinMiter = 0,
        LineJoinBevel = 1,
        LineJoinRound = 2,
        LineJoinMiterClipped = 3
    };

    //--------------------------------------------------------------------------
    // Path point types (only the lowest 8 bits are used.)
    //  The lowest 3 bits are interpreted as point type
    //  The higher 5 bits are reserved for flags.
    //--------------------------------------------------------------------------

    public enum PathPointType
    {
        Start = 0,    // move
        Line = 1,    // line
        Bezier = 3,    // default Bezier (= cubic Bezier)
        PathTypeMask = 0x07, // type mask (lowest 3 bits).
        DashMode = 0x10, // currently in dash mode.
        PathMarker = 0x20, // a marker for the path.
        CloseSubpath = 0x80, // closed flag

        // Path types used for advanced path.

        Bezier3 = 3,         // cubic Bezier
    };


    //--------------------------------------------------------------------------
    // WarpMode constants
    //--------------------------------------------------------------------------

    public enum WarpMode
    {
        Perspective,    // 0
        Bilinear        // 1
    };

    //--------------------------------------------------------------------------
    // LineGradient Mode
    //--------------------------------------------------------------------------

    public enum LinearGradientMode
    {
        Horizontal,         // 0
        Vertical,           // 1
        ForwardDiagonal,    // 2
        BackwardDiagonal    // 3
    };

    //--------------------------------------------------------------------------
    // Region Comine Modes
    //--------------------------------------------------------------------------

    public enum CombineMode
    {
        Replace,     // 0
        Intersect,   // 1
        Union,       // 2
        Xor,         // 3
        Exclude,     // 4
        Complement   // 5 (Exclude From)
    };

    //--------------------------------------------------------------------------
    // Image types
    //--------------------------------------------------------------------------

    public enum ImageType
    {
        Unknown,   // 0
        Bitmap,    // 1
        Metafile   // 2
    };

    //--------------------------------------------------------------------------
    // Interpolation modes
    //--------------------------------------------------------------------------

    public enum InterpolationMode
    {
        Invalid = QualityMode.Invalid,
        Default = QualityMode.Default,
        LowQuality = QualityMode.Low,
        HighQuality = QualityMode.High,
        Bilinear,
        Bicubic,
        NearestNeighbor,
        HighQualityBilinear,
        HighQualityBicubic
    };

    //--------------------------------------------------------------------------
    // Pen types
    //--------------------------------------------------------------------------

    public enum PenAlignment
    {
        Center = 0,
        Inset = 1
    };

    //--------------------------------------------------------------------------
    // Brush types
    //--------------------------------------------------------------------------

    public enum BrushType
    {
        SolidColor = 0,
        HatchFill = 1,
        TextureFill = 2,
        PathGradient = 3,
        LinearGradient = 4
    };

    //--------------------------------------------------------------------------
    // Pen's Fill types
    //--------------------------------------------------------------------------

    public enum PenType
    {
        SolidColor = BrushType.SolidColor,
        HatchFill = BrushType.HatchFill,
        TextureFill = BrushType.TextureFill,
        PathGradient = BrushType.PathGradient,
        LinearGradient = BrushType.LinearGradient,
        Unknown = -1
    };

    //--------------------------------------------------------------------------
    // Matrix Order
    //--------------------------------------------------------------------------

    public enum MatrixOrder
    {
        Prepend = 0,
        Append = 1
    };

#if false
    //--------------------------------------------------------------------------
    // Generic font families
    //--------------------------------------------------------------------------

    public enum GenericFontFamily
    {
        Serif,
        SansSerif,
        Monospace

    };

    //--------------------------------------------------------------------------
    // FontStyle: face types and common styles
    //--------------------------------------------------------------------------

    public enum FontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        BoldItalic = 3,
        Underline = 4,
        Strikeout = 8
    };

#endif

    //---------------------------------------------------------------------------
    // Smoothing Mode
    //---------------------------------------------------------------------------

    public enum SmoothingMode
    {
        Invalid = QualityMode.Invalid,
        Default = QualityMode.Default,
        HighSpeed = QualityMode.Low,
        HighQuality = QualityMode.High,
        None,
        AntiAlias,
    };

    //---------------------------------------------------------------------------
    // Pixel Format Mode
    //---------------------------------------------------------------------------

    public enum PixelOffsetMode
    {
        Invalid,
        Default,
        HighSpeed,
        HighQuality,
        None,    // No pixel offset
        Half     // Offset by -0.5, -0.5 for fast anti-alias perf
    };

    //---------------------------------------------------------------------------
    // Text Rendering Hint
    //---------------------------------------------------------------------------

    public enum TextRenderingHint
    {
        SystemDefault = 0,            // Glyph with system default rendering hint
        SingleBitPerPixelGridFit,     // Glyph bitmap with hinting
        SingleBitPerPixel,            // Glyph bitmap without hinting
        AntiAliasGridFit,             // Glyph anti-alias bitmap with hinting
        AntiAlias,                    // Glyph anti-alias bitmap without hinting
        ClearTypeGridFit              // Glyph CT bitmap with hinting
    };

    //---------------------------------------------------------------------------
    // Metafile Types
    //---------------------------------------------------------------------------

    public enum MetafileType
    {
        Invalid,            // Invalid metafile
        Wmf,                // Standard WMF
        WmfPlaceable,       // Placeable WMF
        Emf,                // EMF (not EMF+)
        EmfPlusOnly,        // EMF+ without dual, down-level records
        EmfPlusDual         // EMF+ with dual, down-level records
    };

    //---------------------------------------------------------------------------
    // Specifies the type of EMF to record
    //---------------------------------------------------------------------------

    public enum EmfType
    {
        EmfOnly = MetafileType.Emf,          // no EMF+, only EMF
        EmfPlusOnly = MetafileType.EmfPlusOnly,  // no EMF, only EMF+
        EmfPlusDual = MetafileType.EmfPlusDual   // both EMF+ and EMF
    };

    //---------------------------------------------------------------------------
    // EMF+ Persistent object types
    //---------------------------------------------------------------------------

    public enum ObjectType
    {
        Invalid,
        Brush,
        Pen,
        Path,
        Region,
        Image,
        Font,
        StringFormat,
        ImageAttributes,
        CustomLineCap,
        Max = CustomLineCap,
        Min = Brush
    }


    public enum StringFormatFlags : uint
    {
        DirectionRightToLeft = 0x00000001,
        DirectionVertical = 0x00000002,
        NoFitBlackBox = 0x00000004,
        DisplayFormatControl = 0x00000020,
        NoFontFallback = 0x00000400,
        MeasureTrailingSpaces = 0x00000800,
        NoWrap = 0x00001000,
        LineLimit = 0x00002000,

        NoClip = 0x00004000,
        BypassGDI = 0x80000000
    };

    //---------------------------------------------------------------------------
    // StringTrimming
    //---------------------------------------------------------------------------

    public enum StringTrimming
    {
        None = 0,
        Character = 1,
        Word = 2,
        EllipsisCharacter = 3,
        EllipsisWord = 4,
        EllipsisPath = 5
    };

    //---------------------------------------------------------------------------
    // National language digit substitution
    //---------------------------------------------------------------------------

    public enum StringDigitSubstitute
    {
        User = 0,  // As NLS setting
        None = 1,
        National = 2,
        Traditional = 3
    };

    //---------------------------------------------------------------------------
    // Hotkey prefix interpretation
    //---------------------------------------------------------------------------

    public enum HotkeyPrefix
    {
        None = 0,
        Show = 1,
        Hide = 2
    };

    //---------------------------------------------------------------------------
    // String alignment flags
    //---------------------------------------------------------------------------

    //public enum StringAlignment
    //{
    //    // Left edge for left-to-right text,
    //    // right for right-to-left text,
    //    // and top for vertical
    //    Near = 0,
    //    Center = 1,
    //    Far = 2
    //};

    //---------------------------------------------------------------------------
    // DriverStringOptions
    //---------------------------------------------------------------------------

    public enum DriverStringOptions
    {
        CmapLookup = 1,
        Vertical = 2,
        RealizedAdvance = 4,
        LimitSubpixel = 8
    };

    //---------------------------------------------------------------------------
    // Flush Intention flags
    //---------------------------------------------------------------------------

    public enum FlushIntention
    {
        Flush = 0,        // Flush all batched rendering operations
        Sync = 1          // Flush all batched rendering operations
        // and wait for them to complete
    };

    //---------------------------------------------------------------------------
    // Image encoder parameter related types
    //---------------------------------------------------------------------------

    public enum EncoderParameterValueType
    {
        Byte = 1,    // 8-bit unsigned int
        ASCII = 2,    // 8-bit byte containing one 7-bit ASCII
        // code. NULL terminated.
        Short = 3,    // 16-bit unsigned int
        Long = 4,    // 32-bit unsigned int
        Rational = 5,    // Two Longs. The first Long is the
        // numerator, the second Long expresses the
        // denomintor.
        LongRange = 6,    // Two longs which specify a range of
        // integer values. The first Long specifies
        // the lower end and the second one
        // specifies the higher end. All values
        // are inclusive at both ends
        Undefined = 7,    // 8-bit byte that can take any value
        // depending on field definition
        RationalRange = 8,    // Two Rationals. The first Rational
        // specifies the lower end and the second
        // specifies the higher end. All values
        // are inclusive at both ends
        //#if (GDIPVER >= 0x0110)
        //    Pointer        = 9     // a pointer to a parameter defined data.
        //#endif //(GDIPVER >= 0x0110)
    };

    //---------------------------------------------------------------------------
    // Image encoder value types
    //---------------------------------------------------------------------------

    public enum EncoderValue
    {
        ColorTypeCMYK,
        ColorTypeYCCK,
        CompressionLZW,
        CompressionCCITT3,
        CompressionCCITT4,
        CompressionRle,
        CompressionNone,
        ScanMethodInterlaced,
        ScanMethodNonInterlaced,
        VersionGif87,
        VersionGif89,
        RenderProgressive,
        RenderNonProgressive,
        TransformRotate90,
        TransformRotate180,
        TransformRotate270,
        TransformFlipHorizontal,
        TransformFlipVertical,
        MultiFrame,
        LastFrame,
        Flush,
        FrameDimensionTime,
        FrameDimensionResolution,
        FrameDimensionPage,
        //#if (GDIPVER >= 0x0110)
        //    ColorTypeGray,
        //    ColorTypeRGB,
        //#endif
    };

    public enum RotateFlipType
    {
        RotateNoneFlipNone = 0,
        Rotate90FlipNone = 1,
        Rotate180FlipNone = 2,
        Rotate270FlipNone = 3,

        RotateNoneFlipX = 4,
        Rotate90FlipX = 5,
        Rotate180FlipX = 6,
        Rotate270FlipX = 7,

        RotateNoneFlipY = Rotate180FlipX,
        Rotate90FlipY = Rotate270FlipX,
        Rotate180FlipY = RotateNoneFlipX,
        Rotate270FlipY = Rotate90FlipX,

        RotateNoneFlipXY = Rotate180FlipNone,
        Rotate90FlipXY = Rotate270FlipNone,
        Rotate180FlipXY = RotateNoneFlipNone,
        Rotate270FlipXY = Rotate90FlipNone
    };

    //----------------------------------------------------------------------------
    // Color Adjust Type
    //----------------------------------------------------------------------------

    public enum ColorAdjustType
    {
        Default,
        Bitmap,
        Brush,
        Pen,
        Text,
        Count,
        Any      // Reserved
    };

    public enum GraphicsState: uint{}
}
