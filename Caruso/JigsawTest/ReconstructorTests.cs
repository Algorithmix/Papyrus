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
            var paths = new List<string>();
            paths.Add(Path.Combine(Drive.GetDriveRoot(),
                                   Helpers.PrimitiveTestDirectory,
                                   Helpers.PrimitiveTestThreeNormal));
            paths.Add(Path.Combine(Drive.GetDriveRoot(),
                                   Helpers.PrimitiveTestDirectory,
                                   Helpers.PrimitiveTestTenNormal));
            foreach (var path in paths)
            {
                var shreds = Shred.Factory("Shred", path);

                var results = Reconstructor.NaiveKruskalAlgorithm(shreds);

                shreds.ForEach(shred => Console.Write(" " + shred.Id + ", "));
                Console.WriteLine();
                results.ForEach(shred => Console.Write(" " + shred.Id + ", "));
                Console.WriteLine();

//                Assert.IsTrue(shreds.Aggregate("", (combo, ss) => combo + ss.Filepath) ==
//                            results.Aggregate("", (combo, ss) => combo + ss.Filepath));
            }
        }

        [TestMethod]
        public void NaiveKruskalAuthentic()
        {
            var path = Path.Combine(Drive.GetDriveRoot(), Helpers.PdfRequiremnetTestDirectory,
                                    Helpers.PdfRequiremnetTestFullOne);
            var shreds = Shred.Factory("image", path);
            var results = Reconstructor.NaiveKruskalAlgorithm(shreds);
            shreds.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            results.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            Helpers.PrintTree(results.First().Root());
        }
    }
}