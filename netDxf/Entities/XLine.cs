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
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents an extension line <see cref="EntityObject">entity</see> (aka construction line).
    /// </summary>
    /// <remarks>An extension line is a line in three-dimensional space that starts in the specified origin and extends to infinity in both directions.</remarks>
    public class XLine :
        EntityObject
    {
        #region private fields

        private Vector3 origin;
        private Vector3 direction;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>XLine</c> class.
        /// </summary>
        public XLine()
            : this(Vector3.Zero, Vector3.UnitX)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>XLine</c> class.
        /// </summary>
        /// <param name="origin">XLine <see cref="Vector2">origin.</see></param>
        /// <param name="direction">XLine <see cref="Vector2">direction.</see></param>
        public XLine(Vector2 origin, Vector2 direction)
            : this(new Vector3(origin.X, origin.Y, 0.0), new Vector3(direction.X, direction.Y, 0.0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>XLine</c> class.
        /// </summary>
        /// <param name="origin">XLine <see cref="Vector3">origin.</see></param>
        /// <param name="direction">XLine <see cref="Vector3">direction.</see></param>
        public XLine(Vector3 origin, Vector3 direction)
            : base(EntityType.XLine, DxfObjectCode.XLine)
        {
            this.origin = origin;
            this.direction = Vector3.Normalize(direction);
            if (Vector3.IsZero(this.direction))
            {
                throw new ArgumentException("The direction can not be the zero vector.", nameof(direction));
            }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the extension line <see cref="netDxf.Vector3">origin</see>.
        /// </summary>
        public Vector3 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        /// <summary>
        /// Gets or sets the extension line <see cref="netDxf.Vector3">direction</see>.
        /// </summary>
        public Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = Vector3.Normalize(value);
                if (Vector3.IsZero(direction))
                {
                    throw new ArgumentException("The direction can not be the zero vector.", nameof(value));
                }
            }
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
            Origin = transformation * Origin + translation;

            Vector3 newDirection = transformation * Direction;
            if (Vector3.Equals(Vector3.Zero, newDirection))
            {
                newDirection = Direction;
            }
            Direction = newDirection;

            Vector3 newNormal = transformation * Normal;
            if (Vector3.Equals(Vector3.Zero, newNormal))
            {
                newNormal = Normal;
            }
            Normal = newNormal;
        }

        /// <summary>
        /// Creates a new XLine that is a copy of the current instance.
        /// </summary>
        /// <returns>A new XLine that is a copy of this instance.</returns>
        public override object Clone()
        {
            XLine entity = new XLine
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
                //XLine properties
                Origin = origin,
                Direction = direction,
            };

            foreach (XData data in XData.Values)
            {
                entity.XData.Add((XData) data.Clone());
            }

            return entity;
        }

        #endregion
    }
}