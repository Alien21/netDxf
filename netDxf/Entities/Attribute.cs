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
    /// Represents an attribute.
    /// </summary>
    /// <remarks>
    /// The attribute position, rotation, height and width factor values also includes the transformation of the <see cref="Insert">Insert</see> entity to which it belongs.<br />
    /// During the attribute initialization a copy of all attribute definition properties will be copied,
    /// so any changes made to the attribute definition will only be applied to new attribute instances and not to existing ones.
    /// This behavior is to allow imported <see cref="Insert">Insert</see> entities to have attributes without definition in the block, 
    /// although this might sound not totally correct it is allowed by AutoCad.
    /// </remarks>
    public class Attribute :
        DxfObject,
        ICloneable
    {
        #region delegates and events

        public delegate void LayerChangedEventHandler(Attribute sender, TableObjectChangedEventArgs<Layer> e);

        public event LayerChangedEventHandler LayerChanged;

        protected virtual Layer OnLayerChangedEvent(Layer oldLayer, Layer newLayer)
        {
            LayerChangedEventHandler ae = LayerChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<Layer> eventArgs = new TableObjectChangedEventArgs<Layer>(oldLayer, newLayer);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newLayer;
        }

        public delegate void LinetypeChangedEventHandler(Attribute sender, TableObjectChangedEventArgs<Linetype> e);

        public event LinetypeChangedEventHandler LinetypeChanged;

        protected virtual Linetype OnLinetypeChangedEvent(Linetype oldLinetype, Linetype newLinetype)
        {
            LinetypeChangedEventHandler ae = LinetypeChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<Linetype> eventArgs = new TableObjectChangedEventArgs<Linetype>(oldLinetype, newLinetype);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newLinetype;
        }

        public delegate void TextStyleChangedEventHandler(Attribute sender, TableObjectChangedEventArgs<TextStyle> e);

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

        private AciColor color;
        private Layer layer;
        private Linetype linetype;
        private Lineweight lineweight;
        private Transparency transparency;
        private double linetypeScale;
        private bool isVisible;
        private Vector3 normal;

        private AttributeDefinition definition;
        private readonly string tag;
        private string attValue;
        private TextStyle style;
        private Vector3 position;
        private AttributeFlags flags;
        private double height;
        private double widthFactor;
        private double width;
        private double obliqueAngle;
        private double rotation;
        private TextAlignment alignment;
        private bool isBackward;
        private bool isUpsideDown;

        #endregion

        #region constructor

        internal Attribute(string tag)
            : base(DxfObjectCode.Attribute)
        {
            this.tag = string.IsNullOrEmpty(tag) ? string.Empty : tag;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Attribute</c> class.
        /// </summary>
        /// <param name="definition"><see cref="AttributeDefinition">Attribute definition</see>.</param>
        public Attribute(AttributeDefinition definition)
            : base(DxfObjectCode.Attribute)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            color = definition.Color;
            layer = definition.Layer;
            linetype = definition.Linetype;
            lineweight = definition.Lineweight;
            linetypeScale = definition.LinetypeScale;
            transparency = definition.Transparency;
            isVisible = definition.IsVisible;
            normal = definition.Normal;

            this.definition = definition;
            tag = definition.Tag;
            attValue = definition.Value;
            style = definition.Style;
            position = definition.Position;
            flags = definition.Flags;
            height = definition.Height;
            width = definition.Width;
            widthFactor = definition.WidthFactor;
            obliqueAngle = definition.ObliqueAngle;
            rotation = definition.Rotation;
            alignment = definition.Alignment;
            isBackward = definition.IsBackward;
            isUpsideDown = definition.IsUpsideDown;
        }

        #endregion

        #region public property

        /// <summary>
        /// Gets or sets the entity <see cref="AciColor">color</see>.
        /// </summary>
        public AciColor Color
        {
            get { return color; }
            set
            {
                color = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="Layer">layer</see>.
        /// </summary>
        public Layer Layer
        {
            get { return layer; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                layer = OnLayerChangedEvent(layer, value);
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="Linetype">line type</see>.
        /// </summary>
        public Linetype Linetype
        {
            get { return linetype; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                linetype = OnLinetypeChangedEvent(linetype, value);
            }
        }

        /// <summary>
        /// Gets or sets the entity line weight, one unit is always 1/100 mm (default = ByLayer).
        /// </summary>
        public Lineweight Lineweight
        {
            get { return lineweight; }
            set { lineweight = value; }
        }

        /// <summary>
        /// Gets or sets layer transparency (default: ByLayer).
        /// </summary>
        public Transparency Transparency
        {
            get { return transparency; }
            set
            {
                transparency = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Gets or sets the entity line type scale.
        /// </summary>
        public double LinetypeScale
        {
            get { return linetypeScale; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The line type scale must be greater than zero.");
                }
                linetypeScale = value;
            }
        }

        /// <summary>
        /// Gets or set the entity visibility.
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="Vector3">normal</see>.
        /// </summary>
        public Vector3 Normal
        {
            get { return normal; }
            set
            {
                normal = Vector3.Normalize(value);
                if (Vector3.IsZero(normal))
                {
                    throw new ArgumentException("The normal can not be the zero vector.", nameof(value));
                }
            }
        }

        /// <summary>
        /// Gets the owner of the actual DXF object.
        /// </summary>
        public new Insert Owner
        {
            get { return (Insert) base.Owner; }
            internal set { base.Owner = value; }
        }

        /// <summary>
        /// Gets the attribute definition.
        /// </summary>
        /// <remarks>If the insert attribute has no definition it will return null.</remarks>
        public AttributeDefinition Definition
        {
            get { return definition; }
            internal set { definition = value; }
        }

        /// <summary>
        /// Gets the attribute tag.
        /// </summary>
        public string Tag
        {
            get { return tag; }
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
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The height should be greater than zero.");
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
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The width factor should be greater than zero.");
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
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The oblique angle valid values range from -85 to 85.");
                }
                obliqueAngle = value;
            }
        }

        /// <summary>
        /// Gets or sets the attribute value.
        /// </summary>
        public string Value
        {
            get { return attValue; }
            set { attValue = string.IsNullOrEmpty(value) ? string.Empty : value; }
        }

        /// <summary>
        /// Gets or sets the attribute text style.
        /// </summary>
        /// <remarks>
        /// The <see cref="TextStyle">text style</see> defines the basic properties of the information text.
        /// </remarks>
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
        /// Gets or sets the attribute <see cref="Vector3">position</see>.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Gets or sets the attribute flags.
        /// </summary>
        public AttributeFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        /// <summary>
        /// Gets or sets the attribute text rotation in degrees.
        /// </summary>
        public double Rotation
        {
            get { return rotation; }
            set { rotation = MathHelper.NormalizeAngle(value); }
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
        /// Gets or sets if the attribute text is backward (mirrored in X).
        /// </summary>
        public bool IsBackward
        {
            get { return isBackward; }
            set { isBackward = value; }
        }

        /// <summary>
        /// Gets or sets if the attribute text is upside down (mirrored in Y).
        /// </summary>
        public bool IsUpsideDown
        {
            get { return isUpsideDown; }
            set { isUpsideDown = value; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Moves, scales, and/or rotates the current attribute given a 3x3 transformation matrix and a translation vector.
        /// </summary>
        /// <param name="transformation">Transformation matrix.</param>
        /// <param name="translation">Translation vector.</param>
        /// <remarks>Matrix3 adopts the convention of using column vectors to represent a transformation matrix.</remarks>
        public void TransformBy(Matrix3 transformation, Vector3 translation)
        {
            bool mirrText;
            if (Owner == null)
            {
                mirrText = Text.DefaultMirrText;
            }
            else if (Owner.Owner == null)
            {
                mirrText = Text.DefaultMirrText;
            }
            else
            {
                mirrText = Owner.Owner.Record.Owner.Owner.DrawingVariables.MirrText;
            }

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
                if (Vector2.CrossProduct(newUvector, newVvector) < 0.0)
                {
                    newObliqueAngle = 90 - (newRotation - newObliqueAngle);
                    if (!(Alignment == TextAlignment.Fit || Alignment == TextAlignment.Aligned))
                    {
                        newRotation += 180;
                    }
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
        /// Moves, scales, and/or rotates the current entity given a 4x4 transformation matrix.
        /// </summary>
        /// <param name="transformation">Transformation matrix.</param>
        /// <remarks>Matrix4 adopts the convention of using column vectors to represent a transformation matrix.</remarks>
        public void TransformBy(Matrix4 transformation)
        {
            Matrix3 m = new Matrix3(transformation.M11, transformation.M12, transformation.M13,
                transformation.M21, transformation.M22, transformation.M23,
                transformation.M31, transformation.M32, transformation.M33);
            Vector3 v = new Vector3(transformation.M14, transformation.M24, transformation.M34);

            TransformBy(m, v);
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Attribute that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Attribute that is a copy of this instance.</returns>
        public object Clone()
        {
            Attribute entity = new Attribute(tag)
            {
                //Attribute properties
                Layer = (Layer) Layer.Clone(),
                Linetype = (Linetype) Linetype.Clone(),
                Color = (AciColor) Color.Clone(),
                Lineweight = Lineweight,
                Transparency = (Transparency) Transparency.Clone(),
                LinetypeScale = LinetypeScale,
                Normal = Normal,
                IsVisible = isVisible,
                Definition = (AttributeDefinition) definition?.Clone(),
                Height = height,
                Width = width,
                WidthFactor = widthFactor,
                ObliqueAngle = obliqueAngle,
                Value = attValue,
                Style = (TextStyle) style.Clone(),
                Position = position,
                Flags = flags,
                Rotation = rotation,
                Alignment = alignment,
                IsBackward = isBackward,
                IsUpsideDown = isUpsideDown
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