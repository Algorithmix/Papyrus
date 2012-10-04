using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Caruso;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing;

namespace CarusoTest
{
    [TestClass]
    public class ForensicTest
    {
        [TestMethod]
        public void LumaCalculationTest()
        {
            Console.WriteLine("Create a new color");

            var expected_b = 0.0;
            var actual_b = Caruso.Forensics.Luma(new Bgra(0, 0, 0, 0));
            var expected_a = 255.0;
            var actual_a = Caruso.Forensics.Luma(new Bgra( Byte.MaxValue,Byte.MaxValue,Byte.MaxValue,0));
            Assert.IsTrue((expected_a == actual_a));
            Assert.IsTrue((expected_b == actual_b));
            Assert.IsTrue((actual_a != actual_b));
            Console.WriteLine("Luma Calculated Succesfully");
        }
    }
}
