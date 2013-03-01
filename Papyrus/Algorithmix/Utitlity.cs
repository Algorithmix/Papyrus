#region

using System;
using System.Drawing;
using Emgu.CV.Structure;

#endregion

namespace Algorithmix
{
    public class Utility
    {
        public static Tuple<double, int> Max(double[] array)
        {
            double max = array[0];
            int index;
            int best = 0;

            for (index = 0; index < array.Length; index++)
            {
                if (array[index] > max)
                {
                    max = array[index];
                    best = index;
                }
            }

            return new Tuple<double, int>(max, best);
        }

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
                        result[ii] += xx[index] * hh[jj];
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


        /// <summary>
        ///   Takes an integer, returns a BGR color
        /// </summary>
        /// <param name="color"> an integer between 0 and 0xFFFFFF inclusive </param>
        /// <returns> </returns>
        public static Bgr IntToBgr(int color)
        {
            if (color > 0xFFFFFF)
            {
                throw new IndexOutOfRangeException("Color must be <= 0xFFFFFF");
            }
            int blue = color & (0xFF0000);
            int green = color & (0x00FF00);
            int red = color & (0x0000FF);
            return new Bgr(blue, green, red);
        }

        /// <summary>
        ///   Returns the Cartesian Distance between from the first the second parameter
        /// </summary>
        /// <param name="pixelA"> Pixel of given color from </param>
        /// <param name="pixelB"> Pixel of given color to </param>
        /// <returns> Cartesian Distance </returns>
        public static double Distance(Color pixelA, Color pixelB)
        {
            int red = pixelB.R - pixelA.R;
            int green = pixelB.G - pixelA.G;
            int blue = pixelB.B - pixelA.B;
            return Math.Sqrt((red * red + green * green + blue * blue));
        }

        /// <summary>
        ///   Checks if two colors are equal
        /// </summary>
        /// <param name="color1"> first color </param>
        /// <param name="color2"> second color </param>
        /// <returns> True if equal, false if not </returns>
        public static bool IsEqual(Bgr color1, Bgr color2)
        {
            return (color1.Red == color2.Red
                    && color1.Blue == color2.Blue
                    && color1.Green == color2.Green);
        }

        /// <summary>
        ///   Determines the cartesian distance between two colors
        /// </summary>
        /// <param name="color1"> First Color </param>
        /// <param name="color2"> Second Color </param>
        /// <returns> A double value that indicates difference in colors, where zero means no zero and numbers greater indicate a difference </returns>
        public static double Distance(Bgr color1, Bgr color2)
        {
            double r1 = color1.Red;
            double r2 = color2.Red;
            double g1 = color1.Green;
            double g2 = color2.Green;
            double b1 = color1.Blue;
            double b2 = color2.Blue;
            return Math.Sqrt((r1 - r2) * (r1 - r2) + (g1 - g2) * (g1 - g2) + (b1 - b2) * (b1 - b2));
        }

        public static double SlopeFromPoints(Point p1, Point p2)
        {
            double y = p1.Y;
            double y1 = p2.Y;
            double x = p1.X;
            double x1 = p2.X;
            double m = (y - y1) / (x - x1);
            return m;
        }

        /// <summary>
        ///   Determines whether the given x and y coordinates are bound with in the given the given image
        /// </summary>
        /// <param name="image"> Bit map image to checked </param>
        /// <param name="xx"> X position of the pixel or Column </param>
        /// <param name="yy"> Y position of the pixel or Row </param>
        /// <returns> True if within the bounds, and false if out of bounds </returns>
        public static bool IsBound(Bitmap image, int xx, int yy)
        {
            if (xx >= 0 && xx < image.Width && yy >= 0 && yy < image.Height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static class Defaults
        {
            public static readonly double Ignore = -1.0;
        }
    }
}