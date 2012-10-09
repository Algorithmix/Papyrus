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
    public class UtilitiesTest
    {
        
        [TestMethod]
        public void KernelIndiciesTest()
        {
            var kernelA = new double[] { 3, 4, 5 };
            var kernelB = new double[] { 4, 0, 4 };
            var actualA = Utility.GetKernelIndicies(kernelA,4);
            var actualB = Utility.GetKernelIndicies(kernelB,-1);
            var expectedA = new double[] { 4, 5, 6 };
            var expectedB = new double[] { -1, 0, 1 };

            Assert.IsTrue(expectedA.Length == actualA.Length);
            Assert.IsTrue(expectedB.Length == actualB.Length);

            // Check equality for A
            for ( int ii =0; ii < kernelA.Length ; ii++ )
            {
                Assert.IsTrue( expectedA[ii] == actualA[ii]);
            }

            // Check equality for B
            for (int ii = 0; ii < kernelA.Length; ii++)
            {
                Assert.IsTrue(expectedB[ii] == actualB[ii]);
            }
            
        }

        [TestMethod]
        public void ConvolutionTest()
        {
            Console.WriteLine("Creating a new data for convolution");

            var xx = new double[] { 3, 4, 5 };
            var hh = new double[] { 2, 1, 0 };
            var indicies = Utility.GetKernelIndicies(hh, 0);
            var actual = Utility.Convolute(xx, hh, indicies);
            var expected = new double[] { 6, 11, 14 };
            Assert.IsTrue(actual.Length == expected.Length);
            for (int ii=0; ii< actual.Length ;ii++)
            {
                Assert.IsTrue(actual[ii] == expected[ii]);
            }
            Console.WriteLine("Convolution was Successful");
        }

        [TestMethod]
        public void WeightingTest()
        {
            Console.WriteLine("Weighting Testing Begin...");
            var expected = new double[]{0.4,0.3,0.2,0.1 };
            var actual = Luminousity.LinearWeighting(4);

            Assert.IsTrue(expected.Length == actual.Length);
            for (int ii = 0; ii < expected.Length; ii++)
            {
                Assert.IsTrue(Math.Round(10*expected[ii]) == Math.Round(10*actual[ii]));
            }
            Console.WriteLine("Weighting Successful");
        }

        [TestMethod]
        public void ThresholdTest()
        {
            Console.WriteLine("Threshold Testing Begin...");
            var input = new double[] { 0, 40, 100, 60, 80, 90, 10, 70, 59, 100 };
            var expected = new double[] { 0, 0, 100, 100, 100, 100, 0, 100, 0, 100 };
            var actual = Utility.Threshold(input,0.6);

            Assert.IsTrue(expected.Length == actual.Length);
            for (int ii = 0; ii < input.Length; ii++)
            {
                Assert.IsTrue(expected[ii] == actual[ii]);
            }
            Console.WriteLine("Threshold Successful");
        }
    }
}
