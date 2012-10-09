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
            var actual = Chamfer.Calculate(features);

            Assert.IsTrue(expected.Length == actual.Length);
            for (int ii = 0; ii < features.Length; ii++)
            {
                Assert.IsTrue(expected[ii] == actual[ii]);
            }
            Console.WriteLine("Chamfer Successful");
        }
    }
}
