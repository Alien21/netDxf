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

namespace netDxf.Objects
{
    /// <summary>
    /// Represents the state of the properties of a layer.
    /// </summary>
    public class LayerStateProperties :
        ICloneable
    {
        #region private fields

        private readonly string name;
        private LayerPropertiesFlags flags;
        private string linetype;
        private AciColor color;
        private Lineweight lineweight;
        private Transparency transparency;
        //private string plotStyle;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <c>LayerStateProperties</c> class.
        /// </summary>
        /// <param name="name">Name of the layer state properties.</param>
        public LayerStateProperties(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            this.name = name;
            flags = LayerPropertiesFlags.Plot;
            linetype = Linetype.DefaultName;
            color = AciColor.Default;
            lineweight = Lineweight.Default;
            transparency = new Transparency(0);
            //this.plotStyle = "Color_7";
        }

        /// <summary>
        /// Initializes a new instance of the <c>LayerStateProperties</c> class.
        /// </summary>
        /// <param name="layer">Layer from which copy the properties.</param>
        public LayerStateProperties(Layer layer)
        {
            name = layer.Name;
            if (!layer.IsVisible) flags |= LayerPropertiesFlags.Hidden;
            if (layer.IsFrozen) flags |= LayerPropertiesFlags.Frozen;
            if (layer.IsLocked) flags |= LayerPropertiesFlags.Locked;
            if (layer.Plot) flags |= LayerPropertiesFlags.Plot;
            linetype = layer.Linetype.Name;
            color = (AciColor) layer.Color.Clone();
            lineweight = layer.Lineweight;
            transparency = (Transparency) layer.Transparency.Clone();
            //this.plotStyle = "Color_" + layer.Color.Index;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the layer properties name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Layer properties flags.
        /// </summary>
        public LayerPropertiesFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        /// <summary>
        /// Layer properties linetype name.
        /// </summary>
        public string LinetypeName
        {
            get { return linetype; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                linetype = value;
            }
        }

        /// <summary>
        /// Layer properties color.
        /// </summary>
        public AciColor Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// Layer properties lineweight.
        /// </summary>
        public Lineweight Lineweight
        {
            get { return lineweight; }
            set { lineweight = value; }
        }

        /// <summary>
        /// Layer properties transparency.
        /// </summary>
        public Transparency Transparency
        {
            get { return transparency; }
            set { transparency = value; }
        }

        ///// <summary>
        ///// Layer properties plot style name.
        ///// </summary>
        //public string PlotStyleName
        //{
        //    get { return this.plotStyle; }
        //    set { this.plotStyle = value; }
        //}

        #endregion

        #region public methods

        /// <summary>
        /// Copy the layer to the current layer state properties.
        /// </summary>
        /// <param name="layer">Layer from which copy the properties.</param>
        /// <param name="options">Layer properties to copy.</param>
        public void CopyFrom(Layer layer, LayerPropertiesRestoreFlags options)
        {
            if (!string.Equals(name, layer.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Only a layer with the same name can be copied.", nameof(layer));
            }

            flags = LayerPropertiesFlags.None;

            if (options.HasFlag(LayerPropertiesRestoreFlags.Hidden))
            {
                if (!layer.IsVisible) flags |= LayerPropertiesFlags.Hidden;
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Frozen))
            {
                if (layer.IsFrozen) flags |= LayerPropertiesFlags.Frozen;
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Locked))
            {
                if (layer.IsLocked) flags |= LayerPropertiesFlags.Locked;
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Plot))
            {
                if (layer.Plot) flags |= LayerPropertiesFlags.Plot;
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Linetype))
            {
                linetype = layer.Linetype.Name;
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Color))
            {
                color = (AciColor) layer.Color.Clone();
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Lineweight))
            {
                lineweight = layer.Lineweight;
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Transparency))
            {
                transparency = (Transparency) layer.Transparency.Clone();
            }
        }

        /// <summary>
        /// Copy the current layer state properties to a layer.
        /// </summary>
        /// <param name="layer">Layer to which copy the properties.</param>
        /// <param name="options">Layer properties to copy.</param>
        public void CopyTo(Layer layer, LayerPropertiesRestoreFlags options)
        {
            if (!string.Equals(name, layer.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Only a layer with the same name can be copied.", nameof(layer));
            }

            if(options.HasFlag(LayerPropertiesRestoreFlags.Hidden))
            {
                layer.IsVisible = !flags.HasFlag(LayerPropertiesFlags.Hidden);
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Frozen))
            {
                layer.IsFrozen = flags.HasFlag(LayerPropertiesFlags.Frozen);
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Locked))
            {
                layer.IsLocked = flags.HasFlag(LayerPropertiesFlags.Locked);
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Plot))
            {
                layer.Plot = flags.HasFlag(LayerPropertiesFlags.Plot);
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Linetype))
            {
                Linetype line = null;
                if (layer.Owner != null)
                {
                    DxfDocument doc = layer.Owner.Owner;
                    line = doc.Linetypes[LinetypeName];
                }
                layer.Linetype = line ?? new Linetype(LinetypeName);
                
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Color))
            {
                layer.Color = (AciColor) Color.Clone();
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Lineweight))
            {
                layer.Lineweight = Lineweight;
            }
            if (options.HasFlag(LayerPropertiesRestoreFlags.Transparency))
            {
                layer.Transparency = (Transparency) Transparency.Clone();
            }
        }

        /// <summary>
        /// Compares the stored properties with the specified layer.
        /// </summary>
        /// <param name="layer">Layer to compare with.</param>
        /// <returns>If the stored properties are the same as the specified layer it returns true, false otherwise.</returns>
        public bool CompareWith(Layer layer)
        {
            if (!string.Equals(layer.Name, name, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (layer.IsVisible != !flags.HasFlag(LayerPropertiesFlags.Hidden))
            {
                return false;
            }

            if (layer.IsFrozen != flags.HasFlag(LayerPropertiesFlags.Frozen))
            {
                return false;
            }

            if (layer.IsLocked != flags.HasFlag(LayerPropertiesFlags.Locked))
            {
                return false;
            }

            if (layer.Plot != flags.HasFlag(LayerPropertiesFlags.Plot))
            {
                return false;
            }

            if (!string.Equals(layer.Linetype.Name, LinetypeName, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (!layer.Color.Equals(color))
            {
                return false;
            }

            if (layer.Lineweight != lineweight)
            {
                return false;
            }

            if (!layer.Transparency.Equals(transparency))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            return new LayerStateProperties(name)
            {
                Flags = flags,
                LinetypeName = linetype,
                Color = (AciColor) color.Clone(),
                Lineweight = lineweight,
                Transparency = (Transparency) transparency.Clone(),
                //PlotStyleName = this.plotStyle
            };
        }

        #endregion

    }
}