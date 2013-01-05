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
    public class ChamferTest
    {
        [TestMethod]
        public void ChamferCalculationTest()
        {
            Console.WriteLine("Chamfer Testing Begin...");
            var features = new double[] { 0, 0, 10, 0, 0, 0, 10, 10, 0, 10 };
            var expected = new double[] { 0, 1, 0, 1, 2, 1, 0, 0, 1, 0 };
            var actual = Chamfer.Measure(features);

            Assert.IsTrue(expected.Length == actual.Length);
            for (int ii = 0; ii < features.Length; ii++)
            {
                Assert.IsTrue(expected[ii] == actual[ii]);
            }
            Console.WriteLine("Chamfer Successful");
        }

        [TestMethod]
        public void ChamferSimilarityTest()
        {
            Console.WriteLine("Begin Chamfer Similarity Test");

            Console.WriteLine("Begin Chamfer Similarity Test with Equal Size Arrays");
            var c1 = new double[] {0, 1, 2, 1, 0, 1, 0, 0, 1, 1, 0, 1, 0};
            var c2 = new double[] {0, 1, 2, 3, 2, 1, 0, 0, 0, 1, 0, 1, 0};

            var c1DotC2 = (new double[] {0, 1, 4, 3, 0, 1, 0, 0, 0, 1, 0, 1, 0}).Sum();
            var c1DotC1 = (new double[] {0, 1, 4, 1, 0, 1, 0, 0, 1, 1, 0, 1, 0}).Sum();
            var c2DotC2 = (new double[] {0, 1, 4, 9, 4, 1, 0, 0, 0, 1, 0, 1, 0 }).Sum();

            var expected = c1DotC2/Math.Max(c1DotC1, c2DotC2);
            var actual = Chamfer.Similarity(c1,c2);

            Assert.IsTrue( Math.Abs(expected - actual)< 0.001 );
            
            Console.WriteLine("Begin Chamfer Similarity Test with Differently Sized Arrays");

            var larger = new double[] {0, 1, 2, 3, 4, 5, 5};
            var smaller = new double[] {0, 1, 2, 3};
            const double expected0 = 1.000;
            var actual0 = Chamfer.Similarity(smaller, larger, 0);
            Assert.IsTrue( Math.Abs( expected0 - actual0) < 0.001 );

            var sDoTs = (new double[] {0, 1, 4, 9}).Sum();
            var lDoTl = (new double[] {1, 4, 9, 16}).Sum();
            var sDoTl = (new double[] {0, 2, 6, 12}).Sum();

            var expected1 = sDoTl/Math.Max(sDoTs, lDoTl); 
            var actual1 = Chamfer.Similarity(smaller, larger, 1);
            Assert.IsTrue(Math.Abs(expected1 - actual1) < 0.001);

            Console.WriteLine("Begin Chamfer Similarity Scan Test");
            var expected2 = Chamfer.Similarity(smaller,larger,2);            
            var expected3 = Chamfer.Similarity(smaller,larger,3);
            var expectedScan = new double[] {expected0,expected1,expected2,expected3};
            var actualScan = Chamfer.ScanSimilarity(smaller,larger);
            var actualReverse = Chamfer.ScanSimilarity(larger, smaller);
            
            var diff = actualScan.Zip(expectedScan, (act,exp) => Math.Abs(act-exp) ).Sum();
            var diffReverse = actualReverse.Zip(actualScan, (rev,scan) => Math.Abs(rev-scan)).Sum();
            Assert.IsTrue(diff < 0.001);
            Assert.IsTrue(diffReverse < 0.001);

            Console.WriteLine("End Chamfer Similarity Test");
        }
    }
}
