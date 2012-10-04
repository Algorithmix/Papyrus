using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using Caruso;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.Util;

namespace CarusoSample
{
    class ForensicsSample
    {
        public static void ConvolutionFromLeft(string filepath)
        {
            double[] kernel = new Double[] { -1.0, 0.0, 1.0 };
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra, Byte>(source);
            double[] lumas = Forensics.Luma(image, 5, 10, Forensics.Direction.fromleft);
            int[] indicies = Forensics.GetKernelIndicies(kernel, -1);
            double[] convolution = Forensics.Convolute( lumas, kernel, indicies);
            Caruso.Visualizer.Plot(convolution, "Convolution Result");
        }

        public static void LumaFromLeft( string filepath)
        {
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra,Byte>(source);
            double[] lumas = Forensics.Luma(image,5,10, Forensics.Direction.fromleft);
            Caruso.Visualizer.Plot(lumas, "Luma Values "+filepath.Split('\\').Last());
        }
    }
}
