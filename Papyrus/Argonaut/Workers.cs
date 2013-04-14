using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algorithmix;
using Algorithmix.Preprocessing;
using Algorithmix.Reconstruction;
using Algorithmix.UnitTest;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace Argonaut
{
    class Workers
    {
        public static void Reconstruct(string prefix, string dir, bool display)
        {
            Console.WriteLine("Loading Shreds");
            var shreds = Shred.Factory(prefix, dir, false);

            Console.WriteLine("Comparing And Clusering");
            var results = Reconstructor.NaiveKruskalAlgorithm(shreds);

            Console.WriteLine("Exporting Results");
            NaiveKruskalTests.ExportResult((Cluster)results.First().Root(), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output.png"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "json.js"));
        }
        public static void Preprocess_Final(string filepath, string outPath, bool displayMode, int thresholding)
        {
            displayMode = false;
            Console.WriteLine("Loading Image : " + filepath);
            Bitmap load = new Bitmap(filepath);

            var start = DateTime.Now;
            Console.WriteLine("Running Background Detection ...");
            Bgr backgroundColor = Heuristics.DetectBackground(load, 20);
            Console.WriteLine("Detected Background : " + backgroundColor.ToString());
            Console.WriteLine("Detected Background Completed in " + (DateTime.Now - start).TotalSeconds.ToString() +
                              " seconds");


            var backgroundGuess = new Image<Bgr, Byte>(100, 100, backgroundColor);


            Console.WriteLine("Running Shred Extraction ");
            Console.WriteLine("Image Size : " + load.Height * load.Width + " Pixels");

            string imagesrc = filepath;
            Bitmap source = new Bitmap(imagesrc);
            Console.WriteLine("beginning flood fill...");
            Point startPoint = Heuristics.GetStartingFloodFillPoint(source,
                                                               Color.FromArgb(255, (int)backgroundColor.Red,
                                                                              (int)backgroundColor.Green,
                                                                              (int)backgroundColor.Blue));
            Bitmap Mask = Preprocessing.FloodFill(source, startPoint.X, startPoint.Y, 50, backgroundColor);
            Console.WriteLine("flood fill complete...");
            Console.WriteLine("extracting objects...");
            List<Bitmap> extractedobj = Preprocessing.ExtractImages(source, Mask);
            Console.WriteLine("Extracted " + extractedobj.Count + " objects");


            // Prompt for input directory and Write to file

            Console.Write("Enter Output Directory (Default is Working): ");
            string directory = outPath;// Console.ReadLine();

            if (String.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                Console.WriteLine("Writing to Working Directory");
                directory = string.Empty;
            }
            else
            {
                directory += "\\";
            }

            Console.WriteLine("Rotating Images");
            int ii = 0;
            int maxLen = extractedobj.Count.ToString().Length;
            foreach (Bitmap bm in extractedobj)
            {
                Bitmap bm2 = Preprocessing.Orient(bm);
                bm2.Save(directory + "image" + ii.ToString("D" + maxLen) + ".png");
                ii++;
            }
            Console.WriteLine("Wrote Files To Disk");
        }

    }
}
