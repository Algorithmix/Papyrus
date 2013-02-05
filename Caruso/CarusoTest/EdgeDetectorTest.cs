#region

using System;
using System.Drawing;
using System.IO;
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
        public void AnalyzeShredTest()
        {
            var path = Path.Combine(Helpers.CarusoTestDirectory, Helpers.EdgeDetectorDirectory);
            var drive = new Drive(path, Drive.Reason.Read);

            var list = drive.GetAllMatching("image");

            double leftEdge = 100;
            double rightEdge = 100;

            string leftEdgeFile = "";
            string rightEdgeFile = "";

            foreach (var filename in list)
            {
                Bitmap bitmap = new Bitmap(filename);
                var variance = EdgeDetector.AnalyzeShred(bitmap);
                Console.WriteLine("Left: " + variance.Item1 + " Right: " + variance.Item2 + " | " + filename);

                if (variance.Item1 < leftEdge)
                {
                    leftEdge = variance.Item1;
                    leftEdgeFile = filename;
                }

                if (variance.Item2 < rightEdge)
                {
                    rightEdge = variance.Item2;
                    rightEdgeFile = filename;
                }
            }

            if (leftEdgeFile != string.Empty)
            {
                Console.WriteLine("Lowest left edge variance: " + leftEdge + " | " + leftEdgeFile);
            }
            if (rightEdgeFile != string.Empty)
            {
                Console.WriteLine("Lowest right edge variance: " + rightEdge + " | " + rightEdgeFile);
            }
        }

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