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
    public class Forensics
    {

        /// <summary>
        /// Converts an multichannel pixel into a single one with the Luma Wieghting
        /// </summary>
        /// <param name="color">Pixel</param>
        /// <returns>Weighted Average of Channels</returns>
        public static double Luma(Bgra color)
        {
            return (0.3*color.Red + 0.59*color.Green + 0.11*color.Blue);
        }

        /// <summary>
        /// Given an array of pixels, a weighted average of the Luma value for that array is returned
        /// </summary>
        /// <param name="colors">Array of pixels to be Luma-ed and averaged</param>
        /// <param name="weightings">A double array of weightings</param>
        /// <returns>Single representative Luminousity Value</returns>
        public static double Luma(Bgra[] colors, double[] weightings )
        {
            double sum = -0.1;
            foreach (Bgra color in colors)
            {
                sum += Luma(color);
            }

            return sum / (double)colors.Length; 
        }

        /// <summary>
        /// Generates an array of linearly decreasing values of size length
        /// </summary>
        /// <param name="length">Size of the array, also determines the steepness of the linear descent</param>
        /// <returns>An array of weightings between 0 and 1</returns>
        public static double[] LinearWeighting(int length)
        {
            // Figure out the weighting of each slice
            double slice = 1.0 / ((double)(length * (length + 1) / 2));

            double[] weighting = new double[length];
            for (int ii = 0; ii < length; ii++)
            {
                weighting[ii] = (double)(ii + 1) * slice;
            }
            return weighting;
        }

        /// <summary>
        /// Scans the row to determine the Luma
        /// </summary>
        /// <param name="image">Image to analyzed</param>
        /// <param name="row">Number of the row or column to be scanned</param>
        /// <param name="buffer">Number off residual pixels to be skipped</param>
        /// <param name="signal_size">Number of pixels to be sampled to determine the representative Luminousity</param>
        /// <param name="weighting">Weighting to average samples set with</param>
        /// <param name="direction">Scan direction</param>
        /// <returns>Single representative luma value</returns>
        public static double ScanRow(Emgu.CV.Image<Bgra, byte> image, int row, int buffer, int signal_size, double[] weighting, Direction direction)
        { 
            if ( direction == Direction.fromleft)
            {
                return ScanRowFromLeft(image, row, buffer, signal_size, weighting );
            }
            else
            {
                return -1.0; // ScanRowFromRight();
            }
        }

        /// <summary>
        /// Scans the row to determine the representative luminousity of the left most edge
        /// </summary>
        /// <param name="image">Image to analyzed</param>
        /// <param name="row">Number of the row to be scanned</param>
        /// <param name="buffer">Number off residual pixels to be skipped</param>
        /// <param name="signal_size">Number of pixels to be sampled to determine the representative Luminousity</param>
        /// <param name="weighting">Weighting to average samples set with</param>
        /// <returns>Single representative double value</returns>
        public static double ScanRowFromLeft( Emgu.CV.Image<Bgra, byte> image, int row, int buffer, int signal_size, double[] weighting)
        {
            int signalStart = -1;
            for (int ii = 0; ii < image.Width; ii++)
            {
                if (image[row,ii].Alpha == byte.MaxValue)
                {
                    signalStart = ii; 
                    break;
                }
            }
            
            if ( signalStart == -1 )
            {    
                return -1.0;
            }

            signalStart += buffer; 
            if (signalStart+signal_size >= image.Width)
            {
                return -1.0;
            }

            Bgra[] pixels = new Bgra[signal_size];
            for( int ii=0; ii<signal_size ; ii++ )
            {
                pixels[ii] = image[row,ii+signalStart];
            }
            
            return Luma( pixels , weighting);
        }

        /// <summary>
        /// 
        /// </summary>
        public enum Direction{
            fromleft,
            fromright,
            fromtop,
            frombottom
        }

        /// <summary>
        /// Given an image, and parameters, this function will scan all the columns or rows for the given direction to determine the representative luminousity along a particular edge 
        /// </summary>
        /// <param name="image">Image</param>
        /// <param name="buffer">number of pixels to ignore, because they are assumed to be residual pixels</param>
        /// <param name="signal_size">number of pixels to be sampled</param>
        /// <param name="direction">Scan Direction</param>
        /// <returns>Representative Luminousity Value</returns>
        public static double[] Luma(Emgu.CV.Image<Bgra, byte> image, int buffer, int signal_size, Direction direction)
        {
            // Generate the weighting array once before hand
            double[] weighting = LinearWeighting(signal_size);

            // Create a lumas array to fill
            double[] lumas = new double[image.Height];

            // Scan all the rows
            try
            {
                for (int row = 0; row < image.Height; row++)
                {
                    lumas[row] = ScanRow(image, row, buffer, signal_size, weighting, direction);
                }
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.StackTrace);
            }

            return lumas;
        }

        //public static double ScanRowFromRight()
        //{ 
        //}

        /// <summary>
        /// Generates the time indicies of kernel given the starting position
        /// </summary>
        /// <param name="kernel">The convolution kernel</param>
        /// <param name="startingKernelIndex">the starting time of the kernel (-1 etc.)</param>
        /// <returns>An array of time indicies for the kernel to use during convolution</returns>
        public static int[] GetKernelIndicies(double[] kernel, int startingKernelIndex)
        {
            int[] indicies = new int[kernel.Length];
            for (int ii = 0 ; ii < kernel.Length ; ii++)
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
            for (int ii = 0; ii < xx.Length;ii++ )
            {
                for (int jj = 0; jj < hh.Length; jj++)
                {
                    int index = (ii - indicies[jj]);
                    if ( index >= 0  && index < xx.Length )
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
        public static int[] Chamfer( double[] convolution)
        { 
            List<int> features = new List<int>();
            features.Add(0);
            for (int ii = 1; ii < convolution.Length-1; ii++)
            {
                if (convolution[ii] > 0.0)
                {
                    features.Add(ii);
                }
            }
            features.Add(convolution.Length-1);

            int[] chamfers = new int[convolution.Length];
            int previous = 0;
            int next = 1;
            for( int ii=0; ii<chamfers.Length ; ii++ )
            {
                if ( ii > features[next] )
                {
                    previous++;
                    next++;
                }
                chamfers[ii] = Math.Min( ii-features[previous], features[next]-ii);
            }

            return chamfers;
        }
    }
}
