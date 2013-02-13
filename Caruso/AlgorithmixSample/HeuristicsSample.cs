#region

using System;
using System.Drawing;
using Algorithmix.Preprocessing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

#endregion

namespace CarusoSample
{
    internal class HeuristicsSample
    {
        public static void DetectBackground(String filepath)
        {
            Console.WriteLine("Running Heuristic Background Detector");

            var bg_color = Heuristics.DetectBackground(new Bitmap(filepath));
            Console.WriteLine("R,G,B : " + bg_color.Red + "," + bg_color.Green + "," + bg_color.Blue);
            var display = new ImageViewer(new Image<Bgr, Byte>(600, 600, bg_color),
                                          "Heuristic Background Detection Result");
            display.ShowDialog();
        }
    }
}