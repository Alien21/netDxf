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
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a shape entity.
    /// </summary>
    public class Shape :
        EntityObject
    {
        #region delegates and events

        public delegate void StyleChangedEventHandler(Shape sender, TableObjectChangedEventArgs<ShapeStyle> e);
        public event StyleChangedEventHandler StyleChanged;
        protected virtual ShapeStyle OnStyleChangedEvent(ShapeStyle oldStyle, ShapeStyle newStyle)
        {
            StyleChangedEventHandler ae = StyleChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<ShapeStyle> eventArgs = new TableObjectChangedEventArgs<ShapeStyle>(oldStyle, newStyle);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newStyle;
        }

        #endregion

        #region private fields

        private string name;
        private ShapeStyle style;
        private Vector3 position;
        private double size;
        private double rotation;
        private double obliqueAngle;
        private double widthFactor;
        private double thickness;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Shape</c> class.
        /// </summary>
        /// <param name="name">Name of the shape which geometry is defined in the shape <see cref="ShapeStyle">style</see>.</param>
        /// <param name="style">Shape <see cref="ShapeStyle">style</see>.</param>
        public Shape(string name, ShapeStyle style) 
            : this(name, style, Vector3.Zero, 1.0, 0.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Shape</c> class.
        /// </summary>
        /// <param name="name">Name of the shape which geometry is defined in the shape <see cref="ShapeStyle">style</see>.</param>
        /// <param name="style">Shape <see cref="ShapeStyle">style</see>.</param>
        /// <param name="position">Shape insertion point.</param>
        /// <param name="size">Shape size.</param>
        /// <param name="rotation">Shape rotation.</param>
        public Shape(string name, ShapeStyle style, Vector3 position, double size, double rotation) 
            : base(EntityType.Shape, DxfObjectCode.Shape)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            this.name = name;
            this.style = style ?? throw new ArgumentNullException(nameof(style));
            this.position = position;
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "The shape size must be greater than zero.");
            }
            this.size = size;
            this.rotation = rotation;
            obliqueAngle = 0.0;
            widthFactor = 1.0;
            thickness = 0.0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the shape name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                name = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ShapeStyle">shape style</see>.
        /// </summary>
        public ShapeStyle Style
        {
            get { return style; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                style = OnStyleChangedEvent(style, value);
            }
        }

        /// <summary>
        /// Gets or sets the shape <see cref="Vector3">insertion point</see> in world coordinates.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets or sets the size of the shape.
        /// </summary>
        /// <remarks>
        /// The shape size is relative to the actual size of the shape definition.
        /// The size value works as an scale value applied to the dimensions of the shape definition.
        /// The DXF allows for negative values but that is the same as rotating the shape 180 degrees.<br />
        /// Size values must be greater than zero. Default: 1.0.
        /// </remarks>
        public double Size
        {
            get { return size; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The shape size must be greater than zero.");
                }
                size = value;
            }
        }

        /// <summary>
        /// Gets or sets the shape rotation in degrees.
        /// </summary>
        public double Rotation
        {
            get { return rotation; }
            set { rotation = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the shape oblique angle in degrees.
        /// </summary>
        public double ObliqueAngle
        {
            get { return obliqueAngle; }
            set { obliqueAngle = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the shape width factor.
        /// </summary>
        /// <remarks>Width factor values cannot be zero. Default: 1.0.</remarks>
        public double WidthFactor
        {
            get { return widthFactor; }
            set
            {
                if (MathHelper.IsZero(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The shape width factor cannot be zero.");
                }
                widthFactor = value;
            }
        }

        /// <summary>
        /// Gets or set the shape thickness.
        /// </summary>
        public double Thickness
        {
            get { return thickness; }
            set { thickness = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Moves, scales, and/or rotates the current entity given a 3x3 transformation matrix and a translation vector.
        /// </summary>
        /// <param name="transformation">Transformation matrix.</param>
        /// <param name="translation">Translation vector.</param>
        /// <remarks>Matrix3 adopts the convention of using column vectors to represent a transformation matrix.</remarks>
        public override void TransformBy(Matrix3 transformation, Vector3 translation)
        {
            bool mirrorShape;

            Vector3 newPosition = transformation * Position + translation;
            Vector3 newNormal = transformation * Normal;
            if (Vector3.Equals(Vector3.Zero, newNormal))
            {
                newNormal = Normal;
            }

            Matrix3 transOW = MathHelper.ArbitraryAxis(Normal);

            Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal);
            transWO = transWO.Transpose();

            List<Vector2> uv = MathHelper.Transform(
                new[]
                {
                    Vector2.UnitX * WidthFactor * Size,
                    new Vector2(Size * Math.Tan(ObliqueAngle * MathHelper.DegToRad), Size)
                },
                Rotation * MathHelper.DegToRad,
                CoordinateSystem.Object, CoordinateSystem.World);

            Vector3 v;
            v = transOW * new Vector3(uv[0].X, uv[0].Y, 0.0);
            v = transformation * v;
            v = transWO * v;
            Vector2 newUvector = new Vector2(v.X, v.Y);

            v = transOW * new Vector3(uv[1].X, uv[1].Y, 0.0);
            v = transformation * v;
            v = transWO * v;
            Vector2 newVvector = new Vector2(v.X, v.Y);

            double newRotation = Vector2.Angle(newUvector) * MathHelper.RadToDeg;
            double newObliqueAngle = Vector2.Angle(newVvector) * MathHelper.RadToDeg;

            if (Vector2.CrossProduct(newUvector, newVvector) < 0)
            {
                newRotation += 180;
                newObliqueAngle = 270 - (newRotation - newObliqueAngle);
                if (newObliqueAngle >= 360)
                {
                    newObliqueAngle -= 360;
                }
                mirrorShape = true;
            }
            else
            {
                newObliqueAngle = 90 + (newRotation - newObliqueAngle);
                mirrorShape = false;
            }

            // the oblique angle is defined between -85 and 85 degrees
            if (newObliqueAngle > 180)
            {
                newObliqueAngle = 180 - newObliqueAngle;
            }

            if (newObliqueAngle < -85)
            {
                newObliqueAngle = -85;
            }
            else if (newObliqueAngle > 85)
            {
                newObliqueAngle = 85;
            }

            // the height must be greater than zero, the cosine is always positive between -85 and 85
            double newHeight = newVvector.Modulus() * Math.Cos(newObliqueAngle * MathHelper.DegToRad);
            newHeight = MathHelper.IsZero(newHeight) ? MathHelper.Epsilon : newHeight;

            // the width factor is defined between 0.01 and 100
            double newWidthFactor = newUvector.Modulus() / newHeight;
            if (newWidthFactor < 0.01)
            {
                newWidthFactor = 0.01;
            }
            else if (newWidthFactor > 100)
            {
                newWidthFactor = 100;
            }

            Position = newPosition;
            Normal = newNormal;
            Rotation = newRotation;
            Size = newHeight;
            WidthFactor = mirrorShape ? -newWidthFactor : newWidthFactor;
            ObliqueAngle = mirrorShape ? -newObliqueAngle : newObliqueAngle;
        }

        /// <summary>
        /// Creates a new Shape that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Shape that is a copy of this instance.</returns>
        public override object Clone()
        {
            Shape entity = new Shape(name, (ShapeStyle)style.Clone())
            {
                //EntityObject properties
                Layer = (Layer)Layer.Clone(),
                Linetype = (Linetype)Linetype.Clone(),
                Color = (AciColor)Color.Clone(),
                Lineweight = Lineweight,
                Transparency = (Transparency)Transparency.Clone(),
                LinetypeScale = LinetypeScale,
                Normal = Normal,
                IsVisible = IsVisible,
                //Shape properties
                Position = position,
                Size = size,
                Rotation = rotation,
                ObliqueAngle = obliqueAngle,
                Thickness = thickness
            };

            foreach (XData data in XData.Values)
            {
                entity.XData.Add((XData)data.Clone());
            }

            return entity;
        }

        #endregion
    }
}
