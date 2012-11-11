using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;
using Picasso;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicassoSample
{
    class PreprocessingSample
    {
        public static void GetBlobsFromImage(String filepath)
        {
            string imagesrc = filepath;
            Bitmap source = new Bitmap(imagesrc);
            Bitmap Mask = Preprocessing.FloodFill(source, 100, 100, 100);
            List<Bitmap> extractedobj = Preprocessing.ExtractImages(source, Mask);
            // Display to the User
            var result = new Image<Bgr, Byte>(source);

            int ii = 0;
            foreach(Bitmap bm in extractedobj)
            {
                bm = Preprocessing.Orient(bm);
                bm.Save("image" + ii++ + ".jpg");
            }

            Emgu.CV.Image<Bgra, Byte> image = new Image<Bgra, byte>(Mask);
            ImageViewer display = new ImageViewer(image, "Mask");
            var scale = Math.Min(800.0 / (double)result.Height, 800.0 / (double)result.Width);
            display.ImageBox.SetZoomScale(scale, new Point(10, 10));
            display.ShowDialog();

            // Save to Working Dir
        }

        public static void GetFloodFillMask(String filepath)
        {
            string imagesrc = filepath;
            var start = System.DateTime.Now;
            Bitmap source = new Bitmap(imagesrc);
            Bitmap mask = Preprocessing.FloodFill(source, 0, 0, 110);

            // Display to the User
            var result = new Image<Bgr, Byte>(mask);
            ImageViewer display = new ImageViewer(result, "Mask");
            var scale = Math.Min(800.0 / (double)result.Height, 800.0 / (double)result.Width);
            display.ImageBox.SetZoomScale(scale, new Point(10, 10));
            var stop = System.DateTime.Now;
            var difference = stop - start;
            Console.WriteLine("Total Time :"+ difference.ToString() );
            Console.WriteLine("Total Pixels: "+ source.Width*source.Height);
            display.ShowDialog();
        }
    }
}
