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
        public static void exec(String filepath)
        {
            string imagesrc = filepath;
            Bitmap source = new Bitmap(imagesrc);
            Bitmap Mask = Utility.FloodFill(source, 20, 20, 99);
            List<Bitmap> extractedobj = Utility.ExtractImages(source, Mask);
            // Display to the User
            var result = new Image<Bgr, Byte>(source);
            int ii = 0;
            foreach(Bitmap bm in extractedobj)
            {
                bm.Save("image" + ii++ + ".jpg");
            }
            Emgu.CV.Image<Bgra, Byte> image = new Image<Bgra, byte>(Mask);
            ImageViewer display = new ImageViewer(image, "Mask");
            var scale = Math.Min(800.0 / (double)result.Height, 800.0 / (double)result.Width);
            display.ImageBox.SetZoomScale(scale, new Point(10, 10));
            display.ShowDialog();

            // Save to Working Dir
        }
    }
}
