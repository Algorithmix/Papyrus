using System;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Caruso.Forensics;
using Caruso;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarusoTest
{
    [TestClass]
    public class Shred
    {
        [TestMethod]
        public void ShredSerializingTest()
        {
            // Create a Square
            Point[] patch1 = new Point[4];
            patch1[0] = new Point(0, 0);
            patch1[1] = new Point(0, 10);
            patch1[2] = new Point(99, 10);
            patch1[3] = new Point(99, 0);

            Point[] patch2 = new Point[4];
            patch2[0] = new Point(0, 50);
            patch2[1] = new Point(0, 60);
            patch2[2] = new Point(99, 60);
            patch2[3] = new Point(99, 50);

            // Create an Original Image
            var original = new Image<Bgr, Byte>(100, 100, new Bgr(Color.HotPink));
            original.FillConvexPoly(patch1, new Bgr(Color.Gray));
            original.FillConvexPoly(patch2, new Bgr(Color.Gray));

            const string filepath = "shredtest.bmp";
            const string serializedpath = "test.shred";
            
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            original.ToBitmap().Save(filepath);
            Assert.IsTrue(File.Exists(filepath));
            Caruso.Shred myshred = new Caruso.Shred(filepath);
            
            if (File.Exists(serializedpath))
            {
                File.Delete(serializedpath);
            }
            
            Assert.IsFalse(File.Exists(serializedpath));
            Caruso.Shred.Save(myshred, serializedpath);
            Assert.IsTrue(File.Exists(serializedpath));

            Caruso.Shred newshred = Caruso.Shred.Load(serializedpath);
            Assert.IsTrue(newshred.Sparsity[(int)Caruso.Direction.FromLeft] == myshred.Sparsity[(int)Caruso.Direction.FromLeft]);
        }
    }
}
