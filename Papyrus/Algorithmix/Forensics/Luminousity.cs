﻿#region

using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

#endregion

namespace Algorithmix
{
    namespace Forensics
    {
        public class Luminousity
        {
            public static bool SMART_BUFFERING = false;
            public static int PaperDiff = 60;
            public static int TextDiff = 300;


           public static int[] Jaccard(double[] luminousity)
           {
               int[] jaccard = new int[luminousity.Length];
               for (int ii=0; ii<luminousity.Length ; ii++)
               {
                   jaccard[ii] = LuminousityToJaccard(luminousity[ii]);
               }
               return jaccard;
           }

           public static int LuminousityToJaccard(double luma)
           {
               return (int)Math.Round(luma/255.0);
           }

            /// <summary>
            ///   Converts an multichannel pixel into a single one with the Luma Wieghting
            /// </summary>
            /// <param name="color"> Pixel </param>
            /// <returns> Weighted Average of Channels </returns>
            public static double Luma(Bgra color)
            {
                return (0.3*color.Red + 0.59*color.Green + 0.11*color.Blue);
            }

            /// <summary>
            ///   Given an array of pixels, a weighted average of the Luma value for that array is returned
            /// </summary>
            /// <param name="colors"> Array of pixels to be Luma-ed and averaged </param>
            /// <param name="weightings"> A double array of weightings </param>
            /// <returns> Single representative Luminousity Value </returns>
            public static double RepresentativeLuma(Bgra[] colors, double[] weightings)
            {
                double sum = 0;
                for (int ii = 0; ii < colors.Length; ii++)
                {
                    sum += weightings[ii]*Luma(colors[ii]);
                }
                return sum;
            }


            public static int BinaryLuma(Bgra[] pixels)
            {
                for (int ii=0; ii< pixels.Length ;ii++)
                {
                   if (Utility.AverageColor(pixels[ii]) == 1)
                   {
                        return 1;
                   }
                }
                return 0;
            }

            /// <summary>
            ///   Generates an array of linearly decreasing values of size length
            /// </summary>
            /// <param name="length"> Size of the array, also determines the steepness of the linear descent </param>
            /// <returns> An array of weightings between 0 and 1 </returns>
            public static double[] LinearWeighting(int length)
            {
                // Figure out the weighting of each slice
                double slice = 1.0/((length*(length + 1)/2));

                double[] weighting = new double[length];
                for (int ii = 0; ii < length; ii++)
                {
                    weighting[length - 1 - ii] = (ii + 1)*slice;
                }
                return weighting;
            }

            /// <summary>
            ///   Scans the row to determine the representative luminousity of the right most edge
            /// </summary>
            /// <param name="image"> Image to analyzed </param>
            /// <param name="row"> Number of the row to be scanned </param>
            /// <param name="buffer"> Number off residual pixels to be skipped </param>
            /// <param name="signal_size"> Number of pixels to be sampled to determine the representative Luminousity </param>
            /// <param name="weighting"> Weighting to average samples set with </param>
            /// <returns> Single representative double value </returns>
            public static double ScanRowFromRight(Image<Bgra, byte> image, int row, int buffer, int signal_size,
                                                  double[] weighting)
            {
                int signalStart = (int) Utility.Defaults.Ignore;
                for (int ii = image.Width - 1; ii >= 0; ii--)
                {
                    if (image[row, ii].Alpha == byte.MaxValue)
                    {
                        signalStart = ii;
                        break;
                    }
                }

                if (signalStart == (int) Utility.Defaults.Ignore)
                {
                    return Utility.Defaults.Ignore;
                }

                if (SMART_BUFFERING)
                {
                    signalStart -= GetBufferFromRight(image, signalStart, row, buffer);
                }
                else
                {
                    signalStart -= buffer;
                }
                
                if (signalStart - signal_size < 0.0)
                {
                    return Utility.Defaults.Ignore;
                }

                Bgra[] pixels = new Bgra[signal_size];
                for (int ii = 0; ii < signal_size; ii++)
                {
                    pixels[ii] = image[row, signalStart - ii];
                }

                //if (Shred.JACCARD)
                //{
                //    return BinaryLuma(pixels);
                //}

                return RepresentativeLuma(pixels, weighting);
            }

            /// <summary>
            ///   Scans the row to determine the representative luminousity of the right most edge
            /// </summary>
            /// <param name="image"> Image to analyzed </param>
            /// <param name="col"> Number of the row to be scanned </param>
            /// <param name="buffer"> Number off residual pixels to be skipped </param>
            /// <param name="signal_size"> Number of pixels to be sampled to determine the representative Luminousity </param>
            /// <param name="weighting"> Weighting to average samples set with </param>
            /// <returns> Single representative double value </returns>
            public static double ScanRowFromBottom(Image<Bgra, byte> image, int col, int buffer, int signal_size,
                                                   double[] weighting)
            {
                int signalStart = (int) Utility.Defaults.Ignore;
                for (int ii = image.Height - 1; ii >= 0; ii--)
                {
                    if (image[ii, col].Alpha == byte.MaxValue)
                    {
                        signalStart = ii;
                        break;
                    }
                }

                if (signalStart == (int) Utility.Defaults.Ignore)
                {
                    return Utility.Defaults.Ignore;
                }

                signalStart -= buffer;
                if (signalStart - signal_size < 0.0)
                {
                    return Utility.Defaults.Ignore;
                }

                Bgra[] pixels = new Bgra[signal_size];
                for (int ii = 0; ii < signal_size; ii++)
                {
                    pixels[ii] = image[signalStart - ii, col];
                }

                return RepresentativeLuma(pixels, weighting);
            }

            /// <summary>
            ///   Scans the row to determine the representative luminousity of the left most edge
            /// </summary>
            /// <param name="image"> Image to analyzed </param>
            /// <param name="row"> Number of the row to be scanned </param>
            /// <param name="buffer"> Number off residual pixels to be skipped </param>
            /// <param name="signal_size"> Number of pixels to be sampled to determine the representative Luminousity </param>
            /// <param name="weighting"> Weighting to average samples set with </param>
            /// <returns> Single representative double value </returns>
            public static double ScanRowFromLeft(Image<Bgra, byte> image, int row, int buffer, int signal_size,
                                                 double[] weighting)
            {
                int signalStart = (int) Utility.Defaults.Ignore;
                for (int ii = 0; ii < image.Width; ii++)
                {
                    if (image[row, ii].Alpha == byte.MaxValue)
                    {
                        signalStart = ii;
                        break;
                    }
                }

                if (signalStart == (int) Utility.Defaults.Ignore)
                {
                    return Utility.Defaults.Ignore;
                }

                if (SMART_BUFFERING)
                {
                    signalStart += GetBufferFromLeft(image, signalStart, row, buffer);
                }
                else
                {
                    signalStart += buffer;
                }
                
                if (signalStart + signal_size >= image.Width)
                {
                    return Utility.Defaults.Ignore;
                }
                
                Bgra[] pixels = new Bgra[signal_size];
                for (int ii = 0; ii < signal_size; ii++)
                {
                    pixels[ii] = image[row, ii + signalStart];
                }
                
                //if (Shred.JACCARD)
                //{
                //    return BinaryLuma(pixels);
                //}
                return RepresentativeLuma(pixels, weighting);
            }

            /// <summary>
            ///   Scans the row to determine the representative luminousity of the top most edge
            /// </summary>
            /// <param name="image"> Image to analyzed </param>
            /// <param name="col"> Number of the row to be scanned </param>
            /// <param name="buffer"> Number off residual pixels to be skipped </param>
            /// <param name="signal_size"> Number of pixels to be sampled to determine the representative Luminousity </param>
            /// <param name="weighting"> Weighting to average samples set with </param>
            /// <returns> Single representative double value </returns>
            public static double ScanRowFromTop(Image<Bgra, byte> image, int col, int buffer, int signal_size,
                                                double[] weighting)
            {
                int signalStart = (int) Utility.Defaults.Ignore;
                for (int ii = 0; ii < image.Height; ii++)
                {
                    if (image[ii, col].Alpha == byte.MaxValue)
                    {
                        signalStart = ii;
                        break;
                    }
                }

                if (signalStart == (int) Utility.Defaults.Ignore)
                {
                    return Utility.Defaults.Ignore;
                }

                signalStart += buffer;
                if (signalStart + signal_size >= image.Height)
                {
                    return Utility.Defaults.Ignore;
                }

                Bgra[] pixels = new Bgra[signal_size];
                for (int ii = 0; ii < signal_size; ii++)
                {
                    pixels[ii] = image[ii + signalStart, col];
                }

                return RepresentativeLuma(pixels, weighting);
            }

            public static int GetBufferFromRight(Image<Bgra, byte> image, int signalStart, int row, int defaultBuffer)
            {
                const int max = 21;

                int buffer = -1;
                const int refSum = 255 * 3;
                int diff;
                
                // i never get a chance to use these do whiles ... 
                do
                {
                    diff = -1;
                    buffer++;
                    int pos = signalStart - buffer;
                    if (pos >= 0)
                    {
                        int currentSum = 
                                        (int)image[row, pos].Red + 
                                        (int)image[row, pos].Blue +
                                        (int)image[row, pos].Green;
                        diff = refSum - currentSum;
                    }

                } while ((buffer <= max) && !(diff >= TextDiff || diff <= PaperDiff));

                if (buffer >= max)
                {
                    image[row, signalStart - Math.Max(0, defaultBuffer - 1)] = new Bgra(Byte.MaxValue, Byte.MinValue, Byte.MinValue, Byte.MaxValue);
                    return defaultBuffer;
                }
                image[row, signalStart - Math.Max(0, buffer - 1)] = new Bgra(Byte.MaxValue, Byte.MinValue, Byte.MinValue, Byte.MaxValue);
                return buffer;
            }

            public static int GetBufferFromLeft(Image<Bgra,byte> image, int signalStart, int row, int defaultBuffer)
            {
                const int max = 21;

                int buffer = -1;
                const int refSum = 255*3;
                int diff;
                do
                {
                    diff = -1;
                    buffer++;
                    int pos = signalStart + buffer;
                    if (pos < image.Width - 1)
                    {

                        int currentSum = (int) image[row, pos].Red +
                                         (int) image[row, pos].Blue +
                                         (int) image[row, pos].Green;
                        diff = refSum - currentSum;
                    }

                } while ((buffer <= max) && !(diff >= TextDiff || diff <= PaperDiff));

                if (buffer >= max)
                {
                    image[row,signalStart+Math.Max(0,defaultBuffer-1)] = new Bgra(Byte.MaxValue,Byte.MinValue,Byte.MinValue,Byte.MaxValue);

                    return defaultBuffer;
                }
                image[row, signalStart+Math.Max(0,buffer-1)] = new Bgra(Byte.MaxValue, Byte.MinValue, Byte.MinValue, Byte.MaxValue);
                return buffer;
            }

            /// <summary>
            ///   Given an image, and parameters, this function will scan all the columns or rows for the given direction to determine the representative luminousity along a particular edge
            /// </summary>
            /// <param name="image"> Image </param>
            /// <param name="buffer"> number of pixels to ignore, because they are assumed to be residual pixels </param>
            /// <param name="signal_size"> number of pixels to be sampled </param>
            /// <param name="direction"> Scan Direction </param>
            /// <returns> Representative Luminousity Value </returns>
            public static double[] RepresentativeLuminousity(Image<Bgra, byte> image, int buffer,
                                                             int signal_size, Direction direction)
            {
                // Generate the weighting array once before hand
                double[] weighting = LinearWeighting(signal_size);

                // Create a lumas array to fill
                double[] lumas = new double[image.Height];

                // Scan all the rows
                //try
                //{
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
                //}
                //catch (Exception ee)
                //{
                //    Console.WriteLine(ee.StackTrace);
                //}

                return lumas;
            }
        }
    }
}