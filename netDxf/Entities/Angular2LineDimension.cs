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
using System.Diagnostics;
using netDxf.Blocks;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a 3 point angular dimension <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Angular2LineDimension :
        Dimension
    {
        #region private fields

        private double offset;
        private Vector2 startFirstLine;
        private Vector2 endFirstLine;
        private Vector2 startSecondLine;
        private Vector2 endSecondLine;
        private Vector2 arcDefinitionPoint;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        public Angular2LineDimension()
            : this(Vector2.Zero, Vector2.UnitX, Vector2.Zero, Vector2.UnitY, 0.1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, double offset)
            : this(firstLine, secondLine, offset, Vector3.UnitZ, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, double offset, Vector3 normal)
            : this(firstLine, secondLine, offset, normal, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, double offset, DimensionStyle style)
            : this(firstLine, secondLine, offset, Vector3.UnitZ, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, double offset, Vector3 normal, DimensionStyle style)
            : base(DimensionType.Angular)
        {
            if (firstLine == null)
            {
                throw new ArgumentNullException(nameof(firstLine));
            }

            if (secondLine == null)
            {
                throw new ArgumentNullException(nameof(secondLine));
            }

            if (Vector3.AreParallel(firstLine.Direction, secondLine.Direction))
            {
                throw new ArgumentException("The two lines that define the dimension are parallel.");
            }

            List<Vector3> ocsPoints =
                MathHelper.Transform(
                    new[]
                    {
                        firstLine.StartPoint,
                        firstLine.EndPoint,
                        secondLine.StartPoint,
                        secondLine.EndPoint
                    },
                    normal, CoordinateSystem.World, CoordinateSystem.Object);

            startFirstLine = new Vector2(ocsPoints[0].X, ocsPoints[0].Y);
            endFirstLine = new Vector2(ocsPoints[1].X, ocsPoints[1].Y);
            startSecondLine = new Vector2(ocsPoints[2].X, ocsPoints[2].Y);
            endSecondLine = new Vector2(ocsPoints[3].X, ocsPoints[3].Y);
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value must be equal or greater than zero.");
            }
            this.offset = offset;
            Style = style ?? throw new ArgumentNullException(nameof(style));
            Normal = normal;
            Elevation = ocsPoints[0].Z;
            Update();
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="startFirstLine">Start <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="endFirstLine">End <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="startSecondLine">Start <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="endSecondLine">End <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        public Angular2LineDimension(Vector2 startFirstLine, Vector2 endFirstLine, Vector2 startSecondLine, Vector2 endSecondLine, double offset)
            : this(startFirstLine, endFirstLine, startSecondLine, endSecondLine, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="startFirstLine">Start <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="endFirstLine">End <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="startSecondLine">Start <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="endSecondLine">End <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular2LineDimension(Vector2 startFirstLine, Vector2 endFirstLine, Vector2 startSecondLine, Vector2 endSecondLine, double offset, DimensionStyle style)
            : base(DimensionType.Angular)
        {
            Vector2 dir1 = endFirstLine - startFirstLine;
            Vector2 dir2 = endSecondLine - startSecondLine;
            if (Vector2.AreParallel(dir1, dir2))
            {
                throw new ArgumentException("The two lines that define the dimension are parallel.");
            }

            this.startFirstLine = startFirstLine;
            this.endFirstLine = endFirstLine;
            this.startSecondLine = startSecondLine;
            this.endSecondLine = endSecondLine;

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value must be equal or greater than zero.");
            }
            this.offset = offset;

            Style = style ?? throw new ArgumentNullException(nameof(style));
            Update();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the center <see cref="Vector2">point</see> of the measured arc in local coordinates.
        /// </summary>
        public Vector2 CenterPoint
        {
            get
            {
                return MathHelper.FindIntersection(
                    startFirstLine, endFirstLine - startFirstLine,
                    startSecondLine, endSecondLine - startSecondLine);
            }
        }

        /// <summary>
        /// Start <see cref="Vector2">point</see> of the first line that defines the angle to measure in local coordinates.
        /// </summary>
        public Vector2 StartFirstLine
        {
            get { return startFirstLine; }
            set { startFirstLine = value; }
        }

        /// <summary>
        /// End <see cref="Vector2">point</see> of the first line that defines the angle to measure in local coordinates.
        /// </summary>
        public Vector2 EndFirstLine
        {
            get { return endFirstLine; }
            set { endFirstLine = value; }
        }

        /// <summary>
        /// Start <see cref="Vector2">point</see> of the second line that defines the angle to measure in OCS (object coordinate system).
        /// </summary>
        public Vector2 StartSecondLine
        {
            get { return startSecondLine; }
            set { startSecondLine = value; }
        }

        /// <summary>
        /// End <see cref="Vector2">point</see> of the second line that defines the angle to measure in OCS (object coordinate system).
        /// </summary>
        public Vector2 EndSecondLine
        {
            get { return endSecondLine; }
            set { endSecondLine = value; }
        }

        /// <summary>
        /// Gets the location of the dimension line arc.
        /// </summary>
        public Vector2 ArcDefinitionPoint
        {
            get { return arcDefinitionPoint; }
            internal set { arcDefinitionPoint = value; }
        }

        /// <summary>
        /// Gets or sets the distance between the center point and the dimension line.
        /// </summary>
        /// <remarks>
        /// Offset values cannot be negative and, even thought, zero values are allowed, they are not recommended.
        /// </remarks>
        public double Offset
        {
            get { return offset; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The offset value must be equal or greater than zero.");
                }
                offset = value;
            }
        }

        /// <summary>
        /// Actual measurement.
        /// </summary>
        public override double Measurement
        {
            get
            {
                Vector2 dirRef1 = endFirstLine - startFirstLine;
                Vector2 dirRef2 = endSecondLine - startSecondLine;
                return Vector2.AngleBetween(dirRef1, dirRef2) * MathHelper.RadToDeg;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculates the dimension offset from a point along the dimension line.
        /// </summary>
        /// <param name="point">Point along the dimension line.</param>
        /// <remarks>
        /// The start and end points of the reference lines will be modified,
        /// the angle measurement is always made from the direction of the first line to the direction of the second line.
        /// </remarks>
        public void SetDimensionLinePosition(Vector2 point)
        {
            SetDimensionLinePosition(point, true);
        }

        #endregion

        #region private methods

        private void SetDimensionLinePosition(Vector2 point, bool updateRefs)
        {
            Vector2 dir1 = endFirstLine - startFirstLine;
            Vector2 dir2 = endSecondLine - startSecondLine;
            if (Vector2.AreParallel(dir1, dir2))
            {
                throw new ArgumentException("The two lines that define the dimension are parallel.");
            }

            Vector2 center = CenterPoint;

            if (updateRefs)
            {
                double cross = Vector2.CrossProduct(EndFirstLine - StartFirstLine, EndSecondLine - StartSecondLine);
                if (cross < 0)
                {
                    (startFirstLine, startSecondLine) = (startSecondLine, startFirstLine);
                    (endFirstLine, endSecondLine) = (endSecondLine, endFirstLine);

                    //MathHelper.Swap(ref this.startFirstLine, ref this.startSecondLine);
                    //MathHelper.Swap(ref this.endFirstLine, ref this.endSecondLine);
                }

                Vector2 ref1Start = StartFirstLine;
                Vector2 ref1End = EndFirstLine;
                Vector2 ref2Start = StartSecondLine;
                Vector2 ref2End = EndSecondLine;
                Vector2 dirRef1 = ref1End - ref1Start;
                Vector2 dirRef2 = ref2End - ref2Start;

                Vector2 dirOffset = point - center;
                double crossStart = Vector2.CrossProduct(dirRef1, dirOffset);
                double crossEnd = Vector2.CrossProduct(dirRef2, dirOffset);

                if (crossStart >= 0 && crossEnd >= 0)
                {
                    StartFirstLine = ref2Start;
                    EndFirstLine = ref2End;
                    StartSecondLine = ref1End;
                    EndSecondLine = ref1Start;
                }
                else if (crossStart < 0 && crossEnd >= 0)
                {
                    StartFirstLine = ref1End;
                    EndFirstLine = ref1Start;
                    StartSecondLine = ref2End;
                    EndSecondLine = ref2Start;
                }
                else if (crossStart < 0 && crossEnd < 0)
                {
                    StartFirstLine = ref2End;
                    EndFirstLine = ref2Start;
                    StartSecondLine = ref1Start;
                    EndSecondLine = ref1End;
                }
            }

            double newOffset = Vector2.Distance(center, point);
            offset = MathHelper.IsZero(newOffset) ? MathHelper.Epsilon : newOffset;
            
            defPoint = endSecondLine;

            double measure = Measurement * MathHelper.DegToRad;
            double startAngle = Vector2.Angle(center, endFirstLine);
            double midRot = startAngle + 0.5 * measure;
            Vector2 midDim = Vector2.Polar(center, offset, midRot);
            arcDefinitionPoint = midDim;

            if (!TextPositionManuallySet)
            {
                DimensionStyleOverride styleOverride;
                double textGap = Style.TextOffset;
                if (StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out styleOverride))
                {
                    textGap = (double) styleOverride.Value;
                }
                double scale = Style.DimScaleOverall;
                if (StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
                {
                    scale = (double) styleOverride.Value;
                }

                double gap = textGap * scale;
                textRefPoint = midDim + gap * Vector2.Normalize(midDim - center);
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Moves, scales, and/or rotates the current entity given a 3x3 transformation matrix and a translation vector.
        /// </summary>
        /// <param name="transformation">Transformation matrix.</param>
        /// <param name="translation">Translation vector.</param>
        /// <remarks>
        /// Non-uniform and zero scaling local to the dimension entity are not supported.<br />
        /// The transformation will not be applied if the resulting reference lines are parallel.<br />
        /// Matrix3 adopts the convention of using column vectors to represent a transformation matrix.
        /// </remarks>
        public override void TransformBy(Matrix3 transformation, Vector3 translation)
        {
            Vector3 newNormal = transformation * Normal;
            if (Vector3.Equals(Vector3.Zero, newNormal)) newNormal = Normal;

            Matrix3 transOW = MathHelper.ArbitraryAxis(Normal);
            Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal).Transpose();

            Vector3 v = transOW * new Vector3(StartFirstLine.X, StartFirstLine.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            Vector2 newStart1 = new Vector2(v.X, v.Y);
            double newElevation = v.Z;

            v = transOW * new Vector3(EndFirstLine.X, EndFirstLine.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            Vector2 newEnd1 = new Vector2(v.X, v.Y);

            v = transOW * new Vector3(StartSecondLine.X, StartSecondLine.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            Vector2 newStart2 = new Vector2(v.X, v.Y);

            v = transOW * new Vector3(EndSecondLine.X, EndSecondLine.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            Vector2 newEnd2 = new Vector2(v.X, v.Y);

            Vector2 dir1 = newEnd1 - newStart1;
            Vector2 dir2 = newEnd2 - newStart2;
            if (Vector2.AreParallel(dir1, dir2))
            {
                Debug.Assert(false, "The transformation cannot be applied, the resulting reference lines are parallel.");
                return;
            }

            v = transOW * new Vector3(ArcDefinitionPoint.X, ArcDefinitionPoint.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            Vector2 newArcDefPoint = new Vector2(v.X, v.Y);

            if (TextPositionManuallySet)
            {
                v = transOW * new Vector3(textRefPoint.X, textRefPoint.Y, Elevation);
                v = transformation * v + translation;
                v = transWO * v;
                textRefPoint = new Vector2(v.X, v.Y);
            }

            v = transOW * new Vector3(defPoint.X, defPoint.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            defPoint = new Vector2(v.X, v.Y);

            StartFirstLine = newStart1;
            EndFirstLine = newEnd1;
            StartSecondLine = newStart2;
            EndSecondLine = newEnd2;
            ArcDefinitionPoint = newArcDefPoint;
            Elevation = newElevation;
            Normal = newNormal;

            SetDimensionLinePosition(newArcDefPoint);
        }

        /// <summary>
        /// Calculate the dimension reference points.
        /// </summary>
        protected override void CalculateReferencePoints()
        {
            Vector2 dir1 = endFirstLine - startFirstLine;
            Vector2 dir2 = endSecondLine - startSecondLine;
            if (Vector2.AreParallel(dir1, dir2))
            {
                throw new ArgumentException("The two lines that define the dimension are parallel.");
            }

            DimensionStyleOverride styleOverride;

            double measure = Measurement * MathHelper.DegToRad;
            Vector2 center = CenterPoint;

            double startAngle = Vector2.Angle(center, endFirstLine);
            double midRot = startAngle + 0.5 * measure;
            Vector2 midDim = Vector2.Polar(center, offset, midRot);

            defPoint = endSecondLine;
            arcDefinitionPoint = midDim;

            if (TextPositionManuallySet)
            {
                DimensionStyleFitTextMove moveText = Style.FitTextMove;
                if (StyleOverrides.TryGetValue(DimensionStyleOverrideType.FitTextMove, out styleOverride))
                {
                    moveText = (DimensionStyleFitTextMove)styleOverride.Value;
                }

                if (moveText == DimensionStyleFitTextMove.BesideDimLine)
                {
                    SetDimensionLinePosition(textRefPoint, false);
                }
            }
            else
            {
                double textGap = Style.TextOffset;
                if (StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out styleOverride))
                {
                    textGap = (double)styleOverride.Value;
                }
                double scale = Style.DimScaleOverall;
                if (StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
                {
                    scale = (double)styleOverride.Value;
                }

                double gap = textGap * scale;
                textRefPoint = midDim + gap * Vector2.Normalize(midDim - center);
            }
        }

        /// <summary>
        /// Gets the block that contains the entities that make up the dimension picture.
        /// </summary>
        /// <param name="name">Name to be assigned to the generated block.</param>
        /// <returns>The block that represents the actual dimension.</returns>
        protected override Block BuildBlock(string name)
        {
            return DimensionBlock.Build(this, name);
        }

        /// <summary>
        /// Creates a new Angular2LineDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Angular2LineDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            Angular2LineDimension entity = new Angular2LineDimension
            {
                //EntityObject properties
                Layer = (Layer) Layer.Clone(),
                Linetype = (Linetype) Linetype.Clone(),
                Color = (AciColor) Color.Clone(),
                Lineweight = Lineweight,
                Transparency = (Transparency) Transparency.Clone(),
                LinetypeScale = LinetypeScale,
                Normal = Normal,
                IsVisible = IsVisible,
                //Dimension properties
                Style = (DimensionStyle) Style.Clone(),
                DefinitionPoint = DefinitionPoint,
                TextReferencePoint = TextReferencePoint,
                TextPositionManuallySet = TextPositionManuallySet,
                TextRotation = TextRotation,
                AttachmentPoint = AttachmentPoint,
                LineSpacingStyle = LineSpacingStyle,
                LineSpacingFactor = LineSpacingFactor,
                UserText = UserText,
                Elevation = Elevation,
                //Angular2LineDimension properties
                StartFirstLine = startFirstLine,
                EndFirstLine = endFirstLine,
                StartSecondLine = startSecondLine,
                EndSecondLine = endSecondLine,
                Offset = offset,
                arcDefinitionPoint = arcDefinitionPoint
            };

            foreach (DimensionStyleOverride styleOverride in StyleOverrides.Values)
            {
                object copy = styleOverride.Value is ICloneable value ? value.Clone() : styleOverride.Value;
                entity.StyleOverrides.Add(new DimensionStyleOverride(styleOverride.Type, copy));
            }

            foreach (XData data in XData.Values)
            {
                entity.XData.Add((XData) data.Clone());
            }

            return entity;
        }

        #endregion
    }
}