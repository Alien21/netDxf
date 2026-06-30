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
using netDxf.Blocks;
using netDxf.Tables;

namespace netDxf.Collections
{
    /// <summary>
    /// Represents a collection of dimension styles.
    /// </summary>
    public sealed class DimensionStyles :
        TableObjects<DimensionStyle>
    {
        #region constructor

        internal DimensionStyles(DxfDocument document)
            : this(document, null)
        {
        }

        internal DimensionStyles(DxfDocument document, string handle)
            : base(document, DxfObjectCode.DimensionStyleTable, handle)
        {
        }

        #endregion

        #region override methods

        /// <summary>
        /// Adds a dimension style to the list.
        /// </summary>
        /// <param name="item"><see cref="DimensionStyle">DimensionStyle</see> to add to the list.</param>
        /// <param name="assignHandle">Specifies if a handle needs to be generated for the dimension style parameter.</param>
        /// <returns>
        /// If a dimension style already exists with the same name as the instance that is being added the method returns the existing dimension style,
        /// if not it will return the new dimension style.
        /// </returns>
        internal override DimensionStyle Add(DimensionStyle item, bool assignHandle)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (List.TryGetValue(item.Name, out DimensionStyle add))
            {
                return add;
            }

            if (assignHandle || string.IsNullOrEmpty(item.Handle))
            {
                Owner.NumHandles = item.AssignHandle(Owner.NumHandles);
            }

            List.Add(item.Name, item);
            References.Add(item.Name, new DxfObjectReferences());

            // add referenced text style
            item.TextStyle = Owner.TextStyles.Add(item.TextStyle, assignHandle);
            Owner.TextStyles.References[item.TextStyle.Name].Add(item);

            // add referenced blocks
            if (item.LeaderArrow != null)
            {
                item.LeaderArrow = Owner.Blocks.Add(item.LeaderArrow, assignHandle);
                Owner.Blocks.References[item.LeaderArrow.Name].Add(item);
            }
            if (item.DimArrow1 != null)
            {
                item.DimArrow1 = Owner.Blocks.Add(item.DimArrow1, assignHandle);
                Owner.Blocks.References[item.DimArrow1.Name].Add(item);
            }
            if (item.DimArrow2 != null)
            {
                item.DimArrow2 = Owner.Blocks.Add(item.DimArrow2, assignHandle);
                Owner.Blocks.References[item.DimArrow2.Name].Add(item);
            }

            // add referenced line types
            item.DimLineLinetype = Owner.Linetypes.Add(item.DimLineLinetype, assignHandle);
            Owner.Linetypes.References[item.DimLineLinetype.Name].Add(item);

            item.ExtLine1Linetype = Owner.Linetypes.Add(item.ExtLine1Linetype, assignHandle);
            Owner.Linetypes.References[item.ExtLine1Linetype.Name].Add(item);

            item.ExtLine2Linetype = Owner.Linetypes.Add(item.ExtLine2Linetype, assignHandle);
            Owner.Linetypes.References[item.ExtLine2Linetype.Name].Add(item);

            item.Owner = this;

            item.NameChanged += Item_NameChanged;
            item.LinetypeChanged += DimensionStyleLinetypeChanged;
            item.TextStyleChanged += DimensionStyleTextStyleChanged;
            item.BlockChanged += DimensionStyleBlockChanged;

            Owner.AddedObjects.Add(item.Handle, item);

            return item;
        }

        /// <summary>
        /// Removes a dimension style.
        /// </summary>
        /// <param name="name"><see cref="DimensionStyle">DimensionStyle</see> name to remove from the document.</param>
        /// <returns>True if the dimension style has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved dimension styles or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(string name)
        {
            return Remove(this[name]);
        }

        /// <summary>
        /// Removes a dimension style.
        /// </summary>
        /// <param name="item"><see cref="DimensionStyle">DimensionStyle</see> to remove from the document.</param>
        /// <returns>True if the dimension style has been successfully removed, or false otherwise.</returns>
        /// <remarks>Reserved dimension styles or any other referenced by objects cannot be removed.</remarks>
        public override bool Remove(DimensionStyle item)
        {
            if (item == null)
            {
                return false;
            }

            if (!Contains(item))
            {
                return false;
            }

            if (item.IsReserved)
            {
                return false;
            }

            if (HasReferences(item))
            {
                return false;
            }

            Owner.AddedObjects.Remove(item.Handle);

            // remove referenced text style
            Owner.TextStyles.References[item.TextStyle.Name].Remove(item);

            // remove referenced blocks
            if (item.LeaderArrow != null)
            {
                Owner.Blocks.References[item.LeaderArrow.Name].Remove(item);
            }
            if (item.DimArrow1 != null)
            {
                Owner.Blocks.References[item.DimArrow1.Name].Remove(item);
            }

            if (item.DimArrow2 != null)
            {
                Owner.Blocks.References[item.DimArrow2.Name].Remove(item);
            }

            // remove referenced line types
            Owner.Linetypes.References[item.DimLineLinetype.Name].Remove(item);
            Owner.Linetypes.References[item.ExtLine1Linetype.Name].Remove(item);
            Owner.Linetypes.References[item.ExtLine2Linetype.Name].Remove(item);

            References.Remove(item.Name);
            List.Remove(item.Name);

            item.Handle = null;
            item.Owner = null;

            item.NameChanged -= Item_NameChanged;
            item.LinetypeChanged -= DimensionStyleLinetypeChanged;
            item.TextStyleChanged -= DimensionStyleTextStyleChanged;
            item.BlockChanged -= DimensionStyleBlockChanged;

            return true;
        }

        #endregion

        #region TableObject events

        private void Item_NameChanged(TableObject sender, TableObjectChangedEventArgs<string> e)
        {
            if (Contains(e.NewValue))
            {
                throw new ArgumentException("There is already another dimension style with the same name.");
            }

            List.Remove(sender.Name);
            List.Add(e.NewValue, (DimensionStyle) sender);

            List<DxfObjectReference> refs = GetReferences(sender.Name);
            References.Remove(sender.Name);
            References.Add(e.NewValue, new DxfObjectReferences());
            References[e.NewValue].Add(refs);
        }

        private void DimensionStyleLinetypeChanged(TableObject sender, TableObjectChangedEventArgs<Linetype> e)
        {
            Owner.Linetypes.References[e.OldValue.Name].Remove(sender);
            e.NewValue = Owner.Linetypes.Add(e.NewValue);
            Owner.Linetypes.References[e.NewValue.Name].Add(sender);
        }

        private void DimensionStyleTextStyleChanged(TableObject sender, TableObjectChangedEventArgs<TextStyle> e)
        {
            Owner.TextStyles.References[e.OldValue.Name].Remove(sender);

            e.NewValue = Owner.TextStyles.Add(e.NewValue);
            Owner.TextStyles.References[e.NewValue.Name].Add(sender);
        }

        private void DimensionStyleBlockChanged(TableObject sender, TableObjectChangedEventArgs<Block> e)
        {
            if (e.OldValue != null)
            {
                Owner.Blocks.References[e.OldValue.Name].Remove(sender);
            }

            e.NewValue = Owner.Blocks.Add(e.NewValue);
            if (e.NewValue != null)
            {
                Owner.Blocks.References[e.NewValue.Name].Add(sender);
            }
        }

        #endregion
    }
}