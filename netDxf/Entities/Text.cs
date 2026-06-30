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
    /// Represents a Text <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Text :
        EntityObject
    {
        #region delegates and events

        public delegate void TextStyleChangedEventHandler(Text sender, TableObjectChangedEventArgs<TextStyle> e);
        public event TextStyleChangedEventHandler TextStyleChanged;
        protected virtual TextStyle OnTextStyleChangedEvent(TextStyle oldTextStyle, TextStyle newTextStyle)
        {
            TextStyleChangedEventHandler ae = TextStyleChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<TextStyle> eventArgs = new TableObjectChangedEventArgs<TextStyle>(oldTextStyle, newTextStyle);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newTextStyle;
        }

        #endregion

        #region private fields

        private TextAlignment alignment;
        private Vector3 position;
        private double obliqueAngle;
        private TextStyle style;
        private string text;
        private double height;
        private double widthFactor;
        private double width;
        private double rotation;
        private bool isBackward;
        private bool isUpsideDown;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Text</c> class.
        /// </summary>
        public Text()
            : this(string.Empty, Vector3.Zero, 1.0, TextStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Text</c> class.
        /// </summary>
        /// <param name="text">Text string.</param>
        public Text(string text)
            : this(text, Vector2.Zero, 1.0, TextStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Text</c> class.
        /// </summary>
        /// <param name="text">Text string.</param>
        /// <param name="position">Text <see cref="Vector2">position</see> in world coordinates.</param>
        /// <param name="height">Text height.</param>
        public Text(string text, Vector2 position, double height)
            : this(text, new Vector3(position.X, position.Y, 0.0), height, TextStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Text</c> class.
        /// </summary>
        /// <param name="text">Text string.</param>
        /// <param name="position">Text <see cref="Vector3">position</see> in world coordinates.</param>
        /// <param name="height">Text height.</param>
        public Text(string text, Vector3 position, double height)
            : this(text, position, height, TextStyle.Default)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <c>Text</c> class.
        /// </summary>
        /// <param name="text">Text string.</param>
        /// <param name="position">Text <see cref="Vector2">position</see> in world coordinates.</param>
        /// <param name="height">Text height.</param>
        /// <param name="style">Text <see cref="TextStyle">style</see>.</param>
        public Text(string text, Vector2 position, double height, TextStyle style)
            : this(text, new Vector3(position.X, position.Y, 0.0), height, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Text</c> class.
        /// </summary>
        /// <param name="text">Text string.</param>
        /// <param name="position">Text <see cref="Vector3">position</see> in world coordinates.</param>
        /// <param name="height">Text height.</param>
        /// <param name="style">Text <see cref="TextStyle">style</see>.</param>
        public Text(string text, Vector3 position, double height, TextStyle style)
            : base(EntityType.Text, DxfObjectCode.Text)
        {
            this.text = text;
            this.position = position;
            alignment = TextAlignment.BaselineLeft;
            Normal = Vector3.UnitZ;
            this.style = style ?? throw new ArgumentNullException(nameof(style));
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), this.text, "The Text height must be greater than zero.");
            }
            this.height = height;
            width = 1.0;
            widthFactor = style.WidthFactor;
            obliqueAngle = style.ObliqueAngle;
            rotation = 0.0;
            isBackward = false;
            isUpsideDown = false;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets if the text will be mirrored when a symmetry is performed, when the current Text entity does not belong to a DXF document.
        /// </summary>
        public static bool DefaultMirrText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Text <see cref="Vector3">position</see> in world coordinates.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets or sets the text rotation in degrees.
        /// </summary>
        public double Rotation
        {
            get { return rotation; }
            set { rotation = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the text height.
        /// </summary>
        /// <remarks>
        /// Valid values must be greater than zero. Default: 1.0.<br />
        /// When Alignment.Aligned is used this value is not applicable, it will be automatically adjusted so the text will fit in the specified width.
        /// </remarks>
        public double Height
        {
            get { return height; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The Text height must be greater than zero.");
                }
                height = value;
            }
        }

        /// <summary>
        /// Gets or sets the text width, only applicable for text Alignment.Fit and Alignment.Align.
        /// </summary>
        /// <remarks>Valid values must be greater than zero. Default: 1.0.</remarks>
        public double Width
        {
            get { return width; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The Text width must be greater than zero.");
                }
                width = value;
            }
        }

        /// <summary>
        /// Gets or sets the width factor.
        /// </summary>
        /// <remarks>
        /// Valid values range from 0.01 to 100. Default: 1.0.<br />
        /// When Alignment.Fit is used this value is not applicable, it will be automatically adjusted so the text will fit in the specified width.
        /// </remarks>
        public double WidthFactor
        {
            get { return widthFactor; }
            set
            {
                if (value < 0.01 || value > 100.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The Text width factor valid values range from 0.01 to 100.");
                }
                widthFactor = value;
            }
        }

        /// <summary>
        /// Gets or sets the font oblique angle.
        /// </summary>
        /// <remarks>Valid values range from -85 to 85. Default: 0.0.</remarks>
        public double ObliqueAngle
        {
            get { return obliqueAngle; }
            set
            {
                if (value < -85.0 || value > 85.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The Text oblique angle valid values range from -85 to 85.");
                }
                obliqueAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        public TextAlignment Alignment
        {
            get { return alignment; }
            set { alignment = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="TextStyle">text style</see>.
        /// </summary>
        public TextStyle Style
        {
            get { return style; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                style = OnTextStyleChangedEvent(style, value);
            }
        }

        /// <summary>
        /// Gets or sets the text string.
        /// </summary>
        public string Value
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Gets or sets if the text is backward (mirrored in X).
        /// </summary>
        public bool IsBackward
        {
            get { return isBackward; }
            set { isBackward = value; }
        }

        /// <summary>
        /// Gets or sets if the text is upside down (mirrored in Y).
        /// </summary>
        public bool IsUpsideDown
        {
            get { return isUpsideDown; }
            set { isUpsideDown = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Moves, scales, and/or rotates the current entity given a 3x3 transformation matrix and a translation vector.
        /// </summary>
        /// <param name="transformation">Transformation matrix.</param>
        /// <param name="translation">Translation vector.</param>
        /// <remarks>
        /// When the current Text entity does not belong to a DXF document, the text will use the DefaultMirrText when a symmetry is performed;
        /// otherwise, the drawing variable MirrText will be used.<br />
        /// A symmetry around the X axis when the text uses an Alignment.BaseLineLeft, Alignment.BaseLineCenter, Alignment.BaseLineRight, Alignment.Fit or an Alignment.Aligned.
        /// A symmetry around the Y axis when the text uses an Alignment.Fit or an Alignment.Aligned.<br />
        /// Matrix3 adopts the convention of using column vectors to represent a transformation matrix.
        /// </remarks>
        public override void TransformBy(Matrix3 transformation, Vector3 translation)
        {
            bool mirrText = Owner == null ? DefaultMirrText : Owner.Record.Owner.Owner.DrawingVariables.MirrText;

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
                    WidthFactor * Height * Vector2.UnitX,
                    new Vector2(Height * Math.Tan(ObliqueAngle * MathHelper.DegToRad), Height)
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

            if (mirrText)
            {
                if (Vector2.CrossProduct(newUvector, newVvector) < 0)
                {
                    newObliqueAngle = 90 - (newRotation - newObliqueAngle);
                    if(!(Alignment == TextAlignment.Fit || Alignment == TextAlignment.Aligned)) newRotation += 180;
                    IsBackward = !IsBackward;
                }
                else
                {
                    newObliqueAngle = 90 + (newRotation - newObliqueAngle);
                }              
            }
            else
            {
                if (Vector2.CrossProduct(newUvector, newVvector) < 0.0)
                {
                    newObliqueAngle = 90 - (newRotation - newObliqueAngle);

                    if (Vector2.DotProduct(newUvector, uv[0]) < 0.0)
                    {
                        newRotation += 180;

                        switch (Alignment)
                        {
                            case TextAlignment.TopLeft:
                                Alignment = TextAlignment.TopRight;
                                break;
                            case TextAlignment.TopRight:
                                Alignment = TextAlignment.TopLeft;
                                break;
                            case TextAlignment.MiddleLeft:
                                Alignment = TextAlignment.MiddleRight;
                                break;
                            case TextAlignment.MiddleRight:
                                Alignment = TextAlignment.MiddleLeft;
                                break;
                            case TextAlignment.BaselineLeft:
                                Alignment = TextAlignment.BaselineRight;
                                break;
                            case TextAlignment.BaselineRight:
                                Alignment = TextAlignment.BaselineLeft;
                                break;
                            case TextAlignment.BottomLeft:
                                Alignment = TextAlignment.BottomRight;
                                break;
                            case TextAlignment.BottomRight:
                                Alignment = TextAlignment.BottomLeft;
                                break;
                        }
                    }
                    else
                    {
                        switch (Alignment)
                        {
                            case TextAlignment.TopLeft:
                                Alignment = TextAlignment.BottomLeft;
                                break;
                            case TextAlignment.TopCenter:
                                Alignment = TextAlignment.BottomCenter;
                                break;
                            case TextAlignment.TopRight:
                                Alignment = TextAlignment.BottomRight;
                                break;
                            case TextAlignment.BottomLeft:
                                Alignment = TextAlignment.TopLeft;
                                break;
                            case TextAlignment.BottomCenter:
                                Alignment = TextAlignment.TopCenter;
                                break;
                            case TextAlignment.BottomRight:
                                Alignment = TextAlignment.TopRight;
                                break;
                        }
                    }
                }
                else
                {
                    newObliqueAngle = 90 + (newRotation - newObliqueAngle);
                }
            }

            // the oblique angle is defined between -85 and 85 degrees
            //if (newObliqueAngle >= 360) newObliqueAngle -= 360;
            newObliqueAngle = MathHelper.NormalizeAngle(newObliqueAngle);
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

            // the height must be greater than zero, the cos is always positive between -85 and 85
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
            Height = newHeight;
            WidthFactor = newWidthFactor;
            ObliqueAngle = newObliqueAngle;
        }

        /// <summary>
        /// Creates a new Text that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Text that is a copy of this instance.</returns>
        public override object Clone()
        {
            Text entity = new Text
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
                //Text properties
                Position = position,
                Rotation = rotation,
                Height = height,
                Width = width,
                WidthFactor = widthFactor,
                ObliqueAngle = obliqueAngle,
                Alignment = alignment,
                IsBackward = isBackward,
                isUpsideDown = isUpsideDown,
                Style = (TextStyle) style.Clone(),
                Value = text
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