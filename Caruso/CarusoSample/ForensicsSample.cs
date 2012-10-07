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
            double[] lumas = Luminousity.Luma(image, 5, 10, Luminousity.Direction.fromleft);
            int[] indicies = Utility.GetKernelIndicies(kernel, -1);
            var convolution = Utility.Convolute(lumas, kernel, indicies);
            var processed = Utility.Threshold(Utility.Absolute(convolution), 0.6);
            var chamfers = Utility.Chamfer(processed);
            Caruso.Visualizer.Plot( chamfers , "Convolution Result");
        }

        public static void ConvolutionFromLeft(string filepath)
        {
            double[] kernel = new Double[] { -1.0, 0.0, 1.0 };
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra, Byte>(source);
            double[] lumas = Luminousity.Luma(image, 5, 10, Luminousity.Direction.fromleft);
            int[] indicies = Utility.GetKernelIndicies(kernel, -1);
            var  convolution = Utility.Convolute( lumas, kernel, indicies);
            var processed = Utility.Threshold(Utility.Absolute(convolution), 0.6);
            Caruso.Visualizer.Plot( processed, "Convolution Result");
        }

        public static void LumaFromLeft( string filepath)
        {
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra,Byte>(source);
            double[] lumas = Luminousity.Luma(image,5,10, Luminousity.Direction.fromleft);
            Caruso.Visualizer.Plot(lumas, "Luma Values "+filepath.Split('\\').Last());
        }
    }
}
