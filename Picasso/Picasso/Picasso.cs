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

    public static class BitmapExtension
    { 

    }

    public class Utility
    {
        /// <summary>
        /// Extracts all objects from the source image
        /// </summary>
        /// <param name="Source">the scan of all the shreds</param>
        /// <param name="Threshold">flood-filling threshold</param>
        /// <returns>a list of bitmaps of the images</returns>
        public static List<Bitmap> ExtractImages(Bitmap Source, Bitmap Mask)
        {
            List<Tuple<Bitmap, Bitmap>> MaskSrc = ApplyBlobExtractor(Mask, Source);
            List<Bitmap> ExtractedObjects = new List<Bitmap>();
            foreach(Tuple<Bitmap, Bitmap> ms in MaskSrc)
            {
                Bitmap mask = ms.Item1;
                Bitmap src = ms.Item2;
                ExtractedObjects.Add(ExtractSingleImage(mask, src));
            }
            return ExtractedObjects;
        }

        /// <summary>
        /// Extracts a single object given the corresponding mask and rectangle
        /// </summary>
        /// <param name="TheBlob">blob taken from mask</param>
        /// <param name="Source">source rectangle from source</param>
        /// <returns>a bitmap of the extracted source</returns>
        private static Bitmap ExtractSingleImage(Bitmap TheBlob, Bitmap Source)
        {
            int width = TheBlob.Width;
            int height = TheBlob.Height;
            Bgr Background = new Bgr(Color.Black);
            Bgra FullAlpha = new Bgra(1, 13, 37, 0); //clear
            Emgu.CV.Image<Bgra, Byte> Extracted = new Image<Bgra, byte>(width, height);
            Emgu.CV.Image<Bgr, Byte> blob = new Image<Bgr, byte>(TheBlob);
            Emgu.CV.Image<Bgr, Byte> src = new Image<Bgr, byte>(Source);
            for(int ii = 0; ii < width; ii++)
            {
                for(int jj = 0; jj < height; jj++)
                {
                    if(IsEqual(Background, blob[jj,ii]))
                    {
                        //set extracted to full alpha
                        Extracted[jj, ii] = FullAlpha;
                    }
                    else
                    {
                        Extracted[jj, ii] = new Bgra(src[jj, ii].Blue, src[jj, ii].Green, src[jj, ii].Red, 255);
                    }
                }
            }
            return Extracted.ToBitmap();
        }

        /// <summary>
        /// Applies the blob extraction feature of Aforge
        /// </summary>
        /// <param name="Mask">Mask from the flood-fill step</param>
        /// <param name="Source">Source image (full image)</param>
        /// <returns>A list of tuples(blob from mask, rectangle from source)</returns>
        private static List<Tuple<Bitmap, Bitmap>> ApplyBlobExtractor(Bitmap Mask, Bitmap Source)
        {
            List<Tuple<Bitmap,Bitmap>> BlobSrcblock= new List<Tuple<Bitmap,Bitmap>>();
            AForge.Imaging.BlobCounter blobCounter = new AForge.Imaging.BlobCounter();

            // Sort order
            blobCounter.ObjectsOrder = AForge.Imaging.ObjectsOrder.XY;
            blobCounter.ProcessImage(Mask);
            AForge.Imaging.Blob[] blobs = blobCounter.GetObjects(Mask, false);

            // Adding images into the image list            
            AForge.Imaging.UnmanagedImage currentImg;
            foreach (AForge.Imaging.Blob blob in blobs)
            {
                Rectangle myRect = blob.Rectangle;
                currentImg = blob.Image;
                Bitmap exBlob = currentImg.ToManagedImage();
                AForge.Imaging.Filters.Crop filter = new AForge.Imaging.Filters.Crop(myRect);
                Bitmap exSrc = filter.Apply(Source);
                BlobSrcblock.Add(new Tuple<Bitmap,Bitmap>(exBlob, exSrc));
            }
            return BlobSrcblock;
        }

        /// <summary>
        /// Lable Connected Components of the scan, Identify unique objects
        /// </summary>
        /// <param name="image">The mask</param>
        /// <returns>A color coded bitmap showing each object in its own color</returns>
        public static int LabelConnectedComponents(ref Bitmap image)
        {
            AForge.Imaging.Filters.ConnectedComponentsLabeling filter = new AForge.Imaging.Filters.ConnectedComponentsLabeling();
            //and apply!
            image = filter.Apply(image);
            //return object count
            return filter.ObjectCount;
        }

        /// <summary>
        /// Takes an integer, returns a BGR color
        /// </summary>
        /// <param name="color">an integer between 0 and 0xFFFFFF inclusive</param>
        /// <returns></returns>
        private static Bgr IntToBgr(int color)
        {
            if(color > 0xFFFFFF)
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
                for (int jj = 0;    jj < image.Height; jj++)
                {
                    imBackground[jj,ii] = white;
                }
            }
            Queue<System.Drawing.Point> pointQueue = new Queue<System.Drawing.Point>();
            pointQueue.Enqueue(new System.Drawing.Point(xpixel, ypixel));
            Bgr gray = new Bgr(Color.Gray);
            Bgr hotpink = new Bgr(Color.Black);
            System.Drawing.Point[] pList = new System.Drawing.Point[4];
            while (!(pointQueue.Count == 0)) //make sure queue isn't empty
            {
                System.Drawing.Point p = pointQueue.Dequeue();
                //add all neighboring points to the a list
                pList[0] = (new System.Drawing.Point(p.X, p.Y - 1)); //above
                pList[1] = (new System.Drawing.Point(p.X, p.Y + 1)); //below
                pList[2] = (new System.Drawing.Point(p.X - 1, p.Y)); //left
                pList[3] = (new System.Drawing.Point(p.X + 1, p.Y)); //right
                foreach (System.Drawing.Point neighbor in pList)
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
                image.SetPixel(p.X, p.Y, Color.Black);
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
