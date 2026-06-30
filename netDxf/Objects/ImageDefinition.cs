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
using System.IO;
using netDxf.Collections;
using netDxf.Tables;
using netDxf.Units;

namespace netDxf.Objects
{
    /// <summary>
    /// Represents an image definition.
    /// </summary>
    public class ImageDefinition :
        TableObject
    {
        #region private fields

        private string file;
        private int width;
        private int height;
        private ImageResolutionUnits resolutionUnits;
        private double horizontalResolution;
        private double verticalResolution;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>ImageDefinition</c> class.
        /// </summary>
        /// <param name="file">Image file name with full or relative path.</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="horizontalResolution">Image horizontal resolution in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="verticalResolution">Image vertical resolution in pixels.</param>
        /// <param name="units">Image resolution units.</param>
        /// <remarks>
        /// <para>
        /// The name of the file without extension will be used as the name of the image definition.
        /// </para>
        /// <para>
        /// This is a generic constructor for all image formats supported by AutoCAD, note that not all AutoCAD versions support the same image formats.
        /// </para>
        /// <para>
        /// Note (this is from the ACAD docs): AutoCAD 2000, AutoCAD LT 2000, and later releases do not support LZW-compressed TIFF files,
        /// with the exception of English language versions sold in the US and Canada.<br />
        /// If you have TIFF files that were created using LZW compression and want to insert them into a drawing 
        /// you must save the TIFF files with LZW compression disabled.
        /// </para>
        /// </remarks>
        public ImageDefinition(string file, int width, double horizontalResolution, int height, double verticalResolution, ImageResolutionUnits units)
            : this(Path.GetFileNameWithoutExtension(file), file, width, horizontalResolution, height, verticalResolution, units)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>ImageDefinition</c> class.
        /// </summary>
        /// <param name="name">Image definition name.</param>
        /// <param name="file">Image file name with full or relative path.</param>
        /// <param name="width">Image width in pixels.</param>
        /// <param name="horizontalResolution">Image horizontal resolution in pixels.</param>
        /// <param name="height">Image height in pixels.</param>
        /// <param name="verticalResolution">Image vertical resolution in pixels.</param>
        /// <param name="units">Image resolution units.</param>
        /// <remarks>
        /// <para>
        /// The name assigned to the image definition must be unique.
        /// </para>
        /// <para>
        /// This is a generic constructor for all image formats supported by AutoCAD, note that not all AutoCAD versions support the same image formats.
        /// </para>
        /// <para>
        /// Note (this is from the ACAD docs): AutoCAD 2000, AutoCAD LT 2000, and later releases do not support LZW-compressed TIFF files,
        /// with the exception of English language versions sold in the US and Canada.<br />
        /// If you have TIFF files that were created using LZW compression and want to insert them into a drawing 
        /// you must save the TIFF files with LZW compression disabled.
        /// </para>
        /// </remarks>
        public ImageDefinition(string name, string file, int width, double horizontalResolution, int height, double verticalResolution, ImageResolutionUnits units)
            : base(name, DxfObjectCode.ImageDef, false)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (file.IndexOfAny(Path.GetInvalidPathChars()) == 0)
            {
                throw new ArgumentException("File path contains invalid characters.", nameof(file));
            }

            this.file = file;

            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), width, "The ImageDefinition width must be greater than zero.");
            }
            this.width = width;

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "The ImageDefinition height must be greater than zero.");
            }
            this.height = height;

            if (horizontalResolution <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(horizontalResolution), horizontalResolution, "The ImageDefinition horizontal resolution must be greater than zero.");
            }
            this.horizontalResolution = horizontalResolution;

            if (verticalResolution <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(verticalResolution), verticalResolution, "The ImageDefinition vertical resolution must be greater than zero.");
            }
            this.verticalResolution = verticalResolution;

            resolutionUnits = units;

        }

#if NET4X

        ///  <summary>
        ///  Initializes a new instance of the <c>ImageDefinition</c> class. Only available for Net Framework 4.5 builds.
        ///  </summary>
        ///  <param name="file">Image file name with full or relative path.</param>
        /// <remarks>
        ///  <para>
        ///  The name of the file without extension will be used as the name of the image definition.
        ///  </para>
        ///  <para>
        ///  Supported image formats: BMP, JPG, PNG, TIFF.<br />
        ///  Even thought AutoCAD supports more image formats, this constructor is restricted to the ones the net framework supports in common with AutoCAD.
        ///  Use the generic constructor instead.
        ///  </para>
        ///  <para>
        ///  Note (this is from the ACAD docs): AutoCAD 2000, AutoCAD LT 2000, and later releases do not support LZW-compressed TIFF files,
        ///  with the exception of English language versions sold in the US and Canada.<br />
        ///  If you have TIFF files that were created using LZW compression and want to insert them into a drawing 
        ///  you must save the TIFF files with LZW compression disabled.
        ///  </para>
        /// </remarks>
        public ImageDefinition(string file)
            : this(Path.GetFileNameWithoutExtension(file), file)
        {
        }

        ///  <summary>
        ///  Initializes a new instance of the <c>ImageDefinition</c> class. Only available for Net Framework 4.5 builds.
        ///  </summary>
        /// <param name="name">Image definition name.</param>
        /// <param name="file">Image file name with full or relative path.</param>
        /// <remarks>
        ///  <para>
        ///  The name assigned to the image definition must be unique.
        ///  </para>
        ///  <para>
        ///  Supported image formats: BMP, JPG, PNG, TIFF.<br />
        ///  Even thought AutoCAD supports more image formats, this constructor is restricted to the ones the .net library supports in common with AutoCAD.
        ///  Use the generic constructor instead.
        ///  </para>
        ///  <para>
        ///  Note (this is from the ACAD docs): AutoCAD 2000, AutoCAD LT 2000, and later releases do not support LZW-compressed TIFF files,
        ///  with the exception of English language versions sold in the US and Canada.<br />
        ///  If you have TIFF files that were created using LZW compression and want to insert them into a drawing 
        ///  you must save the TIFF files with LZW compression disabled.
        ///  </para>
        /// </remarks>
        public ImageDefinition(string name, string file)
            : base(name, DxfObjectCode.ImageDef, false)
        {
            if (string.IsNullOrEmpty(file))
            {
                throw new ArgumentNullException(nameof(file));
            }

            FileInfo info = new FileInfo(file);
            if (!info.Exists)
            {
                throw new FileNotFoundException("Image file not found.", file);
            }

            this.file = file;

            try
            {
                using (System.Drawing.Image bitmap = System.Drawing.Image.FromFile(file))
                {
                    this.width = bitmap.Width;
                    this.height = bitmap.Height;
                    this.horizontalResolution = bitmap.HorizontalResolution;
                    this.verticalResolution = bitmap.VerticalResolution;
                    // the System.Drawing.Image stores the image resolution in inches
                    this.resolutionUnits = ImageResolutionUnits.Inches;
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Image file not supported.", file);
            }

        }

#endif

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the image file.
        /// </summary>
        /// <remarks>
        /// When changing the image file the other properties should also be modified accordingly to avoid distortions in the final image.
        /// </remarks>
        public string File
        {
            get { return file; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (value.IndexOfAny(Path.GetInvalidPathChars()) == 0)
                {
                    throw new ArgumentException("File path contains invalid characters.", nameof(value));
                }

                file = value;
            }
        }

        /// <summary>
        /// Gets or sets the image width in pixels.
        /// </summary>
        public int Width
        {
            get { return width; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The ImageDefinition width must be greater than zero.");
                }

                width = value;
            }
        }

        /// <summary>
        /// Gets or sets the image height in pixels.
        /// </summary>
        public int Height
        {
            get { return height; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The ImageDefinition height must be greater than zero.");
                }

                height = value;
            }
        }

        /// <summary>
        /// Gets or sets the image horizontal resolution in pixels per unit.
        /// </summary>
        public double HorizontalResolution
        {
            get { return horizontalResolution; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The ImageDefinition horizontal resolution must be greater than zero.");
                }

                horizontalResolution = value;
            }
        }

        /// <summary>
        /// Gets or sets the image vertical resolution in pixels per unit.
        /// </summary>
        public double VerticalResolution
        {
            get { return verticalResolution; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The ImageDefinition vertical resolution must be greater than zero.");
                }

                verticalResolution = value;
            }
        }

        /// <summary>
        /// Gets or sets the image resolution units.
        /// </summary>
        public ImageResolutionUnits ResolutionUnits
        {
            get { return resolutionUnits; }
            set
            {
                if (resolutionUnits != value)
                {
                    switch (value)
                    {
                        case ImageResolutionUnits.Centimeters:
                            horizontalResolution /= 2.54;
                            verticalResolution /= 2.54;
                            break;
                        case ImageResolutionUnits.Inches:
                            horizontalResolution *= 2.54;
                            verticalResolution *= 2.54;
                            break;
                        case ImageResolutionUnits.Unitless:
                            break;
                    }
                }
                resolutionUnits = value;
            }
        }


        /// <summary>
        /// Gets the owner of the actual image definition.
        /// </summary>
        public new ImageDefinitions Owner
        {
            get { return (ImageDefinitions) base.Owner; }
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
            if (Owner == null)
            {
                return null;
            }

            return Owner.GetReferences(Name);
        }

        /// <summary>
        /// Creates a new ImageDefinition that is a copy of the current instance.
        /// </summary>
        /// <param name="newName">ImageDefinition name of the copy.</param>
        /// <returns>A new ImageDefinition that is a copy of this instance.</returns>
        public override TableObject Clone(string newName)
        {
            ImageDefinition copy = new ImageDefinition(newName, file, width, horizontalResolution, height, verticalResolution, resolutionUnits);

            foreach (XData data in XData.Values)
            {
                copy.XData.Add((XData)data.Clone());
            }

            return copy;
        }

        /// <summary>
        /// Creates a new ImageDefinition that is a copy of the current instance.
        /// </summary>
        /// <returns>A new ImageDefinition that is a copy of this instance.</returns>
        public override object Clone()
        {
            return Clone(Name);
        }

        #endregion
    }
}