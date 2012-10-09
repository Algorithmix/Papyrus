using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Caruso.Forensics;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing;
using Caruso;

namespace CarusoTest
{
    [TestClass]
    public class LuminousityTest
    {
        [TestMethod]
        public void LumaCalculationTest()
        {
            Console.WriteLine("Create a new color");

            var expected_b = 0.0;
            var actual_b = Luminousity.Luma(new Bgra(0, 0, 0, 0));
            var expected_a = 255.0;
            var actual_a = Luminousity.Luma(new Bgra(Byte.MaxValue, Byte.MaxValue, Byte.MaxValue, 0));
            Assert.IsTrue((expected_a == actual_a));
            Assert.IsTrue((expected_b == actual_b));
            Assert.IsTrue((actual_a != actual_b));
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
                    expected[ii] = Defaults.Ignore;
                }
            }

            // Perform from the top, left right and bottom
            var actualLeft = Luminousity.RepresentativeLuminousity(original, 1, 5, Luminousity.Direction.FromLeft);
            var actualRight = Luminousity.RepresentativeLuminousity(original, 1, 5, Luminousity.Direction.FromRight);
            var actualTop = Luminousity.RepresentativeLuminousity(original, 1, 5, Luminousity.Direction.FromTop);
            var actualBottom = Luminousity.RepresentativeLuminousity(original, 1, 5, Luminousity.Direction.FromBottom);

            // Check that lengths match
            Assert.IsTrue(actualBottom.Length == expected.Length);
            Assert.IsTrue(actualLeft.Length == expected.Length);
            Assert.IsTrue(actualRight.Length == expected.Length);
            Assert.IsTrue(actualTop.Length == expected.Length);

            // Check that the values match
            for (int ii = 0; ii < expected.Length; ii++)
            {
                Assert.IsTrue(actualBottom[ii] == expected[ii]);
                Assert.IsTrue(actualLeft[ii] == expected[ii]);
                Assert.IsTrue(actualRight[ii] == expected[ii]);
                Assert.IsTrue(actualTop[ii] == expected[ii]);
            }
            Console.WriteLine("Luminousity Scanning Tests Succesful!");
        }

        [TestMethod]
        public void RepresentativeTest()
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
