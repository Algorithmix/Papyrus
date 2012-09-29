using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Picasso;

namespace PicassoSample
{
    class BackgroundDetection
    {
        public static void exec(String filepath) 
        {
            Console.WriteLine("Running Heuristic Background Detector");

            var bg_color = Heuristics.DetectBackground( new System.Drawing.Bitmap(filepath));
            Console.WriteLine("R,G,B : " + bg_color.Red + "," + bg_color.Green + "," + bg_color.Blue);
            var display = new ImageViewer(new Emgu.CV.Image<Bgr,Byte>(600,600, bg_color), "Heuristic Background Detection Result");
            display.ShowDialog();
        }
    }
}
