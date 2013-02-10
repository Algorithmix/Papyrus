#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Algorithmix.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class NaiveKruskalTests
    {
        [TestMethod]
        public void NaiveKruskalPrimitive()
        {
            Shred.BUFFER = 0;
            Shred.SAMPLE_SIZE = 1;

            var paths = new List<string>();
            paths.Add(Path.Combine(Drive.GetDriveRoot(),
                                   Dir.PrimitiveTestDirectory,
                                   Dir.PrimitiveTestThreeNormal));
            paths.Add(Path.Combine(Drive.GetDriveRoot(),
                                   Dir.PrimitiveTestDirectory,
                                   Dir.PrimitiveTestSixNormal));
            paths.Add(Path.Combine(Drive.GetDriveRoot(),
                                   Dir.PrimitiveTestDirectory,
                                   Dir.PrimitiveTestTenNormal));
            foreach (var path in paths)
            {
                var shreds = Shred.Factory("Shred", path, false);

                var results = Reconstructor.NaiveKruskalAlgorithm(shreds);

                shreds.ForEach(shred => Console.Write(" " + shred.Id + ", "));
                Console.WriteLine();
                results.ForEach(shred => Console.Write(" " + shred.Id + ", "));
                Console.WriteLine();

                Assert.IsTrue(shreds.Aggregate("", (combo, ss) => combo + ss.Filepath) ==
                            results.Aggregate("", (combo, ss) => combo + ss.Filepath));
            }
        }

        [TestMethod]
        public void NaiveKruskalAuthentic()
        {
            var path = Path.Combine(Drive.GetDriveRoot(), Dir.PdfRequiremnetTestDirectory,
                                    Dir.PdfRequiremnetTestFullOne);
            var shreds = Shred.Factory("image", path, false);
            var results = Reconstructor.NaiveKruskalAlgorithm(shreds);
            shreds.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            results.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            Helpers.PrintTree(results.First().Root());
        }
    }
}