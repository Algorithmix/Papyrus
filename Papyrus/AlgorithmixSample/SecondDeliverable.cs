#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Algorithmix;
using Algorithmix.Preprocessing;
using Algorithmix.Reconstruction;
using Algorithmix.UnitTest;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

#endregion

namespace CarusoSample
{
    public class SecondDeliverable
    {
        public static void Run()
        {
            Console.Write("Preprocessing or Deshredding [p/d]");
            var mode = Console.ReadLine();
            if (mode == null) return;
            if (mode.StartsWith("p"))
            {
                Console.WriteLine("Prepocessing Mode");
                string file = null;
                while (file == null || !File.Exists(file))
                {
                    Console.Write("Enter File Name: ");
                    file = Console.ReadLine();
                }
                Console.WriteLine("File found " + file);
                PreProcess(file, false);
                Console.WriteLine("Completed Preprocessing Please See ");
            }
            else if (mode.StartsWith("d"))
            {
                string directory = null;
                while (directory == null || !Directory.Exists(directory))
                {
                    Console.WriteLine("Please input Directory to Scan");
                    directory = Console.ReadLine();
                }
                string prefix = null;
                while (prefix == null)
                {
                    Console.WriteLine("Please Select a Prefix to Scan: ");
                    prefix = Console.ReadLine();
                }

                PromptFileType();
                Reconstruct(prefix, directory, false);
                Console.WriteLine("Completed Reconstruction");
            }
            else
            {
                Console.WriteLine("UnRecognized Directive");
            }
            Console.WriteLine("hit enter to exit");
            Console.ReadLine();
        }

        private static void PromptFileType()
        {
            Console.Write("Enter Test Type Primitive/Artifical/Authentic [p/r/a]: ");
            string type = Console.ReadLine();
            if (type == null) return;
            if (type.StartsWith("p"))
            {
                Shred.BUFFER = 0;
                Shred.SAMPLE_SIZE = 1;
            }
            else if (type.StartsWith("r"))
            {
                Shred.BUFFER = 0;
                Shred.SAMPLE_SIZE = 4;
            }
        }

        private static void Reconstruct(string prefix, string dir, bool display)
        {
            Console.WriteLine("Loading Shreds");
            var shreds = Shred.Factory(prefix, dir, false);

            Console.WriteLine("Comparing And Clusering");
            var results = Reconstructor.NaiveKruskalAlgorithm(shreds);

            Console.WriteLine("Exporting Results");
            NaiveKruskalTests.ExportResult((Cluster)results.First().Root(), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output.png"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "json.js"));
        }

        private static void PreProcess(string filepath, bool displayMode)
        {
            Console.WriteLine("Loading Image : " + filepath);
            Bitmap load = new Bitmap(filepath);

            var start = DateTime.Now;
            Console.WriteLine("Running Background Detection ...");
            Bgr backgroundColor = Heuristics.DetectBackground(load, 20);
            Console.WriteLine("Detected Background : " + backgroundColor.ToString());
            Console.WriteLine("Detected Background Completed in " + (DateTime.Now - start).TotalSeconds.ToString() +
                              " seconds");


            var backgroundGuess = new Image<Bgr, Byte>(100, 100, backgroundColor);

            if (displayMode)
            {
                ImageViewer display = new ImageViewer(backgroundGuess, "Mask");
                display.ShowDialog();
            }

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

            if (displayMode)
            {
                // Display to the User
                var result = new Image<Bgr, Byte>(source);


                Image<Bgra, Byte> image = new Image<Bgra, byte>(Mask);
                ImageViewer maskView = new ImageViewer(image, "Mask");
                var scale = Math.Min(800.0 / result.Height, 800.0 / result.Width);
                maskView.ImageBox.SetZoomScale(scale, new Point(10, 10));
                maskView.ShowDialog();

                // Display Each Shred That is extracted
                foreach (var shred in extractedobj)
                {
                    Image<Bgra, Byte> cvShred = new Image<Bgra, byte>(shred);
                    ImageViewer box = new ImageViewer(cvShred, "Mask");
                    var shredScale = Math.Min(800.0 / cvShred.Height, 800.0 / cvShred.Width);
                    box.ImageBox.SetZoomScale(shredScale, new Point(10, 10));
                    box.ShowDialog();
                }
            }

            // Prompt for input directory and Write to file

            Console.Write("Enter Output Directory (Default is Working): ");
            string directory = Console.ReadLine();

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