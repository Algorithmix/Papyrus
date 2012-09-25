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
            Color target = image.GetPixel(xpixel, ypixel);
            //create an identically sized "background" image and fill it white
            Bitmap background = new Bitmap(image.Width, image.Height);
            for(int ii = 0; ii < image.Width; ii++)
            {
                for (int jj = 0; jj < image.Height; jj++)
                {
                    background.SetPixel(ii, jj, Color.White);
                }
            }
            Queue<Point> pointQueue = new Queue<Point>();
            pointQueue.Enqueue(new Point(xpixel, ypixel));
            while (!(pointQueue.Count == 0)) //make sure queue isn't empty
            {
                Point p = pointQueue.Dequeue();
                //add all neighboring points to the a list
                List<Point> pList = new List<Point>();
                pList.Add(new Point(p.X, p.Y - 1)); //above
                pList.Add(new Point(p.X, p.Y + 1)); //below
                pList.Add(new Point(p.X - 1, p.Y)); //left
                pList.Add(new Point(p.X + 1, p.Y)); //right
                foreach (Point neighbor in pList)
                {
                    if(!(Bound(image, neighbor.X, neighbor.Y)))
                        continue;
                    double dist = Distance(image.GetPixel(neighbor.X, neighbor.Y), target);
                    Color color = background.GetPixel(neighbor.X, neighbor.Y);
                    if ((dist < threshold) && (Distance(color, Color.White) < 5)) //and hasn't been seen before
                    {
                        background.SetPixel(neighbor.X, neighbor.Y, Color.Gray); //set it as added to the queue
                        pointQueue.Enqueue(neighbor); //and add to the queue
                    }
                }
                background.SetPixel(p.X, p.Y, Color.HotPink); //set the pixel to hot pink
            }
            return background;
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
