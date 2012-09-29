using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Picasso;
using System;
using System.Drawing;

namespace PicassoTest
{
    [TestClass]
    public class PicassoTest
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
            var actual = new Emgu.CV.Image<Bgr, Byte>(Preprocessing.FloodFill(original.ToBitmap(), 0, 0, 1));

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
    }
}