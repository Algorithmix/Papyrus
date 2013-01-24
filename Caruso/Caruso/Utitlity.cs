#region

using System;

#endregion

namespace Algorithmix
{
    public class Utility
    {
        /// <summary>
        ///   Generates the time indicies of kernel given the starting position
        /// </summary>
        /// <param name="kernel"> The convolution kernel </param>
        /// <param name="startingKernelIndex"> the starting time of the kernel (-1 etc.) </param>
        /// <returns> An array of time indicies for the kernel to use during convolution </returns>
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
        ///   Performs 1D convolution an a given input array xx, with a convolution kernel hh
        /// </summary>
        /// <param name="xx"> series input </param>
        /// <param name="hh"> convolution kernel </param>
        /// <param name="indicies"> time index of the kernel </param>
        /// <returns> convolution result </returns>
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
                        result[ii] += xx[index]*hh[jj];
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///   Converts an array of positive and negative values to just their absolute values
        /// </summary>
        /// <param name="input"> values </param>
        /// <returns> only positive (absolute) values of the input </returns>
        public static double[] Absolute(double[] input)
        {
            double[] output = new double[input.Length];
            for (int ii = 0; ii < input.Length; ii++)
            {
                if (input[ii] < 0)
                {
                    output[ii] = input[ii]*(-1.0);
                }
                else
                {
                    output[ii] = input[ii];
                }
            }

            return output;
        }

        /// <summary>
        ///   Thresholds the input array by a factor of the maximum value.
        ///   The Maximum value is defined as the maximum EXCLUDING the first and the Last -
        ///   because the first and the last are implicit features
        ///   All values that are less than factor*max(input) are set to zero, others are set to max
        /// </summary>
        /// <param name="input"> Array to be thresholded </param>
        /// <param name="threshold"> Multiplier value, if this value is 0.5, then we threshold by half the max </param>
        /// <returns> Array of zeros and max value </returns>
        public static double[] Threshold(double[] input, double threshold)
        {
            double maxValue = -1;
            for (int index = 1; index < (input.Length - 1); index++)
            {
                maxValue = Math.Max(maxValue, input[index]);
            }
            double cutoff = maxValue*threshold;
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

        public static double[] Reverse(double[] original)
        {
            double[] reversed = new double[original.Length];
            for (int ii = 0; ii < original.Length; ii++)
            {
                reversed[ii] = original[original.Length - (ii + 1)];
            }
            return reversed;
        }

        public static int[] Reverse(int[] original)
        {
            int[] reversed = new int[original.Length];
            for (int ii = 0; ii < original.Length; ii++)
            {
                reversed[ii] = original[original.Length - (ii + 1)];
            }
            return reversed;
        }
    }

    public static class Defaults
    {
        public static readonly double Ignore = -1.0;
    }
}