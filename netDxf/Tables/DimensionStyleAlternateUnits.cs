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
using netDxf.Units;

namespace netDxf.Tables
{
    /// <summary>
    /// Represents the way alternate units are formatted in dimension entities.
    /// </summary>
    /// <remarks>Alternative units are not applicable for angular dimensions.</remarks>
    public class DimensionStyleAlternateUnits :
        ICloneable
    {
        #region private fields

        private bool dimalt;
        private LinearUnitType dimaltu;
        private bool stackedUnits;
        private short dimaltd;
        private double dimaltf;
        private double dimaltrnd;
        private string dimPrefix;
        private string dimSuffix;
        private bool suppressLinearLeadingZeros;
        private bool suppressLinearTrailingZeros;
        private bool suppressZeroFeet;
        private bool suppressZeroInches;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>DimensionStyleUnitsFormat</c> class.
        /// </summary>
        public DimensionStyleAlternateUnits()
        {
            dimalt = false;
            dimaltd = 2;
            dimPrefix = string.Empty;
            dimSuffix = string.Empty;
            dimaltf = 25.4;
            dimaltu = LinearUnitType.Decimal;
            stackedUnits = false;
            suppressLinearLeadingZeros = false;
            suppressLinearTrailingZeros = false;
            suppressZeroFeet = true;
            suppressZeroInches = true;
            dimaltrnd = 0.0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets if the alternate measurement units are added to the dimension text.  (DIMALT)
        /// </summary>
        public bool Enabled
        {
            get { return dimalt; }
            set { dimalt = value; }
        }

        /// <summary>
        /// Sets the number of decimal places displayed for the alternate units of a dimension. (DIMALTD)
        /// </summary>
        /// <remarks>
        /// Default: 4<br/>
        /// It is recommended to use values in the range 0 to 8.<br/>
        /// For architectural and fractional the precision used for the minimum fraction is 1/2^LinearDecimalPlaces.
        /// </remarks>
        public short LengthPrecision
        {
            get { return dimaltd; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The length precision must be equals or greater than zero.");
                }
                dimaltd = value;
            }
        }

        /// <summary>
        /// Specifies the text prefix for the dimension. (DIMAPOST)
        /// </summary>
        public string Prefix
        {
            get { return dimPrefix; }
            set { dimPrefix = value ?? string.Empty; }
        }

        /// <summary>
        /// Specifies the text suffix for the dimension. (DIMAPOST)
        /// </summary>
        public string Suffix
        {
            get { return dimSuffix; }
            set { dimSuffix = value ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets the multiplier used as the conversion factor between primary and alternate units. (DIMALTF)
        /// </summary>
        /// <remarks>
        /// to convert inches to millimeters, enter 25.4.
        /// The value has no effect on angular dimensions, and it is not applied to the rounding value or the plus or minus tolerance values. 
        /// </remarks>
        public double Multiplier
        {
            get { return dimaltf; }
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The multiplier for alternate units must be greater than zero0.");
                }
                dimaltf = value;
            }
        }

        /// <summary>
        /// Gets or sets the alternate units for all dimension types except angular. (DIMALTU)
        /// </summary>
        /// <remarks>
        /// Scientific<br/>
        /// Decimal<br/>
        /// Engineering<br/>
        /// Architectural<br/>
        /// Fractional
        /// </remarks>
        public LinearUnitType LengthUnits
        {
            get { return dimaltu; }
            set { dimaltu = value; }
        }

        /// <summary>
        /// Gets or set if the Architectural or Fractional linear units will be shown stacked or not. (DIMALTU)
        /// </summary>
        /// <remarks>
        /// This value only is applicable if the <c>DimLengthUnits</c> property has been set to Architectural or Fractional,
        /// for any other value this parameter is not applicable.
        /// </remarks>
        public bool StackUnits
        {
            get { return stackedUnits; }
            set { stackedUnits = value; }
        }

        /// <summary>
        /// Suppresses leading zeros in linear decimal alternate units. (DIMALTZ)
        /// </summary>
        /// <remarks>This value is part of the DIMALTZ variable.</remarks>
        public bool SuppressLinearLeadingZeros
        {
            get { return suppressLinearLeadingZeros; }
            set { suppressLinearLeadingZeros = value; }
        }

        /// <summary>
        /// Suppresses trailing zeros in linear decimal alternate units. (DIMALTZ)
        /// </summary>
        /// <remarks>This value is part of the DIMALTZ variable.</remarks>
        public bool SuppressLinearTrailingZeros
        {
            get { return suppressLinearTrailingZeros; }
            set { suppressLinearTrailingZeros = value; }
        }

        /// <summary>
        /// Suppresses zero feet in architectural alternate units. (DIMALTZ)
        /// </summary>
        /// <remarks>This value is part of the DIMALTZ variable.</remarks>
        public bool SuppressZeroFeet
        {
            get { return suppressZeroFeet; }
            set { suppressZeroFeet = value; }
        }

        /// <summary>
        /// Suppresses zero inches in architectural alternate units. (DIMALTZ)
        /// </summary>
        /// <remarks>This value is part of the DIMALTZ variable.</remarks>
        public bool SuppressZeroInches
        {
            get { return suppressZeroInches; }
            set { suppressZeroInches = value; }
        }

        /// <summary>
        /// Gets or sets the value to round all dimensioning distances. (DIMALTRND)
        /// </summary>
        /// <remarks>
        /// Default: 0 (no rounding off).<br/>
        /// If DIMRND is set to 0.25, all distances round to the nearest 0.25 unit.
        /// If you set DIMRND to 1.0, all distances round to the nearest integer.
        /// Note that the number of digits edited after the decimal point depends on the precision set by DIMDEC.
        /// DIMRND does not apply to angular dimensions.
        /// </remarks>
        public double Roundoff
        {
            get { return dimaltrnd; }
            set
            {
                if (value < 0.000001 && !MathHelper.IsZero(value, double.Epsilon))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The nearest value to round all distances must be equal or greater than 0.000001 or zero (no rounding off).");
                }
                dimaltrnd = value;
            }
        }

        #endregion

        #region implements ICloneable

        /// <summary>
        /// Creates a new <c>DimensionStyle.DimensionStyleAlternateUnits</c> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <c>DimensionStyle.DimensionStyleAlternateUnits</c> that is a copy of this instance.</returns>
        public object Clone()
        {
            DimensionStyleAlternateUnits copy = new DimensionStyleAlternateUnits()
            {
                Enabled = dimalt,
                LengthUnits = dimaltu,
                StackUnits = stackedUnits,
                LengthPrecision = dimaltd,
                Multiplier = dimaltf,
                Roundoff = dimaltrnd,
                Prefix = dimPrefix,
                Suffix = dimSuffix,
                SuppressLinearLeadingZeros = suppressLinearLeadingZeros,
                SuppressLinearTrailingZeros = suppressLinearTrailingZeros,
                SuppressZeroFeet = suppressZeroFeet,
                SuppressZeroInches = suppressZeroInches
            };

            return copy;
        }

        #endregion
    }
}