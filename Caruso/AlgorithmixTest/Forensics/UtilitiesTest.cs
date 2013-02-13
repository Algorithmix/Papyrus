#region

using System;
using System.Linq;
using Algorithmix.Forensics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class UtilitiesTest
    {
        [TestMethod]
        public void MaxTest()
        {
            var original = new[] {1.0, 1.1, 2.3, -1.23, 3.14, -9.0};
            var actual = Utility.Max(original);
            const int maxIndex = 4;
            var expected = new Tuple<double, int>(original.Max(), maxIndex);
            Assert.IsTrue(Math.Abs(expected.Item1 - actual.Item1) < 0.001);
            Assert.IsTrue(actual.Item2 == expected.Item2);
        }

        [TestMethod]
        public void TestReverse()
        {
            var original = new[] {1.0, 1.4, 1.6, 2.0};
            var expected = original.Reverse();
            var actual = Utility.Reverse(original);
            Assert.IsTrue(original.Zip(actual, (first, second) => Math.Abs(first - second) > 0.001).All(eq => eq));
            Assert.IsTrue(expected.Zip(actual, (first, second) => Math.Abs(first - second) < 0.001).All(eq => eq));

            var original2 = new[] {1, 2, 3, 4};
            var expected2 = original2.Reverse();
            var actual2 = Utility.Reverse(original2);
            Assert.IsTrue(original2.Zip(actual2, (first, second) => first != second).All(eq => eq));
            Assert.IsTrue(expected2.Zip(actual2, (first, second) => first == second).All(eq => eq));
        }

        [TestMethod]
        public void KernelIndiciesTest()
        {
            var kernelA = new double[] {3, 4, 5};
            var kernelB = new double[] {4, 0, 4};
            var actualA = Utility.GetKernelIndicies(kernelA, 4);
            var actualB = Utility.GetKernelIndicies(kernelB, -1);
            var expectedA = new double[] {4, 5, 6};
            var expectedB = new double[] {-1, 0, 1};

            Assert.IsTrue(expectedA.Length == actualA.Length);
            Assert.IsTrue(expectedB.Length == actualB.Length);

            // Check equality for A
            for (int ii = 0; ii < kernelA.Length; ii++)
            {
                Assert.IsTrue(Math.Abs(expectedA[ii] - actualA[ii]) < 0.001);
            }

            // Check equality for B
            for (int ii = 0; ii < kernelA.Length; ii++)
            {
                Assert.IsTrue(Math.Abs(expectedB[ii] - actualB[ii]) < 0.001);
            }
        }

        [TestMethod]
        public void ConvolutionTest()
        {
            Console.WriteLine("Creating a new data for convolution");

            var xx = new double[] {3, 4, 5};
            var hh = new double[] {2, 1, 0};
            var indicies = Utility.GetKernelIndicies(hh, 0);
            var actual = Utility.Convolute(xx, hh, indicies);
            var expected = new double[] {6, 11, 14};
            Assert.IsTrue(actual.Length == expected.Length);
            for (int ii = 0; ii < actual.Length; ii++)
            {
                Assert.IsTrue(Math.Abs(actual[ii] - expected[ii]) < 0.001);
            }
            Console.WriteLine("Convolution was Successful");
        }

        [TestMethod]
        public void WeightingTest()
        {
            Console.WriteLine("Weighting Testing Begin...");
            var expected = new[] {0.4, 0.3, 0.2, 0.1};
            var actual = Luminousity.LinearWeighting(4);

            Assert.IsTrue(expected.Length == actual.Length);
            for (int ii = 0; ii < expected.Length; ii++)
            {
                Assert.IsTrue(Math.Abs(Math.Round(10*expected[ii]) - Math.Round(10*actual[ii])) < 0.001);
            }
            Console.WriteLine("Weighting Successful");
        }

        [TestMethod]
        public void ThresholdTest()
        {
            Console.WriteLine("Threshold Testing Begin...");
            var input = new double[] {0, 40, 100, 60, 80, 90, 10, 70, 59, 100};
            var expected = new double[] {0, 0, 100, 100, 100, 100, 0, 100, 0, 100};
            var actual = Utility.Threshold(input, 0.6);

            Assert.IsTrue(expected.Length == actual.Length);
            for (int ii = 0; ii < input.Length; ii++)
            {
                Assert.IsTrue(Math.Abs(expected[ii] - actual[ii]) < 0.001);
            }
            Console.WriteLine("Threshold Successful");
        }
    }
}