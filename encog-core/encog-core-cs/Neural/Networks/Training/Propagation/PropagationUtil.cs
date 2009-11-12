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
using Encog.Neural.Data;

#if logging
using log4net;
using Encog.Neural.Networks.Layers;
using Encog.Util.Logging;
using Encog.Neural.Networks.Synapse;
#endif

namespace Encog.Neural.Networks.Training.Propagation
{
    /// <summary>
    /// Implements basic functionality that is needed by each of the propagation
    /// methods. The specifics of each of the propagation methods is implemented
    /// inside of the PropagationMethod interface implementors.
    /// </summary>
    public class PropagationUtil
    {
        /// <summary>
        /// The network that is being trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// The output from the last iteration.
        /// </summary>
        private INeuralData fire;

        /// <summary>
        /// What sort of propagation method is being used.
        /// </summary>
        private IPropagationMethod method;

        /// <summary>
        /// All of the levels used in training. The layers are grouped into levels.
        /// </summary>
        private IList<PropagationLevel> levels =
            new List<PropagationLevel>();

        /// <summary>
        /// The output holder to use during training.
        /// </summary>
        private NeuralOutputHolder outputHolder = new NeuralOutputHolder();

#if logging
        /// <summary>
        /// The logging object.
        /// </summary>
        private readonly ILog logger = LogManager.GetLogger(typeof(PropagationUtil));
#endif

        /// <summary>
        /// Construct a propagation utility.
        /// </summary>
        /// <param name="network">The network to train.</param>
        /// <param name="method">The method to use.</param>
        public PropagationUtil(BasicNetwork network,
                 IPropagationMethod method)
        {
            this.network = network;
            this.method = method;
            Construct();
            this.method.Init(this);

        }

        /// <summary>
        /// Calculate the error for the recognition just done.
        /// </summary>
        /// <param name="ideal">What the output neurons should have yielded.</param>
        public void BackwardPass(INeuralData ideal)
        {
            ILayer output = this.network.GetLayer(BasicNetwork.TAG_OUTPUT);

            // make sure that the input is of the correct size
            if (ideal.Count != output.NeuronCount)
            {

                String str = "Size mismatch: Can't calcError for "
                       + "ideal input size="
                       + ideal.Count
                       + " for output layer size="
                       + output.NeuronCount;
#if logging
                if (this.logger.IsErrorEnabled)
                {
                    this.logger.Error(str);
                }
#endif
                throw new NeuralNetworkError(str);
            }
#if logging
            // log that we are performing a backward pass
            if (this.logger.IsDebugEnabled)
            {
                this.logger.Debug("Backpropagation backward pass");
            }
#endif
            // calculate the initial deltas from the output layer
            CalculateInitialDeltas(this.fire, ideal);

            // now work these errors backward through the neural network
            for (int i = 0; i < this.levels.Count - 1; i++)
            {
                PropagationLevel fromLevel = this.levels[i + 1];
                PropagationLevel toLevel = this.levels[i];
                this.method.CalculateError(this.outputHolder, fromLevel, toLevel);
            }
        }

        /// <summary>
        /// Calculate the error for the given ideal values.
        /// </summary>
        /// <param name="actual">The actual output from this level.</param>
        /// <param name="ideal">Ideal output values.</param>
        /// <returns>Return the initial propagation level.</returns>
        private PropagationLevel CalculateInitialDeltas(
                 INeuralData actual,
                 INeuralData ideal)
        {

            // get the output layer
            ILayer outputLayer = this.network.GetLayer(BasicNetwork.TAG_OUTPUT);

            // construct the level
            PropagationLevel level = this.levels[0];

            // obtain the output for each output layer neuron
            for (int i = 0; i < outputLayer.NeuronCount; i++)
            {
                level.Deltas[i] = actual[i];
            }

            // take the derivative of these outputs
            outputLayer.ActivationFunction.DerivativeFunction(
                    level.Deltas);

            // multiply by the difference between the actual and idea
            for (int i = 0; i < outputLayer.NeuronCount; i++)
            {
                level.Deltas[i] = level.Deltas[i]
                        * (ideal[i] - actual[i]);
            }

#if logging
            if (this.logger.IsDebugEnabled)
            {
                this.logger.Debug("Initial deltas: "
                        + DumpMatrix.DumpArray(level.Deltas));
            }
#endif
            return level;
        }

        /// <summary>
        /// Begin constructing the level from the layers.
        /// </summary>
        private void Construct()
        {
            // get the output layer
            ILayer outputLayer = this.network.GetLayer(BasicNetwork.TAG_OUTPUT);

            // construct the level
            PropagationLevel level = new PropagationLevel(this, outputLayer);
            this.levels.Add(level);

            // construct the other levels
            Construct(level);
        }

        /// <summary>
        /// Construct one propagation level.
        /// </summary>
        /// <param name="current">The current level being constructed.</param>
        private void Construct(PropagationLevel current)
        {
            IList<ISynapse> previousSynapses = current
                   .DeterminePreviousSynapses();

            // are there more levels to move onto?
            if (previousSynapses.Count > 0)
            {
                PropagationLevel prevLevel = new PropagationLevel(this,
                       previousSynapses);
                this.levels.Add(prevLevel);

                // continue backward
                Construct(prevLevel);
            }
        }

        /// <summary>
        /// Perform the forward pass and determine each layer's output.
        /// </summary>
        /// <param name="input">The input to the neural network.</param>
        /// <returns>The output from the neural network.</returns>
        public INeuralData ForwardPass(INeuralData input)
        {
#if logging
            if (this.logger.IsDebugEnabled)
            {
                this.logger.Debug("Backpropagation forward pass");
            }
#endif
            this.outputHolder.Result.Clear();
            this.fire = this.network.Compute(input, this.outputHolder);
            return this.fire;
        }

        /// <summary>
        /// The propagation levels.
        /// </summary>
        public IList<PropagationLevel> Levels
        {
            get
            {
                return this.levels;
            }
        }

        /// <summary>
        /// Get the current best neural network.
        /// </summary>
        public BasicNetwork Network
        {
            get
            {
                return this.network;
            }
        }

        /// <summary>
        /// The output holder being used.
        /// </summary>
        public NeuralOutputHolder OutputHolder
        {
            get
            {
                return this.outputHolder;
            }
        }

        /// <summary>
        /// The propagation method in use.
        /// </summary>
        public IPropagationMethod Method
        {
            get
            {
                return this.method;
            }
        }

    }
}
