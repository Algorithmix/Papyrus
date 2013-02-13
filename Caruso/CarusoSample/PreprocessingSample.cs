#region

using System;
using System.Collections.Generic;
using System.Drawing;
using Algorithmix.Preprocessing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

#endregion

namespace CarusoSample
{
    internal class PreprocessingSample
    {
        public static void GetBlobsFromImage(String filepath, Bgr color)
        {
            string imagesrc = filepath;
            Bitmap source = new Bitmap(imagesrc);
            Console.WriteLine("beginning flood fill...");
            Bitmap Mask = Preprocessing.FloodFill(source, 100, 100, 120, color);
            Console.WriteLine("flood fill complete...");
            Console.WriteLine("extracting objects...");
            List<Bitmap> extractedobj = Preprocessing.ExtractImages(source, Mask);
            Console.WriteLine("Extracted " + extractedobj.Count + " objects");
            // Display to the User
            var result = new Image<Bgr, Byte>(source);

            int ii = 0;
            foreach (Bitmap bm in extractedobj)
            {
                //  Bitmap bm2 = Preprocessing.Orient(bm);
                bm.Save("image" + ii++ + ".png");
            }

            Console.WriteLine("wrote files to disk");

            Image<Bgra, Byte> image = new Image<Bgra, byte>(Mask);
            ImageViewer display = new ImageViewer(image, "Mask");
            var scale = Math.Min(800.0/result.Height, 800.0/result.Width);
            display.ImageBox.SetZoomScale(scale, new Point(10, 10));
            display.ShowDialog();

            // Display Each Shred That is extracted
            foreach (var shred in  extractedobj)
            {
                Image<Bgra, Byte> cvShred = new Image<Bgra, byte>(shred);
                ImageViewer box = new ImageViewer(cvShred, "Mask");
                var shredScale = Math.Min(800.0/cvShred.Height, 800.0/cvShred.Width);
                display.ImageBox.SetZoomScale(shredScale, new Point(10, 10));
                box.ShowDialog();
            }

            // Save to Working Dir
        }

        public static void GetFloodFillMask(String filepath, Bgr color)
        {
            string imagesrc = filepath;
            var start = DateTime.Now;
            Bitmap source = new Bitmap(imagesrc);
            Bitmap mask = Preprocessing.FloodFill(source, 0, 0, 110, color);

            // Display to the User
            var result = new Image<Bgr, Byte>(mask);
            ImageViewer display = new ImageViewer(result, "Mask");
            var scale = Math.Min(800.0/result.Height, 800.0/result.Width);
            display.ImageBox.SetZoomScale(scale, new Point(10, 10));
            var stop = DateTime.Now;
            var difference = stop - start;
            Console.WriteLine("Total Time :" + difference.ToString());
            Console.WriteLine("Total Pixels: " + source.Width*source.Height);
            display.ShowDialog();
        }
    }
}