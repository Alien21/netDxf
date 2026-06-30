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
using netDxf.Blocks;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a 3 point angular dimension <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Angular3PointDimension :
        Dimension
    {
        #region private fields

        private double offset;
        private Vector2 center;
        private Vector2 start;
        private Vector2 end;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Angular3PointDimension</c> class.
        /// </summary>
        public Angular3PointDimension()
            : this(Vector2.Zero, Vector2.UnitX, Vector2.UnitY, 0.1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular3PointDimension</c> class.
        /// </summary>
        /// <param name="arc"><see cref="Arc">Arc</see> to measure.</param>
        /// <param name="offset">Distance between the center of the arc and the dimension line.</param>
        public Angular3PointDimension(Arc arc, double offset)
            : this(arc, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular3PointDimension</c> class.
        /// </summary>
        /// <param name="arc">Angle <see cref="Arc">arc</see> to measure.</param>
        /// <param name="offset">Distance between the center of the arc and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular3PointDimension(Arc arc, double offset, DimensionStyle style)
            : base(DimensionType.Angular3Point)
        {
            if (arc == null)
            {
                throw new ArgumentNullException(nameof(arc));
            }

            Vector3 refPoint = MathHelper.Transform(arc.Center, arc.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            center = new Vector2(refPoint.X, refPoint.Y);
            start = Vector2.Polar(center, arc.Radius, arc.StartAngle * MathHelper.DegToRad);
            end = Vector2.Polar(center, arc.Radius, arc.EndAngle * MathHelper.DegToRad);
            this.offset = offset;
            Style = style ?? throw new ArgumentNullException(nameof(style));
            Normal = arc.Normal;
            Elevation = refPoint.Z;
            Update();
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular3PointDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center of the angle arc to measure.</param>
        /// <param name="startPoint">Angle start point.</param>
        /// <param name="endPoint">Angle end point.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        public Angular3PointDimension(Vector2 centerPoint, Vector2 startPoint, Vector2 endPoint, double offset)
            : this(centerPoint, startPoint, endPoint, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular3PointDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center of the angle arc to measure.</param>
        /// <param name="startPoint">Angle start point.</param>
        /// <param name="endPoint">Angle end point.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular3PointDimension(Vector2 centerPoint, Vector2 startPoint, Vector2 endPoint, double offset, DimensionStyle style)
            : base(DimensionType.Angular3Point)
        {
            center = centerPoint;
            start = startPoint;
            end = endPoint;
            this.offset = offset;
            Style = style ?? throw new ArgumentNullException(nameof(style));
            Update();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the center <see cref="Vector2">point</see> of the arc in OCS (object coordinate system).
        /// </summary>
        public Vector2 CenterPoint
        {
            get { return center; }
            set { center = value; }
        }

        /// <summary>
        /// Gets or sets the angle start <see cref="Vector2">point</see> of the dimension in OCS (object coordinate system).
        /// </summary>
        public Vector2 StartPoint
        {
            get { return start; }
            set { start = value; }
        }

        /// <summary>
        /// Gets or sets the angle end <see cref="Vector2">point</see> of the dimension in OCS (object coordinate system).
        /// </summary>
        public Vector2 EndPoint
        {
            get { return end; }
            set { end = value; }
        }

        /// <summary>
        /// Gets the location of the dimension line arc.
        /// </summary>
        public Vector2 ArcDefinitionPoint
        {
            get { return defPoint; }
        }

        /// <summary>
        /// Gets or sets the distance between the center point and the dimension line.
        /// </summary>
        /// <remarks>
        /// Positive values will measure the angle between the start point and the end point while negative values will measure the opposite arc angle.
        /// Even thought, zero values are allowed, they are not recommended.
        /// </remarks>
        public double Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        /// <summary>
        /// Actual measurement.
        /// </summary>
        public override double Measurement
        {
            get
            {
                Vector2 dirRef1 = start - center;
                Vector2 dirRef2 = end - center;
                if (Vector2.Equals(dirRef1, dirRef2))
                {
                    return 0.0;
                }

                if (Vector2.AreParallel(dirRef1, dirRef2))
                {
                    return 180.0;
                }

                double angle = Vector2.AngleBetween(dirRef1, dirRef2) * MathHelper.RadToDeg;

                if (offset < 0)
                {
                    return 360 - angle;
                }
                return angle;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculates the dimension offset from a point along the dimension line.
        /// </summary>
        /// <param name="point">Point along the dimension line.</param>
        public void SetDimensionLinePosition(Vector2 point)
        {
            double newOffset = Vector2.Distance(center, point);

            offset = newOffset;
            Vector2 dirPoint = point - center;
            double cross1 = Vector2.CrossProduct(start - center, dirPoint);
            double cross2 = Vector2.CrossProduct(end - center, dirPoint);

            if (!(cross1 >= 0) || !(cross2 < 0))
            {
                offset *= -1;
            }

            double startAngle = offset >= 0 ? Vector2.Angle(center, start) : Vector2.Angle(center, end);
            double midRot = startAngle + 0.5 * Measurement * MathHelper.DegToRad;
            Vector2 midDim = Vector2.Polar(center, Math.Abs(offset), midRot);
            defPoint = midDim;

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
            if (Vector3.Equals(Vector3.Zero, newNormal))
            {
                newNormal = Normal;
            }

            Matrix3 transOW = MathHelper.ArbitraryAxis(Normal);
            Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal).Transpose();

            Vector3 v = transOW * new Vector3(StartPoint.X, StartPoint.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            Vector2 newStart = new Vector2(v.X, v.Y);
            double newElevation = v.Z;

            v = transOW * new Vector3(EndPoint.X, EndPoint.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            Vector2 newEnd = new Vector2(v.X, v.Y);

            v = transOW * new Vector3(CenterPoint.X, CenterPoint.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            Vector2 newCenter = new Vector2(v.X, v.Y);

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

            StartPoint = newStart;
            EndPoint = newEnd;
            CenterPoint = newCenter;
            Elevation = newElevation;
            Normal = newNormal;

            SetDimensionLinePosition(defPoint);
        }

        /// <summary>
        /// Calculate the dimension reference points.
        /// </summary>
        protected override void CalculateReferencePoints()
        {
            DimensionStyleOverride styleOverride;
            double startAngle = offset >= 0 ? Vector2.Angle(center, start) : Vector2.Angle(center, end);
            double midRot = startAngle + 0.5 * Measurement * MathHelper.DegToRad;
            Vector2 midDim = Vector2.Polar(center, Math.Abs(offset), midRot);

            defPoint = midDim;

            if (TextPositionManuallySet)
            {
                DimensionStyleFitTextMove moveText = Style.FitTextMove;
                if (StyleOverrides.TryGetValue(DimensionStyleOverrideType.FitTextMove, out styleOverride))
                {
                    moveText = (DimensionStyleFitTextMove)styleOverride.Value;
                }

                if (moveText == DimensionStyleFitTextMove.BesideDimLine)
                {
                    SetDimensionLinePosition(textRefPoint);
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
        /// Creates a new Angular3PointDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Angular3PointDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            Angular3PointDimension entity = new Angular3PointDimension
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
                //Angular3PointDimension properties
                CenterPoint = center,
                StartPoint = start,
                EndPoint = end,
                Offset = offset
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