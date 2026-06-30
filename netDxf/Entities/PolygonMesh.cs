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
using System.Linq;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a mesh grid <see cref="EntityObject">entity</see>.
    /// </summary>
    public class PolygonMesh :
        EntityObject
    {
        #region private fields

        private static short defaultSurfU = 6;
        private static short defaultSurfV = 6;

        private readonly Vector3[] vertexes;
        private readonly short u;
        private readonly short v;
        private short densityU;
        private short densityV;
        private PolylineTypeFlags flags;
        private PolylineSmoothType smoothType;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <c>PolygonMesh</c> class.
        /// </summary>
        /// <param name="u">Number of vertexes along the U direction (local X axis).</param>
        /// <param name="v">Number of vertexes along the V direction (local Y axis).</param>
        /// <param name="vertexes">Array of UxV vertexes that represents the mesh grid.</param>
        public PolygonMesh(short u, short v, IEnumerable<Vector3> vertexes)
            : base(EntityType.PolygonMesh, DxfObjectCode.Polyline)
        {
            if (u < 2 || u > 256)
            {
                throw new ArgumentOutOfRangeException(nameof(this.u), this.u, "The number of vertexes along the U direction must be between 2 and 256.");
            }
            this.u = u;

            if (v < 2 || v > 256)
            {
                throw new ArgumentOutOfRangeException(nameof(this.v), this.v, "The number of vertexes along the V direction must be between 2 and 256.");
            }
            this.v = v;

            if (vertexes == null)
            {
                throw new ArgumentNullException(nameof(vertexes));
            }

            this.vertexes = vertexes.ToArray();

            if (this.vertexes.Length != u * v)
            {
                throw new ArgumentException("The number of vertexes must be equal to UxV.", nameof(vertexes));
            }

            densityU = 0;
            densityV = 0;
            smoothType = PolylineSmoothType.NoSmooth;
            flags = PolylineTypeFlags.PolygonMesh;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the mesh vertexes.
        /// </summary>
        public Vector3[] Vertexes
        {
            get { return vertexes; }
        }

        /// <summary>
        /// Set a PolygonMesh vertex by its indexes.
        /// </summary>
        /// <param name="i0">Index of the vertex in the U direction.</param>
        /// <param name="i1">Index of the vertex in the V direction.</param>
        /// <param name="vertex">A Vector3.</param>
        public void SetVertex(int i0, int i1, Vector3 vertex)
        {
            if (0 <= i0 && i0 < u && 0 <= i1 && i1 < v)
            {
                vertexes[i0 + u * i1] = vertex;
            }
        }

        /// <summary>
        /// Gets a PolygonMesh vertex by its indexes.
        /// </summary>
        /// <param name="i0">Index of the vertex in the U direction.</param>
        /// <param name="i1">Index of the vertex in the V direction.</param>
        public Vector3 GetVertex(int i0, int i1)
        {
            if (0 <= i0 && i0 < u && 0 <= i1 && i1 < v)
            {
                return vertexes[i0 + u * i1];
            }

            return vertexes[0];
        }
        
        /// <summary>
        /// Gets the number of vertexes along the U direction (local X axis).
        /// </summary>
        public short U
        {
            get { return u; }
        }

        /// <summary>
        /// Gets the number of vertexes along the V direction (local Y axis).
        /// </summary>
        public short V
        {
            get { return v; }
        }

        /// <summary>
        /// Smooth surface U density.
        /// </summary>
        /// <remarks>Valid values range from 3 to 201.</remarks>
        public short DensityU
        {
            get { return densityU; }
            set
            {
                if (value < 3 || value > 201)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The density value must be between 3 and 201.");
                }
                densityU = value;
            }
        }

        /// <summary>
        /// Smooth surface V density
        /// </summary>
        /// <remarks>Valid values range from 3 to 201.</remarks>
        public short DensityV
        {
            get { return densityV; }
            set
            {
                if (value < 3 || value > 201)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The density value must be between 3 and 201.");
                }
                densityV = value;
            }
        }

        /// <summary>
        /// Gets or sets if the polygon mesh is closed along the U direction (local X axis).
        /// </summary>
        public bool IsClosedInU
        {
            get { return flags.HasFlag(PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM); }
            set
            {
                if (value)
                {
                    flags |= PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM;
                }
                else
                {
                    flags &= ~PolylineTypeFlags.ClosedPolylineOrClosedPolygonMeshInM;
                }
            }
        }

        /// <summary>
        /// Gets or sets if the polygon mesh is closed along the V direction (local Y axis).
        /// </summary>
        public bool IsClosedInV
        {
            get { return flags.HasFlag(PolylineTypeFlags.ClosedPolygonMeshInN); }
            set
            {
                if (value)
                {
                    flags |= PolylineTypeFlags.ClosedPolygonMeshInN;
                }
                else
                {
                    flags &= ~PolylineTypeFlags.ClosedPolygonMeshInN;
                }
            }
        }

        /// <summary>
        /// Gets or sets the polyline smooth type.
        /// </summary>
        /// <remarks>
        /// The additional polygon meshes vertexes corresponding to the SplineFit will be created when writing the DXF file.
        /// </remarks>
        public PolylineSmoothType SmoothType
        {
            get { return smoothType; }
            set
            {
                if (value == PolylineSmoothType.NoSmooth)
                {
                    flags &= ~PolylineTypeFlags.SplineFit;
                }
                else
                {
                    flags |= PolylineTypeFlags.SplineFit;
                }
                smoothType = value;
            }
        }

        /// <summary>
        /// Gets or sets if the default SurfU value.
        /// </summary>
        /// <remarks>
        /// This value is used by smoothed polygon meshes when they not belong to a DXF document and the density values are left at the default 0.<br/>
        /// The minimum vertexes generated for smoothed polygon meshes is 3.
        /// </remarks>
        public static short DefaultSurfU
        {
            get { return defaultSurfU; }
            set
            {
                if (value < 0 || value > 200)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Values must be between 0 and 200.");
                }
                defaultSurfU = value;
            }
        }

        /// <summary>
        /// Gets or sets if the default SurfV value.
        /// </summary>
        /// <remarks>
        /// This value is used by smoothed polygon meshes when they not belong to a DXF document and the density values are left at the default 0.<br/>
        /// The minimum vertexes generated for smoothed polygon meshes is 3.
        /// </remarks>
        public static short DefaultSurfV
        {
            get { return defaultSurfV; }
            set
            {
                if (value < 0 || value > 200)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Values must be between 0 and 200.");
                }
                defaultSurfV = value;
            }
        }

        #endregion

        #region internal properties

        /// <summary>
        /// Gets the polygon mesh flags.
        /// </summary>
        internal PolylineTypeFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Obtains a list of vertexes that represent the polygon mesh approximating the surface faces as necessary.
        /// </summary>
        /// <returns>A list of vertexes that represent the mesh.</returns>
        /// <remarks>
        /// The minimum vertexes generated for smoothed polygon meshes is 3 in each direction.
        /// </remarks>
        public List<Vector3> MeshVertexes()
        {
            int precisionU = densityU == 0 ? Owner == null ? DefaultSurfU + 1 : Owner.Record.Owner.Owner.DrawingVariables.SurfU + 1 : densityU;
            int precisionV = densityV == 0 ? Owner == null ? DefaultSurfV + 1 : Owner.Record.Owner.Owner.DrawingVariables.SurfV + 1 : densityV;

            // the minimum vertexes generated is 3.
            if (precisionU < 3)
            {
                precisionU = 3;
            }
            if (precisionV < 3)
            {
                precisionV = 3;
            }
            return MeshVertexes(precisionU, precisionV);
        }

        /// <summary>
        /// Obtains a list of vertexes that represent the polygon mesh approximating the surface faces as necessary.
        /// </summary>
        /// <param name="precisionU">Number of vertexes created along the U direction.</param>
        /// <param name="precisionV">Number of vertexes created along the V direction.</param>
        /// <returns>A list of vertexes that represent the mesh.</returns>
        public List<Vector3> MeshVertexes(int precisionU, int precisionV)
        {
            if (precisionU < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(precisionU), precisionU, "The precisionU must be equal or greater than three.");
            }
            if (precisionV < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(precisionV), precisionV, "The precisionV must be equal or greater than three.");
            }

            int degree;
            if (smoothType == PolylineSmoothType.Quadratic)
            {
                degree = 2;
            }
            else if (smoothType == PolylineSmoothType.Cubic)
            {
                degree = 3;
            }
            else
            {
                return new List<Vector3>(vertexes);
            }

            List<Vector3> controlsUV = new List<Vector3>();
            int numU = u;
            int numV = v;

            // duplicate vertexes to handle periodic BSpline surfaces
            if (IsClosedInU)
            {
                numU += degree;
                for (int i = 0; i < v; i++)
                {
                    for (int j = 0; j < u + degree; j++)
                    {
                        if (j < u)
                        {
                            controlsUV.Add(vertexes[i * u + j]);
                        }
                        else
                        {
                            for (int k = 0; k < degree; k++, j++)
                            {
                                controlsUV.Add(vertexes[i * u + k]);
                            }
                        }
                    }
                }
                if (IsClosedInV)
                {
                    numV += degree;
                    for (int i = 0; i < degree; i++)
                    {
                        for (int j = 0; j < numU; j++)
                        {
                            controlsUV.Add(controlsUV[i * numU + j]);
                        }
                    }
                }
            }
            else if (IsClosedInV)
            {
                controlsUV.AddRange(vertexes);
                numV += degree;
                for (int i = 0; i < degree; i++)
                {
                    for (int j = 0; j < u; j++)
                    {
                        controlsUV.Add(vertexes[i * u + j]);
                    }
                }
            }
            else
            {
                controlsUV.AddRange(vertexes);
            }

            GTE.BasisFunctionInput bfU = new GTE.BasisFunctionInput(numU, degree);
            GTE.BasisFunctionInput bfV = new GTE.BasisFunctionInput(numV, degree);

            GTE.BSplineSurface surface = new GTE.BSplineSurface(bfU, bfV, controlsUV.ToArray());

            //change the knot vector to handle periodic BSplines
            if (IsClosedInU)
            {
                double factor = 1.0 / u;
                for (int i = 0; i < surface.BasisFunction(0).NumKnots; i++)
                {
                    surface.BasisFunction(0).Knots[i] = (i - surface.BasisFunction(0).Degree) * factor;
                }
            }

            if (IsClosedInV)
            {
                double factor = 1.0 / v;
                for (int i = 0; i < surface.BasisFunction(1).NumKnots; i++)
                {
                    surface.BasisFunction(1).Knots[i] = (i - surface.BasisFunction(1).Degree) * factor;
                }
            }

            double stepU = IsClosedInU ? 1.0 / precisionU : 1.0 / (precisionU - 1);
            double stepV = IsClosedInV ? 1.0 / precisionV : 1.0 / (precisionV - 1);
            double tU = 0.0;
            double tV = 0.0;
            List<Vector3> ocsVertexes = new List<Vector3>(precisionU * precisionV);
            for (int i = 0; i < precisionV; i++)
            {
                for (int j = 0; j < precisionU; j++)
                {
                    ocsVertexes.Add(surface.GetPosition(tU, tV));
                    tU += stepU;
                }

                tV += stepV;
                tU = 0;
            }

            return ocsVertexes;
        }

        /// <summary>
        /// Converts the actual polygon mesh into a mesh entity approximating the surface faces as necessary.
        /// </summary>
        /// <returns>A <see cref="Mesh">Mesh entity</see>.</returns>
        public Mesh ToMesh()
        {
            int precisionU = densityU == 0 ? Owner == null ? DefaultSurfU + 1 : Owner.Record.Owner.Owner.DrawingVariables.SurfU + 1 : densityU;
            int precisionV = densityV == 0 ? Owner == null ? DefaultSurfV + 1 : Owner.Record.Owner.Owner.DrawingVariables.SurfV + 1 : densityV;

            return ToMesh(precisionU, precisionV);
        }

        /// <summary>
        /// Converts the actual polygon mesh into a mesh entity approximating the surface faces as necessary.
        /// </summary>
        /// <param name="precisionU">Number of vertexes created along the U direction.</param>
        /// <param name="precisionV">Number of vertexes created along the V direction.</param>
        /// <returns>A <see cref="Mesh">Mesh entity</see>.</returns>
        /// <remarks>
        /// The minimum vertexes generated for smoothed polygon meshes is 3 in each direction.
        /// </remarks>
        public Mesh ToMesh(int precisionU, int precisionV)
        {
            List<Vector3> meshVertexes = MeshVertexes(precisionU, precisionV);

            int precU;
            int precV;
            if (smoothType == PolylineSmoothType.NoSmooth)
            {
                precU = u;
                precV = v;
            }
            else
            {
                precU = precisionU;
                precV = precisionV;
            }

            List<int[]> faces = new List<int[]>();

            for (int i = 0; i < precV; i++)
            {
                for (int j = 0; j < precU; j++)
                {
                    int v1, v2, v3, v4;
                    
                    if (j == precU - 1)
                    {
                        if (IsClosedInU && i < precV - 1)
                        {
                            v1 = i * precU + j;
                            v2 = i * precU;
                            v3 = (i + 1) * precU;
                            v4 = (i + 1) * precU + j;
                            faces.Add(new []{v1, v2, v3, v4});
                        }
                        continue;
                    }

                    if (i == precV - 1)
                    {
                        if (IsClosedInV && j < precU - 1)
                        {
                            v1 = i * precU + j;
                            v2 = v1 + 1;
                            v4 = j;
                            v3 = v4 + 1;
                            faces.Add(new []{v1, v2, v3, v4});
                        }
                        continue;
                    }

                    v1 = i * precU + j;
                    v2 = v1 + 1;
                    v4 = (i + 1) * precU + j;
                    v3 = v4 + 1;
                    faces.Add(new []{v1, v2, v3, v4});
                }
            }

            return new Mesh(meshVertexes, faces);
        }

        /// <summary>
        /// Decompose the actual polygon mesh into <see cref="Face3D">faces 3D</see>.
        /// </summary>
        /// <returns>A list of <see cref="Face3D">faces 3D</see> that made up the polygon mesh.</returns>
        public List<Face3D> Explode()
        {
            List<Vector3> meshVertexes = MeshVertexes();

            int precU;
            int precV;
            if (smoothType == PolylineSmoothType.NoSmooth)
            {
                precU = u;
                precV = v;
            }
            else
            {
                precU = densityU;
                precV = densityV;
            }

            List<Face3D> faces = new List<Face3D>();

            for (int i = 0; i < precV; i++)
            {
                for (int j = 0; j < precU; j++)
                {
                    int v1, v2, v3, v4;
                    if (j == precU - 1)
                    {
                        if (IsClosedInU && i < precV - 1)
                        {
                            v1 = i * precU + j;
                            v2 = i * precU;
                            v3 = (i + 1) * precU;
                            v4 = (i + 1) * precU + j;
                            faces.Add(new Face3D(meshVertexes[v1], meshVertexes[v2], meshVertexes[v3], meshVertexes[v4]));
                        }
                        continue;
                    }

                    if (i == precV - 1)
                    {
                        if (IsClosedInV && j < precU - 1)
                        {
                            v1 = i * precU + j;
                            v2 = v1 + 1;
                            v4 = j;
                            v3 = v4 + 1;
                            faces.Add(new Face3D(meshVertexes[v1], meshVertexes[v2], meshVertexes[v3], meshVertexes[v4]));
                        }
                        continue;
                    }

                    v1 = i * precU + j;
                    v2 = v1 + 1;
                    v4 = (i + 1) * precU + j;
                    v3 = v4 + 1;
                    faces.Add(new Face3D(meshVertexes[v1], meshVertexes[v2], meshVertexes[v3], meshVertexes[v4]));
                }
            }

            return faces;
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
            for (int i = 0; i < vertexes.Length; i++)
            {
                vertexes[i] = transformation * vertexes[i] + translation;
            }

            Vector3 newNormal = transformation * Normal;
            if (Vector3.Equals(Vector3.Zero, newNormal))
            {
                newNormal = Normal;
            }
            Normal = newNormal;
        }

        /// <summary>
        /// Creates a new PolygonMesh that is a copy of the current instance.
        /// </summary>
        /// <returns>A new PolygonMesh that is a copy of this instance.</returns>
        public override object Clone()
        {
            PolygonMesh entity = new PolygonMesh(u, v, vertexes)
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
                //PolygonMesh properties
                DensityU = densityU,
                DensityV = densityV,
                Flags = flags
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
