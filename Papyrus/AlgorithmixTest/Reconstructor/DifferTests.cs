#region

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class DifferTests
    {
        [TestMethod]
        public void DiffByShredOrderTest()
        {
            var first = new List<long> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            var second = new List<long> {1, 0, 3, 2, 5, 4, 7, 6, 10, 9, 8};
            var third = new List<long> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            var fourth = new List<long> {0, 1, 2, 3, 4, 5, 10, 9, 8, 7, 6};
            var actual = Differ.DiffShredByOrder(first, second);
            var expected = 6.0/10.0;
            Assert.IsTrue(Math.Abs(actual - expected) < 0.001);
            actual = Differ.DiffShredByOrder(first, third);
            expected = 1.0;
            Assert.IsTrue(Math.Abs(expected - actual) < 0.0001);
            actual = Differ.DiffShredByOrder(first, fourth);
            expected = 9.0/10.0;
            Assert.IsTrue(Math.Abs(expected - actual) < 0.0001);
        }
    }
}