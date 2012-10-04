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
        public static double Luma(Bgra color)
        {
            return (0.3*color.Red + 0.59*color.Green + 0.11*color.Blue);
        }

        public static double Luma(Bgra[] colors, double[] weightings )
        {
            double sum = -0.1;
            foreach (Bgra color in colors)
            {
                sum += Luma(color);
            }

            return sum / (double)colors.Length; 
        }

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

        public enum Direction{
            fromleft,
            fromright,
            fromtop,
            frombottom
        }

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

        public static int[] GetKernelIndicies(double[] kernel, int startingKernelIndex)
        {
            int[] indicies = new int[kernel.Length];
            for (int ii = 0 ; ii < kernel.Length ; ii++)
            {
                indicies[ii] = (startingKernelIndex) + ii;
            }

            return indicies; 
        }

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
