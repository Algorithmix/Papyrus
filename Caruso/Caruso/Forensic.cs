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
            for (int ii = (kernel.Length - 1); ii >= 0; ii--)
            {
                indicies[ii] = (startingKernelIndex * -1) - (kernel.Length - 1 - ii);
            }

            return indicies; 
        }

        public static double[] Convolute(double[] xx, double[] kernel, int[] indicies)
        {
            double[] result = new double[xx.Length];
            
            // Flip the kernel
            double[] hh = new double[kernel.Length];
            Array.Copy(kernel,hh,kernel.Length);
            hh.Reverse();
            
            // Shift and Sum
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
    }
}
