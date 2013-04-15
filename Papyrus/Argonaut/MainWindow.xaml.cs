using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Algorithmix.UnitTest;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using MahApps.Metro.Controls;
using Algorithmix.Forensics;
using Algorithmix.Preprocessing;
using Algorithmix;
using Algorithmix.Reconstruction;
using Algorithmix.Tools;
using Color = System.Windows.Media.Color;
using Path = System.IO.Path;
using Point = System.Windows.Point;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        TextWriter _writer = null;
        private Thread WorkerThread;
        private string outPath;
        private int thresh;
        private string inputFile;
        private string updateLog;
        public string GetTempDirectory()
        {
            string path = System.IO.Path.GetRandomFileName();
            string tempPath = System.IO.Path.GetTempPath();
            Directory.CreateDirectory(System.IO.Path.Combine(tempPath, path));
            return tempPath + path + "\\";
            //return @"C:\users\jeff\Desktop\";
        }

        private delegate void LogUpdate(string text);
        public void WriteToLog(string logText)
        {

            if (this.bReconstruct.Dispatcher.CheckAccess())
            {
                tbLog.Text += "\n" + logText;
                tbLog.ScrollToEnd();
            }
            else
            {
                this.tbLog.Dispatcher.BeginInvoke(
 System.Windows.Threading.DispatcherPriority.Normal,
 new LogUpdate(WriteToLog), logText);

            }
        }


        public MainWindow()
        {
            InitializeComponent();
            sliderFill.Value = 50;
        }

        private void AlgorithmixButtonClick(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void sliderFill_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lFloodFill_Value.Content = sliderFill.Value.ToString();
        }

        private void bBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbFileName.Text = ofd.FileName;
            }
            WriteToLog("[LOG] Selected File : " + ofd.FileName);
        }

        private void bReconstruct_Click(object sender, RoutedEventArgs e)
        {
            WorkerThread = new Thread(new ThreadStart(Do_Work));
            string inFile = tbFileName.Text;
            //first check to see that the file is an input file exists
            if (!File.Exists(inFile))
            {
                WriteToLog("[ERROR] Input File " + inFile + " does not exist!");
                return;
            }
            outPath = GetTempDirectory();
            thresh = (int)sliderFill.Value;
            inputFile = inFile;
            bReconstruct.IsEnabled = false;
            // CarusoSample.SecondDeliverable.Preprocess_Final(inputFile, outPath, false, thresh);
            WorkerThread.Start();
            //Workers.Preprocess_Final(inputFile, outPath, thresh);
            //Workers.Reconstruct("image", outPath, false);
        }

        private void fArgonaut_Loaded(object sender, RoutedEventArgs e)
        {
            // Instantiate the writer
            _writer = new TextBoxStreamWriter(tbLog);
            // Redirect the out Console stream
            // Console.SetOut(_writer);

            Console.WriteLine("Now redirecting output to the text box");
            WorkerThread = new Thread(new ThreadStart(Do_Work));
        }

        private void Do_Work()
        {
            string preprocessString = Preprocess_Final(inputFile, outPath, false, thresh);
            //WriteToLog(preprocessString);
            //CarusoSample.SecondDeliverable.Preprocess_Final(inputFile, outPath, false, thresh);
            // Workers.Preprocess_Final(inputFile, outPath, thresh);
            string reconstructString = Reconstruct("image", outPath, false);
            //WriteToLog(reconstructString);
            EnablebReconstruct();
        }
        private delegate void GuiUpdate();
        private void EnablebReconstruct()
        {
            if (this.bReconstruct.Dispatcher.CheckAccess())
            {
                this.bReconstruct.IsEnabled = true;
            }
            else
            {
                this.bReconstruct.Dispatcher.Invoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new GuiUpdate(this.EnablebReconstruct));
            }
        }

        #region WORKER_THREADS
        public string Reconstruct(string prefix, string dir, bool display)
        {
            StringBuilder sb = new StringBuilder();
            WriteToLog("Loading Shreds");
            var shreds = Shred.Factory(prefix, dir, false);

            WriteToLog("Comparing And Clusering");
            var results = Reconstructor.NaiveKruskalAlgorithm(shreds);

            WriteToLog("Exporting Results");
            NaiveKruskalTests.ExportResult((Cluster)results.First().Root(), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "output.png"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "json.js"));
            return sb.ToString();
        }


        public string Preprocess_Final(string filepath, string outPath, bool displayMode, int thresholding)
        {
            StringBuilder sb = new StringBuilder();
            displayMode = false;
            WriteToLog("Loading Image : " + filepath);
            Bitmap load = new Bitmap(filepath);

            var start = DateTime.Now;
            WriteToLog("Running Background Detection ...");
            Bgr backgroundColor = Heuristics.DetectBackground(load, 20);
            WriteToLog("Detected Background : " + backgroundColor.ToString());
            WriteToLog("Detected Background Completed in " + (DateTime.Now - start).TotalSeconds.ToString() +
                              " seconds");


            var backgroundGuess = new Image<Bgr, Byte>(100, 100, backgroundColor);


            WriteToLog("Running Shred Extraction ");
            WriteToLog("Image Size : " + load.Height * load.Width + " Pixels");

            string imagesrc = filepath;
            Bitmap source = new Bitmap(imagesrc);
            WriteToLog("beginning flood fill...");
            System.Drawing.Point startPoint = Heuristics.GetStartingFloodFillPoint(source,
                                                               System.Drawing.Color.FromArgb(255, (int)backgroundColor.Red,
                                                                              (int)backgroundColor.Green,
                                                                              (int)backgroundColor.Blue));
            Bitmap Mask = Preprocessing.FloodFill(source, startPoint.X, startPoint.Y, 50, backgroundColor);
            WriteToLog("flood fill complete...");
            WriteToLog("extracting objects...");
            List<Bitmap> extractedobj = Preprocessing.ExtractImages(source, Mask);
            WriteToLog("Extracted " + extractedobj.Count + " objects");


            // Prompt for input directory and Write to file

            Console.Write("Enter Output Directory (Default is Working): ");
            string directory = outPath;// Console.ReadLine();

            if (String.IsNullOrEmpty(directory) || !Directory.Exists(directory))
            {
                WriteToLog("Writing to Working Directory");
                directory = string.Empty;
            }
            else
            {
                directory += "\\";
            }

            WriteToLog("Rotating Images");
            int ii = 0;
            int maxLen = extractedobj.Count.ToString().Length;
            foreach (Bitmap bm in extractedobj)
            {
                Bitmap bm2 = Preprocessing.Orient(bm);
                bm2.Save(directory + "image" + ii.ToString("D" + maxLen) + ".png");
                ii++;
            }
            WriteToLog("Wrote Files To Disk");
            return sb.ToString();
        }
        #endregion
    }
}
