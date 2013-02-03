using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Algorithmix;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Algorithmix.Forensics;
using System.Drawing;
using Algorithmix.TestTools;


namespace CarusoTest
{
    [TestClass]
    public class EdgeDetectorTest
    {
        private static string EdgeDetectorDirectory = "EdgeDetectorTest";
        [TestMethod]
        public void AnalyzeShredTest()
        {
            var path = Path.Combine(Helpers.CarusoTestDirectory, EdgeDetectorDirectory);
            var drive = new Drive(path, Drive.Reason.Read);

            var list = drive.GetAllMatching("image");

            double leftEdge = 100;
            double rightEdge = 100;

            string leftEdgeFile = "";
            string rightEdgeFile = "";
            
            foreach (var filename in list)
            {
                Bitmap bitmap = new Bitmap(filename);
                var variance = Algorithmix.Forensics.EdgeDetector.AnalyzeShred(bitmap);
                Console.WriteLine("Left: " + variance.Item1 + " Right: " + variance.Item2 + " | " + filename);

                if (variance.Item1 < leftEdge)
                {
                    leftEdge = (double)variance.Item1;
                    leftEdgeFile = filename;
                }
                    
                if (variance.Item2 < rightEdge)
                {
                    rightEdge = (double)variance.Item2;
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

        [TestMethod]
        public void GetAverageTest()
        {
            Queue<int> testQueue = new Queue<int>();
            var total = 0;
            var count = 400;
            for (int ii = 1; ii <= count; ii++)
            {
                total += ii;
                testQueue.Enqueue(ii);
            }

            var expected = (double)total/count;
            var actual = EdgeDetector.getAverage(testQueue);
            Assert.IsTrue(Math.Abs(expected - actual) < 0.001);
        }
    }
    
}


