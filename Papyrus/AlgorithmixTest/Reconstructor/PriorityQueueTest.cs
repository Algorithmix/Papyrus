#region

using System;
using System.IO;
using Algorithmix.Reconstruction;
using Algorithmix.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    /// <summary>
    ///   Tests PriorityQueueRelated Methods
    /// </summary>
    [TestClass]
    public class PriorityQueueTest
    {
        [TestMethod]
        public void TestQueue()
        {
            var max = 1.00;
            var path = Path.Combine(Drive.GetDriveRoot(), Dir.PdfRequiremnetTestDirectory,
                                    Dir.PdfRequiremnetTestFullOne);
            var shreds = Shred.Factory("image", path, false);
            var queue = Reconstructor.BuildQueue(shreds);
            while (queue.Count > 0)
            {
                var next = queue.Dequeue().ChamferSimilarity;
                Assert.IsTrue(next <= max);
                Console.WriteLine(next);
                max = next;
            }
        }
    }
}