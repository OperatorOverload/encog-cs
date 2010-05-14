﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encog.Neural.Data;
using Encog.Util;
using Encog.Neural.Data.Basic;
using Encog.Neural.NeuralData;
using Encog.Neural.Activation;
using Encog.Neural.Networks.Synapse;

namespace Encog.Neural.Networks.Training.LMA
{
    /// <summary>
    /// Compute the Jaccobian using the chain rule.
    /// </summary>
    public class JacobianChainRule : IComputeJacobian
    {
        /// <summary>
        /// The network that is to be trained.
        /// </summary>
        private BasicNetwork network;

        /// <summary>
        /// THe training set to use. Must be indexable.
        /// </summary>
        private IIndexable indexableTraining;

        /// <summary>
        /// The number of training set elements.
        /// </summary>
        private int inputLength;

        /// <summary>
        /// The number of weights and weights in the neural network.
        /// </summary>
        private int parameterSize;

        /// <summary>
        /// The Jacobian matrix that was calculated.
        /// </summary>
        private double[][] jacobian;

        /// <summary>
        /// The current row in the Jacobian matrix.
        /// </summary>
        private int jacobianRow;

        /// <summary>
        /// The current column in the Jacobian matrix.
        /// </summary>
        private int jacobianCol;

        /// <summary>
        /// Used to read the training data.
        /// </summary>
        private INeuralDataPair pair;

        /// <summary>
        /// The errors for each row in the Jacobian.
        /// </summary>
        private double[] rowErrors;

        /// <summary>
        /// The current error.
        /// </summary>
        private double error;

        /// <summary>
        /// Construct the chain rule calculation. 
        /// </summary>
        /// <param name="network">The network to use.</param>
        /// <param name="indexableTraining">The training set to use.</param>
        public JacobianChainRule(BasicNetwork network,
                IIndexable indexableTraining)
        {
            this.indexableTraining = indexableTraining;
            this.network = network;
            this.parameterSize = network.Structure.CalculateSize();
            this.inputLength = (int)this.indexableTraining.Count;
            this.jacobian = EncogArray.AllocateDouble2D(this.inputLength, this.parameterSize);
            this.rowErrors = new double[this.inputLength];

            BasicNeuralData input = new BasicNeuralData(
                    this.indexableTraining.InputSize);
            BasicNeuralData ideal = new BasicNeuralData(
                    this.indexableTraining.IdealSize);
            this.pair = new BasicNeuralDataPair(input, ideal);
        }

        
        /// <summary>
        /// Calculate the derivative. 
        /// </summary>
        /// <param name="a">The value to calculate for.</param>
        /// <param name="d">The derivative.</param>
        /// <returns></returns>
        private double CalcDerivative(IActivationFunction a, double d)
        {
            double[] temp = new double[1];
            temp[0] = d;
            a.ActivationFunction(temp);
            a.DerivativeFunction(temp);
            return temp[0];
        }


        /// <summary>
        /// Calculate the Jacobian matrix.
        /// </summary>
        /// <param name="weights">The weights for the neural network.</param>
        /// <returns>The sum squared of the weights.</returns>
        public double Calculate(double[] weights)
        {
            double result = 0.0;

            for (int i = 0; i < this.inputLength; i++)
            {
                this.jacobianRow = i;
                this.jacobianCol = 0;

                this.indexableTraining.GetRecord(i, this.pair);

                double e = CalculateDerivatives(this.pair);
                this.rowErrors[i] = e;
                result += e * e;

            }

            return result / 2.0;
        }

        /// <summary>
        /// Calculate the derivatives for this training set element. 
        /// </summary>
        /// <param name="pair">The training set element.</param>
        /// <returns>The sum squared of errors.</returns>
        private double CalculateDerivatives(INeuralDataPair pair)
        {
            // error values
            double e = 0.0;
            double sum = 0.0;

            IActivationFunction function = this.network.GetLayer(
                    BasicNetwork.TAG_INPUT).ActivationFunction;

            NeuralOutputHolder holder = new NeuralOutputHolder();
            this.network.Compute(pair.Input, holder);

            IList<ISynapse> synapses = this.network.Structure
                    .Synapses;

            int synapseNumber = 0;

            ISynapse synapse = synapses[synapseNumber++];

            double output = holder.Output[0];
            e = pair.Ideal[0] - output;

            this.jacobian[this.jacobianRow][this.jacobianCol++] = CalcDerivative(
                    function, output);

            for (int i = 0; i < synapse.FromNeuronCount; i++)
            {
                double lastOutput = holder.Result[synapse][i];

                this.jacobian[this.jacobianRow][this.jacobianCol++] = CalcDerivative(
                        function, lastOutput)
                        * lastOutput;
            }

            while (synapseNumber < synapses.Count)
            {
                synapse = synapses[synapseNumber++];
                INeuralData outputData = holder.Result[synapse];

                // for each neuron in the input layer
                for (int neuron = 0; neuron < synapse.FromNeuronCount; neuron++)
                {
                    output = outputData[neuron];

                    int biasCol = this.jacobianCol++;

                    // for each weight of the input neuron
                    for (int i = 0; i < synapse.FromNeuronCount; i++)
                    {
                        sum = 0.0;
                        // for each neuron in the next layer
                        for (int j = 0; j < synapse.ToNeuronCount; j++)
                        {
                            // for each weight of the next neuron
                            for (int k = 0; k < synapse.FromNeuronCount; k++)
                            {
                                sum += synapse.WeightMatrix[k, j]
                                        * outputData[k];
                            }
                            sum += synapse.ToLayer.BiasWeights[j];
                        }

                        double w = synapse.WeightMatrix[neuron, i];
                        double val = CalcDerivative(function, output)
                                * CalcDerivative(function, sum) * w;

                        this.jacobian[this.jacobianRow][this.jacobianCol++] = val
                                * holder.Result[synapse][i];
                        this.jacobian[this.jacobianRow][biasCol] = val;
                    }
                }
            }

            // return error
            return e;
        }

        /// <summary>
        /// The sum squared errors.
        /// </summary>
        public double Error
        {
            get
            {
                return this.error;
            }
        }

        /// <summary>
        /// The Jacobian matrix.
        /// </summary>
        public double[][] Jacobian
        {
            get
            {
                return this.jacobian;
            }
        }

        /// <summary>
        /// The errors for each row of the Jacobian.
        /// </summary>
        public double[] RowErrors
        {
            get
            {
                return this.rowErrors;
            }
        }
    }
}
