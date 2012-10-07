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

    public class Utility
    {
        /// <summary>
        /// Generates the time indicies of kernel given the starting position
        /// </summary>
        /// <param name="kernel">The convolution kernel</param>
        /// <param name="startingKernelIndex">the starting time of the kernel (-1 etc.)</param>
        /// <returns>An array of time indicies for the kernel to use during convolution</returns>
        public static int[] GetKernelIndicies(double[] kernel, int startingKernelIndex)
        {
            int[] indicies = new int[kernel.Length];
            for (int ii = 0; ii < kernel.Length; ii++)
            {
                indicies[ii] = (startingKernelIndex) + ii;
            }

            return indicies;
        }

        /// <summary>
        /// Performs 1D convolution an a given input array xx, with a convolution kernel hh
        /// </summary>
        /// <param name="xx">series input</param>
        /// <param name="hh">convolution kernel</param>
        /// <param name="indicies">time index of the kernel</param>
        /// <returns>convolution result</returns>
        public static double[] Convolute(double[] xx, double[] hh, int[] indicies)
        {
            double[] result = new double[xx.Length];

            //  Flip Shift and Aggregate
            for (int ii = 0; ii < xx.Length; ii++)
            {
                for (int jj = 0; jj < hh.Length; jj++)
                {
                    int index = (ii - indicies[jj]);
                    if (index >= 0 && index < xx.Length)
                    {
                        result[ii] += xx[index] * hh[jj];
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Converts an array of positive and negative values to just their absolute values
        /// </summary>
        /// <param name="input">values</param>
        /// <returns>only positive (absolute) values of the input</returns>
        public static double[] Absolute(double[] input)
        {
            double[] output = new double[input.Length];
            for (int ii = 0; ii < input.Length; ii++)
            {
                if (input[ii] < 0)
                {
                    output[ii] = input[ii] * (-1.0);
                }
                else
                {
                    output[ii] = input[ii];
                }
            }

            return output;
        }

        /// <summary>
        /// Thresholds the input array by a factor of the maximum value. All values that are less than factor*max(input) are set to zero, others are set to max
        /// </summary>
        /// <param name="input">Array to be thresholded</param>
        /// <param name="threshold">Multiplier value, if this value is 0.5, then we threshold by half the max</param>
        /// <returns>Array of zeros and max value</returns>
        public static double[] Threshold(double[] input, double threshold)
        {
            double maxValue = input.Max();
            double cutoff = maxValue * threshold;
            double[] output = new double[input.Length];
            for (int ii = 0; ii < input.Length; ii++)
            {
                if (input[ii] < cutoff)
                {
                    output[ii] = 0;
                }
                else
                {
                    output[ii] = maxValue;
                }
            }
            return output;
        }

        /// <summary>
        /// Calculates the chamfer from a convolution
        /// </summary>
        /// <param name="convolution">An array of doubles, where 0 indicates no feature and non zero indicates a feature</param>
        /// <returns>A chamfer value for each pixel</returns>
        public static int[] Chamfer(double[] convolution)
        {
            List<int> features = new List<int>();
            features.Add(0);
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
    }
}