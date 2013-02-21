#region

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Algorithmix.Forensics;
using Algorithmix.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class EdgeDetectorTest
    {
        [TestMethod]
        public void EdgeTrackingTest()
        {
            var drive = new Drive( Path.Combine(Dir.CarusoTestDirectory,Dir.EdgeTrackingDirectory), Drive.Reason.Read);
            var image = new Bitmap(drive.Files("simple").First());
            var left = EdgeDetector.EdgePoints( image , Direction.FromLeft);
            
            Assert.IsTrue( left[0]==-1 );
            Assert.IsTrue(left[75] == 46);
            Assert.IsTrue(left[363] == 46);
            Assert.IsTrue(left[364] == 6);
            Assert.IsTrue(left[405] == 6);
            Assert.IsTrue(left[406] == 46);
            Assert.IsTrue(left[424] == 46);
            Assert.IsTrue(left[425] == -1);
            Assert.IsTrue(left[499] == -1);
            
            var right = EdgeDetector.EdgePoints(image, Direction.FromRight);

            Assert.IsTrue(right[0] == -1);
            Assert.IsTrue(right[75] == 154);
            Assert.IsTrue(right[218] == 154);
            Assert.IsTrue(right[219] == 188);
            Assert.IsTrue(right[294] == 188);
            Assert.IsTrue(right[295] == 154);
            Assert.IsTrue(right[424] == 154);
            Assert.IsTrue(right[425] == -1);
            Assert.IsTrue(right[499] == -1);

        }

        //[TestMethod]
        //public void AnalyzeShredTest()
        //{
        //    var path = Path.Combine(Dir.CarusoTestDirectory, Dir.EdgeDetectorDirectory);
        //    var drive = new Drive(path, Drive.Reason.Read);

        //    var list = drive.GetAllMatching("image");

        //    double leftEdge = 100;
        //    double rightEdge = 100;

        //    string leftEdgeFile = "";
        //    string rightEdgeFile = "";

        //    foreach (var filename in list)
        //    {
        //        Bitmap bitmap = new Bitmap(filename);
        //        var variance = EdgeDetector.AnalyzeShred(bitmap);
        //        Console.WriteLine("Left: " + variance.Item1 + " Right: " + variance.Item2 + " | " + filename);

        //        if (variance.Item1 < leftEdge)
        //        {
        //            leftEdge = variance.Item1;
        //            leftEdgeFile = filename;
        //        }

        //        if (variance.Item2 < rightEdge)
        //        {
        //            rightEdge = variance.Item2;
        //            rightEdgeFile = filename;
        //        }
        //    }

        //    if (leftEdgeFile != string.Empty)
        //    {
        //        Console.WriteLine("Lowest left edge variance: " + leftEdge + " | " + leftEdgeFile);
        //    }
        //    if (rightEdgeFile != string.Empty)
        //    {
        //        Console.WriteLine("Lowest right edge variance: " + rightEdge + " | " + rightEdgeFile);
        //    }
        //}

        //[TestMethod]
        //public void GetAverageTest()
        //{
        //    Queue<int> testQueue = new Queue<int>();
        //    var total = 0;
        //    var count = 400;
        //    for (int ii = 1; ii <= count; ii++)
        //    {
        //        total += ii;
        //        testQueue.Enqueue(ii);
        //    }

        //    var expected = (double)total/count;
        //    var actual = EdgeDetector.getPrediction(testQueue);
        //    Assert.IsTrue(Math.Abs(expected - actual) < 0.001);
        //}
    }
}