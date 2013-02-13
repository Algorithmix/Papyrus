using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;
using Picasso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicassoSample
{
    class HeuristicsSample
    {
        public static void DetectBackground(String filepath) 
        {
            Console.WriteLine("Running Heuristic Background Detector");

            var bg_color = Heuristics.DetectBackground( new System.Drawing.Bitmap(filepath));
            Console.WriteLine("R,G,B : " + bg_color.Red + "," + bg_color.Green + "," + bg_color.Blue);
            var display = new ImageViewer(new Emgu.CV.Image<Bgr,Byte>(600,600, bg_color), "Heuristic Background Detection Result");
            display.ShowDialog();
        }
    }
}
