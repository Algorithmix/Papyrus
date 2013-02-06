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
        public static Image<Gray, byte> RgbToGray(Image<Bgra, byte> image)
        {
            return image.Convert<Gray, byte>();
        }

        public static Bitmap RgbToGray(Bitmap image)
        {
            Image<Bgra, byte> temp = new Image<Bgra, byte>(image);
            Bitmap gray = temp.Bitmap;
            temp.Dispose();
            return gray;
        }

        public static Image<Gray, byte> Threshold(Image<Gray, byte> image)
        {
            OtsuThreshold filter = new OtsuThreshold();
            return new Image<Gray, byte>(filter.Apply(image.Bitmap));
        }

        public static Bitmap Threshold(Bitmap image)
        {
            OtsuThreshold filter = new OtsuThreshold();
            return filter.Apply(image);
        }

        public static Image<Gray, byte> CannyEdge(Image<Gray, byte> image)
        {
            CannyEdgeDetector filter = new CannyEdgeDetector();
            return new Image<Gray, byte>(filter.Apply(image.Bitmap));
        }

        public static Bitmap CannyEdge(Bitmap image)
        {
            CannyEdgeDetector filter = new CannyEdgeDetector();
            return filter.Apply(image);
        }

        public static void EmbossText(Image<Gray, byte> image)
        {
            image._Erode(1);
            image._Dilate(1);
        }

        public static List<Rectangle> Segment(Image<Bgra, byte> original)
        {
            var gray = original.Convert<Gray, byte>();
            var binary = new Image<Gray, byte>(new OtsuThreshold().Apply(gray.Bitmap));
            var canny = new Image<Gray, byte>(new CannyEdgeDetector().Apply(gray.Bitmap));
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
            return list;
        }
    }
}