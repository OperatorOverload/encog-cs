﻿// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009, Heaton Research Inc., and individual contributors.
// See the copyright.txt in the distribution for a full listing of 
// individual contributors.
//
// This is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation; either version 2.1 of
// the License, or (at your option) any later version.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this software; if not, write to the Free
// Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
// 02110-1301 USA, or see the FSF site: http://www.fsf.org.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encog.Normalize.Output
{
    /// <summary>
    /// Output fields can be grouped together if they are calculated together.
    /// This interface defines how a field group works.
    /// </summary>
    public interface IOutputFieldGroup
    {
        /// <summary>
        /// Add an output field to the group.
        /// </summary>
        /// <param name="field">The field to add.</param>
        void AddField(OutputFieldGrouped field);

        /// <summary>
        /// All of the output fields in this group.
        /// </summary>
        ICollection<OutputFieldGrouped> GroupedFields { get; }

        /// <summary>
        /// Init the group for a new row.
        /// </summary>
        void RowInit();
    }
}
