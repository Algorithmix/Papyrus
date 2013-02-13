﻿#region

using System;
using System.Drawing;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class UtilityTest
    {
        [TestMethod]
        public void TestDistance()
        {
            // First Test the Regular Colors Distance function
            Console.WriteLine("Generating Cartesian Differences between Colors");
            double red_blue = Utility.Distance(Color.Red, Color.Blue);
            double red_black = Utility.Distance(Color.Red, Color.Black);
            double black_white = Utility.Distance(Color.Black, Color.White);

            Assert.AreEqual(red_blue, Math.Sqrt((255*255 + 255*255)));
            Assert.AreEqual(red_black, Math.Sqrt((255*255)));
            Assert.AreEqual(black_white, Math.Sqrt((255*255 + 255*255 + 255*255)));
            Console.WriteLine("Algorithmix.Preprocessing.Utility.Distance for colors test Successful");

            Console.WriteLine("Generating Cartesian Differences between Bgr Colors");
            double red_blue_bgr = Utility.Distance(new Bgr(Color.Red), new Bgr(Color.Blue));
            double red_black_bgr = Utility.Distance(new Bgr(Color.Red), new Bgr(Color.Black));
            double black_white_bgr = Utility.Distance(new Bgr(Color.Black), new Bgr(Color.White));

            Assert.AreEqual(red_blue_bgr, Math.Sqrt((255*255 + 255*255)));
            Assert.AreEqual(red_black_bgr, Math.Sqrt((255*255)));
            Assert.AreEqual(black_white_bgr, Math.Sqrt((255*255 + 255*255 + 255*255)));
            Console.WriteLine("Picasso.Utility.Distance for Bgr Colors Test Successful");
        }

        [TestMethod]
        public void TestBound()
        {
            // Create an image and test if pixels are correctly within or out of bounds
            Bitmap image = new Bitmap(3000, 3000);

            Assert.IsTrue(Utility.IsBound(image, 2000, 2000));
            Assert.IsFalse(Utility.IsBound(image, 3010, 2000));
            Assert.IsFalse(Utility.IsBound(image, 2000, 3010));
            Assert.IsFalse(Utility.IsBound(image, -200, 2000));
            Assert.IsFalse(Utility.IsBound(image, 2000, -200));
            Console.WriteLine("Algorithmix.Preprocessing.Utility.Bound Test Successful");
        }

        [TestMethod]
        public void IsEqualTest()
        {
            // Create equal and not equal colors and ensure the IsEqual Methods returns values as expected
            Console.WriteLine("Starting Algorithmix.Preprocessing.Utility.IsEqual method testing");

            var color_a = new Bgr(0, 0, 255);
            var color_b = new Bgr(Color.Red);
            var color_c = new Bgr(Color.Green);

            Assert.IsTrue(Utility.IsEqual(color_a, color_b));
            Assert.IsFalse(Utility.IsEqual(color_a, color_c));
            Assert.IsFalse(Utility.IsEqual(color_b, color_c));
            Console.WriteLine("Successfully completed Algorithmix.Preprocessing.Utility.IsEqual Test");
        }

        [TestMethod]
        public void TestSlope()
        {
            Point trPoint = new Point(10, 10);
            Point blPoint = new Point(5, 4);

            double actual = Utility.SlopeFromPoints(trPoint, blPoint);
            double expected = (float) 6/5;

            Assert.IsTrue(actual == expected);
        }
    }
}