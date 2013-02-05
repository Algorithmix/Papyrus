using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var dataAB = Data.CompareShred(a, b, 
                                            Direction.FromRight, 
                                            Orientation.Regular, 
                                            Direction.FromLeft, 
                                            Orientation.Regular);
            var dataBA = Data.CompareShred(a, b, 
                                            Direction.FromLeft, 
                                            Orientation.Regular, 
                                            Direction.FromRight, 
                                            Orientation.Regular);
            
            Assert.IsTrue(dataAB.ChamferSimilarity > dataBA.ChamferSimilarity );

            var dataBC = Data.CompareShred(b, c, 
                                            Direction.FromRight, 
                                            Orientation.Regular, 
                                            Direction.FromLeft, 
                                            Orientation.Regular);
            
            var dataCB = Data.CompareShred(b, c, 
                                            Direction.FromLeft, 
                                            Orientation.Regular, 
                                            Direction.FromRight, 
                                            Orientation.Regular);
            
            Assert.IsTrue(dataBC.ChamferSimilarity > dataCB.ChamferSimilarity);

            var dataAC = Data.CompareShred(a, c, 
                                            Direction.FromRight, 
                                            Orientation.Regular, 
                                            Direction.FromLeft, 
                                            Orientation.Regular);
            
            Assert.IsTrue(dataBC.ChamferSimilarity > dataAC.ChamferSimilarity);
        }
    }
}
