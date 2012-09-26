using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Picasso;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.UI;

namespace PicassoSample
{
    class Program
    {
        static void Main(string[] args)
        {
            string imagesrc = args[0];
            Bitmap source = new Bitmap(imagesrc);
            Bitmap mask = Utility.FloodFill(source, 0, 0, 95);

            // Display to the User
            var result = new Image<Bgr, Byte>(mask);
            ImageViewer display = new ImageViewer(result, "Mask");
            var scale = Math.Min(800.0 / (double)result.Height, 800.0 / (double)result.Width);
            display.ImageBox.SetZoomScale(scale, new Point(10, 10));
            display.ShowDialog();

            // Save to Working Dir
            mask.Save("mask.png");
        }
    }
}
