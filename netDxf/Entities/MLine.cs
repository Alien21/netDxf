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
using netDxf.Objects;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a multiline <see cref="EntityObject">entity</see>.
    /// </summary>
    public class MLine :
        EntityObject
    {
        #region delegates and events

        public delegate void MLineStyleChangedEventHandler(MLine sender, TableObjectChangedEventArgs<MLineStyle> e);
        public event MLineStyleChangedEventHandler MLineStyleChanged;
        protected virtual MLineStyle OnMLineStyleChangedEvent(MLineStyle oldMLineStyle, MLineStyle newMLineStyle)
        {
            MLineStyleChangedEventHandler ae = MLineStyleChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<MLineStyle> eventArgs = new TableObjectChangedEventArgs<MLineStyle>(oldMLineStyle, newMLineStyle);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newMLineStyle;
        }

        #endregion

        #region private fields

        private double scale;
        private MLineStyle style;
        private MLineJustification justification;
        private double elevation;
        private MLineFlags flags;
        private readonly List<MLineVertex> vertexes;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>MLine</c> class.
        /// </summary>
        public MLine()
            : this(new List<Vector2>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MLine</c> class.
        /// </summary>
        /// <param name="vertexes">Multiline <see cref="Vector2">vertex</see> location list in object coordinates.</param>
        public MLine(IEnumerable<Vector2> vertexes)
            : this(vertexes, MLineStyle.Default, 1.0, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MLine</c> class.
        /// </summary>
        /// <param name="vertexes">Multiline <see cref="Vector2">vertex</see> location list in object coordinates.</param>
        /// <param name="isClosed">Sets if the multiline is closed  (default: false).</param>
        public MLine(IEnumerable<Vector2> vertexes, bool isClosed)
            : this(vertexes, MLineStyle.Default, 1.0, isClosed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MLine</c> class.
        /// </summary>
        /// <param name="vertexes">Multiline <see cref="Vector2">vertex</see> location list in object coordinates.</param>
        /// <param name="scale">Multiline scale.</param>
        public MLine(IEnumerable<Vector2> vertexes, double scale)
            : this(vertexes, MLineStyle.Default, scale, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MLine</c> class.
        /// </summary>
        /// <param name="vertexes">Multiline <see cref="Vector2">vertex</see> location list in object coordinates.</param>
        /// <param name="scale">Multiline scale.</param>
        /// <param name="isClosed">Sets if the multiline is closed (default: false).</param>
        public MLine(IEnumerable<Vector2> vertexes, double scale, bool isClosed)
            : this(vertexes, MLineStyle.Default, scale, isClosed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MLine</c> class.
        /// </summary>
        /// <param name="vertexes">MLine <see cref="Vector2">vertex</see> location list in object coordinates.</param>
        /// <param name="style">MLine <see cref="MLineStyle">style.</see></param>
        /// <param name="scale">MLine scale.</param>
        public MLine(IEnumerable<Vector2> vertexes, MLineStyle style, double scale)
            : this(vertexes, style, scale, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MLine</c> class.
        /// </summary>
        /// <param name="vertexes">MLine <see cref="Vector2">vertex</see> location list in object coordinates.</param>
        /// <param name="style">MLine <see cref="MLineStyle">style.</see></param>
        /// <param name="scale">MLine scale.</param>
        /// <param name="isClosed">Sets if the multiline is closed (default: false).</param>
        public MLine(IEnumerable<Vector2> vertexes, MLineStyle style, double scale, bool isClosed)
            : base(EntityType.MLine, DxfObjectCode.MLine)
        {
            this.scale = scale;
            if (style == null)
            {
                throw new ArgumentNullException(nameof(style));
            }

            if (isClosed)
            {
                flags = MLineFlags.Has | MLineFlags.Closed;
            }
            else
            {
                flags = MLineFlags.Has;
            }

            this.style = style;
            justification = MLineJustification.Zero;
            elevation = 0.0;
            if (vertexes == null)
            {
                throw new ArgumentNullException(nameof(vertexes));
            }
            this.vertexes = new List<MLineVertex>();
            foreach (Vector2 point in vertexes)
            {
                this.vertexes.Add(new MLineVertex(point, Vector2.Zero, Vector2.Zero, null));
            }
            Update();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the multiline <see cref="MLineVertex">vertexes</see> list.
        /// </summary>
        public List<MLineVertex> Vertexes
        {
            get { return vertexes; }
        }

        /// <summary>
        /// Gets or sets the multiline elevation.
        /// </summary>
        public double Elevation
        {
            get { return elevation; }
            set { elevation = value; }
        }

        /// <summary>
        /// Gets or sets the multiline scale.
        /// </summary>
        public double Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Gets or sets if the multiline is closed.
        /// </summary>
        public bool IsClosed
        {
            get { return flags.HasFlag(MLineFlags.Closed); }
            set
            {
                if (value)
                {
                    flags |= MLineFlags.Closed;
                }
                else
                {
                    flags &= ~MLineFlags.Closed;
                }
            }
        }

        /// <summary>
        /// Gets or sets the suppression of start caps.
        /// </summary>
        public bool NoStartCaps
        {
            get { return flags.HasFlag(MLineFlags.NoStartCaps); }
            set
            {
                if (value)
                {
                    flags |= MLineFlags.NoStartCaps;
                }
                else
                {
                    flags &= ~MLineFlags.NoStartCaps;
                }
            }
        }

        /// <summary>
        /// Gets or sets the suppression of end caps.
        /// </summary>
        public bool NoEndCaps
        {
            get { return flags.HasFlag(MLineFlags.NoEndCaps); }
            set
            {
                if (value)
                {
                    flags |= MLineFlags.NoEndCaps;
                }
                else
                {
                    flags &= ~MLineFlags.NoEndCaps;
                }
            }
        }

        /// <summary>
        /// Gets or sets the multiline justification.
        /// </summary>
        public MLineJustification Justification
        {
            get { return justification; }
            set { justification = value; }
        }

        /// <summary>
        /// Gets or set the multiline style.
        /// </summary>
        public MLineStyle Style
        {
            get { return style; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                style = OnMLineStyleChangedEvent(style, value);
            }
        }

        #endregion

        #region internal properties

        /// <summary>
        /// MLine flags.
        /// </summary>
        internal MLineFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        #endregion

        #region private methods

        private List<EntityObject> CreateRoundCap(Vector2 start, Vector2 end, Matrix3 transformation, AciColor color1, Linetype linetype1, AciColor color2, Linetype linetype2)
        {
            List<EntityObject> entities = new List<EntityObject>();

            Vector2 center = Vector2.MidPoint(start, end);
            double startAngle = Vector2.Angle(start - center) * MathHelper.RadToDeg;
            double endAngle = startAngle + 180.0;
            double radius = (start - center).Modulus();

            if (!MathHelper.IsZero(radius))
            {
                if (!color1.Equals(color2) || !linetype1.Equals(linetype2))
                {
                    double midAngle = startAngle + 90.0;
                    Arc arc1 = CreateArc(center, radius, startAngle, midAngle, color1, linetype1);
                    arc1.TransformBy(transformation, Vector3.Zero);
                    entities.Add(arc1);
                    Arc arc2 = CreateArc(center, radius, midAngle, endAngle, color2, linetype2);
                    arc2.TransformBy(transformation, Vector3.Zero);
                    entities.Add(arc2);
                }
                else
                {
                    Arc arc = CreateArc(center, radius, startAngle, endAngle, color1, linetype1);
                    arc.TransformBy(transformation, Vector3.Zero);
                    entities.Add(arc);
                }
            }

            return entities;
        }

        private List<EntityObject> CreateSquareCap(Vector2 start, Vector2 end, Matrix3 transformation, AciColor color1, Linetype linetype1, AciColor color2, Linetype linetype2)
        {
            List<EntityObject> entities = new List<EntityObject>();

            Vector2 midPoint = Vector2.MidPoint(start, end);

            if (!color1.Equals(color2) || !linetype1.Equals(linetype2))
            {
                Line line1 = CreateLine(start, midPoint, color1, linetype1);
                line1.TransformBy(transformation, Vector3.Zero);
                entities.Add(line1);
                Line line2 = CreateLine(midPoint, end, color2, linetype2);
                line2.TransformBy(transformation, Vector3.Zero);
                entities.Add(line2);
            }
            else
            {
                Line line = CreateLine(start, end, color1, linetype1);
                line.TransformBy(transformation, Vector3.Zero);
                entities.Add(line);
            }

            return entities;
        }

        private Line CreateLine(Vector2 start, Vector2 end, AciColor color, Linetype linetype)
        {
            return new Line(start, end)
            {
                Layer = (Layer) Layer.Clone(),
                Linetype = (Linetype) linetype.Clone(),
                Color = (AciColor) color.Clone(),
                Lineweight = Lineweight,
                Transparency = (Transparency) Transparency.Clone(),
                LinetypeScale = LinetypeScale,
                Normal = Normal
            };
        }

        private Arc CreateArc(Vector2 center, double radius, double startAngle, double endAngle, AciColor color, Linetype linetype)
        {
            return new Arc(center, radius, startAngle, endAngle)
            {
                Layer = (Layer) Layer.Clone(),
                Linetype = (Linetype) linetype.Clone(),
                Color = (AciColor) color.Clone(),
                Lineweight = Lineweight,
                Transparency = (Transparency) Transparency.Clone(),
                LinetypeScale = LinetypeScale,
                Normal = Normal,
                IsVisible = IsVisible,
            };
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculates the internal information of the multiline vertexes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This function needs to be called manually when any modification is done that affects the final shape of the multiline.
        /// </para>
        /// <para>
        /// If the vertex distance list needs to be edited to represent trimmed multilines this function needs to be called prior to any modification.
        /// It will calculate the minimum information needed to build a correct multiline.
        /// </para>
        /// </remarks>
        public void Update()
        {
            if (vertexes.Count == 0)
            {
                return;
            }

            double reference = 0.0;
            switch (justification)
            {
                case MLineJustification.Top:
                    reference = -style.Elements[0].Offset;
                    break;
                case MLineJustification.Zero:
                    reference = 0.0;
                    break;
                case MLineJustification.Bottom:
                    reference = -style.Elements[style.Elements.Count - 1].Offset;
                    break;
            }

            Vector2 prevDir;
            if (vertexes[0].Position.Equals(vertexes[vertexes.Count - 1].Position))
            {
                prevDir = Vector2.UnitY;
            }
            else
            {
                prevDir = vertexes[0].Position - vertexes[vertexes.Count - 1].Position;
                prevDir.Normalize();
            }

            for (int i = 0; i < vertexes.Count; i++)
            {
                Vector2 position = vertexes[i].Position;
                Vector2 miter;
                Vector2 dir;
                if (i == 0)
                {
                    if (vertexes[i + 1].Position.Equals(position))
                    {
                        dir = Vector2.UnitY;
                    }
                    else
                    {
                        dir = vertexes[i + 1].Position - position;
                        dir.Normalize();
                    }
                    if (IsClosed)
                    {
                        miter = dir - prevDir;
                        miter.Normalize();
                    }
                    else
                    {
                        miter = -MathHelper.Transform(dir, style.StartAngle*MathHelper.DegToRad, CoordinateSystem.Object, CoordinateSystem.World);
                        miter.Normalize();
                    }
                }
                else if (i + 1 == vertexes.Count)
                {
                    if (IsClosed)
                    {
                        if (vertexes[0].Position.Equals(position))
                        {
                            dir = Vector2.UnitY;
                        }
                        else
                        {
                            dir = vertexes[0].Position - position;
                            dir.Normalize();
                        }
                        miter = dir - prevDir;
                        miter.Normalize();
                    }
                    else
                    {
                        dir = prevDir;
                        miter = -MathHelper.Transform(dir, style.EndAngle*MathHelper.DegToRad, CoordinateSystem.Object, CoordinateSystem.World);
                        miter.Normalize();
                    }
                }
                else
                {
                    if (vertexes[i + 1].Position.Equals(position))
                    {
                        dir = Vector2.UnitY;
                    }
                    else
                    {
                        dir = vertexes[i + 1].Position - position;
                        dir.Normalize();
                    }

                    miter = dir - prevDir;
                    miter.Normalize();
                }
                prevDir = dir;

                List<double>[] distances = new List<double>[style.Elements.Count];
                double angleMiter = Vector2.Angle(miter);
                double angleDir = Vector2.Angle(dir);
                double cos = Math.Cos(angleMiter - (MathHelper.HalfPI + angleDir));
                for (int j = 0; j < style.Elements.Count; j++)
                {
                    double distance = (style.Elements[j].Offset + reference) / cos;
                    distances[j] = new List<double>
                    {
                        distance * scale,
                        0.0
                    };
                }

                vertexes[i] = new MLineVertex(position, dir, miter, distances);
            }
        }

        /// <summary>
        /// Decompose the actual multiline in its internal entities, <see cref="Line">lines</see> and <see cref="Arc">arcs</see>.
        /// </summary>
        /// <returns>A list of <see cref="Line">lines</see> and <see cref="Arc">arcs</see> that made up the multiline.</returns>
        public List<EntityObject> Explode()
        {
            List<EntityObject> entities = new List<EntityObject>();

            Matrix3 transformation = MathHelper.ArbitraryAxis(Normal);

            // precomputed points at multiline vertexes for start and end caps calculations
            Vector2[][] cornerVertexes = new Vector2[vertexes.Count][];

            for (int i = 0; i < vertexes.Count; i++)
            {
                MLineVertex vertex = vertexes[i];
                MLineVertex nextVertex;

                if (IsClosed && i == vertexes.Count - 1)
                {
                    nextVertex = vertexes[0];
                }
                else if (!IsClosed && i == vertexes.Count - 1)
                {
                    continue;
                }
                else
                {
                    nextVertex = vertexes[i + 1];
                    cornerVertexes[i + 1] = new Vector2[style.Elements.Count];
                }

                cornerVertexes[i] = new Vector2[style.Elements.Count];

                for (int j = 0; j < style.Elements.Count; j++)
                {
                    if (vertex.Distances[j].Count == 0)
                    {
                        continue;
                    }

                    Vector2 refStart = vertex.Position + vertex.Miter * vertex.Distances[j][0];
                    cornerVertexes[i][j] = refStart;
                    for (int k = 1; k < vertex.Distances[j].Count; k++)
                    {
                        Vector2 start = refStart + vertex.Direction * vertex.Distances[j][k];
                        Vector2 end;
                        if (k >= vertex.Distances[j].Count - 1)
                        {
                            end = nextVertex.Position + nextVertex.Miter * nextVertex.Distances[j][0];                       
                            if(!IsClosed) cornerVertexes[i + 1][j] = end;
                        }
                        else
                        {
                            end = refStart + vertex.Direction * vertex.Distances[j][k + 1];
                            k++; // skip next segment it is a blank space
                        }

                        Line line = CreateLine(start, end, style.Elements[j].Color, style.Elements[j].Linetype);
                        line.TransformBy(transformation, Vector3.Zero);
                        entities.Add(line);
                    }
                }                
            }

            if (style.Flags.HasFlag(MLineStyleFlags.DisplayJoints))
            {
                AciColor color1 = style.Elements[0].Color;
                AciColor color2 = style.Elements[style.Elements.Count - 1].Color;
                Linetype linetype1 = style.Elements[0].Linetype;
                Linetype linetype2 = style.Elements[style.Elements.Count - 1].Linetype;

                for (int i = 0; i < cornerVertexes.Length; i++)
                {
                    if (!IsClosed && (i == 0 || i == cornerVertexes.Length - 1))
                    {
                        continue;
                    }

                    Vector2 start = cornerVertexes[i][0];
                    Vector2 end = cornerVertexes[i][cornerVertexes[0].Length - 1];

                    entities.AddRange(CreateSquareCap(start, end, transformation, color1, linetype1, color2, linetype2));
                }
            }

            // when the multiline is closed there are no caps
            if (IsClosed) return entities;

            if (!NoStartCaps)
            {
                if (style.Flags.HasFlag(MLineStyleFlags.StartRoundCap))
                {
                    AciColor color1 = style.Elements[0].Color;
                    AciColor color2 = style.Elements[style.Elements.Count - 1].Color;
                    Linetype linetype1 = style.Elements[0].Linetype;
                    Linetype linetype2 = style.Elements[style.Elements.Count - 1].Linetype;

                    Vector2 start = cornerVertexes[0][0];
                    Vector2 end = cornerVertexes[0][cornerVertexes[0].Length - 1];

                    entities.AddRange(scale >= 0 ?
                        CreateRoundCap(start, end, transformation, color1, linetype1, color2, linetype2) :
                        CreateRoundCap(end, start, transformation, color2, linetype2, color1, linetype1));
                }

                if (style.Flags.HasFlag(MLineStyleFlags.StartInnerArcsCap))
                {
                    int j = (int) (style.Elements.Count * 0.5); // Math.Floor

                    for (int i = 1; i < j; i++)
                    {
                        AciColor color1 = style.Elements[i].Color;
                        AciColor color2 = style.Elements[style.Elements.Count - 1 - i].Color;
                        Linetype linetype1 = style.Elements[i].Linetype;
                        Linetype linetype2 = style.Elements[style.Elements.Count - 1 - i].Linetype;

                        Vector2 start = cornerVertexes[0][i];
                        Vector2 end = cornerVertexes[0][cornerVertexes[0].Length - 1 - i];

                        entities.AddRange(scale >= 0 ?
                            CreateRoundCap(start, end, transformation, color1, linetype1, color2, linetype2) :
                            CreateRoundCap(end, start, transformation, color2, linetype2, color1, linetype1));
                    }
                }

                if (style.Flags.HasFlag(MLineStyleFlags.StartSquareCap))
                {
                    AciColor color1 = style.Elements[0].Color;
                    AciColor color2 = style.Elements[style.Elements.Count - 1].Color;
                    Linetype linetype1 = style.Elements[0].Linetype;
                    Linetype linetype2 = style.Elements[style.Elements.Count - 1].Linetype;

                    Vector2 start = cornerVertexes[0][0];
                    Vector2 end = cornerVertexes[0][cornerVertexes[0].Length - 1];

                    entities.AddRange(CreateSquareCap(start, end, transformation, color1, linetype1, color2, linetype2));
                }
            }

            if (!NoEndCaps)
            {
                if (style.Flags.HasFlag(MLineStyleFlags.EndRoundCap))
                {
                    AciColor color1 = style.Elements[style.Elements.Count - 1].Color;
                    AciColor color2 = style.Elements[0].Color;
                    Linetype linetype1 = style.Elements[style.Elements.Count - 1].Linetype;
                    Linetype linetype2 = style.Elements[0].Linetype;
                   
                    Vector2 start = cornerVertexes[vertexes.Count - 1][cornerVertexes[0].Length - 1];
                    Vector2 end = cornerVertexes[vertexes.Count - 1][0];

                    entities.AddRange(scale >= 0 ?
                        CreateRoundCap(start, end, transformation, color1, linetype1, color2, linetype2) :
                        CreateRoundCap(end, start, transformation, color2, linetype2, color1, linetype1));
                }

                if (style.Flags.HasFlag(MLineStyleFlags.EndInnerArcsCap))
                {
                    int j = (int) (style.Elements.Count * 0.5); // Math.Floor

                    for (int i = 1; i < j; i++)
                    {
                        AciColor color1 = style.Elements[style.Elements.Count - 1 - i].Color;
                        AciColor color2 = style.Elements[i].Color;
                        Linetype linetype1 = style.Elements[style.Elements.Count - 1 - i].Linetype;
                        Linetype linetype2 = style.Elements[i].Linetype;

                        Vector2 start = cornerVertexes[vertexes.Count - 1][cornerVertexes[0].Length - 1 - i];
                        Vector2 end = cornerVertexes[vertexes.Count - 1][i];

                        entities.AddRange(scale >= 0 ?
                            CreateRoundCap(start, end, transformation, color1, linetype1, color2, linetype2) :
                            CreateRoundCap(end, start, transformation, color2, linetype2, color1, linetype1));
                    }
                }

                if (style.Flags.HasFlag(MLineStyleFlags.EndSquareCap))
                {
                    AciColor color1 = style.Elements[style.Elements.Count - 1].Color;
                    AciColor color2 = style.Elements[0].Color;
                    Linetype linetype1 = style.Elements[style.Elements.Count - 1].Linetype;
                    Linetype linetype2 = style.Elements[0].Linetype;

                    Vector2 start = cornerVertexes[vertexes.Count - 1][cornerVertexes[0].Length - 1];
                    Vector2 end = cornerVertexes[vertexes.Count - 1][0];

                    entities.AddRange(CreateSquareCap(start, end, transformation, color1, linetype1, color2, linetype2));
                }
            }

            return entities;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Moves, scales, and/or rotates the current entity given a 3x3 transformation matrix and a translation vector.
        /// </summary>
        /// <param name="transformation">Transformation matrix.</param>
        /// <param name="translation">Translation vector.</param>
        /// <remarks>
        /// Non-uniform scaling is not supported for multilines.
        /// Explode the entity and, in case round end caps has been applied, convert the arcs into ellipse arcs and transform them instead.<br />
        /// Matrix3 adopts the convention of using column vectors to represent a transformation matrix.
        /// </remarks>
        public override void TransformBy(Matrix3 transformation, Vector3 translation)
        {
            Vector3 newNormal = transformation * Normal;
            if (Vector3.Equals(Vector3.Zero, newNormal))
            {
                newNormal = Normal;
            }

            double newElevation = Elevation;

            Matrix3 transOW = MathHelper.ArbitraryAxis(Normal);
            Matrix3 transWO = MathHelper.ArbitraryAxis(newNormal).Transpose();

            Vector3 axis = transOW * Vector3.UnitX;
            axis = transformation * axis;
            axis = transWO * axis;
            Vector2 axisPoint = new Vector2(axis.X, axis.Y);
            double newScale = axisPoint.Modulus();

            for (int i = 0; i < Vertexes.Count; i++)
            {                
                Vector2 p = Vertexes[i].Position;
                Vector3 v = transOW * new Vector3(p.X, p.Y, Elevation);
                v = transformation * v + translation;
                v = transWO * v;
                Vector2 position = new Vector2(v.X, v.Y);
                newElevation = v.Z;

                Vector2 d = Vertexes[i].Direction;
                v = transOW * new Vector3(d.X, d.Y, 0.0);
                v = transformation * v;
                v = transWO * v;
                Vector2 direction = new Vector2(v.X, v.Y);

                Vector2 m = Vertexes[i].Miter;
                v = transOW * new Vector3(m.X, m.Y, 0.0);
                v = transformation * v;
                v = transWO * v;
                Vector2 miter = new Vector2(v.X, v.Y);

                List<double>[] newDistances = new List<double>[style.Elements.Count];
                for (int j = 0; j < style.Elements.Count; j++)
                {
                    newDistances[j] = new List<double>(); 
                    for (int k = 0; k < Vertexes[i].Distances[j].Count; k++)
                    {
                        newDistances[j].Add(Vertexes[i].Distances[j][k]*newScale);
                    }
                }
                vertexes[i] = new MLineVertex(position, direction, miter, newDistances);
            }

            if (Vector2.CrossProduct(Vertexes[0].Miter, Vertexes[0].Direction) < 0) newScale = -newScale;

            Elevation = newElevation;
            Normal = newNormal;
            Scale *= newScale;
        }

        /// <summary>
        /// Creates a new MLine that is a copy of the current instance.
        /// </summary>
        /// <returns>A new MLine that is a copy of this instance.</returns>
        public override object Clone()
        {
            MLine entity = new MLine
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
                //MLine properties
                Elevation = elevation,
                Scale = scale,
                Justification = justification,
                Style = (MLineStyle) style.Clone(),
                Flags = flags
            };

            foreach (MLineVertex vertex in vertexes)
            {
                entity.vertexes.Add((MLineVertex) vertex.Clone());
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