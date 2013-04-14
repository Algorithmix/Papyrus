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
        public static string Reconstruct(string prefix, string dir, bool display)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Loading Shreds");
            var shreds = Shred.Factory(prefix, dir, false);

            sb.AppendLine("Comparing And Clusering");
            var results = Reconstructor.NaiveKruskalAlgorithm(shreds);

            sb.AppendLine("Exporting Results");
            NaiveKruskalTests.ExportResult((Cluster)results.First().Root(), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output.png"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "json.js"));
            return sb.ToString();
        }


        public static string Preprocess_Final(string filepath, string outPath, bool displayMode, int thresholding)
        {
            StringBuilder sb = new StringBuilder();
            displayMode = false;
            sb.AppendLine("Loading Image : " + filepath);
            Bitmap load = new Bitmap(filepath);

            var start = DateTime.Now;
            sb.AppendLine("Running Background Detection ...");
            Bgr backgroundColor = Heuristics.DetectBackground(load, 20);
            sb.AppendLine("Detected Background : " + backgroundColor.ToString());
            sb.AppendLine("Detected Background Completed in " + (DateTime.Now - start).TotalSeconds.ToString() +
                              " seconds");


            var backgroundGuess = new Image<Bgr, Byte>(100, 100, backgroundColor);


            sb.AppendLine("Running Shred Extraction ");
            sb.AppendLine("Image Size : " + load.Height * load.Width + " Pixels");

            string imagesrc = filepath;
            Bitmap source = new Bitmap(imagesrc);
            sb.AppendLine("beginning flood fill...");
            Point startPoint = Heuristics.GetStartingFloodFillPoint(source,
                                                               Color.FromArgb(255, (int)backgroundColor.Red,
                                                                              (int)backgroundColor.Green,
                                                                              (int)backgroundColor.Blue));
            Bitmap Mask = Preprocessing.FloodFill(source, startPoint.X, startPoint.Y, 50, backgroundColor);
            sb.AppendLine("flood fill complete...");
            sb.AppendLine("extracting objects...");
            List<Bitmap> extractedobj = Preprocessing.ExtractImages(source, Mask);
            sb.AppendLine("Extracted " + extractedobj.Count + " objects");


            // Prompt for input directory and Write to file

            Console.Write("Enter Output Directory (Default is Working): ");
            string directory = outPath;// Console.ReadLine();

            if (String.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                sb.AppendLine("Writing to Working Directory");
                directory = string.Empty;
            }
            else
            {
                directory += "\\";
            }

            sb.AppendLine("Rotating Images");
            int ii = 0;
            int maxLen = extractedobj.Count.ToString().Length;
            foreach (Bitmap bm in extractedobj)
            {
                Bitmap bm2 = Preprocessing.Orient(bm);
                bm2.Save(directory + "image" + ii.ToString("D" + maxLen) + ".png");
                ii++;
            }
            sb.AppendLine("Wrote Files To Disk");
            return sb.ToString();
        }

    }
}
