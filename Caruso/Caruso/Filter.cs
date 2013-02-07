#region

using System.Collections.Generic;
using System.Drawing;
using AForge.Imaging.Filters;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

#endregion

namespace Algorithmix.Forensics
{
    public class Filter
    {
        /// <summary>
        /// Converts an RGB Image to Grayscale
        /// </summary>
        /// <param name="image">Input image to be converted to grayscale</param>
        /// <returns>new Grayscale copy of input image</returns>
        public static Image<Gray, byte> RgbToGray(Image<Bgra, byte> image)
        {
            return image.Convert<Gray, byte>();
        }

        /// <summary>
        /// Converts an RGB Image to Grayscale
        /// </summary>
        /// <param name="image">Input image to be converted to grayscale</param>
        /// <returns>new Grayscale copy of input image</returns>
        public static Bitmap RgbToGray(Bitmap image)
        {
            Image<Bgra, byte> temp = new Image<Bgra, byte>(image);
            Bitmap gray = temp.Bitmap;
            temp.Dispose();
            return gray;
        }

        /// <summary>
        /// Performs a binary thresholding,
        /// the threshold is determined automatically by the otsu method
        /// </summary>
        /// <param name="image">Input image to be thresholded</param>
        /// <returns>new BW image</returns>
        public static Image<Gray, byte> Threshold(Image<Gray, byte> image)
        {
            OtsuThreshold filter = new OtsuThreshold();
            return new Image<Gray, byte>(filter.Apply(image.Bitmap));
        }

        /// <summary>
        /// Performs a binary thresholding,
        /// the threshold is determined automatically by the otsu method
        /// </summary>
        /// <param name="image">Input image to be thresholded</param>
        /// <returns>new BW image</returns>
        public static Bitmap Threshold(Bitmap image)
        {
            OtsuThreshold filter = new OtsuThreshold();
            return filter.Apply(image);
        }

        /// <summary>
        /// Performs Canny edge Detection. Given a grayscale image,
        /// a canny edge filter is applied an new filtered image returned
        /// </summary>
        /// <param name="image">Input Grayscale image</param>
        /// <returns>New Filter Image</returns>
        public static Image<Gray, byte> CannyEdge(Image<Gray, byte> image)
        {
            CannyEdgeDetector filter = new CannyEdgeDetector();
            return new Image<Gray, byte>(filter.Apply(image.Bitmap));
        }

        /// <summary>
        /// Performs Canny edge Detection. Given a bitmap image,
        /// a canny edge filter is applied an new filtered image returned
        /// </summary>
        /// <param name="image">Input bitmap image</param>
        /// <returns>New Filter Image</returns>
        public static Bitmap CannyEdge(Bitmap image)
        {
            CannyEdgeDetector filter = new CannyEdgeDetector();
            return filter.Apply(image);
        }

        /// <summary>
        /// Emboss Edges, altering the input image, 
        /// first it erodes the image and then dilates to give the
        /// text a more bold feeling
        /// </summary>
        /// <param name="image">Input Image to be Manipulated</param>
        public static void EmbossText(Image<Gray, byte> image)
        {
            image._Erode(1);
            image._Dilate(1);
        }

        /// <summary>
        /// Retrieves Bounding Box segments from the contours of a thresholded image
        /// </summary>
        /// <param name="source">Input a grayscale image</param>
        /// <returns>A list of the rectangle bounding boxes for all the contoured features</returns>
        public static List<Rectangle> Segment(Image<Gray, byte> source)
        {
            var canny = new Image<Gray, byte>(new CannyEdgeDetector().Apply(source.Bitmap));
            var list = new List<Rectangle>();
            using (MemStorage stor = new MemStorage())
            {
                for (
                    Contour<Point> contours = canny.FindContours(
                        CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                        RETR_TYPE.CV_RETR_EXTERNAL,
                        stor);
                    contours != null;
                    contours = contours.HNext)
                {
                    Rectangle rect = contours.BoundingRectangle;
                    list.Add(rect);
                }
            }
            canny.Dispose();
            return list;
        }
    }
}