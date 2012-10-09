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

            var c1 = new double[] {0, 1, 2, 1, 0, 1, 0, 0, 1, 1, 0, 1, 0};
            var c2 = new double[] {0, 1, 2, 3, 2, 1, 0, 0, 0, 1, 0, 1, 0};

            var c1DotC2 = (new double[] {0, 1, 4, 3, 0, 1, 0, 0, 0, 1, 0, 1, 0}).Sum();
            var c1DotC1 = (new double[] {0, 1, 4, 1, 0, 1, 0, 0, 1, 1, 0, 1, 0}).Sum();
            var c2DotC2 = (new double[] {0, 1, 4, 9, 4, 1, 0, 0, 0, 1, 0, 1, 0 }).Sum();

            var expected = c1DotC2/Math.Max(c1DotC1, c2DotC2);
            var actual = Chamfer.Similarity(c1,c2);

            Assert.IsTrue( Math.Abs(expected - actual)< 0.001 );

            Console.WriteLine("End Chamfer Similarity Test");
        }
    }
}
