﻿#region

using System;
using System.Linq;
using Algorithmix.Forensics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class ChamferTest
    {
        [TestMethod]
        public void ChamferCalculationTest()
        {
            Console.WriteLine("Chamfer Testing Begin...");
            var features = new double[] {0, 0, 10, 0, 0, 0, 10, 10, 0, 10};
            var expected = new double[] {0, 1, 0, 1, 2, 1, 0, 0, 1, 0};
            var actual = Chamfer.Measure(features);

            Assert.IsTrue(expected.Length == actual.Length);
            for (int ii = 0; ii < features.Length; ii++)
            {
                Assert.IsTrue(Math.Abs(expected[ii] - actual[ii]) < 0.001);
            }
            Console.WriteLine("Chamfer Successful");
        }

        [TestMethod]
        public void ChamferSparsityTest()
        {
            Console.WriteLine("Begin Chamfer Sparsity Test");
            var chamfer = new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            var expected = chamfer.Sum();
            var actual = Chamfer.Sparsity(chamfer);

            Assert.IsTrue(expected == actual);
            Console.WriteLine("Sparsity Calculated Successfully");
        }

        [TestMethod]
        public void ChamferSimilarityTest()
        {
            Console.WriteLine("Begin Chamfer Similarity Test");

            Console.WriteLine("Begin Chamfer Similarity Test with Equal Size Arrays");
            var c1 = new[] {0, 1, 2, 1, 0, 1, 0, 0, 1, 1, 0, 1, 0};
            var c2 = new[] {0, 1, 2, 3, 2, 1, 0, 0, 0, 1, 0, 1, 0};

            var c1DotC2 = (new[] {0, 1, 4, 3, 0, 1, 0, 0, 0, 1, 0, 1, 0}).Sum();
            var c1DotC1 = (new[] {0, 1, 4, 1, 0, 1, 0, 0, 1, 1, 0, 1, 0}).Sum();
            var c2DotC2 = (new[] {0, 1, 4, 9, 4, 1, 0, 0, 0, 1, 0, 1, 0}).Sum();

            var expected = c1DotC2/(double) Math.Max(c1DotC1, c2DotC2);
            var actual = Chamfer.Similarity(c1, c2);

            Assert.IsTrue(Math.Abs(expected - actual) < 0.001);

            Console.WriteLine("Begin Chamfer Similarity Test with Differently Sized Arrays");

            var larger = new[] {0, 1, 2, 3, 4, 5, 5};
            var smaller = new[] {0, 1, 2, 3};
            const double expected0 = 1.000;
            var actual0 = Chamfer.Similarity(smaller, larger);
            Assert.IsTrue(Math.Abs(expected0 - actual0) < 0.001);

            var sDoTs = (new[] {0, 1, 4, 9}).Sum();
            var lDoTl = (new[] {1, 4, 9, 16}).Sum();
            var sDoTl = (new[] {0, 2, 6, 12}).Sum();

            var expected1 = sDoTl/(double) Math.Max(sDoTs, lDoTl);
            var actual1 = Chamfer.Similarity(smaller, larger, 1);
            Assert.IsTrue(Math.Abs(expected1 - actual1) < 0.001);

            Console.WriteLine("Begin Chamfer Similarity Scan Test");
            var expected2 = Chamfer.Similarity(smaller, larger, 2);
            var expected3 = Chamfer.Similarity(smaller, larger, 3);
            var expectedScan = new[] {expected0, expected1, expected2, expected3};
            var actualScan = Chamfer.ScanSimilarity(smaller, larger);
            var actualReverse = Chamfer.ScanSimilarity(larger, smaller);

            var diff = actualScan.Zip(expectedScan, (act, exp) => Math.Abs(act - exp)).Sum();
            var diffReverse = actualReverse.Zip(actualScan, (rev, scan) => Math.Abs(rev - scan)).Sum();
            Assert.IsTrue(diff < 0.001);
            Assert.IsTrue(diffReverse < 0.001);

            Console.WriteLine("End Chamfer Similarity Test");
        }
    }
}