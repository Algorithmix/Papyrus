using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

namespace Caruso
{
    namespace Forensics
    {
        public class Chamfer
        {
            /// <summary>
            /// Calculates the chamfer from a convolution
            /// </summary>
            /// <param name="convolution">An array of doubles, where 0 indicates no feature and non zero indicates a feature</param>
            /// <returns>A chamfer value for each pixel</returns>
            public static int[] Measure(double[] convolution)
            {
                List<int> features = new List<int> {0};
                for (int ii = 1; ii < convolution.Length - 1; ii++)
                {
                    if (convolution[ii] > 0.0)
                    {
                        features.Add(ii);
                    }
                }
                features.Add(convolution.Length - 1);

                int[] chamfers = new int[convolution.Length];
                int previous = 0;
                int next = 1;
                for (int ii = 0; ii < chamfers.Length; ii++)
                {
                    if (ii > features[next])
                    {
                        previous++;
                        next++;
                    }
                    chamfers[ii] = Math.Min(ii - features[previous], features[next] - ii);
                }

                return chamfers;
            }

            public static double[] ScanSimilarity(double[] chamfer1, double[] chamfer2)
            {
                var size1 = chamfer1.Length;
                var size2 = chamfer2.Length;
                var smaller = chamfer2;
                var larger = chamfer1;
                if ( size1 < size2 )
                {
                    smaller = chamfer1;
                    larger = chamfer2;
                }
                var chamfers = new double[larger.Length - smaller.Length +1];
                for (int start=0; start+smaller.Length <= larger.Length; start++)
                {
                    chamfers[start] = Similarity(smaller, larger, start);
                }
                return chamfers;
            }

            public static long Sparsity(int[] chamfer)
            {
                long total = 0; 
                foreach( int ii in chamfer)
                {
                    total += ii;
                }
                return total;
            }

            public static double Similarity(double[] smaller, double[] larger, int start=0)
            {
                if ( smaller.Length > larger.Length)
                {
                    throw new ArgumentException("Smaller Chamfer is greater in length than Larger Chamfer!");
                }
                if ( larger.Length < start+smaller.Length )
                {
                    throw new ArgumentException("Start position is too far offset for calculation");
                }

                double c1Dotc2 = 0;
                double c2Dotc2 = 0;
                double c1Dotc1 = 0;

                // Compute all the scalar products in one pass
                for (int ii=0; ii< smaller.Length ;ii++)
                {
                    c1Dotc2 += smaller[ii] * larger[ii+start];
                    c1Dotc1 += smaller[ii] * smaller[ii];
                    c2Dotc2 += larger[ii+start] * larger[ii+start];
                }

                return (c1Dotc2)/Math.Max(c2Dotc2, c1Dotc1);
            }
        }
    }
}
