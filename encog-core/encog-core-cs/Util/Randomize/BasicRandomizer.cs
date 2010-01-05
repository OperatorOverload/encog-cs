﻿// Encog Artificial Intelligence Framework v2.x
// DotNet Version
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2009-2010, Heaton Research Inc., and individual contributors.
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
using Encog.Neural.Networks;
using Encog.Neural.Networks.Synapse;
using Encog.Neural.Networks.Layers;
#if logging
using log4net;
#endif
namespace Encog.Util.Randomize
{
    /// <summary>
    /// Provides basic functionality that most randomizers will need.
    /// </summary>
    public abstract class BasicRandomizer : IRandomizer
    {
#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(BasicRandomizer));
#endif
        /// <summary>
        /// Randomize the synapses and thresholds in the basic network based on an
        /// array, modify the array. Previous values may be used, or they may be
        /// discarded, depending on the randomizer.
        /// </summary>
        /// <param name="network">A network to randomize.</param>
        public virtual void Randomize(BasicNetwork network)
        {

            // randomize the weight matrix
            foreach (ISynapse synapse in network.Structure.Synapses)
            {
                if (synapse.WeightMatrix != null)
                {
                    Randomize(synapse.WeightMatrix);
                }
            }

            // randomize the thresholds
            foreach (ILayer layer in network.Structure.Layers)
            {
                if (layer.HasThreshold)
                {
                    Randomize(layer.Threshold);
                }
            }
        }

        /// <summary>
        /// Randomize the array based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        /// <param name="d">An array to randomize.</param>
        public virtual void Randomize(double[] d)
        {
            for (int i = 0; i < d.Length; i++)
            {
                d[i] = Randomize(d[i]);
            }

        }
          


        /// <summary>
        /// Randomize the 2d array based on an array, modify the array. Previous 
        /// values may be used, or they may be discarded, depending on the 
        /// randomizer.
        /// </summary>
        /// <param name="d">An array to randomize.</param>
        public virtual void Randomize(double[][] d)
        {
            for (int r = 0; r < d.Length; r++)
            {
                for (int c = 0; c < d[0].Length; c++)
                {
                    d[r][c] = Randomize(d[r][c]);
                }
            }

        }

        /// <summary>
        /// Randomize the matrix based on an array, modify the array. Previous values
        /// may be used, or they may be discarded, depending on the randomizer.
        /// </summary>
        /// <param name="m">A matrix to randomize.</param>
        public virtual void Randomize(Matrix.Matrix m)
        {
            double[][] mData = m.Data;
            for (int r = 0; r < m.Rows; r++)
            {
                for (int c = 0; c < m.Cols; c++)
                {
                    mData[r][c] = Randomize(mData[r][c]);
                }
            }
        }

        /// <summary>
        /// Starting with the specified number, randomize it to the degree specified
        /// by this randomizer. This could be a totally new random number, or it
        /// could be based on the specified number.
        /// </summary>
        /// <param name="d">The number to randomize.</param>
        /// <returns>A randomized number.</returns>
        abstract public double Randomize(double d);

    }

}
