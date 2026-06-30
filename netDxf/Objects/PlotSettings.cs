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

namespace netDxf.Objects
{
    /// <summary>
    /// Represents the plot settings of a layout.
    /// </summary>
    public class PlotSettings :
        ICloneable
    {
        #region private fields

        private string pageSetupName;
        private string plotterName;
        private string paperSizeName;
        private string viewName;
        private string currentStyleSheet;

        private PaperMargin paperMargin;
        private Vector2 paperSize;
        private Vector2 origin;
        private Vector2 windowUpRight;
        private Vector2 windowBottomLeft;

        private bool scaleToFit ;
        private double numeratorScale;
        private double denominatorScale;
        private PlotFlags flags;
        private PlotType plotType;

        private PlotPaperUnits paperUnits;
        private PlotRotation rotation;

        private ShadePlotMode shadePlotMode;
        private ShadePlotResolutionMode shadePlotResolutionMode;
        private short shadePlotDPI;
        private Vector2 paperImageOrigin;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of <c>PlotSettings</c>.
        /// </summary>
        public PlotSettings()
        {
            pageSetupName = string.Empty;
            plotterName = "none_device";
            paperSizeName = "ISO_A4_(210.00_x_297.00_MM)";
            viewName = string.Empty;
            currentStyleSheet = string.Empty;

            paperMargin = new PaperMargin(7.5, 20.0, 7.5, 20.0);

            paperSize = new Vector2(210.0, 297.0);
            origin = Vector2.Zero;
            windowUpRight = Vector2.Zero;
            windowBottomLeft = Vector2.Zero;

            scaleToFit = true;
            numeratorScale = 1.0;
            denominatorScale = 1.0;
            flags = PlotFlags.DrawViewportsFirst | PlotFlags.PrintLineweights | PlotFlags.PlotPlotStyles | PlotFlags.UseStandardScale;
            plotType = PlotType.DrawingExtents;

            paperUnits = PlotPaperUnits.Milimeters;
            rotation = PlotRotation.Degrees90;

            shadePlotMode = ShadePlotMode.AsDisplayed;
            shadePlotResolutionMode = ShadePlotResolutionMode.Normal;
            shadePlotDPI = 300;
            paperImageOrigin = Vector2.Zero;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the page setup name.
        /// </summary>
        public string PageSetupName
        {
            get { return pageSetupName; }
            set { pageSetupName = value; }
        }

        /// <summary>
        /// Gets or sets the name of system printer or plot configuration file.
        /// </summary>
        public string PlotterName
        {
            get { return plotterName; }
            set { plotterName = value; }
        }

        /// <summary>
        /// Gets or set the paper size name.
        /// </summary>
        public string PaperSizeName
        {
            get { return paperSizeName; }
            set { paperSizeName = value; }
        }

        /// <summary>
        /// Gets or sets the plot view name.
        /// </summary>
        public string ViewName
        {
            get { return viewName; }
            set { viewName = value; }
        }

        /// <summary>
        /// Gets or sets the current style sheet name.
        /// </summary>
        public string CurrentStyleSheet
        {
            get { return currentStyleSheet; }
            set { currentStyleSheet = value; }
        }

        /// <summary>
        /// Gets or set the size, in millimeters, of unprintable margins of paper.
        /// </summary>
        public PaperMargin PaperMargin
        {
            get { return paperMargin; }
            set { paperMargin = value; }
        }

        /// <summary>
        /// Gets or sets the plot paper size: physical paper width and height in millimeters.
        /// </summary>
        public Vector2 PaperSize
        {
            get { return paperSize; }
            set { paperSize = value; }
        }

        /// <summary>
        /// Gets or sets the plot origin in millimeters.
        /// </summary>
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        /// <summary>
        /// Gets or sets the plot upper-right window corner.
        /// </summary>
        public Vector2 WindowUpRight
        {
            get { return windowUpRight; }
            set { windowUpRight = value; }
        }

        /// <summary>
        /// Gets or sets the plot lower-left window corner.
        /// </summary>
        public Vector2 WindowBottomLeft
        {
            get { return windowBottomLeft; }
            set { windowBottomLeft = value; }
        }

        /// <summary>
        /// Gets or sets if the plot scale will be automatically computed show the drawing fits the media.
        /// </summary>
        /// <remarks>
        /// If <c>ScaleToFit</c> is set to false the values specified by <c>PrintScaleNumerator</c> and <c>PrintScaleDenomiator</c> will be used.
        /// </remarks>
        public bool ScaleToFit
        {
            get { return scaleToFit; }
            set { scaleToFit = value; }
        }

        /// <summary>
        /// Gets or sets the numerator of custom print scale: real world paper units.
        /// </summary>
        /// <remarks>
        /// The paper units used are specified by the <c>PaperUnits</c> value.
        /// </remarks>
        public double PrintScaleNumerator
        {
            get { return numeratorScale; }
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The print scale numerator must be a number greater than zero.");
                }
                numeratorScale = value;
            }
        }

        /// <summary>
        /// Gets or sets the denominator of custom print scale: drawing units.
        /// </summary>
        public double PrintScaleDenominator
        {
            get { return denominatorScale; }
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The print scale denominator must be a number greater than zero.");
                }
                denominatorScale = value;
            }
        }

        /// <summary>
        /// Gets the scale factor.
        /// </summary>
        public double PrintScale
        {
            get { return numeratorScale / denominatorScale; }
        }

        /// <summary>
        /// Gets or sets the plot layout flags.
        /// </summary>
        public PlotFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        /// <summary>
        /// Gets or sets the portion of paper space to output to the media.
        /// </summary>
        public PlotType PlotType
        {
            get { return plotType; }
            set { plotType = value; }
        }

        /// <summary>
        /// Gets or sets the paper units.
        /// </summary>
        /// <remarks>This value is only applicable to the scale parameter <c>PrintScaleNumerator</c>.</remarks>
        public PlotPaperUnits PaperUnits
        {
            get { return paperUnits; }
            set { paperUnits = value; }
        }

        /// <summary>
        /// Gets or sets the paper rotation.
        /// </summary>
        public PlotRotation PaperRotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        /// <summary>
        /// Gets or sets the shade plot mode.
        /// </summary>
        public ShadePlotMode ShadePlotMode
        {
            get { return shadePlotMode; }
            set { shadePlotMode = value; }
        }

        /// <summary>
        /// Gets or sets the plot resolution mode.
        /// </summary>
        /// <remarks>
        /// if the <c>ShadePlotResolutionMode</c> is set to Custom the value specified by the <c>ShadPloDPI</c> will be used.
        /// </remarks>
        public ShadePlotResolutionMode ShadePlotResolutionMode
        {
            get { return shadePlotResolutionMode; }
            set { shadePlotResolutionMode = value; }
        }

        /// <summary>
        /// Gets or sets the shade plot custom DPI.
        /// </summary>
        public short ShadePlotDPI
        {
            get { return shadePlotDPI; }
            set
            {
                if (value < 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The valid shade plot DPI values range from 100 to the maximum value allowed by the plot device.");
                }
                shadePlotDPI = value;
            }
        }

        /// <summary>
        /// Gets or sets the paper image origin.
        /// </summary>
        public Vector2 PaperImageOrigin
        {
            get { return paperImageOrigin; }
            set { paperImageOrigin = value; }
        }

        #endregion

        #region implements ICloneable

        /// <summary>
        /// Creates a new plot settings that is a copy of the current instance.
        /// </summary>
        /// <returns>A new plot settings that is a copy of this instance.</returns>
        public object Clone()
        {
            return new PlotSettings
            {
                PageSetupName = pageSetupName,
                PlotterName = plotterName,
                PaperSizeName = paperSizeName,
                ViewName = viewName,
                CurrentStyleSheet = currentStyleSheet,
                PaperMargin = PaperMargin,
                PaperSize = paperSize,
                Origin = origin,
                WindowUpRight = windowUpRight,
                WindowBottomLeft = windowBottomLeft,
                ScaleToFit = scaleToFit,
                PrintScaleNumerator = numeratorScale,
                PrintScaleDenominator = denominatorScale,
                Flags = flags,
                PlotType = plotType,
                PaperUnits = paperUnits,
                PaperRotation = rotation,
                ShadePlotMode = shadePlotMode,
                ShadePlotResolutionMode = shadePlotResolutionMode,
                ShadePlotDPI = shadePlotDPI,
                PaperImageOrigin = paperImageOrigin
            };
        }

        #endregion
    }
}