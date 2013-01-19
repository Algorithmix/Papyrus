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
using NLog;
using NLog.Config;
using NLog.Targets;

namespace CarusoSample
{
    class ForensicsSample
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        public static void ShredFactory( string filepath)
        {
            Caruso.Shred shred = new Caruso.Shred(filepath,false);
            shred.VisualizeChamfers(Direction.FromLeft);
            shred.VisualizeChamfers(Direction.FromRight);
            shred.VisualizeChamfers(Direction.FromTop);
            shred.VisualizeChamfers(Direction.FromBottom);
        }

        public static void ChamferFromLeft(string filepath)
        {
            logger.Trace("Starting Chamfer From Left");
            double[] kernel = new Double[] { -1.0, 0.0, 1.0 };
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra, Byte>(source);
            double[] lumas = Luminousity.RepresentativeLuminousity(image, 2, 4, Caruso.Direction.FromLeft);
            int[] indicies = Utility.GetKernelIndicies(kernel, -1);
            var convolution = Utility.Convolute(lumas, kernel, indicies);
            var processed = Utility.Threshold(Utility.Absolute(convolution), 0.3);
            var chamfers = Chamfer.Measure(processed);
            Caruso.Visualizer.Plot( chamfers , "Convolution Result");
        }

        public static void ConvolutionFromLeft(string filepath)
        {
            logger.Trace("Starting Convolution From Left");
            double[] kernel = new Double[] { -1.0, 0.0, 1.0 };
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra, Byte>(source);
            double[] lumas = Luminousity.RepresentativeLuminousity(image, 1, 4, Caruso.Direction.FromRight);
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
            logger.Trace("Starting Luma From Left");
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra,Byte>(source);
            double[] lumas = Luminousity.RepresentativeLuminousity(image, 5, 10, Caruso.Direction.FromRight);
            Caruso.Visualizer.Plot(lumas, "RepresentativeLuminousity Values "+filepath.Split('\\').Last());
        }
    }
}
