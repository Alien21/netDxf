#region netDxf library licensed under the MIT License
// 
//                       netDxf library
// Copyright (c) Daniel Carvajal (haplokuon@gmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
#endregion

using System;
using System.Collections.Generic;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Units;

namespace netDxf.Tables
{
    /// <summary>
    /// Represents a dimension style.
    /// </summary>
    public class DimensionStyle :
        TableObject
    {
        #region delegates and events

        public delegate void LinetypeChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<Linetype> e);
        public event LinetypeChangedEventHandler LinetypeChanged;
        protected virtual Linetype OnLinetypeChangedEvent(Linetype oldLinetype, Linetype newLinetype)
        {
            LinetypeChangedEventHandler ae = LinetypeChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<Linetype> eventArgs = new TableObjectChangedEventArgs<Linetype>(oldLinetype, newLinetype);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newLinetype;
        }

        public delegate void TextStyleChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<TextStyle> e);
        public event TextStyleChangedEventHandler TextStyleChanged;
        protected virtual TextStyle OnTextStyleChangedEvent(TextStyle oldTextStyle, TextStyle newTextStyle)
        {
            TextStyleChangedEventHandler ae = TextStyleChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<TextStyle> eventArgs = new TableObjectChangedEventArgs<TextStyle>(oldTextStyle, newTextStyle);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newTextStyle;
        }

        public delegate void BlockChangedEventHandler(TableObject sender, TableObjectChangedEventArgs<Block> e);
        public event BlockChangedEventHandler BlockChanged;
        protected virtual Block OnBlockChangedEvent(Block oldBlock, Block newBlock)
        {
            BlockChangedEventHandler ae = BlockChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<Block> eventArgs = new TableObjectChangedEventArgs<Block>(oldBlock, newBlock);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newBlock;
        }

        #endregion

        #region private fields

        // dimension and extension lines
        private AciColor dimclrd;
        private Linetype dimltype;
        private Lineweight dimlwd;
        private bool dimsd1;
        private bool dimsd2;
        private double dimdle;
        private double dimdli;

        private AciColor dimclre;
        private Linetype dimltex1;
        private Linetype dimltex2;
        private Lineweight dimlwe;
        private bool dimse1;
        private bool dimse2;
        private double dimexo;
        private double dimexe;
        private bool dimfxlon;
        private double dimfxl;

        // symbols and arrows
        private double dimasz;
        private double dimcen;
        private Block dimldrblk;
        private Block dimblk1;
        private Block dimblk2;

        // text
        private TextStyle dimtxsty;
        private AciColor dimclrt;
        private AciColor dimtfillclr;
        private double dimtxt;
        private DimensionStyleTextHorizontalPlacement dimjust;
        private DimensionStyleTextVerticalPlacement dimtad;
        private double dimgap;
        private bool dimtih;
        private bool dimtoh;
        private DimensionStyleTextDirection dimtxtdirection;
        private double dimtfac;

        // fit
        private bool dimtofl;
        private bool dimsoxd;
        private double dimscale;
        private DimensionStyleFitOptions dimatfit;
        private bool dimtix;
        private DimensionStyleFitTextMove dimtmove;

        // primary units
        private short dimadec;
        private short dimdec;
        private string dimPrefix;
        private string dimSuffix;
        private char dimdsep;
        private double dimlfac;
        private LinearUnitType dimlunit;
        private AngleUnitType dimaunit;
        private FractionFormatType dimfrac;
        private bool suppressLinearLeadingZeros;
        private bool suppressLinearTrailingZeros;
        private bool suppressZeroFeet;
        private bool suppressZeroInches;
        private bool suppressAngularLeadingZeros;
        private bool suppressAngularTrailingZeros;
        private double dimrnd;

        // alternate units
        private DimensionStyleAlternateUnits alternateUnits;

        // tolerances
        private DimensionStyleTolerances tolerances;

        #endregion

        #region constants

        /// <summary>
        /// Default dimension style name.
        /// </summary>
        public const string DefaultName = "Standard";

        /// <summary>
        /// Gets the default dimension style.
        /// </summary>
        public static DimensionStyle Default
        {
            get { return new DimensionStyle(DefaultName); }
        }

        /// <summary>
        /// Gets the ISO-25 dimension style as defined in AutoCad.
        /// </summary>
        public static DimensionStyle Iso25
        {
            get
            {
                DimensionStyle style = new DimensionStyle("ISO-25")
                {
                    DimBaselineSpacing = 3.75,
                    ExtLineExtend = 1.25,
                    ExtLineOffset = 0.625,
                    ArrowSize = 2.5,
                    CenterMarkSize = 2.5,
                    TextHeight = 2.5,
                    TextOffset = 0.625,
                    TextOutsideAlign = true,
                    TextInsideAlign = true,
                    TextVerticalPlacement = DimensionStyleTextVerticalPlacement.Above,
                    FitDimLineForce = true,
                    DecimalSeparator = ',',
                    LengthPrecision = 2,
                    SuppressLinearTrailingZeros = true,
                    AlternateUnits =
                    {
                        LengthPrecision = 3,
                        Multiplier = 0.0394
                    },
                    Tolerances =
                    {
                        VerticalPlacement = DimensionStyleTolerancesVerticalPlacement.Bottom,
                        Precision = 2,
                        SuppressLinearTrailingZeros = true,
                        AlternatePrecision = 3
                    }
                };
                return style;
            }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>DimensionStyle</c> class.
        /// </summary>
        /// <param name="name">The dimension style name.</param>
        public DimensionStyle(string name)
            : this(name, true)
        {
        }

        internal DimensionStyle(string name, bool checkName)
            : base(name, DxfObjectCode.DimStyle, checkName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "The dimension style name should be at least one character long.");
            }

            IsReserved = name.Equals(DefaultName, StringComparison.OrdinalIgnoreCase);

            // dimension and extension lines
            dimclrd = AciColor.ByBlock;
            dimltype = Linetype.ByBlock;
            dimlwd = Lineweight.ByBlock;
            dimdle = 0.0;
            dimdli = 0.38;
            dimsd1 = false;
            dimsd2 = false;

            dimclre = AciColor.ByBlock;
            dimltex1 = Linetype.ByBlock;
            dimltex2 = Linetype.ByBlock;
            dimlwe = Lineweight.ByBlock;
            dimse1 = false;
            dimse2 = false;
            dimexo = 0.0625;
            dimexe = 0.18;
            dimfxlon = false;
            dimfxl = 1.0;

            // symbols and arrows
            dimldrblk = null;
            dimblk1 = null;
            dimblk2 = null;
            dimasz = 0.18;
            dimcen = 0.09;

            // text
            dimtxsty = TextStyle.Default;
            dimclrt = AciColor.ByBlock;
            dimtfillclr = null;
            dimtxt = 0.18;
            dimtad = DimensionStyleTextVerticalPlacement.Centered;
            dimjust = DimensionStyleTextHorizontalPlacement.Centered;
            dimgap = 0.09;
            dimtih = false;
            dimtoh = false;
            dimtxtdirection = DimensionStyleTextDirection.LeftToRight;
            dimtfac = 1.0;

            // fit
            dimtofl = false;
            dimsoxd = true;
            dimscale = 1.0;
            dimatfit = DimensionStyleFitOptions.BestFit;
            dimtix = false;
            dimtmove = DimensionStyleFitTextMove.BesideDimLine;

            // primary units
            dimdec = 4;
            dimadec = 0;
            dimPrefix = string.Empty;
            dimSuffix = string.Empty;
            dimdsep = '.';
            dimlfac = 1.0;
            dimaunit = AngleUnitType.DecimalDegrees;
            dimlunit = LinearUnitType.Decimal;
            dimfrac = FractionFormatType.Horizontal;
            suppressLinearLeadingZeros = false;
            suppressLinearTrailingZeros = false;
            suppressZeroFeet = true;
            suppressZeroInches = true;
            suppressAngularLeadingZeros = false;
            suppressAngularTrailingZeros = false;
            dimrnd = 0.0;

            // alternate units
            alternateUnits = new DimensionStyleAlternateUnits();

            // tolerances
            tolerances = new DimensionStyleTolerances();
        }

        #endregion

        #region public properties

        #region dimension and extension lines

        /// <summary>
        /// Gets or set the color assigned to dimension lines, arrowheads, and dimension leader lines. (DIMCLRD)
        /// </summary>
        /// <remarks>
        /// Default: ByBlock<br />
        /// Only indexed AciColors are supported.
        /// </remarks>
        public AciColor DimLineColor
        {
            get { return dimclrd; }
            set
            {
                dimclrd = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Gets or sets the line type of the dimension line. (DIMLTYPE)
        /// </summary>
        /// <remarks>
        /// Default: ByBlock
        /// </remarks>
        public Linetype DimLineLinetype
        {
            get { return dimltype; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                dimltype = OnLinetypeChangedEvent(dimltype, value);
            }
        }

        /// <summary>
        /// Gets or sets the line weight to dimension lines. (DIMLWD)
        /// </summary>
        /// <remarks>
        /// Default: ByBlock
        /// </remarks>
        public Lineweight DimLineLineweight
        {
            get { return dimlwd; }
            set { dimlwd = value; }
        }

        /// <summary>
        /// Suppresses display of the first dimension line. (DIMSD1)
        /// </summary>
        /// <remarks>
        /// Default: false<br />
        /// To completely suppress the dimension line set both <c>DimLine1Off</c> and <c>DimLine2Off</c> to false.
        /// </remarks>
        public bool DimLine1Off
        {
            get { return dimsd1; }
            set { dimsd1 = value; }
        }

        /// <summary>
        /// Suppresses display of the second dimension line. (DIMSD2)
        /// </summary>
        /// <remarks>
        /// Default: false<br />
        /// To completely suppress the dimension line set both <c>DimLine1Off</c> and <c>DimLine2Off</c> to false.
        /// </remarks>
        public bool DimLine2Off
        {
            get { return dimsd2; }
            set { dimsd2 = value; }
        }

        /// <summary>
        /// Gets or sets the distance the dimension line extends beyond the extension line when
        /// oblique, architectural tick, integral, or no marks are drawn for arrowheads. (DIMDLE)
        /// </summary>
        /// <remarks>
        /// Default: 0.0
        /// </remarks>
        public double DimLineExtend
        {
            get { return dimdle; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DimLineExtend must be equals or greater than zero.");
                }
                dimdle = value;
            }
        }

        /// <summary>
        /// Gets or sets the spacing of the dimension lines in baseline dimensions. (DIMDLI)
        /// </summary>
        /// <remarks>
        /// Default: 0.38<br />
        /// This value is stored only for information purposes.
        /// Base dimensions are a compound entity made of several dimensions, there is no actual DXF entity that represents that.
        /// </remarks>
        public double DimBaselineSpacing
        {
            get { return dimdli; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DimBaselineSpacing must be equals or greater than zero.");
                }
                dimdli = value;
            }
        }

        /// <summary>
        /// Gets or sets the color assigned to extension lines, center marks, and centerlines. (DIMCLRE)
        /// </summary>
        /// <remarks>
        /// Default: ByBlock<br />
        /// Only indexed AciColors are supported.
        /// </remarks>
        public AciColor ExtLineColor
        {
            get { return dimclre; }
            set
            {
                dimclre = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Gets or sets the line type of the first extension line. (DIMLTEX1)
        /// </summary>
        /// <remarks>
        /// Default: ByBlock
        /// </remarks>
        public Linetype ExtLine1Linetype
        {
            get { return dimltex1; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                dimltex1 = OnLinetypeChangedEvent(dimltex1, value);
            }
        }

        /// <summary>
        /// Gets or sets the line type of the second extension line. (DIMLTEX2)
        /// </summary>
        /// <remarks>
        /// Default: ByBlock
        /// </remarks>
        public Linetype ExtLine2Linetype
        {
            get { return dimltex2; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                dimltex2 = OnLinetypeChangedEvent(dimltex2, value);
            }
        }

        /// <summary>
        /// Gets or sets line weight of extension lines. (DIMLWE)
        /// </summary>
        /// <remarks>
        /// Default: ByBlock
        /// </remarks>
        public Lineweight ExtLineLineweight
        {
            get { return dimlwe; }
            set { dimlwe = value; }
        }

        /// <summary>
        /// Suppresses display of the first extension line. (DIMSE1)
        /// </summary>
        /// <remarks>
        /// Default: false
        /// </remarks>
        public bool ExtLine1Off
        {
            get { return dimse1; }
            set { dimse1 = value; }
        }

        /// <summary>
        /// Suppresses display of the second extension line. (DIMSE2)
        /// </summary>
        /// <remarks>
        /// Default: false
        /// </remarks>
        public bool ExtLine2Off
        {
            get { return dimse2; }
            set { dimse2 = value; }
        }

        /// <summary>
        /// Gets or sets how far extension lines are offset from origin points. (DIMEXO)
        /// </summary>
        /// <remarks>
        /// Default: 0.0625
        /// </remarks>
        public double ExtLineOffset
        {
            get { return dimexo; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The ExtLineOffset must be equals or greater than zero.");
                }
                dimexo = value;
            }
        }

        /// <summary>
        /// Gets or sets how far to extend the extension line beyond the dimension line. (DIMEXE)
        /// </summary>
        /// <remarks>
        /// Default: 0.18
        /// </remarks>
        public double ExtLineExtend
        {
            get { return dimexe; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The ExtLineExtend must be equals or greater than zero.");
                }
                dimexe = value;
            }
        }

        /// <summary>
        /// Enables fixed length extension lines. (DIMFXLON)
        /// </summary>
        /// <remarks>
        /// Default: false
        /// </remarks>
        public bool ExtLineFixed
        {
            get { return dimfxlon; }
            set { dimfxlon = value; }
        }

        /// <summary>
        /// Gets or sets the total length of the extension lines starting from the dimension line toward the dimension origin. (DIMFXL)
        /// </summary>
        /// <remarks>
        /// Default: 1.0
        /// </remarks>
        public double ExtLineFixedLength
        {
            get { return dimfxl; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The ExtLineFixedLength must be equals or greater than zero.");
                }
                dimfxl = value;
            }
        }

        #endregion

        #region symbols and arrows

        /// <summary>
        /// Gets or sets the arrowhead block for the first end of the dimension line. (DIMBLK1)
        /// </summary>
        /// <remarks>
        /// Default: null. Closed filled.
        /// </remarks>
        public Block DimArrow1
        {
            get { return dimblk1; }
            set { dimblk1 = value == null ? null : OnBlockChangedEvent(dimblk1, value); }
        }

        /// <summary>
        /// Gets or sets the arrowhead block for the second end of the dimension line. (DIMBLK2)
        /// </summary>
        /// <remarks>
        /// Default: null. Closed filled.
        /// </remarks>
        public Block DimArrow2
        {
            get { return dimblk2; }
            set { dimblk2 = value == null ? null : OnBlockChangedEvent(dimblk2, value); }
        }

        /// <summary>
        /// Gets or sets the arrowhead block for leaders. (DIMLDRBLK)
        /// </summary>
        /// <remarks>
        /// Default: null. Closed filled.
        /// </remarks>
        public Block LeaderArrow
        {
            get { return dimldrblk; }
            set { dimldrblk = value == null ? null : OnBlockChangedEvent(dimldrblk, value); }
        }

        /// <summary>
        /// Controls the size of dimension line and leader line arrowheads. Also controls the size of hook lines. (DIMASZ)
        /// </summary>
        /// <remarks>
        /// Default: 0.18
        /// </remarks>
        public double ArrowSize
        {
            get { return dimasz; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The ArrowSize must be equals or greater than zero.");
                }
                dimasz = value;
            }
        }

        /// <summary>
        /// Controls the drawing of circle or arc center marks and centerlines. (DIMCEN)
        /// </summary>
        /// <remarks>
        /// Default: 0.09<br/>
        /// 0 - No center marks or lines are drawn.<br />
        /// greater than 0 - Center marks are drawn.<br />
        /// lower than 0 - Center marks and centerlines are drawn.<br />
        /// The absolute value specifies the size of the center mark or center line. 
        /// The size of the center line is the length of the center line segment that extends outside the circle or arc.
        /// It is also the size of the gap between the center mark and the start of the center line. 
        /// The size of the center mark is the distance from the center of the circle or arc to the end of the center mark.
        /// </remarks>
        public double CenterMarkSize
        {
            get { return dimcen; }
            set { dimcen = value; }
        }

        #endregion

        #region text appearance

        /// <summary>
        /// Gets or sets the text style of the dimension. (DIMTXTSTY)
        /// </summary>
        /// <remarks>
        /// Default: Standard
        /// </remarks>
        public TextStyle TextStyle
        {
            get { return dimtxsty; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                dimtxsty = OnTextStyleChangedEvent(dimtxsty, value);
            }
        }

        /// <summary>
        /// Gets or set the color of dimension text. (DIMCLRT)
        /// </summary>
        /// <remarks>
        /// Default: ByBlock<br />
        /// Only indexed AciColors are supported.
        /// </remarks>
        public AciColor TextColor
        {
            get { return dimclrt; }
            set
            {
                dimclrt = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Gets or set the background color of dimension text. Set to null to specify no color. (DIMTFILLCLR)
        /// </summary>
        /// <remarks>
        /// Default: null<br />
        /// Only indexed AciColors are supported.
        /// </remarks>
        public AciColor TextFillColor
        {
            get { return dimtfillclr; }
            set { dimtfillclr = value; }
        }

        /// <summary>
        /// Gets or sets the height of dimension text, unless the current text style has a fixed height. (DIMTXT)
        /// </summary>
        /// <remarks>
        /// Default: 0.18
        /// </remarks>
        public double TextHeight
        {
            get { return dimtxt; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The TextHeight must be greater than zero.");
                }
                dimtxt = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal positioning of dimension text. (DIMJUST)
        /// </summary>
        /// <remarks>
        /// Default: Centered
        /// </remarks>
        public DimensionStyleTextHorizontalPlacement TextHorizontalPlacement
        {
            get { return dimjust; }
            set { dimjust = value; }
        }

        /// <summary>
        /// Gets or sets the vertical position of text in relation to the dimension line. (DIMTAD)
        /// </summary>
        /// <remarks>
        /// Default: Centered
        /// </remarks>
        public DimensionStyleTextVerticalPlacement TextVerticalPlacement
        {
            get { return dimtad; }
            set { dimtad = value; }
        }

        /// <summary>
        /// Gets or sets the distance around the dimension text when the dimension line breaks to accommodate dimension text. (DIMGAP)
        /// </summary>
        /// <remarks>
        /// Default: 0.09<br />
        /// Displays a rectangular frame around the dimension text when negative values are used.
        /// </remarks>
        public double TextOffset
        {
            get { return dimgap; }
            set { dimgap = value; }
        }

        /// <summary>
        /// Gets or sets if the dimension text is placed horizontally when inside extension lines. (DIMTIH)
        /// </summary>
        /// <remarks>
        /// Default: false
        /// </remarks>
        public bool TextInsideAlign
        {
            get { return dimtih; }
            set { dimtih = value; }
        }

        /// <summary>
        /// Gets or sets if the dimension text is placed horizontally when outside extension lines. (DIMTOH)
        /// </summary>
        /// <remarks>
        /// Default: false
        /// </remarks>
        public bool TextOutsideAlign
        {
            get { return dimtoh; }
            set { dimtoh = value; }
        }

        /// <summary>
        /// Gets or sets the direction of the dimension text. (DIMTXTDIRECTION)
        /// </summary>
        /// <remarks>
        /// Default: LeftToRight
        /// </remarks>
        public DimensionStyleTextDirection TextDirection
        {
            get { return dimtxtdirection; }
            set { dimtxtdirection = value; }
        }

        /// <summary>
        /// Gets or sets the scale of fractions relative to dimension text height. (DIMTFAC)
        /// </summary>
        /// <remarks>
        /// Default: 1.0<br />
        /// This value is only applicable to Architectural and Fractional units, and also
        /// controls the height factor applied to the tolerance text in relation with the dimension text height.
        /// </remarks>
        public double TextFractionHeightScale
        {
            get { return dimtfac; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The TextFractionHeightScale must be greater than zero.");
                }
                dimtfac = value;
            }
        }

        #endregion

        #region fit

        /// <summary>
        /// Gets or sets the drawing of a dimension line between the extension lines even when the text is placed outside the extension lines. (DIMTOFL)
        /// </summary>
        /// <remarks>
        /// Default: false
        /// </remarks>
        public bool FitDimLineForce
        {
            get { return dimtofl; }
            set { dimtofl = value; }
        }

        /// <summary>
        /// Gets or sets the drawing of the dimension line and arrowheads even if not enough space is available inside the extension lines. (DIMSOXD)
        /// </summary>
        /// <remarks>
        /// Default: true<br />
        /// If not enough space is available inside the extension lines and FitTextInside is true,
        /// setting FitDimLineInside to false suppresses the arrowheads. If FitDimLineInside is false,
        /// FitDimLineInside has no effect.
        /// </remarks>
        public bool FitDimLineInside
        {
            get { return dimsoxd; }
            set { dimsoxd = value; }
        }

        /// <summary>
        /// Get or set the overall scale factor applied to dimensioning variables that specify sizes, distances, or offsets. (DIMSCALE)
        /// </summary>
        /// <remarks>
        /// Default: 1.0<br/>
        /// DIMSCALE does not affect measured lengths, coordinates, or angles.<br/>
        /// DIMSCALE values of zero are not supported, any imported drawing with a zero value will set the scale to the default 1.0.
        /// </remarks>
        public double DimScaleOverall
        {
            get { return dimscale; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The DimScaleOverall must be greater than zero.");
                }
                dimscale = value;
            }
        }

        /// <summary>
        /// Gets or sets the placement of text and arrowheads based on the space available between the extension lines. (DIMATFIT)
        /// </summary>
        /// <remarks>
        /// Default: BestFit<br/>
        /// Not implemented in the dimension drawing.
        /// </remarks>
        public DimensionStyleFitOptions FitOptions
        {
            get { return dimatfit; }
            set { dimatfit = value; }
        }

        /// <summary>
        /// Gets or sets the drawing of text between the extension lines. (DIMTIX)
        /// </summary>
        /// <remarks>
        /// Default: false
        /// </remarks>
        public bool FitTextInside
        {
            get { return dimtix; }
            set { dimtix = value; }
        }

        /// <summary>
        /// Gets or sets the position of the text when it's moved either manually or automatically. (DIMTMOVE)
        /// </summary>
        /// <remarks>
        /// Default: BesideDimLine
        /// </remarks>
        public DimensionStyleFitTextMove FitTextMove
        {
            get { return dimtmove; }
            set { dimtmove = value; }
        }

        #endregion

        #region primary units

        /// <summary>
        /// Gets or sets the number of precision places displayed in angular dimensions. (DIMADEC)
        /// </summary>
        /// <remarks>
        /// Default: 0<br/>
        /// If set to -1 angular dimensions display the number of decimal places specified by LengthPrecision.
        /// It is recommended to use values in the range 0 to 8.
        /// </remarks>
        public short AngularPrecision
        {
            get { return dimadec; }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The AngularPrecision must be greater than -1.");
                }
                dimadec = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of decimal places displayed for the primary units of a dimension. (DIMDEC)
        /// </summary>
        /// <remarks>
        /// Default: 2<br/>
        /// It is recommended to use values in the range 0 to 8.<br/>
        /// For architectural and fractional the precision used for the minimum fraction is 1/2^LinearDecimalPlaces.
        /// </remarks>
        public short LengthPrecision
        {
            get { return dimdec; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The LengthPrecision must be equals or greater than zero.");
                }
                dimdec = value;
            }
        }

        /// <summary>
        /// Gets or sets the text prefix for the dimension. (DIMPOST)
        /// </summary>
        /// <remarks>
        /// Default: string.Empty
        /// </remarks>
        public string DimPrefix
        {
            get { return dimPrefix; }
            set { dimPrefix = value ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets the text suffix for the dimension. (DIMPOST)
        /// </summary>
        /// <remarks>
        /// Default: string.Empty
        /// </remarks>
        public string DimSuffix
        {
            get { return dimSuffix; }
            set { dimSuffix = value ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets a single-character decimal separator to use when creating dimensions whose unit format is decimal. (DIMDSEP)
        /// </summary>
        /// <remarks>
        /// Default: "."
        /// </remarks>
        public char DecimalSeparator
        {
            get { return dimdsep; }
            set { dimdsep = value; }
        }

        /// <summary>
        /// Gets or sets a scale factor for linear dimension measurements. (DIMLFAC)
        /// </summary>
        /// <remarks>
        /// All linear dimension distances, including radii, diameters, and coordinates, are multiplied by DimScaleLinear before being converted to dimension text.<br />
        /// Positive values of DimScaleLinear are applied to dimensions in both model space and paper space; negative values are applied to paper space only.<br />
        /// DimScaleLinear has no effect on angular dimensions.
        /// </remarks>
        public double DimScaleLinear
        {
            get { return dimlfac; }
            set
            {
                if (MathHelper.IsZero(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The scale factor cannot be zero.");
                }
                dimlfac = value;
            }
        }

        /// <summary>
        /// Gets or sets the units for all dimension types except angular. (DIMLUNIT)
        /// </summary>
        /// <remarks>
        /// Scientific<br/>
        /// Decimal<br/>
        /// Engineering<br/>
        /// Architectural<br/>
        /// Fractional
        /// </remarks>
        public LinearUnitType DimLengthUnits
        {
            get { return dimlunit; }
            set { dimlunit = value; }
        }

        /// <summary>
        /// Gets or sets the units format for angular dimensions. (DIMAUNIT)
        /// </summary>
        /// <remarks>
        /// Decimal degrees<br/>
        /// Degrees/minutes/seconds<br/>
        /// Gradians<br/>
        /// Radians
        /// </remarks>
        public AngleUnitType DimAngularUnits
        {
            get { return dimaunit; }
            set
            {
                if (value == AngleUnitType.SurveyorUnits)
                {
                    throw new ArgumentException("Surveyor's units are not applicable in angular dimensions.");
                }
                dimaunit = value;
            }
        }

        /// <summary>
        /// Gets or sets the fraction format when DIMLUNIT is set to Architectural or Fractional. (DIMFRAC)
        /// </summary>
        /// <remarks>
        /// Horizontal stacking<br/>
        /// Diagonal stacking<br/>
        /// Not stacked (for example, 1/2)
        /// </remarks>
        public FractionFormatType FractionType
        {
            get { return dimfrac; }
            set { dimfrac = value; }
        }

        /// <summary>
        /// Suppresses leading zeros in linear decimal dimensions; for example, 0.5000 becomes .5000. (DIMZIN)
        /// </summary>
        /// <remarks>
        /// This value is part of the DIMZIN variable.
        /// </remarks>
        public bool SuppressLinearLeadingZeros
        {
            get { return suppressLinearLeadingZeros; }
            set { suppressLinearLeadingZeros = value; }
        }

        /// <summary>
        /// Suppresses trailing zeros in linear decimal dimensions. (DIMZIN)
        /// </summary>
        /// <remarks>
        /// This value is part of the DIMZIN variable.
        /// </remarks>
        public bool SuppressLinearTrailingZeros
        {
            get { return suppressLinearTrailingZeros; }
            set { suppressLinearTrailingZeros = value; }
        }

        /// <summary>
        /// Suppresses zero feet in architectural dimensions. (DIMZIN)
        /// </summary>
        /// <remarks>
        /// This value is part of the DIMZIN variable.
        /// </remarks>
        public bool SuppressZeroFeet
        {
            get { return suppressZeroFeet; }
            set { suppressZeroFeet = value; }
        }

        /// <summary>
        /// Suppresses zero inches in architectural dimensions. (DIMZIN)
        /// </summary>
        /// <remarks>
        /// This value is part of the DIMZIN variable.
        /// </remarks>
        public bool SuppressZeroInches
        {
            get { return suppressZeroInches; }
            set { suppressZeroInches = value; }
        }

        /// <summary>
        /// Suppresses leading zeros in angular decimal dimensions. (DIMZIN)
        /// </summary>
        /// <remarks>
        /// This value is part of the DIMAZIN variable.
        /// </remarks>
        public bool SuppressAngularLeadingZeros
        {
            get { return suppressAngularLeadingZeros; }
            set { suppressAngularLeadingZeros = value; }
        }

        /// <summary>
        /// Suppresses trailing zeros in angular decimal dimensions. (DIMZIN)
        /// </summary>
        /// <remarks>
        /// This value is part of the DIMAZIN variable.
        /// </remarks>
        public bool SuppressAngularTrailingZeros
        {
            get { return suppressAngularTrailingZeros; }
            set { suppressAngularTrailingZeros = value; }
        }

        /// <summary>
        /// Gets or sets the value to round all dimensioning distances. (DIMRND)
        /// </summary>
        /// <remarks>
        /// Default: 0 (no rounding off).<br/>
        /// If DIMRND is set to 0.25, all distances round to the nearest 0.25 unit.
        /// If you set DIMRND to 1.0, all distances round to the nearest integer.
        /// Note that the number of digits edited after the decimal point depends on the precision set by DIMDEC.
        /// DIMRND does not apply to angular dimensions.
        /// </remarks>
        public double DimRoundoff
        {
            get { return dimrnd; }
            set
            {
                if (value < 0.000001 && !MathHelper.IsZero(value, double.Epsilon))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The nearest value to round all distances must be equal or greater than 0.000001 or zero (no rounding off).");
                }
                dimrnd = value;
            }
        }

        #endregion

        #region alternate units

        /// <summary>
        /// Gets or sets the alternate units format for dimensions.
        /// </summary>
        /// <remarks>Alternative units are not applicable for angular dimensions.</remarks>
        public DimensionStyleAlternateUnits AlternateUnits
        {
            get { return alternateUnits; }
            set
            {
                alternateUnits = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        #endregion

        #region tolerances

        /// <summary>
        /// Gets or sets the tolerances format for dimensions.
        /// </summary>
        public DimensionStyleTolerances Tolerances
        {
            get { return tolerances; }
            set
            {
                tolerances = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        #endregion

        /// <summary>
        /// Gets the owner of the actual dimension style.
        /// </summary>
        public new DimensionStyles Owner
        {
            get { return (DimensionStyles) base.Owner; }
            internal set { base.Owner = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Checks if this instance has been referenced by other DxfObjects. 
        /// </summary>
        /// <returns>
        /// Returns true if this instance has been referenced by other DxfObjects, false otherwise.
        /// It will always return false if this instance does not belong to a document.
        /// </returns>
        /// <remarks>
        /// This method returns the same value as the HasReferences method that can be found in the TableObjects class.
        /// </remarks>
        public override bool HasReferences()
        {
            return Owner != null && Owner.HasReferences(Name);
        }

        /// <summary>
        /// Gets the list of DxfObjects referenced by this instance.
        /// </summary>
        /// <returns>
        /// A list of DxfObjectReference that contains the DxfObject referenced by this instance and the number of times it does.
        /// It will return null if this instance does not belong to a document.
        /// </returns>
        /// <remarks>
        /// This method returns the same list as the GetReferences method that can be found in the TableObjects class.
        /// </remarks>
        public override List<DxfObjectReference> GetReferences()
        {
            return Owner?.GetReferences(Name);
        }

        /// <summary>
        /// Creates a new DimensionStyle that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">DimensionStyle name of the copy.</param>
        /// <returns>A new DimensionStyle that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            DimensionStyle copy = new DimensionStyle(newName)
            {
                // dimension lines
                DimLineColor = (AciColor) dimclrd.Clone(),
                DimLineLinetype = (Linetype) dimltype.Clone(),
                DimLineLineweight = dimlwd,
                DimLine1Off = dimsd1,
                DimLine2Off = dimsd2,
                DimBaselineSpacing = dimdli,
                DimLineExtend = dimdle,

                // extension lines
                ExtLineColor = (AciColor) dimclre.Clone(),
                ExtLine1Linetype = (Linetype) dimltex1.Clone(),
                ExtLine2Linetype = (Linetype) dimltex2.Clone(),
                ExtLineLineweight = dimlwe,
                ExtLine1Off = dimse1,
                ExtLine2Off = dimse2,
                ExtLineOffset = dimexo,
                ExtLineExtend = dimexe,

                // symbols and arrows
                ArrowSize = dimasz,
                CenterMarkSize = dimcen,
                LeaderArrow  = (Block) dimldrblk?.Clone(),
                DimArrow1 = (Block) dimblk1?.Clone(),
                DimArrow2 = (Block) dimblk2?.Clone(),

                // text appearance
                TextStyle = (TextStyle) dimtxsty.Clone(),
                TextColor = (AciColor) dimclrt.Clone(),
                TextFillColor = (AciColor) dimtfillclr?.Clone(),
                TextHeight = dimtxt,
                TextHorizontalPlacement = dimjust,
                TextVerticalPlacement = dimtad,
                TextOffset = dimgap,
                TextFractionHeightScale = dimtfac,

                // fit
                FitDimLineForce = dimtofl,
                FitDimLineInside = dimsoxd,
                DimScaleOverall = dimscale,
                FitOptions = dimatfit,
                FitTextInside = dimtix,
                FitTextMove = dimtmove,

                // primary units
                AngularPrecision = dimadec,
                LengthPrecision = dimdec,
                DimPrefix = dimPrefix,
                DimSuffix = dimSuffix,
                DecimalSeparator = dimdsep,
                DimScaleLinear = dimlfac,
                DimLengthUnits = dimlunit,
                DimAngularUnits = dimaunit,
                FractionType = dimfrac,
                SuppressLinearLeadingZeros = suppressLinearLeadingZeros,
                SuppressLinearTrailingZeros = suppressLinearTrailingZeros,
                SuppressZeroFeet = suppressZeroFeet,
                SuppressZeroInches = suppressZeroInches,
                SuppressAngularLeadingZeros = suppressAngularLeadingZeros,
                SuppressAngularTrailingZeros = suppressAngularTrailingZeros,
                DimRoundoff = dimrnd,

                // alternate units
                AlternateUnits = (DimensionStyleAlternateUnits) alternateUnits.Clone(),

                // tolerances
                Tolerances = (DimensionStyleTolerances) tolerances.Clone()
            };

            foreach (XData data in XData.Values)
            {
                copy.XData.Add((XData)data.Clone());
            }

            return copy;
        }

        /// <summary>
        /// Creates a new DimensionStyle that is a copy of the current instance.
        /// </summary>
        /// <returns>A new DimensionStyle that is a copy of this instance.</returns>
        public override object Clone()
        {
            return Clone(Name);
        }

        #endregion
    }
}