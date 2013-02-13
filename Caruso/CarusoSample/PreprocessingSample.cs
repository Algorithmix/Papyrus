using Algorithmix.Preprocessing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarusoSample
{
    class PreprocessingSample
    {
        public static void GetBlobsFromImage(String filepath, Bgr color)
        {
            string imagesrc = filepath;
            Bitmap source = new Bitmap(imagesrc);
            System.Console.WriteLine("beginning flood fill...");
            Bitmap Mask = Preprocessing.FloodFill(source, 100, 100, 120, color);
            System.Console.WriteLine("flood fill complete...");
            System.Console.WriteLine("extracting objects...");
            List<Bitmap> extractedobj = Preprocessing.ExtractImages(source, Mask);
            System.Console.WriteLine("Extracted " + extractedobj.Count + " objects");
            // Display to the User
            var result = new Image<Bgr, Byte>(source);

            int ii = 0;
            foreach(Bitmap bm in extractedobj)
            {
              //  Bitmap bm2 = Preprocessing.Orient(bm);
                bm.Save("image" + ii++ + ".png");
            }

            System.Console.WriteLine("wrote files to disk");

            Emgu.CV.Image<Bgra, Byte> image = new Image<Bgra, byte>(Mask);
            ImageViewer display = new ImageViewer(image, "Mask");
            var scale = Math.Min(800.0 / (double)result.Height, 800.0 / (double)result.Width);
            display.ImageBox.SetZoomScale(scale, new Point(10, 10));
            display.ShowDialog();

            // Display Each Shred That is extracted
            foreach (var shred in  extractedobj)
            {
                Emgu.CV.Image<Bgra,Byte> cvShred = new Image<Bgra, byte>(shred);
                ImageViewer box = new ImageViewer(cvShred, "Mask");
                var shredScale = Math.Min(800.0 / (double)cvShred.Height, 800.0 / (double)cvShred.Width);
                display.ImageBox.SetZoomScale(shredScale, new Point(10, 10));
                box.ShowDialog();
            }

            // Save to Working Dir
        }

        public static void GetFloodFillMask(String filepath, Bgr color)
        {
            string imagesrc = filepath;
            var start = System.DateTime.Now;
            Bitmap source = new Bitmap(imagesrc);
            Bitmap mask = Preprocessing.FloodFill(source, 0, 0, 110, color);

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
