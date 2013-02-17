#region

using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class Concept
    {
        [TestMethod]
        public void PrimitiveTest()
        {
            var shreds = Helpers.BootstrapPrimitiveThree();
            // 3 Shreds
            var a = shreds[0];
            var b = shreds[1];
            var c = shreds[2];

            var dataAB = MatchData.CompareShred(a, b,
                                           Direction.FromRight,
                                           Orientation.Regular,
                                           Direction.FromLeft,
                                           Orientation.Regular);
            var dataBA = MatchData.CompareShred(a, b,
                                           Direction.FromLeft,
                                           Orientation.Regular,
                                           Direction.FromRight,
                                           Orientation.Regular);

            Assert.IsTrue(dataAB.ChamferSimilarity > dataBA.ChamferSimilarity);

            var dataBC = MatchData.CompareShred(b, c,
                                           Direction.FromRight,
                                           Orientation.Regular,
                                           Direction.FromLeft,
                                           Orientation.Regular);

            var dataCB = MatchData.CompareShred(b, c,
                                           Direction.FromLeft,
                                           Orientation.Regular,
                                           Direction.FromRight,
                                           Orientation.Regular);

            Assert.IsTrue(dataBC.ChamferSimilarity > dataCB.ChamferSimilarity);

            var dataAC = MatchData.CompareShred(a, c,
                                           Direction.FromRight,
                                           Orientation.Regular,
                                           Direction.FromLeft,
                                           Orientation.Regular);

            Assert.IsTrue(dataBC.ChamferSimilarity > dataAC.ChamferSimilarity);
        }
    }
}