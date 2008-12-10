﻿// Encog Neural Network and Bot Library v1.x (DotNet)
// http://www.heatonresearch.com/encog/
// http://code.google.com/p/encog-cs/
// 
// Copyright 2008, Heaton Research Inc., and individual contributors.
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
using Encog.Neural.Persist;
using Encog.Neural.NeuralData;

namespace Encog.Neural.Data.Basic
{
    public class BasicNeuralDataSet : INeuralDataSet, IEnumerable<INeuralDataPair>, IEncogPersistedObject
    {
        public virtual String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public virtual String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public class BasicNeuralIterator : IEnumerator<INeuralDataPair>
        {
            private int current;
            private BasicNeuralDataSet owner;

            public BasicNeuralIterator(BasicNeuralDataSet owner)
            {
                this.current = -1;
                this.owner = owner;
            }

            public INeuralDataPair Current
            {
                get
                {
                    return owner.data[this.current];
                }
            }

            public void Dispose()
            {
                // nothing needed
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (this.current < 0)
                    {
                        throw new InvalidOperationException("Must call MoveNext before reading Current.");
                    }
                    return this.owner.data[this.current];
                }
            }

            public bool MoveNext()
            {
                this.current++;
                if (current >= this.owner.data.Count)
                    return false;
                return true;
            }

            public void Reset()
            {
                this.current = -1;
            }
        }

        public virtual IList<INeuralDataPair> Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }


        /// <summary>
        /// The data held by this object.
        /// </summary>
        private IList<INeuralDataPair> data = new List<INeuralDataPair>();

        /// <summary>
        /// The enumerators created for this list.
        /// </summary>
        private IList<BasicNeuralIterator> enumerators =
            new List<BasicNeuralIterator>();

        private String description;
        private String name;


        /// <summary>
        /// Construct a data set from an input and idea array.
        /// </summary>
        /// <param name="input">The input into the neural network for training.</param>
        /// <param name="ideal">The idea into the neural network for training.</param>
        public BasicNeuralDataSet(double[][] input, double[][] ideal)
        {
            for (int i = 0; i < input.Length; i++)
            {
                double[] tempInput = new double[input[0].Length];
                double[] tempIdeal = new double[ideal[0].Length];

                for (int j = 0; j < tempInput.Length; j++)
                {
                    tempInput[j] = input[i][j];
                }

                for (int j = 0; j < tempIdeal.Length; j++)
                {
                    tempIdeal[j] = ideal[i][j];
                }

                BasicNeuralData inputData = new BasicNeuralData(tempInput);
                BasicNeuralData idealData = new BasicNeuralData(tempIdeal);
                this.Add(inputData, idealData);
            }
        }

        public BasicNeuralDataSet()
        {
        }

        public virtual int IdealSize
        {
            get
            {
                INeuralDataPair pair = this.data[0];
                return pair.Ideal.Count;
            }
        }

        public virtual int InputSize
        {
            get
            {
                INeuralDataPair pair = this.data[0];
                return pair.Input.Count;
            }
        }

        public virtual void Add(INeuralData data1)
        {
            INeuralDataPair pair = new BasicNeuralDataPair(data1, null);
            this.data.Add(pair);
        }

        public virtual void Add(INeuralData inputData, INeuralData idealData)
        {
            INeuralDataPair pair = new BasicNeuralDataPair(inputData, idealData);
            this.data.Add(pair);
        }

        public virtual void Add(INeuralDataPair inputData)
        {
            this.data.Add(inputData);
        }

        public virtual void Close()
        {
            // not needed
        }

        public IEnumerator<INeuralDataPair> GetEnumerator()
        {
            return new BasicNeuralIterator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new BasicNeuralIterator(this);
        }
    }
}
