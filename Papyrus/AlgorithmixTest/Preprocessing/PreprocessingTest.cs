#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Algorithmix;
using Algorithmix.Preprocessing;
using Algorithmix.TestTools;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

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
            var expected = new Image<Bgr, Byte>(100, 100, new Bgr(Preprocessing.MASK_COLOR));
            expected.FillConvexPoly(square, new Bgr(Color.White));

            // Perform the Flood fill
            Console.WriteLine("Perform Flood Fill ... ");
            var actual = new Image<Bgr, Byte>(Preprocessing.FloodFill(original.ToBitmap(), 0, 0, 1, new Bgr(255, 0, 0)));

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
        ///   This test checks to see if the ExtractImages function correctly determines how many shreds
        ///   were in the document
        ///   This test works for all 3 paths shown below
        /// </summary>
        [TestMethod]
        public void ExtractImagesTest()
        {
            //var path1 = "PicassoUnitTest/PreprocessingTest/10images.jpg";
            //var path2 = "PicassoUnitTest/PreprocessingTest/12images.jpg";
            //var path3 = "PicassoUnitTest/PreprocessingTest/19images.jpg";
            var path1 = "PicassoUnitTest/PreprocessingTest/17images.jpg";

            var filepath1 = Path.Combine(Drive.GetDriveRoot(), path1);
            Bitmap image1 = new Bitmap(filepath1);
            Bgr backgroundColor = Heuristics.DetectBackground(image1, 10);
            Bitmap mask1 = Preprocessing.FloodFill(image1, 100, 100, 50, backgroundColor);

            List<Bitmap> List1 = new List<Bitmap>();
            List1 = Preprocessing.ExtractImages(image1, mask1);
            Assert.IsTrue(List1.Count == 17);
        }

        [TestMethod]
        public void AspectRatioTest()
        {
            var passFolder = "PicassoUnitTest/PreprocessingTest/AspectRatioTest";
            var failFolder = "PicassoUnitTest/PreprocessingTest/AspectRatioFailTest";

            var passFolderPath = Path.Combine(Drive.GetDriveRoot(), passFolder);
            var failFolderPath = Path.Combine(Drive.GetDriveRoot(), failFolder);

            var passDrive = new Drive(passFolderPath, Drive.Reason.Read);
            var failDrive = new Drive(failFolderPath, Drive.Reason.Read);

            var pass = passDrive.GetAllMatching("image");
            var fail = failDrive.GetAllMatching("image");

            foreach (var image in pass)
            {
                Bitmap mask = new Bitmap(image);
                Assert.IsTrue(Preprocessing.AspectRatioFilter(mask));
            }

            foreach (var image in fail)
            {
                Bitmap mask = new Bitmap(image);
                Assert.IsFalse(Preprocessing.AspectRatioFilter(mask));
            }
        }

        [TestMethod]
        public void TransparencyFilterTest()
        {
            var passFolder = "PicassoUnitTest/PreprocessingTest/TransparencyFilterTest";
            var failFolder = "PicassoUnitTest/PreprocessingTest/TransparencyFilterFailTest";

            var passFolderPath = Path.Combine(Drive.GetDriveRoot(), passFolder);
            var failFolderPath = Path.Combine(Drive.GetDriveRoot(), failFolder);

            var passDrive = new Drive(passFolderPath, Drive.Reason.Read);
            var failDrive = new Drive(failFolderPath, Drive.Reason.Read);

            var pass = passDrive.GetAllMatching("image");
            var fail = failDrive.GetAllMatching("image");

            foreach (var image in pass)
            {
                Bitmap shred = new Bitmap(image);
                Assert.IsTrue(Preprocessing.TransparencyFilter(shred));
            }

            foreach (var image in fail)
            {
                Bitmap shred = new Bitmap(image);
                Assert.IsFalse(Preprocessing.TransparencyFilter(shred));
            }
        }
    }
}