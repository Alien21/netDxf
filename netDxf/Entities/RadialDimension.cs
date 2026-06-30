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
using System.Diagnostics;
using netDxf.Blocks;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a radial dimension <see cref="EntityObject">entity</see>.
    /// </summary>
    public class RadialDimension :
        Dimension
    {
        #region private fields

        private Vector2 center;
        private Vector2 refPoint;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        public RadialDimension()
            : this(Vector2.Zero, Vector2.UnitX, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        /// <param name="arc"><see cref="Arc">Arc</see> to measure.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public RadialDimension(Arc arc, double rotation)
            : this(arc, rotation, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        /// <param name="arc"><see cref="Arc">Arc</see> to measure.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public RadialDimension(Arc arc, double rotation, DimensionStyle style)
            : base(DimensionType.Radius)
        {
            if (arc == null)
            {
                throw new ArgumentNullException(nameof(arc));
            }

            Vector3 ocsCenter = MathHelper.Transform(arc.Center, arc.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            center = new Vector2(ocsCenter.X, ocsCenter.Y);
            refPoint = Vector2.Polar(center, arc.Radius, rotation*MathHelper.DegToRad);

            Style = style ?? throw new ArgumentNullException(nameof(style));
            Normal = arc.Normal;
            Elevation = ocsCenter.Z;
            Update();
        }

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        /// <param name="circle"><see cref="Circle">Circle</see> to measure.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public RadialDimension(Circle circle, double rotation)
            : this(circle, rotation, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        /// <param name="circle"><see cref="Circle">Circle</see> to measure.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public RadialDimension(Circle circle, double rotation, DimensionStyle style)
            : base(DimensionType.Radius)
        {
            if (circle == null)
            {
                throw new ArgumentNullException(nameof(circle));
            }

            Vector3 ocsCenter = MathHelper.Transform(circle.Center, circle.Normal, CoordinateSystem.World, CoordinateSystem.Object);
            center = new Vector2(ocsCenter.X, ocsCenter.Y);
            refPoint = Vector2.Polar(center, circle.Radius, rotation*MathHelper.DegToRad);

            Style = style ?? throw new ArgumentNullException(nameof(style));
            Normal = circle.Normal;
            Elevation = ocsCenter.Z;
            Update();
        }

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center <see cref="Vector2">point</see> of the circumference.</param>
        /// <param name="referencePoint"><see cref="Vector2">Point</see> on circle or arc.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public RadialDimension(Vector2 centerPoint, Vector2 referencePoint)
            : this(centerPoint, referencePoint, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>RadialDimension</c> class.
        /// </summary>
        /// <param name="centerPoint">Center <see cref="Vector2">point</see> of the circumference.</param>
        /// <param name="referencePoint"><see cref="Vector2">Point</see> on circle or arc.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The center point and the definition point define the distance to be measure.</remarks>
        public RadialDimension(Vector2 centerPoint, Vector2 referencePoint, DimensionStyle style)
            : base(DimensionType.Radius)
        {
            if (Vector2.Equals(centerPoint, referencePoint))
            {
                throw new ArgumentException("The center and the reference point cannot be the same");
            }
            center = centerPoint;
            refPoint = referencePoint;
            Style = style ?? throw new ArgumentNullException(nameof(style));

            Update();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the center <see cref="Vector2">point</see> of the circumference in OCS (object coordinate system).
        /// </summary>
        public Vector2 CenterPoint
        {
            get { return center; }
            set { center = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Vector2">point</see> on circumference or arc in OCS (object coordinate system).
        /// </summary>
        public Vector2 ReferencePoint
        {
            get { return refPoint; }
            set { refPoint = value; }
        }

        /// <summary>
        /// Actual measurement.
        /// </summary>
        public override double Measurement
        {
            get { return Vector2.Distance(center, refPoint); }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculates the reference point and dimension offset from a point along the dimension line.
        /// </summary>
        /// <param name="point">Point along the dimension line.</param>
        public void SetDimensionLinePosition(Vector2 point)
        {
            double radius = Vector2.Distance(center, refPoint);
            double rotation = Vector2.Angle(center, point);
            defPoint = center;
            refPoint = Vector2.Polar(center, radius, rotation);

            if (!TextPositionManuallySet)
            {
                DimensionStyleOverride styleOverride;
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
                double arrowSize = Style.ArrowSize;
                if (StyleOverrides.TryGetValue(DimensionStyleOverrideType.ArrowSize, out styleOverride))
                {
                    arrowSize = (double)styleOverride.Value;
                }

                Vector2 vect = Vector2.Normalize(refPoint - center);
                if (Vector2.IsNaN(vect))
                {
                    vect = Vector2.Zero;
                }
                double minOffset = (2 * arrowSize + textGap) * scale;
                textRefPoint = refPoint + minOffset * vect;
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
        /// The transformation will not be applied if the resulting center and reference points are the same.<br />
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

            Vector3 v = transOW * new Vector3(CenterPoint.X, CenterPoint.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            Vector2 newCenter = new Vector2(v.X, v.Y);
            double newElevation = v.Z;

            v = transOW * new Vector3(ReferencePoint.X, ReferencePoint.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            Vector2 newRefPoint = new Vector2(v.X, v.Y);

            if(Vector2.Equals(newCenter, newRefPoint))
            {
                Debug.Assert(false, "The transformation cannot be applied, the resulting center and reference points are the same.");
                return;
            }

            v = transOW * new Vector3(textRefPoint.X, textRefPoint.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            textRefPoint = new Vector2(v.X, v.Y);

            v = transOW * new Vector3(defPoint.X, defPoint.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            defPoint = new Vector2(v.X, v.Y);

            CenterPoint = newCenter;
            ReferencePoint = newRefPoint;
            Elevation = newElevation;
            Normal = newNormal;
        }

        /// <summary>
        /// Calculate the dimension reference points.
        /// </summary>
        protected override void CalculateReferencePoints()
        {
            if (Vector2.Equals(center, refPoint))
            {
                throw new ArgumentException("The center and the reference point cannot be the same");
            }

            defPoint = center;

            if (TextPositionManuallySet)
            {
                SetDimensionLinePosition(textRefPoint);
            }
            else
            {
                DimensionStyleOverride styleOverride;

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
                double arrowSize = Style.ArrowSize;
                if (StyleOverrides.TryGetValue(DimensionStyleOverrideType.ArrowSize, out styleOverride))
                {
                    arrowSize = (double)styleOverride.Value;
                }

                Vector2 vec = Vector2.Normalize(refPoint - center);
                double minOffset = (2 * arrowSize + textGap) * scale;
                textRefPoint = refPoint + minOffset * vec;
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
        /// Creates a new RadialDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new RadialDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            RadialDimension entity = new RadialDimension
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
                //RadialDimension properties
                CenterPoint = center,
                ReferencePoint = refPoint
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