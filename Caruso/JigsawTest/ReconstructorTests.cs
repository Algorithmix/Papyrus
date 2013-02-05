using System;
using System.Linq;
using CarusoTest;
using System.IO;
using Algorithmix;
using Algorithmix.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JigsawTest
{
    [TestClass]
    public class Basic
    {
        
        public static readonly string PdfRequiremnetTestDirectory = "PDFRequirement";
        public static readonly string PdfRequiremnetTestFullOne = @"Full1";
        [TestMethod]
        public void NaiveKruskalTest()
        {
            var path = Path.Combine(Drive.GetDriveRoot(), PdfRequiremnetTestDirectory, PdfRequiremnetTestFullOne);
            var shreds = Algorithmix.Shred.Factory("image", path);
            var results = Reconstructor.NaiveKruskalAlgorithm(shreds);
            shreds.ForEach(shred => Console.Write(" "+shred.Id + ", "));
            Console.WriteLine();
            results.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            Helpers.PrintTree(results.First().Root());
        }

        [TestMethod]
        public void TestQueue()
        {
            var max = 1.00;
            var path = Path.Combine(Drive.GetDriveRoot(), PdfRequiremnetTestDirectory, PdfRequiremnetTestFullOne);
            var shreds = Algorithmix.Shred.Factory("image", path);
            var queue = Algorithmix.Reconstructor.BuildQueue(shreds);
            while( queue.Count > 0)
            {
                var next = queue.Dequeue().ChamferSimilarity;
                Assert.IsTrue(next <= max);
                Console.WriteLine(next);
                max = next;
            }
        }
    }
}
