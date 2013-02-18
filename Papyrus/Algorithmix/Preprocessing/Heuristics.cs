using System.Drawing.Imaging;
using AForge;
using AForge.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.Util;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Imaging;
using AForge;
using AForge.Math;

namespace Algorithmix.Preprocessing
{
    public class Heuristics
    {
        public static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Given a Document, this heuristic attempts to determine what the background is by finding the most common rgb
        /// colors in a border around the image
        /// </summary>
        /// <param name="document">Image to be analyzed</param>
        /// <param name="border">Size of the border in percent</param>
        /// <returns>Bgr color best-guess for background color</returns>
        public static Bgr DetectBackground( Bitmap document , int border = 10 )
        {
            double border2 = (double)border / 100f;
            System.Drawing.Point tl = new System.Drawing.Point((int)(border2*document.Width), (int)(border2*document.Height));
            System.Drawing.Point br = new System.Drawing.Point((int) (document.Width - (border2 * document.Width)), (int)(document.Height - (border2*document.Height)));
            Rectangle rect = new Rectangle(tl.X, tl.Y, br.X-tl.X, br.Y - tl.Y);
            UnmanagedImage blackened = UnmanagedImage.FromManagedImage(document);
            AForge.Imaging.Drawing.FillRectangle(blackened, rect, Color.Black);
            Bitmap blacknew = blackened.ToManagedImage();
            AForge.Imaging.ImageStatistics stat = new ImageStatistics(blacknew);
            Histogram red = stat.RedWithoutBlack;
            Histogram green = stat.GreenWithoutBlack;
            Histogram blue = stat.BlueWithoutBlack;
            int indexR = (int)red.Median;
            int indexB = (int)blue.Median;
            int indexG = (int)green.Median;
            Emgu.CV.Image<Bgra, Byte> blackcv = new Image<Bgra, byte>(blacknew);

            return new Bgr((double) indexB, (double) indexG, (double) indexR);
        }
        public static int CompareColors(Color a, Color b)
        {
            return 100 * (int)(
                1.0 - ((double)(
                    Math.Abs(a.R - b.R) +
                    Math.Abs(a.G - b.G) +
                    Math.Abs(a.B - b.B)
                ) / (256.0 * 3))
            );
        }

        public static System.Drawing.Point GetStartingFloodFillPoint(Bitmap document, Color color)
        {
            int mindiff = 5;
            for (int ii = 0; ii < document.Height; ii++)
            {
                for (int jj = 0; jj < document.Width; jj++)
                {
                    Color sample = document.GetPixel(jj, ii);
                    if (CompareColors(color, sample) <= mindiff)
                    {
                        return new System.Drawing.Point(jj, ii);
                    }
                }
            }
            return new System.Drawing.Point(100, 100); //just because we failed :(
        }

    }
}
