using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;
using NLog;
using Picasso;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicassoSample
{
    class Deliverable
    {
        public static void Test(String[] args)
        {

            Console.Write("Enter FileName: ");
            string filepath  = Console.ReadLine();
            if (!File.Exists( filepath ))
            {
                Console.WriteLine("File Path " + filepath + " does not exist");
                //filepath = @"C:\Users\Algorithmix\Downloads\4shredsc.jpg";
                //Environment.Exit(1);
            }
            Run(filepath);
        }

        public static void Run( string filepath )
        {
            System.Console.WriteLine("Loading Image : " + filepath);
            Bitmap load = new Bitmap(filepath);

            var start = DateTime.Now;
            System.Console.WriteLine("Running Background Detection ...");
            Bgr backgroundColor = Picasso.Heuristics.DetectBackground(load, 83);
            System.Console.WriteLine("Detected Background : " + backgroundColor.ToString());
            System.Console.WriteLine("Detected Background Completed in "+ (DateTime.Now-start).TotalSeconds.ToString() + " seconds");


            var backgroundGuess = new Image<Bgr, Byte>(100, 100, backgroundColor);
            ImageViewer display = new ImageViewer(backgroundGuess, "Mask");
            display.ShowDialog();

            System.Console.WriteLine("Running Shred Extraction ");
            System.Console.WriteLine("Image Size : " + load.Height * load.Width + " Pixels");

            string imagesrc = filepath;
            Bitmap source = new Bitmap(imagesrc);
            System.Console.WriteLine("beginning flood fill...");
            Bitmap Mask = Preprocessing.FloodFill(source, 100, 100, 40, backgroundColor);
            System.Console.WriteLine("flood fill complete...");
            System.Console.WriteLine("extracting objects...");
            List<Bitmap> extractedobj = Preprocessing.ExtractImages(source, Mask);
            System.Console.WriteLine("Extracted " + extractedobj.Count + " objects");
           
            
            // Display to the User
            var result = new Image<Bgr, Byte>(source);



            Emgu.CV.Image<Bgra, Byte> image = new Image<Bgra, byte>(Mask);
            ImageViewer maskView = new ImageViewer(image, "Mask");
            var scale = Math.Min(800.0 / (double)result.Height, 800.0 / (double)result.Width);
            maskView.ImageBox.SetZoomScale(scale, new Point(10, 10));
            maskView.ShowDialog();

            // Display Each Shred That is extracted
            foreach (var shred in extractedobj)
            {
                Emgu.CV.Image<Bgra, Byte> cvShred = new Image<Bgra, byte>(shred);
                ImageViewer box = new ImageViewer(cvShred, "Mask");
                var shredScale = Math.Min(800.0 / (double)cvShred.Height, 800.0 / (double)cvShred.Width);
                display.ImageBox.SetZoomScale(shredScale, new Point(10, 10));
                box.ShowDialog();
            }

            // Prompt for input directory and Write to file
            Console.Write("Enter Output Directory (Default is Working): ");
            string directory = Console.ReadLine();

            if (!Directory.Exists(directory))
            {
                Console.WriteLine("Writing to Working Directory");
                directory = string.Empty;
            }
            else 
            {
                directory += "\\";
            }
 
            System.Console.WriteLine("wrote files to disk");
            int ii = 0;
            StringBuilder sb = new StringBuilder();
            foreach (Bitmap bm in extractedobj)
            {
                Bitmap bm2 = Preprocessing.Orient(bm);
                sb.Append(ii.ToString() + " : width: " + bm.Width + " height: " + bm.Height + " area: " +
                          bm.Height*bm.Width);
                bm2.Save(directory+ "image" + ii++ + ".png");
            }
            using (StreamWriter sw = new StreamWriter(@"C:\Users\Richard\Desktop\imageqr.txt", true))
            {
                sw.WriteLine(sb.ToString());
            }
        }
    }
}
