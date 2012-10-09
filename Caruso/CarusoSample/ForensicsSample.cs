using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using Caruso.Forensics;
using Caruso;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.Util;

namespace CarusoSample
{
    class ForensicsSample
    {
        public static void ChamberFromLeft(string filepath)
        {
            double[] kernel = new Double[] { -1.0, 0.0, 1.0 };
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra, Byte>(source);
            double[] lumas = Luminousity.RepresentativeLuminousity(image, 2, 4, Luminousity.Direction.FromRight);
            int[] indicies = Utility.GetKernelIndicies(kernel, -1);
            var convolution = Utility.Convolute(lumas, kernel, indicies);
            var processed = Utility.Threshold(Utility.Absolute(convolution), 0.3);
            var chamfers = Chamfer.Calculate(processed);
            Caruso.Visualizer.Plot( chamfers , "Convolution Result");
        }

        public static void ConvolutionFromLeft(string filepath)
        {
            double[] kernel = new Double[] { -1.0, 0.0, 1.0 };
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra, Byte>(source);
            double[] lumas = Luminousity.RepresentativeLuminousity(image, 1, 4, Luminousity.Direction.FromRight);
            int[] indicies = Utility.GetKernelIndicies(kernel, -1);
            var  convolution = Utility.Convolute( lumas, kernel, indicies);
            var result = Utility.Absolute(convolution);
            var processed = Utility.Threshold(Utility.Absolute(convolution), 0.3);
            for (int ii = 0; ii< result.Length ; ii++)
            {
                if (processed[ii] != 0.0)
                {
                    result[ii] = processed[ii];
                }
            }
            Caruso.Visualizer.Plot(result,"Convolution");
        }

        public static void LumaFromLeft( string filepath)
        {
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra,Byte>(source);
            double[] lumas = Luminousity.RepresentativeLuminousity(image, 5, 10, Luminousity.Direction.FromRight);
            Caruso.Visualizer.Plot(lumas, "RepresentativeLuminousity Values "+filepath.Split('\\').Last());
        }
    }
}
