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

// This is a translation to C# from the original C++ code of the Geometric Tool Library
// Original license
// David Eberly, Geometric Tools, Redmond WA 98052
// Copyright (c) 1998-2022
// Distributed under the Boost Software License, Version 1.0.
// https://www.boost.org/LICENSE_1_0.txt
// https://www.geometrictools.com/License/Boost/LICENSE_1_0.txt
// Version: 6.0.2022.01.06

using System;
using System.Diagnostics;

namespace netDxf.GTE
{
    public struct UniqueKnot
    {
        private double t;
        private int multiplicity;

        public UniqueKnot(double t, int multiplicity)
        {
            this.t = t;
            this.multiplicity = multiplicity;
        }

        public double T
        {
            get { return t; }
            set { t = value; }
        }

        public int Multiplicity
        {
            get { return multiplicity; }
            set { multiplicity = value; }
        }
    }

    public struct BasisFunctionInput
    {
        private int numControls;
        private int degree;
        private bool uniform;
        private bool periodic;
        private int numUniqueKnots;
        private UniqueKnot[] uniqueKnots;

        // Construct an open uniform curve with t in [0,1].
        public BasisFunctionInput(int inNumControls, int inDegree)
        {
            numControls = inNumControls;
            degree = inDegree;
            uniform = true;
            periodic = false;
            numUniqueKnots = numControls - degree + 1;
            uniqueKnots = new UniqueKnot[numUniqueKnots];
            uniqueKnots[0] = new UniqueKnot(0, degree + 1);
            for (int i = 1; i <= numUniqueKnots - 2; i++)
            {
                uniqueKnots[i] = new UniqueKnot(i / (numUniqueKnots - 1.0), 1);
            }

            uniqueKnots[uniqueKnots.Length - 1] = new UniqueKnot(1, degree + 1);
        }

        public int NumControls
        {
            get { return numControls; }
            set { numControls = value; }
        }

        public int Degree
        {
            get { return degree; }
            set { degree = value; }
        }

        public bool Uniform
        {
            get { return uniform; }
            set { uniform = value; }
        }

        public bool Periodic
        {
            get { return periodic; }
            set { periodic = value; }
        }

        public int NumUniqueKnots
        {
            get { return numUniqueKnots; }
            set { numUniqueKnots = value; }
        }

        public UniqueKnot[] UniqueKnots
        {
            get { return uniqueKnots; }
            set { uniqueKnots = value; }
        }
    }

    // Let n be the number of control points. Let d be the degree, where
    // 1 <= d <= n-1.  The number of knots is k = n + d + 1.  The knots
    // are t[i] for 0 <= i < k and must be non decreasing, t[i] <= t[i+1],
    // but a knot value can be repeated.  Let s be the number of distinct
    // knots.  Let the distinct knots be u[j] for 0 <= j < s, so u[j] <
    // u[j+1] for all j.  The set of u[j] is called a 'breakpoint
    // sequence'.  Let m[j] >= 1 be the multiplicity; that is, if t[i] is
    // the first occurrence of u[j], then t[i+r] = t[i] for 1 <= r < m[j].
    // The multiplicities have the constraints m[0] <= d+1, m[s-1] <= d+1,
    // and m[j] <= d for 1 <= j <= s-2.  Also, k = sum_{j=0}^{s-1} m[j],
    // which says the multiplicities account for all k knots.
    //
    // Given a knot vector (t[0],...,t[n+d]), the domain of the
    // corresponding B-spline curve is the interval [t[d],t[n]].
    //
    // The corresponding B-spline or NURBS curve is characterized as
    // follows.  See "Geometric Modeling with Splines: An Introduction" by
    // Elaine Cohen, Richard F. Riesenfeld and Gershon Elber, AK Peters,
    // 2001, Natick MA.  The curve is 'open' when m[0] = m[s-1] = d+1;
    // otherwise, it is 'floating'.  An open curve is uniform when the
    // knots t[d] through t[n] are equally spaced; that is, t[i+1] - t[i]
    // are a common value for d <= i <= n-1.  By implication, s = n-d+1
    // and m[j] = 1 for 1 <= j <= s-2.  An open curve that does not
    // satisfy these conditions is said to be nonuniform.  A floating
    // curve is uniform when m[j] = 1 for 0 <= j <= s-1 and t[i+1] - t[i]
    // are a common value for 0 <= i <= k-2; otherwise, the floating curve
    // is nonuniform.
    //
    // A special case of a floating curve is a periodic curve.  The intent
    // is that the curve is closed, so the first and last control points
    // should be the same, which ensures C^{0} continuity.  Higher-order
    // continuity is obtained by repeating more control points.  If the
    // control points are P[0] through P[n-1], append the points P[0]
    // through P[d-1] to ensure C^{d-1} continuity.  Additionally, the
    // knots must be chosen properly.  You may choose t[d] through t[n] as
    // you wish.  The other knots are defined by
    //   t[i] - t[i-1] = t[n-d+i] - t[n-d+i-1]
    //   t[n+i] - t[n+i-1] = t[d+i] - t[d+i-1]
    // for 1 <= i <= d.
    public class BasisFunction
    {
        private readonly struct Key
        {
            private readonly double knotValue;
            private readonly int knotIndex;

            public Key(double knotValue, int knotIndex)
            {
                this.knotValue = knotValue;
                this.knotIndex = knotIndex;
            }

            public double KnotValue
            {
                get { return knotValue; }
            }

            public int KnotIndex
            {
                get { return knotIndex; }
            }
        }


        // Constructor inputs and values derived from them.
        private int numControls;
        private int degree;
        private double tMin, tMax, tLength;
        private bool open;
        private bool uniform;
        private bool periodic;
        private UniqueKnot[] uniqueKnots;
        private double[] knots;

        // Lookup information for the GetIndex() function.  The first element
        // of the pair is a unique knot value.  The second element is the
        // index in mKnots[] for the last occurrence of that knot value.
        private Key[] keys;

        // Storage for the basis functions and their first three derivatives;
        // mJet[i] is array[d+1][n+d].
        private double[][][] jet;

        // Construction and destruction.  The determination that the curve is
        // open or floating is based on the multiplicities.  The 'uniform'
        // input is used to avoid misclassifications due to floating-point
        // rounding errors.  Specifically, the breakpoints might be equally
        // spaced (uniform) as real numbers, but the floating-point
        // representations can have rounding errors that cause the knot
        // differences not to be exactly the same constant.  A periodic curve
        // can have uniform or nonuniform knots.  This object makes copies of
        // the input arrays.
        public BasisFunction(BasisFunctionInput input)
        {
            Create(input);
        }

        // Support for explicit creation in classes that have std::array
        // members involving BasisFunction.  This is a call-once function.
        public void Create(BasisFunctionInput input)
        {
            Debug.Assert(input.NumControls >= 2, "Invalid number of control points.");
            Debug.Assert(1 <= input.Degree && input.Degree < input.NumControls, "Invalid degree.");
            Debug.Assert(input.NumUniqueKnots >= 2, "Invalid number of unique knots.");

            numControls = input.Periodic ? input.NumControls + input.Degree : input.NumControls;
            degree = input.Degree;
            tMin = 0;
            tMax = 0;
            tLength = 0;
            open = false;
            uniform = input.Uniform;
            periodic = input.Periodic;
            jet = new double[4][][];

            uniqueKnots = new UniqueKnot[input.UniqueKnots.Length];
            input.UniqueKnots.CopyTo(uniqueKnots, 0);

            double u = uniqueKnots[0].T;
            for (int i = 1; i < input.NumUniqueKnots - 1; i++)
            {
                double uNext = uniqueKnots[i].T;
                Debug.Assert(u < uNext, "Unique knots are not strictly increasing.");
                u = uNext;
            }

            int mult0 = uniqueKnots[0].Multiplicity;
            Debug.Assert(mult0 >= 1 && mult0 <= degree + 1, "Invalid first multiplicity.");

            int mult1 = uniqueKnots[uniqueKnots.Length - 1].Multiplicity;
            Debug.Assert(mult1 >= 1 && mult1 <= degree + 1, "Invalid last multiplicity.");

            for (int i = 1; i <= input.NumUniqueKnots - 2; i++)
            {
                int mult = uniqueKnots[i].Multiplicity;
                Debug.Assert(mult >= 1 && mult <= degree + 1, "Invalid interior multiplicity.");
            }

            open = mult0 == mult1 && mult0 == degree + 1;

            knots = new double[numControls + degree + 1];
            keys = new Key[input.NumUniqueKnots];
            int sum = 0;
            for (int i = 0, j = 0; i < input.NumUniqueKnots; i++)
            {
                double tCommon = uniqueKnots[i].T;
                int mult = uniqueKnots[i].Multiplicity;
                for (int k = 0; k < mult; k++, j++)
                {
                    knots[j] = tCommon;
                }

                keys[i] = new Key(tCommon, sum - 1);
                sum += mult;
            }
            
            tMin = knots[degree];
            tMax = knots[numControls];
            tLength = tMax - tMin;

            int numRows = degree + 1;
            int numCols = numControls + degree;
            for (int i = 0; i < 4; ++i)
            {
                jet[i] = new double[numRows][];
                for (int j = 0; j < numRows; j++)
                {
                    jet[i][j] = new double[numCols];
                }
            }
        }

        // Member access.
        public int NumControls
        {
            get { return numControls; }
        }

        public int Degree
        {
            get { return degree; }
        }

        public int NumUniqueKnots
        {
            get { return uniqueKnots.Length; }
        }

        public int NumKnots
        {
            get { return knots.Length; }
        }

        public double MinDomain
        {
            get { return tMin; }
        }

        public double MaxDomain
        {
            get { return tMax; }
        }

        public bool IsOpen
        {
            get { return open; }
        }

        public bool IsUniform
        {
            get { return uniform; }
        }

        public bool IsPeriodic
        {
            get { return periodic; }
        }

        public UniqueKnot[] UniqueKnots
        {
            get { return uniqueKnots; }
        }

        public double[] Knots
        {
            get { return knots; }
        }

        // Evaluation of the basis function and its derivatives through 
        // order 3.  For the function value only, pass order 0.  For the
        // function and first derivative, pass order 1, and so on.
        public void Evaluate(double t, int order, out int minIndex, out int maxIndex)
        {
            Debug.Assert(order <= 3, "Invalid order.");

            int i = GetIndex(ref t);
            jet[0][0][i] = 1.0;

            if (order >= 1)
            {
                jet[1][0][i] = 0.0;
                if (order >= 2)
                {
                    jet[2][0][i] = 0.0;
                    if (order >= 3)
                    {
                        jet[3][0][i] = 0.0;
                    }
                }
            }

            double n0 = t - knots[i], n1 = knots[i + 1] - t;
            double e0, e1, d0, d1, invD0, invD1;
            int j;
            for (j = 1; j <= degree; j++)
            {
                d0 = knots[i + j] - knots[i];
                d1 = knots[i + 1] - knots[i - j + 1];
                invD0 = d0 > 0.0 ? 1.0 / d0 : 0.0;
                invD1 = d1 > 0.0 ? 1.0 / d1 : 0.0;

                e0 = n0 * jet[0][j - 1][i];
                jet[0][j][i] = e0 * invD0;
                e1 = n1 * jet[0][j - 1][i - j + 1];
                jet[0][j][i - j] = e1 * invD1;

                if (order >= 1)
                {
                    e0 = n0 * jet[1][j - 1][i] + jet[0][j - 1][i];
                    jet[1][j][i] = e0 * invD0;
                    e1 = n1 * jet[1][j - 1][i - j + 1] - jet[0][j - 1][i - j + 1];
                    jet[1][j][i - j] = e1 * invD1;

                    if (order >= 2)
                    {
                        e0 = n0 * jet[2][j - 1][i] + 2 * jet[1][j - 1][i];
                        jet[2][j][i] = e0 * invD0;
                        e1 = n1 * jet[2][j - 1][i - j + 1] - 2 * jet[1][j - 1][i - j + 1];
                        jet[2][j][i - j] = e1 * invD1;

                        if (order >= 3)
                        {
                            e0 = n0 * jet[3][j - 1][i] + 3 * jet[2][j - 1][i];
                            jet[3][j][i] = e0 * invD0;
                            e1 = n1 * jet[3][j - 1][i - j + 1] - 3 * jet[2][j - 1][i - j + 1];
                            jet[3][j][i - j] = e1 * invD1;
                        }
                    }
                }
            }

            for (j = 2; j <= degree; j++)
            {
                for (int k = i - j + 1; k < i; k++)
                {
                    n0 = t - knots[k];
                    n1 = knots[k + j + 1] - t;
                    d0 = knots[k + j] - knots[k];
                    d1 = knots[k + j + 1] - knots[k + 1];
                    invD0 = d0 > 0 ? 1 / d0 : 0;
                    invD1 = d1 > 0 ? 1 / d1 : 0;

                    e0 = n0 * jet[0][j - 1][k];
                    e1 = n1 * jet[0][j - 1][k + 1];
                    jet[0][j][k] = e0 * invD0 + e1 * invD1;

                    if (order >= 1)
                    {
                        e0 = n0 * jet[1][j - 1][k] + jet[0][j - 1][k];
                        e1 = n1 * jet[1][j - 1][k + 1] - jet[0][j - 1][k + 1];
                        jet[1][j][k] = e0 * invD0 + e1 * invD1;

                        if (order >= 2)
                        {
                            e0 = n0 * jet[2][j - 1][k] + 2 * jet[1][j - 1][k];
                            e1 = n1 * jet[2][j - 1][k + 1] - 2 * jet[1][j - 1][k + 1];
                            jet[2][j][k] = e0 * invD0 + e1 * invD1;

                            if (order >= 3)
                            {
                                e0 = n0 * jet[3][j - 1][k] + 3 * jet[2][j - 1][k];
                                e1 = n1 * jet[3][j - 1][k + 1] - 3 * jet[2][j - 1][k + 1];
                                jet[3][j][k] = e0 * invD0 + e1 * invD1;
                            }
                        }
                    }
                }
            }

            minIndex = i - degree;
            maxIndex = i;
        }

        // Access the results of the call to Evaluate(...).  The index i must
        // satisfy minIndex <= i <= maxIndex.  If it is not, the function
        // returns zero.  The separation of evaluation and access is based on
        // local control of the basis function; that is, only the accessible
        // values are (potentially) not zero.
        public double GetValue(int order, int i)
        {
            if (order < 4)
            {
                if (0 <= i && i < numControls + degree)
                {
                    return jet[order][degree][i];
                }

                throw new ArgumentException("Invalid index.", nameof(i));
            }

            throw new ArgumentException("Invalid order.", nameof(order));
        }

        // Determine the index i for which knot[i] <= t < knot[i+1].  The
        // t-value is modified (wrapped for periodic splines, clamped for
        // non periodic splines).
        private int GetIndex(ref double t)
        {
            // Find the index i for which knot[i] <= t < knot[i+1].
            if (periodic)
            {
                // Wrap to [tmin,tmax].
                double r = (t - tMin) % tLength;
                if (r < 0.0)
                {
                    r += tLength;
                }

                t = tMin + r;
            }

            // Clamp to [tmin,tmax]. For the periodic case, this handles
            // small numerical rounding errors near the domain endpoints.
            if (t <= tMin)
            {
                t = tMin;
                return degree;
            }

            if (t >= tMax)
            {
                t = tMax;
                return numControls - 1;
            }

            // At this point, tmin < t < tmax.
            foreach (Key key in keys)
            {
                if (t < key.KnotValue)
                {
                    return key.KnotIndex;
                }
            }

            // We should not reach this code.
            throw new Exception("Unexpected condition.");
        }
    }
}