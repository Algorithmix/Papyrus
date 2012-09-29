using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using AForge;
using AForge.Imaging;

namespace Picasso
{
    public class Heuristics
    {

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
                throw new ArgumentException("Border is defined larger then Image Dimensions allow for");
            } 

            // Number of boarder pixels =  2*(Horizontal)+2*(Vertical)-(4*Overlap)
            //int border_count = 2 * (border * document.Width) + 2 * (border * document.Height) - 4 * (border * border);
            var red = new int[Byte.MaxValue+1];
            var green = new int[Byte.MaxValue + 1];
            var blue = new int[Byte.MaxValue + 1];

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

            // Get the most prevelant color
            for (int ii = 0; ii <= Byte.MaxValue; ii++)
            {
                max_red =  red[max_red] < red[ii] ? ii : max_red ;
                max_green = green[max_green] < green[ii] ? ii : max_green; 
                max_blue = blue[max_blue] < blue[ii] ? ii : max_blue;
            }

            // Return the BG color
            return new Bgr(max_blue, max_green, max_red);
        }
    }
}
