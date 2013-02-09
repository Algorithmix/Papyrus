using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
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
using System.Drawing.Drawing2D;

namespace Picasso
{
    public class Preprocessing
    {
        public readonly static Color MASK_COLOR = Color.Black;
        public static Logger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Simple helper function to filter blobs.  Currently works solely on size.
        /// </summary>
        /// <param name="mask">the bitmap "mask" to be used</param>
        /// <returns>true iff mask is considered a good blob</returns>
        private static bool FilterBlob(Bitmap mask)
        {
            int MIN_DIMENSION = 20000;
            return (mask.Height * mask.Width > MIN_DIMENSION);
        }

        public static int FindTopTransparent(Image<Bgra, Byte> myImg)
        {
            for(int ii = 0; ii < myImg.Rows; ii++)
            {
                for(int jj = 0; jj < myImg.Cols; jj++)
                {
                    if(myImg[ii, jj].Alpha > 55)
                    {
                        return ii;
                    }
                }
            }
            return 0;
        }
        public static int FindBottomTransparent(Image<Bgra, Byte> myImg)
        {
            for (int ii = myImg.Rows - 1; ii >= 0; ii--)
            {
                for (int jj = 0; jj < myImg.Cols; jj++)
                {
                    if (myImg[ii, jj].Alpha > 55)
                    {
                        return ii;
                    }
                }
            }
            return 0;
        }
        public static int FindLeftTransparent(Image<Bgra, Byte> myImg)
        {
            for (int ii = 0; ii < myImg.Cols; ii++)
            {
                for (int jj = 0; jj < myImg.Rows; jj++)
                {
                    if (myImg[jj, ii].Alpha > 55)
                    {
                        return ii;
                    }
                }
            }
            return 0;
        }
        public static int FindRightTransparent(Image<Bgra, Byte> myImg)
        {
            for (int ii = myImg.Cols - 1; ii >= 0; ii--)
            {
                for (int jj = 0; jj < myImg.Rows; jj++)
                {
                    if (myImg[jj, ii].Alpha > 55)
                    {
                        return ii;
                    }
                }
            }
            return 0;
        }
        /// <summary>
        /// Simple helper function for cropping the image after rotation
        /// </summary>
        /// <param name="image">rotated image</param>
        /// <returns></returns>
        private static Rectangle GetCropZone(Image<Bgra, Byte> myImg)
        {
            int top = 0;
            int left = 0;
            int bottom = myImg.Rows - 1;
            int right = myImg.Cols - 1;

            top = FindTopTransparent(myImg);
            left = FindLeftTransparent(myImg);
            bottom = FindBottomTransparent(myImg);
            right = FindRightTransparent(myImg);

            return new Rectangle(left, top, (right - left), (bottom - top));
        }

        /// <summary>
        /// Rotate the image by angle degrees, with bkColor as new background
        /// Uses some magic with the translatetransform and pixel2d :P WOW!
        /// </summary>
        /// <param name="bmp">the image to be rotated</param>
        /// <param name="angle">the angle to rotate by</param>
        /// <param name="bkColor">background color for new background pixels</param>
        /// <returns>bmp rotated by angle degrees</returns>
        public static Bitmap RotateImg(Bitmap bmp, float angle, Color bkColor)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            System.Drawing.Imaging.PixelFormat pf = default(System.Drawing.Imaging.PixelFormat);
            if (bkColor == Color.Transparent)
            {
                pf = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }
            else
            {
                pf = bmp.PixelFormat;
            }

            Bitmap tempImg = new Bitmap(w, h, pf);
            Graphics g = Graphics.FromImage(tempImg);
            g.Clear(bkColor);
            g.DrawImageUnscaled(bmp, 1, 1);
            g.Dispose();

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, w, h));
            System.Drawing.Drawing2D.Matrix mtrx = new System.Drawing.Drawing2D.Matrix();
            mtrx.Rotate(angle);
            RectangleF rct = path.GetBounds(mtrx);
            Bitmap newImg = new Bitmap(Convert.ToInt32(rct.Width), Convert.ToInt32(rct.Height), pf);
            g = Graphics.FromImage(newImg);
            g.Clear(bkColor);
            g.TranslateTransform(-rct.X, -rct.Y);
            g.RotateTransform(angle);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(tempImg, 0, 0);
            g.Dispose();
            tempImg.Dispose();
            return newImg;
        }
        
        /// <summary>
        /// Rotates the blobs to be vertical
        /// </summary>
        /// <param name="blob">image blob to orient</param>
        /// <returns>blob but vertical</returns>
        public static Bitmap Orient(Bitmap blob)
        {
            int width = blob.Width;
            int height = blob.Height;
            int area = width * height;
            Image<Bgra, Byte> blobb = new Image<Bgra, byte>(blob);
            Rectangle crop = GetCropZone(blobb);
            System.Drawing.Point trPoint = new System.Drawing.Point(crop.Right, crop.Top);
            System.Drawing.Point blPoint = new System.Drawing.Point(crop.Left, crop.Bottom);
            double slope = Utility.SlopeFromPoints(trPoint, blPoint);
            double angle = Math.Atan(slope);
            float angleToRotate = (float)(90.0 - angle);
            Bitmap rotated = RotateImg(blobb.ToBitmap(), angleToRotate, Color.Transparent);
            if(rotated.Height < rotated.Width)
            {
                return RotateImg(blobb.ToBitmap(), angleToRotate + 90, Color.Transparent);
            }
            return rotated;
        }

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
            foreach (Tuple<Bitmap, Bitmap> ms in MaskSrc)
            {
                Bitmap mask = ms.Item1;
                Bitmap src = ms.Item2;
                if (FilterBlob(mask))
                {
                    log.Debug("Extracted object");
                    ExtractedObjects.Add(ExtractSingleImage(mask, src));
                }
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

            log.Debug("Extract Single Image out of original using Blob Mask");
            for (int ii = 0; ii < width; ii++)
            {
                for (int jj = 0; jj < height; jj++)
                {
                    if (Utility.IsEqual(Background, blob[jj, ii]))
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
            log.Info("Return processed blob");
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
            List<Tuple<Bitmap, Bitmap>> BlobSrcblock = new List<Tuple<Bitmap, Bitmap>>();

            log.Debug("Using AForge Blob Counter to Process Mask");
            AForge.Imaging.BlobCounter blobCounter = new AForge.Imaging.BlobCounter();

            // Sort order
            blobCounter.ObjectsOrder = AForge.Imaging.ObjectsOrder.XY;
            blobCounter.ProcessImage(Mask);
            AForge.Imaging.Blob[] blobs = blobCounter.GetObjects(Mask, false);

            log.Info("Use the Blob Extraction Results to reverse extract blobs from images");
            // Adding images into the image list            
            AForge.Imaging.UnmanagedImage currentImg;
            foreach (AForge.Imaging.Blob blob in blobs)
            {
                Rectangle myRect = blob.Rectangle;
                currentImg = blob.Image;
                Bitmap exBlob = currentImg.ToManagedImage();
                AForge.Imaging.Filters.Crop filter = new AForge.Imaging.Filters.Crop(myRect);
                Bitmap exSrc = filter.Apply(Source);
                BlobSrcblock.Add(new Tuple<Bitmap, Bitmap>(exBlob, exSrc));
            }
            log.Info("Extraction Complete: returning List of ( blob bitmap, src bitmap)");
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
        /// Flood fill in a BFS manner, so as not to overwhelm the stack
        /// </summary>
        /// <param name="image">the image we wish to fill on</param>
        /// <param name="xpixel">the x pixel to sample from</param>
        /// <param name="ypixel">the y pixel to sample from</param>
        /// <param name="threshold">the threshold of difference</param>
        /// <returns>the background which can be subtracted</returns>
        public static Bitmap FloodFill(Bitmap image, int xpixel, int ypixel, double threshold, Bgr myColor)
        {
            AForge.Imaging.Filters.PointedColorFloodFill filter = new PointedColorFloodFill();
            int thresh = (int) threshold;
            filter.Tolerance = Color.FromArgb(thresh, thresh, thresh);
            filter.FillColor = Color.Black;
            filter.StartingPoint = new IntPoint(xpixel, ypixel);
            return filter.Apply(image);
        }
    }
}
