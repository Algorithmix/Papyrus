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
            /*string path = System.IO.Path.GetRandomFileName();
            string tempPath = System.IO.Path.GetTempPath();
            Directory.CreateDirectory(System.IO.Path.Combine(tempPath, path));
            return tempPath + path + "\\";*/
            return @"C:\users\jeff\Desktop\";
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
            string preprocessString = Workers.Preprocess_Final(inputFile, outPath, false, thresh);
            WriteToLog(preprocessString);
            //CarusoSample.SecondDeliverable.Preprocess_Final(inputFile, outPath, false, thresh);
            // Workers.Preprocess_Final(inputFile, outPath, thresh);
            string reconstructString = Workers.Reconstruct("image", outPath, false);
            WriteToLog(reconstructString);
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
    }
}
