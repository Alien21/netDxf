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

namespace netDxf.Entities
{
    /// <summary>
    /// Represent a loop of a <see cref="Hatch">hatch</see> boundaries.
    /// </summary>
    /// <remarks>
    /// The entities that make a loop can be any combination of lines, light weight polylines, polylines, circles, arcs, ellipses, and splines.<br />
    /// The entities that define a loop should define a closed path, they have to be on the same plane as the hatch, and with the same normal;
    /// if these conditions are not met the result might be unpredictable. <br />
    /// Entities expressed in 3d coordinates like lines, polylines, and splines will be projected into its local plane.
    /// All edges that make a HatchBoundaryPath are 2d objects, only have X and Y coordinates.
    /// This is why to avoid unexpected results, it is important that all entities that define the paths have the same normal, same reference plane.
    /// </remarks>
    public class HatchBoundaryPath :
        ICloneable
    {
        #region Hatch boundary path edge classes

        /// <summary>
        /// Specifies the type of HatchBoundaryPath.Edge.
        /// </summary>
        public enum EdgeType
        {
            Polyline = 0,
            Line = 1,
            Arc = 2,
            Ellipse = 3,
            Spline = 4
        }

        /// <summary>
        /// Base class for all types of HatchBoundaryPath edges.
        /// </summary>
        public abstract class Edge :
            ICloneable
        {
            /// <summary>
            /// Gets the HatchBoundaryPath edge type
            /// </summary>
            public readonly EdgeType Type;

            protected Edge(EdgeType type)
            {
                Type = type;
            }

            /// <summary>
            /// Converts the actual edge to its entity equivalent.
            /// </summary>
            /// <returns>An EntityObject equivalent to the actual edge.</returns>
            public abstract EntityObject ConvertTo();

            /// <summary>
            /// Clones the actual edge.
            /// </summary>
            /// <returns>A copy of the actual edge.</returns>
            public abstract object Clone();
        }

        /// <summary>
        /// Represents a polyline edge of a HatchBoundaryPath.
        /// </summary>
        public class Polyline :
            Edge
        {
            /// <summary>
            /// Gets or sets the list of polyline vertexes.
            /// </summary>
            /// <remarks>
            /// The position of the vertex is defined by the X and Y coordinates, the Z value represents the bulge at that vertex.
            /// </remarks>
            public Vector3[] Vertexes;

            /// <summary>
            /// Gets if the polyline is closed.
            /// </summary>
            public bool IsClosed;

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Polyline</c> class.
            /// </summary>
            public Polyline()
                : base(EdgeType.Polyline)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Polyline</c> class.
            /// </summary>
            /// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
            public Polyline(EntityObject entity)
                : base(EdgeType.Polyline)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }

                if (entity.Type == EntityType.Polyline2D)
                {
                    Polyline2D poly = (Polyline2D) entity;
                    IsClosed = poly.IsClosed;
                    Vertexes = new Vector3[poly.Vertexes.Count];
                    for (int i = 0; i < poly.Vertexes.Count; i++)
                    {
                        Vertexes[i] = new Vector3(poly.Vertexes[i].Position.X, poly.Vertexes[i].Position.Y, poly.Vertexes[i].Bulge);
                    }
                }
                else if (entity.Type == EntityType.Polyline3D)
                {
                    Matrix3 trans = MathHelper.ArbitraryAxis(entity.Normal).Transpose();

                    Polyline3D poly = (Polyline3D) entity;
                    IsClosed = poly.IsClosed;
                    Vertexes = new Vector3[poly.Vertexes.Count];
                    for (int i = 0; i < poly.Vertexes.Count; i++)
                    {
                        Vector3 point = trans * poly.Vertexes[i];
                        Vertexes[i] = new Vector3(point.X, point.Y, 0.0);
                    }
                }
                else
                    throw new ArgumentException("The entity is not a Polyline2D or a Polyline3D", nameof(entity));
            }

            /// <summary>
            /// Decompose the actual polyline in its internal entities, <see cref="HatchBoundaryPath.Line">lines</see> and <see cref="HatchBoundaryPath.Arc">arcs</see>.
            /// </summary>
            /// <returns>A list of <see cref="HatchBoundaryPath.Line">lines</see> and <see cref="HatchBoundaryPath.Arc">arcs</see> that made up the polyline.</returns>
            public List<Edge> Explode()
            {
                List<Edge> edges = new List<Edge>();

                int index = 0;
                foreach (Vector3 vertex in Vertexes)
                {
                    double bulge = vertex.Z;
                    Vector2 p1;
                    Vector2 p2;

                    if (index == Vertexes.Length - 1)
                    {
                        if (!IsClosed)
                        {
                            break;
                        }

                        p1 = new Vector2(vertex.X, vertex.Y);
                        p2 = new Vector2(Vertexes[0].X, Vertexes[0].Y);
                    }
                    else
                    {
                        p1 = new Vector2(vertex.X, vertex.Y);
                        p2 = new Vector2(Vertexes[index + 1].X, Vertexes[index + 1].Y);
                    }

                    if (MathHelper.IsZero(bulge))
                    {
                        // the polyline edge is a line
                        Line line = new Line
                        {
                            Start = p1,
                            End = p2
                        };
                        edges.Add(line);
                    }
                    else
                    {
                        // the polyline edge is an arc
                        Tuple<Vector2, double, double, double> arcData = MathHelper.ArcFromBulge(p1, p2, bulge);
                        Vector2 center = arcData.Item1;
                        double radius = arcData.Item2;
                        double startAngle = arcData.Item3;
                        double endAngle = arcData.Item4;

                        // avoid arcs with very small radius, draw a line instead
                        if (MathHelper.IsZero(radius))
                        {
                            // the polyline edge is a line
                            Line line = new Line
                            {
                                Start = p1,
                                End = p2
                            };
                            edges.Add(line);
                        }
                        else
                        {
                            Arc arc = new Arc
                            {
                                Center = center,
                                Radius = radius,
                                StartAngle = startAngle,
                                EndAngle = endAngle,
                            };
                            edges.Add(arc);
                        }
                    }

                    index++;
                }
                return edges;
            }

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Polyline</c> class.
            /// </summary>
            /// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
            public static Polyline ConvertFrom(EntityObject entity)
            {
                return new Polyline(entity);
            }

            /// <summary>
            /// Converts the actual edge to its entity equivalent.
            /// </summary>
            /// <returns>An <see cref="EntityObject">entity</see> equivalent to the actual edge.</returns>
            public override EntityObject ConvertTo()
            {
                List<Polyline2DVertex> points = new List<Polyline2DVertex>(Vertexes.Length);
                foreach (Vector3 point in Vertexes)
                {
                    points.Add(new Polyline2DVertex(point.X, point.Y, point.Z));
                }
                return new Polyline2D(points, IsClosed);
            }

            /// <summary>
            /// Clones the actual edge.
            /// </summary>
            /// <returns>A copy of the actual edge.</returns>
            public override object Clone()
            {
                Polyline copy = new Polyline
                {
                    Vertexes = new Vector3[Vertexes.Length]
                };

                for (int i = 0; i < Vertexes.Length; i++)
                {
                    copy.Vertexes[i] = Vertexes[i];
                }
                return copy;
            }
        }

        /// <summary>
        /// Represents a line edge of a HatchBoundaryPath.
        /// </summary>
        public class Line :
            Edge
        {
            /// <summary>
            /// Gets or sets the start point of the line.
            /// </summary>
            public Vector2 Start;

            /// <summary>
            /// Gets or sets the end point of the line.
            /// </summary>
            public Vector2 End;

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Line</c> class.
            /// </summary>
            public Line()
                : base(EdgeType.Line)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Line</c> class.
            /// </summary>
            /// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
            public Line(EntityObject entity)
                : base(EdgeType.Line)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.Line line = entity as Entities.Line;
                if (line == null)
                    throw new ArgumentException("The entity is not a Line", nameof(entity));

                Vector3 point;
                Matrix3 trans = MathHelper.ArbitraryAxis(entity.Normal).Transpose();

                point = trans * line.StartPoint;
                Start = new Vector2(point.X, point.Y);
                point = trans * line.EndPoint;
                End = new Vector2(point.X, point.Y);
            }

            /// <summary>
            /// Creates a BoundaryBoundaryPath from an <see cref="EntityObject">entity</see>.
            /// </summary>
            /// <param name="entity">An <see cref="EntityObject">entity</see>.</param>
            /// <returns>A HatchBoundaryPath line.</returns>
            public static Line ConvertFrom(EntityObject entity)
            {
                return new Line(entity);
            }

            /// <summary>
            /// Converts the actual edge to its entity equivalent.
            /// </summary>
            /// <returns>An <see cref="EntityObject">entity</see> equivalent to the actual edge.</returns>
            public override EntityObject ConvertTo()
            {
                return new Entities.Line(Start, End);
            }

            /// <summary>
            /// Clones the actual edge.
            /// </summary>
            /// <returns>A copy of the actual edge.</returns>
            public override object Clone()
            {
                Line copy = new Line
                {
                    Start = Start,
                    End = End
                };

                return copy;
            }
        }

        /// <summary>
        /// Represents an arc edge of a HatchBoundaryPath.
        /// </summary>
        public class Arc :
            Edge
        {
            /// <summary>
            /// Gets or set the center of the arc.
            /// </summary>
            public Vector2 Center;

            /// <summary>
            /// Gets or sets the radius of the arc.
            /// </summary>
            public double Radius;

            /// <summary>
            /// Gets or sets the start angle of the arc.
            /// </summary>
            public double StartAngle;

            /// <summary>
            /// Gets or sets the end angle of the arc.
            /// </summary>
            public double EndAngle;

            /// <summary>
            /// Gets or sets if the arc is counter clockwise.
            /// </summary>
            public bool IsCounterclockwise;

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Arc</c> class.
            /// </summary>
            public Arc()
                : base(EdgeType.Arc)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Arc</c> class.
            /// </summary>
            /// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
            public Arc(EntityObject entity)
                : base(EdgeType.Arc)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));
                Vector3 point;
                Matrix3 trans = MathHelper.ArbitraryAxis(entity.Normal).Transpose();
                switch (entity.Type)
                {
                    case EntityType.Arc:
                        Entities.Arc arc = (Entities.Arc) entity;
                        point = trans * arc.Center;
                        Center = new Vector2(point.X, point.Y);
                        Radius = arc.Radius;
                        StartAngle = arc.StartAngle;
                        EndAngle = arc.EndAngle;
                        IsCounterclockwise = true;
                        break;
                    case EntityType.Circle:
                        Circle circle = (Circle) entity;
                        point = trans * circle.Center;
                        Center = new Vector2(point.X, point.Y);
                        Radius = circle.Radius;
                        StartAngle = 0.0;
                        EndAngle = 360.0;
                        IsCounterclockwise = true;
                        break;
                    default:
                        throw new ArgumentException("The entity is not a Circle or an Arc", nameof(entity));
                }
            }

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Arc</c> class.
            /// </summary>
            /// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
            public static Arc ConvertFrom(EntityObject entity)
            {
                return new Arc(entity);
            }

            /// <summary>
            /// Converts the actual edge to its entity equivalent.
            /// </summary>
            /// <returns>An <see cref="EntityObject">entity</see> equivalent to the actual edge.</returns>
            public override EntityObject ConvertTo()
            {
                if (MathHelper.IsEqual(MathHelper.NormalizeAngle(StartAngle), MathHelper.NormalizeAngle(EndAngle)))
                {
                    return new Circle(Center, Radius);
                }

                if (IsCounterclockwise)
                {
                    return new Entities.Arc(Center, Radius, StartAngle, EndAngle);
                }

                return new Entities.Arc(Center, Radius, 360 - EndAngle, 360 - StartAngle);
            }

            /// <summary>
            /// Clones the actual edge.
            /// </summary>
            /// <returns>A copy of the actual edge.</returns>
            public override object Clone()
            {
                Arc copy = new Arc
                {
                    Center = Center,
                    Radius = Radius,
                    StartAngle = StartAngle,
                    EndAngle = EndAngle,
                    IsCounterclockwise = IsCounterclockwise
                };

                return copy;
            }
        }

        /// <summary>
        /// Represents a ellipse edge of a HatchBoundaryPath.
        /// </summary>
        public class Ellipse :
            Edge
        {
            /// <summary>
            /// Gets or sets the center of the ellipse.
            /// </summary>
            public Vector2 Center;

            /// <summary>
            /// Gets or sets the position of the end of the major axis.
            /// </summary>
            public Vector2 EndMajorAxis;

            /// <summary>
            /// Gets or sets the scale of the minor axis in respect of the major axis.
            /// </summary>
            public double MinorRatio;

            /// <summary>
            /// Gets or sets the start angle of the ellipse.
            /// </summary>
            public double StartAngle;

            /// <summary>
            /// Gets or sets the end angle of the ellipse.
            /// </summary>
            public double EndAngle;

            /// <summary>
            /// Gets or sets if the ellipse is counter clockwise.
            /// </summary>
            public bool IsCounterclockwise;

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Ellipse</c> class.
            /// </summary>
            public Ellipse()
                : base(EdgeType.Ellipse)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Ellipse</c> class.
            /// </summary>
            /// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
            public Ellipse(EntityObject entity)
                : base(EdgeType.Ellipse)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.Ellipse ellipse = entity as Entities.Ellipse;
                if (ellipse == null)
                    throw new ArgumentException("The entity is not an Ellipse", nameof(entity));

                Matrix3 trans = MathHelper.ArbitraryAxis(entity.Normal).Transpose();

                Vector3 point = trans * ellipse.Center;
                Center = new Vector2(point.X, point.Y);

                double sine = 0.5*ellipse.MajorAxis*Math.Sin(ellipse.Rotation*MathHelper.DegToRad);
                double cosine = 0.5*ellipse.MajorAxis*Math.Cos(ellipse.Rotation*MathHelper.DegToRad);
                EndMajorAxis = new Vector2(cosine, sine);
                MinorRatio = ellipse.MinorAxis/ellipse.MajorAxis;
                if (ellipse.IsFullEllipse)
                {
                    StartAngle = 0.0;
                    EndAngle = 360.0;
                }
                else
                {
                    StartAngle = ellipse.StartAngle;
                    EndAngle = ellipse.EndAngle;
                }
                IsCounterclockwise = true;
            }

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Ellipse</c> class.
            /// </summary>
            /// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
            public static Ellipse ConvertFrom(EntityObject entity)
            {
                return new Ellipse(entity);
            }

            /// <summary>
            /// Converts the actual edge to its entity equivalent.
            /// </summary>
            /// <returns>An <see cref="EntityObject">entity</see> equivalent to the actual edge.</returns>
            public override EntityObject ConvertTo()
            {
                Vector3 center = new Vector3(Center.X, Center.Y, 0.0);
                Vector3 axisPoint = new Vector3(EndMajorAxis.X, EndMajorAxis.Y, 0.0);
                Vector3 ocsAxisPoint = MathHelper.Transform(axisPoint,
                    Vector3.UnitZ,
                    CoordinateSystem.World,
                    CoordinateSystem.Object);
                double rotation = Vector2.Angle(new Vector2(ocsAxisPoint.X, ocsAxisPoint.Y))*MathHelper.RadToDeg;
                double majorAxis = 2*axisPoint.Modulus();
                return new Entities.Ellipse(center, majorAxis, majorAxis*MinorRatio)
                {
                    Rotation = rotation,
                    StartAngle = IsCounterclockwise ? StartAngle : 360 - EndAngle,
                    EndAngle = IsCounterclockwise ? EndAngle : 360 - StartAngle,
                };
            }

            /// <summary>
            /// Clones the actual edge.
            /// </summary>
            /// <returns>A copy of the actual edge.</returns>
            public override object Clone()
            {
                Ellipse copy = new Ellipse
                {
                    Center = Center,
                    EndMajorAxis = EndMajorAxis,
                    MinorRatio = MinorRatio,
                    StartAngle = StartAngle,
                    EndAngle = EndAngle,
                    IsCounterclockwise = IsCounterclockwise
                };

                return copy;
            }
        }

        /// <summary>
        /// Represents a spline edge of a HatchBoundaryPath.
        /// </summary>
        public class Spline :
            Edge
        {
            /// <summary>
            /// Gets or sets the degree of the spline
            /// </summary>
            public short Degree;

            /// <summary>
            /// Gets or sets if the spline is rational.
            /// </summary>
            public bool IsRational;

            /// <summary>
            /// Gets or sets if the spline is periodic.
            /// </summary>
            public bool IsPeriodic;

            /// <summary>
            /// Gets or sets the list of knots of the spline.
            /// </summary>
            public double[] Knots;

            /// <summary>
            /// Gets or sets the list of control points of the spline.
            /// </summary>
            /// <remarks>
            /// The position of the control point is defined by the X and Y coordinates, the Z value represents its weight.
            /// </remarks>
            public Vector3[] ControlPoints; // location: (x, y) weight: z

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Spline</c> class.
            /// </summary>
            public Spline()
                : base(EdgeType.Spline)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Spline</c> class.
            /// </summary>
            /// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
            public Spline(EntityObject entity)
                : base(EdgeType.Spline)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }

                Entities.Spline spline = entity as Entities.Spline;
                if (spline == null)
                {
                    throw new ArgumentException("The entity is not an Spline", nameof(entity));
                }

                Degree = spline.Degree;
                IsRational = true;
                IsPeriodic = spline.IsClosedPeriodic;
                if (spline.ControlPoints.Length == 0)
                {
                    throw new ArgumentException("The HatchBoundaryPath spline edge requires a spline entity with control points.", nameof(entity));
                }

                Matrix3 trans = MathHelper.ArbitraryAxis(entity.Normal).Transpose();

                ControlPoints = new Vector3[spline.ControlPoints.Length];
                for (int i = 0; i < spline.ControlPoints.Length; i++)
                {
                    Vector3 point = trans * spline.ControlPoints[i];
                    ControlPoints[i] = new Vector3(point.X, point.Y, spline.Weights[i]);
                }

                Knots = new double[spline.Knots.Length];
                for (int i = 0; i < spline.Knots.Length; i++)
                {
                    Knots[i] = spline.Knots[i];
                }
            }

            /// <summary>
            /// Initializes a new instance of the <c>HatchBoundaryPath.Spline</c> class.
            /// </summary>
            /// <param name="entity"><see cref="EntityObject">Entity</see> that represents the edge.</param>
            public static Spline ConvertFrom(EntityObject entity)
            {
                return new Spline(entity);
            }

            /// <summary>
            /// Converts the actual edge to its entity equivalent.
            /// </summary>
            /// <returns>An <see cref="EntityObject">entity</see> equivalent to the actual edge.</returns>
            public override EntityObject ConvertTo()
            {
                List<Vector3> ctrl = new List<Vector3>();
                List<double> weights = new List<double>();
                List<double> knots = new List<double>(Knots);

                foreach (Vector3 point in ControlPoints)
                {
                    ctrl.Add(new Vector3(point.X, point.Y, 0.0));
                    weights.Add(point.Z);
                }
                return new Entities.Spline(ctrl, weights, knots, Degree, IsPeriodic);
            }

            /// <summary>
            /// Clones the actual edge.
            /// </summary>
            /// <returns>A copy of the actual edge.</returns>
            public override object Clone()
            {
                Spline copy = new Spline
                {
                    Degree = Degree,
                    IsRational = IsRational,
                    IsPeriodic = IsPeriodic,
                    Knots = new double[Knots.Length],
                    ControlPoints = new Vector3[ControlPoints.Length],
                };
                for (int i = 0; i < Knots.Length; i++)
                {
                    copy.Knots[i] = Knots[i];
                }
                for (int i = 0; i < ControlPoints.Length; i++)
                {
                    copy.ControlPoints[i] = ControlPoints[i];
                }
                return copy;
            }
        }

        #endregion

        #region private fields

        private readonly List<EntityObject> entities;
        private readonly List<Edge> edges;
        private HatchBoundaryPathTypeFlags pathType;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <c>HatchBoundaryPath</c> class.
        /// </summary>
        /// <param name="edges">List of entities that makes a loop for the hatch boundary paths.</param>
        public HatchBoundaryPath(IEnumerable<EntityObject> edges)
        {
            if (edges == null)
            {
                throw new ArgumentNullException(nameof(edges));
            }
            this.edges = new List<Edge>();
            pathType = HatchBoundaryPathTypeFlags.Derived | HatchBoundaryPathTypeFlags.External;
            entities = new List<EntityObject>(edges);
            Update();
        }

        /// <summary>
        /// Initializes a new instance of the <c>HatchBoundaryPath</c> class.
        /// </summary>
        /// <param name="edges">List of edges that makes a loop for the hatch boundary paths.</param>
        public HatchBoundaryPath(IEnumerable<Edge> edges)
        {
            if (edges == null)
            {
                throw new ArgumentNullException(nameof(edges));
            }
            pathType = HatchBoundaryPathTypeFlags.Derived | HatchBoundaryPathTypeFlags.External;
            entities = new List<EntityObject>();
            this.edges = new List<Edge>();
            foreach (Edge edge in edges)
            {
                if (edges.Count() == 1 && edge.Type == EdgeType.Polyline)
                {
                    pathType |= HatchBoundaryPathTypeFlags.Polyline;
                    this.edges.Add(edge);
                }
                else
                {
                    if (edge.Type == EdgeType.Polyline)
                    {
                        // Only a single polyline edge can be part of a HatchBoundaryPath. The polyline will be automatically exploded.
                        Polyline polyline = (Polyline)edge;
                        this.edges.AddRange(polyline.Explode());
                    }
                    else
                    {
                        this.edges.Add(edge);
                    }
                }
            }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the list of edges that makes a loop for the hatch boundary path.
        /// </summary>
        public IReadOnlyList<Edge> Edges
        {
            get { return edges; }
        }

        /// <summary>
        /// Gets the boundary path type flag.
        /// </summary>
        public HatchBoundaryPathTypeFlags PathType
        {
            get { return pathType; }
            internal set { pathType = value; }
        }

        /// <summary>
        /// Gets the list of entities that makes the boundary.
        /// </summary>
        /// <remarks>If the boundary path belongs to a non-associative hatch this list will contain zero entities.</remarks>
        public IReadOnlyList<EntityObject> Entities
        {
            get { return entities; }
        }

        #endregion

        #region internal methods

        internal void AddContour(EntityObject entity)
        {
            entities.Add(entity);
        }

        internal void ClearContour()
        {
            entities.Clear();
        }

        internal bool RemoveContour(EntityObject entity)
        {
            return entities.Remove(entity);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Updates the internal HatchBoundaryPath data. 
        /// </summary>
        /// <remarks>
        /// It is necessary to manually call this method when changes to the boundary entities are made. This is only applicable to associative hatches,
        /// non-associative hatches has no associated boundary entities.
        /// </remarks>
        public void Update()
        {
            SetInternalInfo(entities, true);
        }

        #endregion

        #region private methods

        private void SetInternalInfo(IEnumerable<EntityObject> contour, bool clearEdges)
        {
            bool containsPolyline = false;
            if (clearEdges)
            {
                edges.Clear();
            }

            foreach (EntityObject entity in contour)
            {
                if (containsPolyline)
                {
                    throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                }

                // it seems that AutoCad does not have problems on creating loops that theoretically does not make sense,
                // like, for example, an internal loop that is made of a single arc.
                // so if AutoCAD is OK with that I am too, the program that make use of this information will take care of this inconsistencies
                switch (entity.Type)
                {
                    case EntityType.Arc:
                        edges.Add(Arc.ConvertFrom(entity));
                        break;
                    case EntityType.Circle:
                        edges.Add(Arc.ConvertFrom(entity));
                        break;
                    case EntityType.Ellipse:
                        edges.Add(Ellipse.ConvertFrom(entity));
                        break;
                    case EntityType.Line:
                        edges.Add(Line.ConvertFrom(entity));
                        break;
                    case EntityType.Polyline2D:
                        Polyline2D lwpoly = (Polyline2D)entity;
                        if (lwpoly.IsClosed)
                        {
                            if (edges.Count != 0)
                            {
                                throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                            }
                            edges.Add(Polyline.ConvertFrom(entity));
                            pathType |= HatchBoundaryPathTypeFlags.Polyline;
                            containsPolyline = true;
                        }
                        else
                            SetInternalInfo(lwpoly.Explode(), false); // open polylines will always be exploded, only one polyline can be present in a path
                        break;
                    case EntityType.Polyline3D:
                        Polyline3D poly = (Polyline3D) entity;
                        if (poly.IsClosed)
                        {
                            if (edges.Count != 0)
                            {
                                throw new ArgumentException("Closed polylines cannot be combined with other entities to make a hatch boundary path.");
                            }
                            edges.Add(Polyline.ConvertFrom(entity));
                            pathType |= HatchBoundaryPathTypeFlags.Polyline;
                            containsPolyline = true;
                        }
                        else
                            SetInternalInfo(poly.Explode(), false); // open polylines will always be exploded, only one polyline can be present in a path
                        break;
                    case EntityType.Spline:
                        edges.Add(Spline.ConvertFrom(entity));
                        break;
                    default:
                        throw new ArgumentException(string.Format("The entity type {0} cannot be part of a hatch boundary. Only Arc, Circle, Ellipse, Line, Polyline2D, Polyline3D, and Spline entities are allowed.", entity.Type));
                }
            }
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Creates a new HatchBoundaryPath that is a copy of the current instance.
        /// </summary>
        /// <returns>A new HatchBoundaryPath that is a copy of this instance.</returns>
        /// <remarks>When cloning a HatchBoundaryPath, if it has entities that defines its contour, they will not be cloned.</remarks>
        public object Clone()
        {
            List<Edge> copyEdges = new List<Edge>();
            foreach (Edge edge in edges)
            {
                copyEdges.Add((Edge) edge.Clone());
            }

            return new HatchBoundaryPath(copyEdges);
        }

        #endregion
    }
}