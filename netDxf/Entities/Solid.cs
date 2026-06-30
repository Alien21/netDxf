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

using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a solid <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Solid :
        EntityObject
    {
        #region private fields

        private Vector2 firstVertex;
        private Vector2 secondVertex;
        private Vector2 thirdVertex;
        private Vector2 fourthVertex;
        private double elevation;
        private double thickness;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Solid</c> class.
        /// </summary>
        public Solid()
            : this(Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Solid</c> class.
        /// </summary>
        /// <param name="firstVertex">Solid <see cref="Vector2">first vertex</see> in OCS (object coordinate system).</param>
        /// <param name="secondVertex">Solid <see cref="Vector2">second vertex</see> in OCS (object coordinate system).</param>
        /// <param name="thirdVertex">Solid <see cref="Vector2">third vertex</see> in OCS (object coordinate system).</param>
        public Solid(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex)
            : this(new Vector2(firstVertex.X, firstVertex.Y),
                new Vector2(secondVertex.X, secondVertex.Y),
                new Vector2(thirdVertex.X, thirdVertex.Y),
                new Vector2(thirdVertex.X, thirdVertex.Y))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Solid</c> class.
        /// </summary>
        /// <param name="firstVertex">Solid <see cref="Vector2">first vertex</see> in OCS (object coordinate system).</param>
        /// <param name="secondVertex">Solid <see cref="Vector2">second vertex</see> in OCS (object coordinate system).</param>
        /// <param name="thirdVertex">Solid <see cref="Vector2">third vertex</see> in OCS (object coordinate system).</param>
        /// <param name="fourthVertex">Solid <see cref="Vector2">fourth vertex</see> in OCS (object coordinate system).</param>
        public Solid(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex, Vector2 fourthVertex)
            : base(EntityType.Solid, DxfObjectCode.Solid)
        {
            this.firstVertex = firstVertex;
            this.secondVertex = secondVertex;
            this.thirdVertex = thirdVertex;
            this.fourthVertex = fourthVertex;
            elevation = 0.0;
            thickness = 0.0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the first solid <see cref="Vector2">vertex in OCS (object coordinate system).</see>.
        /// </summary>
        public Vector2 FirstVertex
        {
            get { return firstVertex; }
            set { firstVertex = value; }
        }

        /// <summary>
        /// Gets or sets the second solid <see cref="Vector2">vertex in OCS (object coordinate system).</see>.
        /// </summary>
        public Vector2 SecondVertex
        {
            get { return secondVertex; }
            set { secondVertex = value; }
        }

        /// <summary>
        /// Gets or sets the third solid <see cref="Vector2">vertex in OCS (object coordinate system).</see>.
        /// </summary>
        public Vector2 ThirdVertex
        {
            get { return thirdVertex; }
            set { thirdVertex = value; }
        }

        /// <summary>
        /// Gets or sets the fourth solid <see cref="Vector2">vertex in OCS (object coordinate system).</see>.
        /// </summary>
        public Vector2 FourthVertex
        {
            get { return fourthVertex; }
            set { fourthVertex = value; }
        }

        /// <summary>
        /// Gets or sets the solid elevation.
        /// </summary>
        /// <remarks>This is the distance from the origin to the plane of the solid.</remarks>
        public double Elevation
        {
            get { return elevation; }
            set { elevation = value; }
        }

        /// <summary>
        /// Gets or sets the thickness of the solid.
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
            Vector3 newNormal = transformation * Normal;
            if (Vector3.Equals(Vector3.Zero, newNormal))
            {
                newNormal = Normal;
            }

            Matrix3 transOW = MathHelper.ArbitraryAxis(Normal);
            Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal).Transpose();

            Vector3 v;

            v = transOW * new Vector3(FirstVertex.X, FirstVertex.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            FirstVertex = new Vector2(v.X, v.Y);

            v = transOW * new Vector3(SecondVertex.X, SecondVertex.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            SecondVertex = new Vector2(v.X, v.Y);

            v = transOW * new Vector3(ThirdVertex.X, ThirdVertex.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            ThirdVertex = new Vector2(v.X, v.Y);

            v = transOW * new Vector3(FourthVertex.X, FourthVertex.Y, Elevation);
            v = transformation * v + translation;
            v = transWO * v;
            FourthVertex = new Vector2(v.X, v.Y);

            Normal = newNormal;
            Elevation = v.Z;
        }

        /// <summary>
        /// Creates a new Solid that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Solid that is a copy of this instance.</returns>
        public override object Clone()
        {
            Solid entity = new Solid
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
                //Solid properties
                FirstVertex = firstVertex,
                SecondVertex = secondVertex,
                ThirdVertex = thirdVertex,
                FourthVertex = fourthVertex,
                Thickness = thickness
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