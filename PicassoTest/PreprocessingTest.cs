using System;
using System.Collections.Generic;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Picasso;
using Algorithmix.TestTools;
using System.Drawing;

namespace PicassoTest
{
    [TestClass]
    public class PreprocessingTest
    {
        [TestMethod]
        public void FloodFillTest()
        {
            // Create a Square
            Point[] square = new Point[4];
            square[0] = new Point(25, 25);
            square[1] = new Point(75, 25);
            square[2] = new Point(75, 75);
            square[3] = new Point(25, 75);

            // Create an Original Image
            var original = new Image<Bgr, Byte>(100, 100, new Bgr(255, 0, 0));
            original.FillConvexPoly(square, new Bgr(Color.Green));

            // Create an Expected Output Image
            var expected = new Emgu.CV.Image<Bgr, Byte>(100, 100, new Bgr(Preprocessing.MASK_COLOR));
            expected.FillConvexPoly(square, new Bgr(Color.White));

            // Perform the Flood fill
            Console.WriteLine("Perform Flood Fill ... ");
            var actual = new Emgu.CV.Image<Bgr, Byte>(Preprocessing.FloodFill(original.ToBitmap(), 0, 0, 1, new Bgr(255,0,0)));

            bool identical = true;
            for (int ii = 0; ii < expected.Width; ii++)
            {
                for (int jj = 0; jj < expected.Height; jj++)
                {
                    identical = identical && (Utility.IsEqual(expected[jj, ii], actual[jj, ii]));
                }
            }

            Assert.IsTrue(identical);
        }

        /// <summary>
        /// This test checks to see if the ExtractImages function correctly determines how many shreds
        /// were in the document
        /// This test works for all 3 paths shown below
        /// </summary>
        [TestMethod]
        public void ExtractImagesTest()
        {
            var path1 = "PicassoUnitTest/PreprocessingTest/10images.jpg";
            //var path2 = "PicassoUnitTest/PreprocessingTest/12images.jpg";
            //var path3 = "PicassoUnitTest/PreprocessingTest/19images.jpg";

            var filepath1 = Path.Combine(Drive.GetDriveRoot(), path1);
            Bitmap image1 = new Bitmap(filepath1);
            Bgr backgroundColor = Picasso.Heuristics.DetectBackground(image1, 10);
            Bitmap mask1 = Preprocessing.FloodFill(image1, 100, 100, 50, backgroundColor);

            List<Bitmap> List1 = new List<Bitmap>();
            List1 = Picasso.Preprocessing.ExtractImages(image1, mask1);
            Assert.IsTrue(List1.Count == 10);

        }
    }
}