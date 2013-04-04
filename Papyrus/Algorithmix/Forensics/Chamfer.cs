#region

using System;
using System.Collections.Generic;

#endregion

namespace Algorithmix
{
    namespace Forensics
    {
        public class Chamfer
        {
            /// <summary>
            ///   Calculates the chamfer from a convolution
            /// </summary>
            /// <param name="convolution"> An array of doubles, where 0 indicates no feature and non zero indicates a feature </param>
            /// <returns> A chamfer value for each pixel </returns>
            public static int[] Measure(double[] convolution)
            {
                List<int> features = new List<int> { 0 };
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

            public static int[] ScaleChamfer(int[] input, int size)
            {
                double multiplier = size/(double)input.Length;
                List<double> features = new List<double> { 0 };
                features.Add(0);
                for (int ii = 1; ii < input.Length - 1; ii++)
                {
                    if (input[ii] == 0)
                    {
                        features.Add((double)ii);
                    }
                }
                features.Add(input.Length - 1);

                int[] scaledfeature = new int[features.Count];
                for(int ii=1; ii<features.Count-1; ii++)
                {
                    scaledfeature[ii] =  (int) (features[ii]*multiplier);
                }
                scaledfeature[0] = 0;
                scaledfeature[scaledfeature.Length-1] = size - 1;

                int[] chamfers = new int[size];
                int previous = 0;
                int next = 1;
                for (int ii = 0; ii < chamfers.Length; ii++)
                {
                    if (ii > scaledfeature[next])
                    {
                        previous++;
                        next++;
                    }
                    chamfers[ii] = Math.Min(ii - scaledfeature[previous], scaledfeature[next] - ii);
                }

                return chamfers;
            }

            public static double NormalizedSimilarity(int[] chamfer1, int[] chamfer2)
            {
                int size1 = chamfer1.Length;
                int size2 = chamfer2.Length;
                int[] smaller = chamfer2;
                int[] larger = chamfer1;
                if (size1 < size2)
                {
                    smaller = chamfer1;
                    larger = chamfer2;
                }
                int[] scaled = ScaleChamfer(smaller, larger.Length);
                return Similarity(scaled, larger, 0);
            }

            /// <summary>
            ///   Scan Similarity calculates the similarity for possible alignments of two chamfers
            ///   such that the smaller edge is always bound by the larger edge
            /// </summary>
            /// <param name="chamfer1"> Chamfer vector </param>
            /// <param name="chamfer2"> Other shreds Chamfer vector </param>
            /// <returns> Similarity array </returns>
            public static double[] ScanSimilarity(int[] chamfer1, int[] chamfer2)
            {
                var size1 = chamfer1.Length;
                var size2 = chamfer2.Length;
                var smaller = chamfer2;
                var larger = chamfer1;
                if (size1 < size2)
                {
                    smaller = chamfer1;
                    larger = chamfer2;
                }
                var chamfers = new double[larger.Length - smaller.Length + 1];
                for (int start = 0; start + smaller.Length <= larger.Length; start++)
                {
                    chamfers[start] = Similarity(smaller, larger, start);
                }
                return chamfers;
            }

            /// <summary>
            ///   Indicates the sparsity of the chamfer by sum(integrating) 
            ///   the chamfer vector
            /// </summary>
            /// <param name="chamfer"> chamfer vector </param>
            /// <returns> Sparsity value </returns>
            public static long Sparsity(int[] chamfer)
            {
                long total = 0;
                foreach (int ii in chamfer)
                {
                    total += ii;
                }
                return total;
            }

            /// <summary>
            ///   Calculates how similar two chamfers are 'i.e' opposite of the chamfer distance
            ///   Does so by taking the dot product of the two chamfer vectors normalized by the 
            ///   max( dot product of either of the chamfers).
            /// </summary>
            /// <param name="smaller"> Smaller Chamfer Vector </param>
            /// <param name="larger"> Larger Chamfer vector </param>
            /// <param name="start"> Starting offset </param>
            /// <returns> A single value representing similarity </returns>
            public static double Similarity(int[] smaller, int[] larger, int start = 0)
            {
                if (smaller.Length > larger.Length)
                {
                    throw new ArgumentException("Smaller Chamfer is greater in length than Larger Chamfer!");
                }
                if (larger.Length < start + smaller.Length)
                {
                    throw new ArgumentException("Start position is too far offset for calculation");
                }

                double c1Dotc2 = 0;
                double c2Dotc2 = 0;
                double c1Dotc1 = 0;

                // Compute all the scalar products in one pass
                for (int ii = 0; ii < smaller.Length; ii++)
                {
                    c1Dotc2 += smaller[ii] * larger[ii + start];
                    c1Dotc1 += smaller[ii] * smaller[ii];
                    c2Dotc2 += larger[ii + start] * larger[ii + start];
                }

                return (c1Dotc2) / Math.Max(c2Dotc2, c1Dotc1);
            }
        }
    }
}