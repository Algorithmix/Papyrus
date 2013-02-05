#region

using System;
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