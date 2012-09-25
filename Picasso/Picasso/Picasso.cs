using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

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

        /// <summary>
        /// Flood fill in a BFS manner, so as not to overwhelm the stack
        /// </summary>
        /// <param name="image">the image we wish to fill on</param>
        /// <param name="xpixel">the x pixel to sample from</param>
        /// <param name="ypixel">the y pixel to sample from</param>
        /// <param name="threshold">the threshold of difference</param>
        /// <returns>the background which can be subtracted</returns>
        public static Bitmap FloodFill(Bitmap image, int xpixel, int ypixel, double threshold)
        {
            //create an identically sized "background" image and fill it white
            Emgu.CV.Image<Bgr, Byte> imBackground = new Image<Bgr, byte>(image.Width, image.Height);
            Emgu.CV.Image<Bgr, Byte> imImage = new Image<Bgr, byte>(image);
            Bgr bgrTarget = imImage[xpixel, ypixel];
            Bgr color = new Bgr(255, 255, 255);
            Bgr white = new Bgr(255, 255, 255);
            for (int ii = 0; ii < image.Width; ii++)
            {
                for (int jj = 0; jj < image.Height; jj++)
                {
                    imBackground[jj,ii] = white;
                }
            }
            Queue<Point> pointQueue = new Queue<Point>();
            pointQueue.Enqueue(new Point(xpixel, ypixel));
            Bgr gray = new Bgr(Color.Gray);
            Bgr hotpink = new Bgr(Color.HotPink);
            Point[] pList = new Point[4];
            while (!(pointQueue.Count == 0)) //make sure queue isn't empty
            {
                Point p = pointQueue.Dequeue();
                //add all neighboring points to the a list
                pList[0] = (new Point(p.X, p.Y - 1)); //above
                pList[1] = (new Point(p.X, p.Y + 1)); //below
                pList[2] = (new Point(p.X - 1, p.Y)); //left
                pList[3] = (new Point(p.X + 1, p.Y)); //right
                foreach (Point neighbor in pList)
                {
                    if (!(Bound(image, neighbor.X, neighbor.Y)))
                        continue;
                    color = imBackground[neighbor.Y, neighbor.X];
                    if (IsEqual(white, color) && (Distance(imImage[neighbor.Y, neighbor.X], bgrTarget) < threshold)) //and hasn't been seen before
                    {
                        imBackground[neighbor.Y, neighbor.X] = gray; //set as added to the queue
                        pointQueue.Enqueue(neighbor); //and add to the queue
                    }
                }
                imBackground[p.Y, p.X] = hotpink; //set the pixel to hot pink
            }
            return imBackground.ToBitmap();
        }

        /// <summary>
        /// Checks if two colors are equal
        /// </summary>
        /// <param name="color1">first color</param>
        /// <param name="color2">second color</param>
        /// <returns></returns>
        public static bool IsEqual(Bgr color1, Bgr color2)
        {
            return (color1.Red == color2.Red
                    && color1.Blue == color2.Blue
                    && color1.Green == color2.Green);
        }

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

        public static bool Bound(Bitmap image, int xx, int yy)
        {
            if ( xx >= 0 && xx < image.Width && yy >= 0 && yy < image.Height )
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
