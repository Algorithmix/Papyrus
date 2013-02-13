#region

using System;
using System.Drawing;
using Algorithmix.Forensics;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class LuminousityTest
    {
        [TestMethod]
        public void LumaCalculationTest()
        {
            Console.WriteLine("Create a new color");

            const double expectedB = 0.0;
            var actualB = Luminousity.Luma(new Bgra(0, 0, 0, 0));
            const double expectedA = 255.0;
            var actualA = Luminousity.Luma(new Bgra(Byte.MaxValue, Byte.MaxValue, Byte.MaxValue, 0));
            Assert.IsTrue((Math.Abs(expectedA - actualA) < 0.0001));
            Assert.IsTrue((Math.Abs(expectedB - actualB) < 0.001));
            Assert.IsTrue((Math.Abs(actualA - actualB) > 0.001));
            Console.WriteLine("RepresentativeLuminousity Calculated Succesfully");
        }

        [TestMethod]
        public void RepresentativeLuminousityTest()
        {
            const int imgSize = 100;
            // Create a Square
            Point[] square = new Point[4];
            square[0] = new Point(25, 25);
            square[1] = new Point(75, 25);
            square[2] = new Point(75, 75);
            square[3] = new Point(25, 75);

            var backgroundColor = new Bgra(0, 0, 0, 0);
            var foregroundColor = new Bgra(255, 255, 255, 255);

            // Create an Original Image
            var original = new Image<Bgra, Byte>(imgSize, imgSize, backgroundColor);
            original.FillConvexPoly(square, foregroundColor);

            // Create an Expected output array
            var expected = new double[imgSize];
            for (int ii = 0; ii < imgSize; ii++)
            {
                if (ii >= 25 && ii <= 75)
                {
                    expected[ii] = Luminousity.Luma(foregroundColor);
                }
                else
                {
                    expected[ii] = Utility.Defaults.Ignore;
                }
            }

            // Perform from the top, left right and bottom
            var actualLeft = Luminousity.RepresentativeLuminousity(original, 1, 5, Direction.FromLeft);
            var actualRight = Luminousity.RepresentativeLuminousity(original, 1, 5, Direction.FromRight);
            var actualTop = Luminousity.RepresentativeLuminousity(original, 1, 5, Direction.FromTop);
            var actualBottom = Luminousity.RepresentativeLuminousity(original, 1, 5, Direction.FromBottom);

            // Check that lengths match
            Assert.IsTrue(actualBottom.Length == expected.Length);
            Assert.IsTrue(actualLeft.Length == expected.Length);
            Assert.IsTrue(actualRight.Length == expected.Length);
            Assert.IsTrue(actualTop.Length == expected.Length);

            // Check that the values match
            for (int ii = 0; ii < expected.Length; ii++)
            {
                Assert.IsTrue(Math.Abs(actualBottom[ii] - expected[ii]) < 0.001);
                Assert.IsTrue(Math.Abs(actualLeft[ii] - expected[ii]) < 0.001);
                Assert.IsTrue(Math.Abs(actualRight[ii] - expected[ii]) < 0.001);
                Assert.IsTrue(Math.Abs(actualTop[ii] - expected[ii]) < 0.001);
            }
            Console.WriteLine("Luminousity Scanning Tests Succesful!");
        }

        [TestMethod]
        public void ArrayRepresentativeTest()
        {
            Console.WriteLine("Starting Luma( Color Array) Test");

            var colors = new Bgra[4];
            colors[0] = new Bgra(0, 0, 100, 0);
            colors[1] = new Bgra(0, 0, 90, 0);
            colors[2] = new Bgra(0, 0, 100, 0);
            colors[3] = new Bgra(0, 0, 100, 0);
            var expected = Luminousity.Luma(new Bgra(0, 0, (40 + 27 + 20 + 10), 0));
            var actual = Luminousity.RepresentativeLuma(colors, Luminousity.LinearWeighting(colors.Length));

            Assert.IsTrue(Math.Abs(actual - expected) < 0.01);
            Console.WriteLine("Representative of Array Luma Test Successful");
        }
    }
}