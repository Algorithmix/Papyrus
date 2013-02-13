using AForge;
using AForge.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picasso
{

    public class Utility
    {

        /// <summary>
        /// Takes an integer, returns a BGR color
        /// </summary>
        /// <param name="color">an integer between 0 and 0xFFFFFF inclusive</param>
        /// <returns></returns>
        public static Bgr IntToBgr(int color)
        {
            if (color > 0xFFFFFF)
            {
                throw new IndexOutOfRangeException("Color must be <= 0xFFFFFF");
            }
            int blue = color & (0xFF0000);
            int green = color & (0x00FF00);
            int red = color & (0x0000FF);
            return new Bgr(blue, green, red);
        }

        /// <summary>
        /// Returns the Cartesian Distance between from the first the second parameter
        /// </summary>
        /// <param name="pixelA">Pixel of given color from</param>
        /// <param name="pixelB">Pixel of given color to</param>
        /// <returns>Cartesian Distance</returns>
        public static double Distance(Color pixelA, Color pixelB)
        {
            int red = pixelB.R - pixelA.R;
            int green = pixelB.G - pixelA.G;
            int blue = pixelB.B - pixelA.B;
            return Math.Sqrt((double)(red * red + green * green + blue * blue));
        }

        /// <summary>
        /// Checks if two colors are equal
        /// </summary>
        /// <param name="color1">first color</param>
        /// <param name="color2">second color</param>
        /// <returns>True if equal, false if not</returns>
        public static bool IsEqual(Bgr color1, Bgr color2)
        {
            return (color1.Red == color2.Red
                    && color1.Blue == color2.Blue
                    && color1.Green == color2.Green);
        }

        /// <summary>
        /// Determines the cartesian distance between two colors
        /// </summary>
        /// <param name="color1">First Color</param>
        /// <param name="color2">Second Color</param>
        /// <returns>A double value that indicates difference in colors, 
        /// where zero means no zero and numbers greater indicate a difference</returns>
        public static double Distance(Bgr color1, Bgr color2)
        {
            double r1 = color1.Red;
            double r2 = color2.Red;
            double g1 = color1.Green;
            double g2 = color2.Green;
            double b1 = color1.Blue;
            double b2 = color2.Blue;
            return Math.Sqrt((r1 - r2) * (r1 - r2) + (g1 - g2) * (g1 - g2) + (b1 - b2) * (b1 - b2));
        }

        /// <summary>
        /// Determines whether the given x and y coordinates are bound with in the given the given image
        /// </summary>
        /// <param name="image">Bit map image to checked</param>
        /// <param name="xx">X position of the pixel or Column</param>
        /// <param name="yy">Y position of the pixel or Row</param>
        /// <returns>True if within the bounds, and false if out of bounds</returns>
        public static bool IsBound(Bitmap image, int xx, int yy)
        {
            if (xx >= 0 && xx < image.Width && yy >= 0 && yy < image.Height)
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
