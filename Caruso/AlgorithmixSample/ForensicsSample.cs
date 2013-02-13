#region

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Algorithmix;
using Algorithmix.Forensics;
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;
using Algorithmix.TestTools;

#endregion

namespace CarusoSample
{
    internal class ForensicsSample
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void RunWithDrive()
        {
            ShredFactory(Path.Combine(Drive.GetDriveRoot(), @"ArtificialTest\HTTPDocumentScanned\image0.png"));
            ShredFactory(Path.Combine(Drive.GetDriveRoot(), @"ArtificialTest\HTTPDocumentScanned\image1.png"));
        }

        public static void ShredFactory(string filepath)
        {
            Shred shred = new Shred(filepath, false);
            shred.VisualizeChamfers(Direction.FromLeft);
            shred.VisualizeChamfers(Direction.FromRight);
            //shred.VisualizeChamfers(Direction.FromTop);
            //shred.VisualizeChamfers(Direction.FromBottom);
        }

        public static void ChamferFromLeft(string filepath)
        {
            logger.Trace("Starting Chamfer From Left");
            double[] kernel = new[] {-1.0, 0.0, 1.0};
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra, Byte>(source);
            double[] lumas = Luminousity.RepresentativeLuminousity(image, 2, 4, Direction.FromLeft);
            int[] indicies = Utility.GetKernelIndicies(kernel, -1);
            var convolution = Utility.Convolute(lumas, kernel, indicies);
            var processed = Utility.Threshold(Utility.Absolute(convolution), 0.3);
            var chamfers = Chamfer.Measure(processed);
            Visualizer.Plot(chamfers, "Convolution Result");
        }

        public static void ConvolutionFromLeft(string filepath)
        {
            logger.Trace("Starting Convolution From Left");
            double[] kernel = new[] {-1.0, 0.0, 1.0};
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra, Byte>(source);
            double[] lumas = Luminousity.RepresentativeLuminousity(image, 1, 4, Direction.FromRight);
            int[] indicies = Utility.GetKernelIndicies(kernel, -1);
            var convolution = Utility.Convolute(lumas, kernel, indicies);
            var result = Utility.Absolute(convolution);
            var processed = Utility.Threshold(Utility.Absolute(convolution), 0.3);
            for (int ii = 0; ii < result.Length; ii++)
            {
                if (processed[ii] != 0.0)
                {
                    result[ii] = processed[ii];
                }
            }
            Visualizer.Plot(result, "Convolution");
        }

        public static void LumaFromLeft(string filepath)
        {
            logger.Trace("Starting Luma From Left");
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra, Byte>(source);
            double[] lumas = Luminousity.RepresentativeLuminousity(image, 5, 10, Direction.FromRight);
            Visualizer.Plot(lumas, "RepresentativeLuminousity Values " + filepath.Split('\\').Last());
        }
    }
}