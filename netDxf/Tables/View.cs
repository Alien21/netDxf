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
using netDxf.Collections;

namespace netDxf.Tables
{
    public class View :
        TableObject
    {
        #region private fields

        private Vector3 target;
        private Vector3 camera;
        private double height;
        private double width;
        private double rotation;
        private ViewModeFlags viewmode;
        private double fov;
        private double frontClippingPlane;
        private double backClippingPlane;

        #endregion

        #region constants

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>View</c> class.
        /// </summary>
        public View(string name)
            : this(name, true)
        {
        }

        internal View(string name, bool checkName)
            : base(name, DxfObjectCode.View, checkName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "The view name should be at least one character long.");
            }

            IsReserved = false;
            target = Vector3.Zero;
            camera = Vector3.UnitZ;
            height = 1.0;
            width = 1.0;
            rotation = 0.0;
            viewmode = ViewModeFlags.Off;
            fov = 40.0;
            frontClippingPlane = 0.0;
            backClippingPlane = 0.0;
        }

        #endregion

        #region public properties

        public Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }

        public Vector3 Camera
        {
            get { return camera; }
            set { camera = value; }
        }

        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        public double Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public ViewModeFlags Viewmode
        {
            get { return viewmode; }
            set { viewmode = value; }
        }

        public double Fov
        {
            get { return fov; }
            set { fov = value; }
        }

        public double FrontClippingPlane
        {
            get { return frontClippingPlane; }
            set { frontClippingPlane = value; }
        }

        public double BackClippingPlane
        {
            get { return backClippingPlane; }
            set { backClippingPlane = value; }
        }

        /// <summary>
        /// Gets the owner of the actual view.
        /// </summary>
        public new Views Owner
        {
            get { return (Views) base.Owner; }
            internal set { base.Owner = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Checks if this instance has been referenced by other DxfObjects. 
        /// </summary>
        /// <returns>
        /// Returns true if this instance has been referenced by other DxfObjects, false otherwise.
        /// It will always return false if this instance does not belong to a document.
        /// </returns>
        /// <remarks>
        /// This method returns the same value as the HasReferences method that can be found in the TableObjects class.
        /// </remarks>
        public override bool HasReferences()
        {
            return Owner != null && Owner.HasReferences(Name);
        }

        /// <summary>
        /// Gets the list of DxfObjects referenced by this instance.
        /// </summary>
        /// <returns>
        /// A list of DxfObjectReference that contains the DxfObject referenced by this instance and the number of times it does.
        /// It will return null if this instance does not belong to a document.
        /// </returns>
        /// <remarks>
        /// This method returns the same list as the GetReferences method that can be found in the TableObjects class.
        /// </remarks>
        public override List<DxfObjectReference> GetReferences()
        {
            return Owner?.GetReferences(Name);
        }

        /// <summary>
        /// Creates a new View that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">View name of the copy.</param>
        /// <returns>A new View that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            View copy = new View(newName)
            {
                Target = target,
                Camera = camera,
                Height = height,
                Width = width,
                Rotation = rotation,
                Viewmode = viewmode,
                Fov = fov,
                FrontClippingPlane = frontClippingPlane,
                BackClippingPlane = backClippingPlane
            };

            foreach (XData data in XData.Values)
            {
                copy.XData.Add((XData)data.Clone());
            }

            return copy;
        }

        /// <summary>
        /// Creates a new View that is a copy of the current instance.
        /// </summary>
        /// <returns>A new View that is a copy of this instance.</returns>
        public override object Clone()
        {
            return Clone(Name);
        }

        #endregion
    }
}