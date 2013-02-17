#region

using System;
using System.Drawing;
using System.IO;
using Algorithmix.TestTools;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class ShredTest
    {
        public static readonly string CarusoTestDirectory = "CarusoUnitTest";
        public static readonly string CarusoTestMaterialsFolder = "GettysburgAddressIsolated";

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

            // Ensure filepaths are clear for writing files
            const string filepath = "shredtest.bmp";
            const string serializedpath = "test.shred";

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            original.ToBitmap().Save(filepath);
            Assert.IsTrue(File.Exists(filepath));
            Shred myshred = new Shred(filepath);

            if (File.Exists(serializedpath))
            {
                File.Delete(serializedpath);
            }

            // Save and load shred
            Assert.IsFalse(File.Exists(serializedpath));
            Shred.Save(myshred, serializedpath);
            Assert.IsTrue(File.Exists(serializedpath));

            Shred newshred = Shred.Load(serializedpath);
            Assert.IsTrue(newshred.Sparsity[(int) Direction.FromLeft] == myshred.Sparsity[(int) Direction.FromLeft]);
        }

        [TestMethod]
        public void ShredChamferSimilarityTest()
        {
            //Create original Square
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

            // Create a Tester Square to compare
            Point[] patch3 = new Point[4];
            patch3[0] = new Point(0, 100);
            patch3[1] = new Point(0, 110);
            patch3[2] = new Point(99, 110);
            patch3[3] = new Point(99, 100);

            Point[] patch4 = new Point[4];
            patch4[0] = new Point(0, 150);
            patch4[1] = new Point(0, 160);
            patch4[2] = new Point(99, 160);
            patch4[3] = new Point(99, 150);

            // Create an Original Image
            var original = new Image<Bgr, Byte>(100, 100, new Bgr(Color.HotPink));
            original.FillConvexPoly(patch1, new Bgr(Color.Gray));
            original.FillConvexPoly(patch2, new Bgr(Color.Gray));

            //Create Image to compare with
            var tester = new Image<Bgr, Byte>(100, 200, new Bgr(Color.HotPink));
            tester.FillConvexPoly(patch3, new Bgr(Color.Gray));
            tester.FillConvexPoly(patch4, new Bgr(Color.Gray));

            const string filepath = "originalshrd.bmp";
            const string filepath2 = "testshred.bmp";

            // Delete Shred Files
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            if (File.Exists(filepath2))
            {
                File.Delete(filepath2);
            }

            // Save bitmaps to load as shreds
            original.ToBitmap().Save(filepath);
            tester.ToBitmap().Save(filepath2);

            // Create new shreds
            Shred originalshred = new Shred(filepath);
            Shred testershred = new Shred(filepath2);

            // Run Similarity test
            var actual = MatchData.CompareShred(
                originalshred,
                testershred,
                Direction.FromLeft,
                Orientation.Regular,
                Direction.FromRight,
                Orientation.Regular
                ).Offset;

            const int expected = 100;
            Assert.IsTrue(actual == expected);
        }

        [TestMethod]
        public void ShredFactoryTest()
        {
            Console.WriteLine("Loading File from TestDrive And From Factory Independently");
            var relativeDirectory = Path.Combine(CarusoTestDirectory, CarusoTestMaterialsFolder);
            var fullDirectory = Path.Combine(Drive.GetDriveRoot(), relativeDirectory);
            var mydrive = new Drive(relativeDirectory, Drive.Reason.Read);
            var shreds = Shred.Factory("image", fullDirectory);
            Assert.IsTrue(shreds.Count == mydrive.FileCount("image"));

            foreach (Shred shred in shreds)
            {
                Assert.IsTrue(File.Exists(shred.Filepath));
            }

            Console.WriteLine("Factory Test Successful");
        }
    }
}