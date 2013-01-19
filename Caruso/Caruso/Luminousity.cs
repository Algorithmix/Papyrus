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
        public class Luminousity
        {
            /// <summary>
            /// Converts an multichannel pixel into a single one with the Luma Wieghting
            /// </summary>
            /// <param name="color">Pixel</param>
            /// <returns>Weighted Average of Channels</returns>
            public static double Luma(Bgra color)
            {
                return (0.3 * color.Red + 0.59 * color.Green + 0.11 * color.Blue);
            }

            /// <summary>
            /// Given an array of pixels, a weighted average of the Luma value for that array is returned
            /// </summary>
            /// <param name="colors">Array of pixels to be Luma-ed and averaged</param>
            /// <param name="weightings">A double array of weightings</param>
            /// <returns>Single representative Luminousity Value</returns>
            public static double RepresentativeLuma(Bgra[] colors, double[] weightings)
            {
                double sum = 0;
                for (int ii = 0; ii < colors.Length; ii++ )
                {
                    sum += weightings[ii]*Luma(colors[ii]);
                }
                return sum;
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
                    weighting[length-1-ii] = (double)(ii + 1) * slice;
                }
                return weighting;
            }

            /// <summary>
            /// Scans the row to determine the representative luminousity of the right most edge
            /// </summary>
            /// <param name="image">Image to analyzed</param>
            /// <param name="row">Number of the row to be scanned</param>
            /// <param name="buffer">Number off residual pixels to be skipped</param>
            /// <param name="signal_size">Number of pixels to be sampled to determine the representative Luminousity</param>
            /// <param name="weighting">Weighting to average samples set with</param>
            /// <returns>Single representative double value</returns>
            public static double ScanRowFromRight(Emgu.CV.Image<Bgra, byte> image, int row, int buffer, int signal_size, double[] weighting)
            {
                int signalStart = (int)Defaults.Ignore;
                for (int ii = image.Width-1; ii >=0; ii--)
                {
                    if (image[row, ii].Alpha == byte.MaxValue)
                    {
                        signalStart = ii;
                        break;
                    }
                }

                if (signalStart == (int)Defaults.Ignore)
                {
                    return (double)Defaults.Ignore;
                }

                signalStart -= buffer;
                if (signalStart - signal_size < 0.0 )
                {
                    return Defaults.Ignore;
                }

                Bgra[] pixels = new Bgra[signal_size];
                for (int ii = 0; ii < signal_size; ii++)
                {
                    pixels[ii] = image[row, signalStart-ii];
                }

                return RepresentativeLuma(pixels, weighting);
            }

            /// <summary>
            /// Scans the row to determine the representative luminousity of the right most edge
            /// </summary>
            /// <param name="image">Image to analyzed</param>
            /// <param name="col">Number of the row to be scanned</param>
            /// <param name="buffer">Number off residual pixels to be skipped</param>
            /// <param name="signal_size">Number of pixels to be sampled to determine the representative Luminousity</param>
            /// <param name="weighting">Weighting to average samples set with</param>
            /// <returns>Single representative double value</returns>
            public static double ScanRowFromBottom(Emgu.CV.Image<Bgra, byte> image, int col, int buffer, int signal_size, double[] weighting)
            {
                int signalStart = (int)Defaults.Ignore;
                for (int ii = image.Height - 1; ii >= 0; ii--)
                {
                    if (image[ii,col].Alpha == byte.MaxValue)
                    {
                        signalStart = ii;
                        break;
                    }
                }

                if (signalStart == (int)Defaults.Ignore)
                {
                    return (double)Defaults.Ignore;
                }

                signalStart -= buffer;
                if (signalStart - signal_size < 0.0)
                {
                    return Defaults.Ignore;
                }

                Bgra[] pixels = new Bgra[signal_size];
                for (int ii = 0; ii < signal_size; ii++)
                {
                    pixels[ii] = image[ signalStart - ii,col];
                }

                return RepresentativeLuma(pixels, weighting);
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
            public static double ScanRowFromLeft(Emgu.CV.Image<Bgra, byte> image, int row, int buffer, int signal_size, double[] weighting)
            {
                int signalStart = (int) Defaults.Ignore;
                for (int ii = 0; ii < image.Width; ii++)
                {
                    if (image[row, ii].Alpha == byte.MaxValue)
                    {
                        signalStart = ii;
                        break;
                    }
                }

                if (signalStart == (int) Defaults.Ignore)
                {
                    return (double)Defaults.Ignore;
                }

                signalStart += buffer;
                if (signalStart + signal_size >= image.Width)
                {
                    return Defaults.Ignore;
                }

                Bgra[] pixels = new Bgra[signal_size];
                for (int ii = 0; ii < signal_size; ii++)
                {
                    pixels[ii] = image[row, ii + signalStart];
                }

                return RepresentativeLuma(pixels, weighting);
            }

            /// <summary>
            /// Scans the row to determine the representative luminousity of the top most edge
            /// </summary>
            /// <param name="image">Image to analyzed</param>
            /// <param name="col">Number of the row to be scanned</param>
            /// <param name="buffer">Number off residual pixels to be skipped</param>
            /// <param name="signal_size">Number of pixels to be sampled to determine the representative Luminousity</param>
            /// <param name="weighting">Weighting to average samples set with</param>
            /// <returns>Single representative double value</returns>
            public static double ScanRowFromTop(Emgu.CV.Image<Bgra, byte> image, int col, int buffer, int signal_size, double[] weighting)
            {
                int signalStart = (int)Defaults.Ignore;
                for (int ii = 0; ii < image.Height; ii++)
                {
                    if (image[ ii, col].Alpha == byte.MaxValue)
                    {
                        signalStart = ii;
                        break;
                    }
                }

                if (signalStart == (int)Defaults.Ignore)
                {
                    return (double)Defaults.Ignore;
                }

                signalStart += buffer;
                if (signalStart + signal_size >= image.Height)
                {
                    return Defaults.Ignore;
                }

                Bgra[] pixels = new Bgra[signal_size];
                for (int ii = 0; ii < signal_size; ii++)
                {
                    pixels[ii] = image[ii + signalStart, col];
                }

                return RepresentativeLuma(pixels, weighting);
            }



            /// <summary>
            /// Given an image, and parameters, this function will scan all the columns or rows for the given direction to determine the representative luminousity along a particular edge 
            /// </summary>
            /// <param name="image">Image</param>
            /// <param name="buffer">number of pixels to ignore, because they are assumed to be residual pixels</param>
            /// <param name="signal_size">number of pixels to be sampled</param>
            /// <param name="direction">Scan Direction</param>
            /// <returns>Representative Luminousity Value</returns>
            public static double[] RepresentativeLuminousity(Emgu.CV.Image<Bgra, byte> image, int buffer, int signal_size, Direction direction)
            {
                // Generate the weighting array once before hand
                double[] weighting = LinearWeighting(signal_size);

                // Create a lumas array to fill
                double[] lumas = new double[image.Height];

                // Scan all the rows
                try
                {
                    if (direction == Direction.FromLeft || direction == Direction.FromRight)
                    {
                        for (int row = 0; row < image.Height; row++)
                        {
                            switch (direction)
                            {
                                case Direction.FromLeft:
                                    lumas[row] = ScanRowFromLeft(image, row, buffer, signal_size, weighting);
                                    break;
                                case Direction.FromRight:
                                    lumas[row] = ScanRowFromRight(image, row, buffer, signal_size, weighting);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        for (int col = 0; col < image.Width; col++)
                        {
                            switch (direction)
                            {
                                case Direction.FromTop:
                                    lumas[col] = ScanRowFromTop(image, col, buffer, signal_size, weighting);
                                    break;
                                case Direction.FromBottom:
                                    lumas[col] = ScanRowFromBottom(image, col, buffer, signal_size, weighting);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.StackTrace);
                }

                return lumas;
            }

        }
    }
}
