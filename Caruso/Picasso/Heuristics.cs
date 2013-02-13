using AForge;
using AForge.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picasso
{
    public class Heuristics
    {
        public static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Given a Document, this heuristic attempts to determine what the background is by finding the most common rgb
        /// colors in a border around the image
        /// </summary>
        /// <param name="document">Image to be analyzed</param>
        /// <param name="border">Size of the border in pixels</param>
        /// <returns>Bgr color best-guess for background color</returns>
        public static Bgr DetectBackground( Bitmap document , int border = 10 )
        {
            if ( (border*2) > document.Height ||  (border*2) > document.Width )
            {
                log.Error("Border is defined larger than the Image");
                log.Error("Border is"+border);
                log.Error("Image.Width =" + document.Width);
                log.Error("Image.Height=" + document.Height);
                throw new ArgumentException("Border is defined larger then Image Dimensions allow for");
            } 

            // Number of boarder pixels =  2*(Horizontal)+2*(Vertical)-(4*Overlap)
            //int border_count = 2 * (border * document.Width) + 2 * (border * document.Height) - 4 * (border * border);
            var red = new int[Byte.MaxValue+1];
            var green = new int[Byte.MaxValue + 1];
            var blue = new int[Byte.MaxValue + 1];

            log.Info("Scanning Image and counting pixels in the frame");
            // Scan all pixels with border length of the image boundaries
            for (int row = 0; row < document.Height; row++)
            {
                for (int col = 0; col < document.Width; col++)
                {
                    if ( (col < border || col >= (document.Width - border) ) ||
                         (row < border || row >= (document.Height - border) ))
                    {
                        red[document.GetPixel(col, row).R]++;
                        green[document.GetPixel(col, row).G]++;
                        blue[document.GetPixel(col, row).B]++;
                    }
                }
            }

            int max_red = 0;
            int max_green = 0;
            int max_blue = 0;

            log.Info("Determing most prevalant bg color");
            for (int ii = 0; ii <= Byte.MaxValue; ii++)
            {
                max_red =  red[max_red] < red[ii] ? ii : max_red ;
                max_green = green[max_green] < green[ii] ? ii : max_green; 
                max_blue = blue[max_blue] < blue[ii] ? ii : max_blue;
            }

            log.Info("R: "+max_red);
            log.Info("G: "+max_green);
            log.Info("B: "+max_blue);

            return new Bgr(max_blue, max_green, max_red);
        }
    }
}
