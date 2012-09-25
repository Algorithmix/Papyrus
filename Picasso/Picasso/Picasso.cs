using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Picasso
{

    public static class BitmapExtension
    { 

    }

    public class Utility
    {
        /// <summary>
        /// Returns the Cartesian Distance between from the first the second parameter
        /// </summary>
        /// <param name="pixelA">Pixel of given color from</param>
        /// <param name="pixelB">Pixel of given color to</param>
        /// <returns>Cartesian Distance</returns>
        public static double Distance( Color pixelA, Color pixelB  )
        {
            int red = pixelB.R - pixelA.R ;
            int green = pixelB.G - pixelA.G;
            int blue = pixelB.B - pixelA.B;
            return Math.Sqrt((double)(red * red + green * green + blue * blue));
        }


        public static bool Bound(Bitmap image, int xx, int yy)
        {
            if ( xx > 0 && xx < image.Width && yy>0 && yy<image.Height )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
