using System;
namespace Encog.MathUtil.LIBSVM
{
    // This class was taken from the libsvm package.  We have made some
    // modifications for use in Encog.
    // 
    // http://www.csie.ntu.edu.tw/~cjlin/libsvm/
    //
    // The libsvm Copyright/license is listed here.
    // 
    // Copyright (c) 2000-2010 Chih-Chung Chang and Chih-Jen Lin
    // All rights reserved.
    // 
    // Redistribution and use in source and binary forms, with or without
    // modification, are permitted provided that the following conditions
    // are met:
    // 
    // 1. Redistributions of source code must retain the above copyright
    // notice, this list of conditions and the following disclaimer.
    // 
    // 2. Redistributions in binary form must reproduce the above copyright
    // notice, this list of conditions and the following disclaimer in the
    // documentation and/or other materials provided with the distribution.
    //
    // 3. Neither name of copyright holders nor the names of its contributors
    // may be used to endorse or promote products derived from this software
    // without specific prior written permission.
    // 
    // 
    // THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
    // ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
    // LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
    // A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE REGENTS OR
    // CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
    // EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
    // PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
    // PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
    // LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
    // NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
    // SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#if !SILVERLIGHT
	[Serializable]
#endif
	public class svm_parameter 
#if !SILVERLIGHT
        : System.ICloneable
#endif
	{
		/* svm_type */
		public const int C_SVC = 0;
		public const int NU_SVC = 1;
		public const int ONE_CLASS = 2;
		public const int EPSILON_SVR = 3;
		public const int NU_SVR = 4;
		
		/* kernel_type */
		public const int LINEAR = 0;
		public const int POLY = 1;
		public const int RBF = 2;
		public const int SIGMOID = 3;
		
		public int svm_type;
		public int kernel_type;
		public double degree; // for poly
		public double gamma; // for poly/rbf/sigmoid
		public double coef0; // for poly/sigmoid
		
		// these are for training only
		public double cache_size; // in MB
		public double eps; // stopping criteria
		public double C; // for C_SVC, EPSILON_SVR and NU_SVR
		public int nr_weight; // for C_SVC
		public int[] weight_label; // for C_SVC
		public double[] weight; // for C_SVC
		public double nu; // for NU_SVC, ONE_CLASS, and NU_SVR
		public double p; // for EPSILON_SVR
		public int shrinking; // use the shrinking heuristics
		public int probability; // do probability estimates
		
		public virtual System.Object Clone()
		{
			try
			{
				return base.MemberwiseClone();
			}
			//UPGRADE_NOTE: Exception 'java.lang.CloneNotSupportedException' was converted to 'System.Exception' which has different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1100_3"'
			catch (System.Exception)
			{
				return null;
			}
		}
	}
}