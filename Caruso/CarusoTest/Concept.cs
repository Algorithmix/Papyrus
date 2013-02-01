using System;
using System.Linq;
using System.Collections.Generic;
using Algorithmix;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarusoTest
{
    [TestClass]
    public class Concept
    {

        /// <summary>
        /// This tests the chamfter similarity between artifically created
        /// These shreds have straight edges
        /// </summary>
        [TestMethod]
        public void PrimitiveTest()
        {
            var shreds = Helpers.BootstrapPrimitiveThree();
            // 3 Shreds
            var a = shreds[0];
            var b = shreds[1];
            var c = shreds[2];

            var dataAB = Data.CompareShred(a, b, Direction.FromRight, Orientation.Regular, Direction.FromLeft, Orientation.Regular);
            var dataBA = Data.CompareShred(a, b, Direction.FromLeft, Orientation.Regular, Direction.FromRight, Orientation.Regular);
            Assert.IsTrue(dataAB.ChamferSimilarity > dataBA.ChamferSimilarity );

            var dataBC = Data.CompareShred(b, c, Direction.FromRight, Orientation.Regular, Direction.FromLeft, Orientation.Regular);
            var dataCB = Data.CompareShred(b, c, Direction.FromLeft, Orientation.Regular, Direction.FromRight, Orientation.Regular);
            Assert.IsTrue(dataBC.ChamferSimilarity > dataCB.ChamferSimilarity);

            var dataAC = Data.CompareShred(a, c, Direction.FromRight, Orientation.Regular, Direction.FromLeft, Orientation.Regular);
            Assert.IsTrue(dataBC.ChamferSimilarity > dataAC.ChamferSimilarity);
        }


        /// <summary>
        /// This tests the chamfer similarity between shreds that are created using our virtual shredder
        /// The virutal shredder creates shreds with jagged edges 
        /// </summary>
        [TestMethod]
        public void ScannedDocumentPreliminaryTest()
        {
            Algorithmix.Shred.BUFFER = 1;
            for (int param = 0; param < 10; param++)
            {
                Algorithmix.Shred.SAMPLE_SIZE = param;

                var shreds = Helpers.BootstrapPrimitiveScanned();

                Assert.IsTrue(shreds.Count > 6);

                var bests = new List<long>(shreds.Count * shreds.Count);
                foreach (var shred in shreds)
                {
                    var datas = new List<Data>(shreds.Count);
                    for (int ii = 0; ii < shreds.Count; ii++)
                    {
                        var other = shreds[ii];
                        if (shred.Id == other.Id)
                        {
                            continue;
                        }

                        var dataRegular = Data.CompareShred(shred, other, Direction.FromRight, Orientation.Regular, Direction.FromLeft, Orientation.Regular);
                        //var dataBackwardsRegular = Data.CompareShred(shred, other, Direction.FromLeft, Orientation.Regular, Direction.FromRight, Orientation.Regular);
                        datas.Add(dataRegular);
                        //datas.Add(dataBackwardsRegular);
                    }

                    var tuples = datas.Select(data => new Tuple<double, long>(data.ChamferSimilarity, data.Second.Shred.Id));
                    var max = tuples.Max(x => x.Item1);
                    bests.Add(tuples.First(x => x.Item1 == max).Item2);
                }

                int index = (int)shreds.First().Id;
                int desired = 0;
                foreach (long best in bests)
                {
                    //Console.WriteLine("Best Match to the shred " + index + " is " + best);
                    
                    if (index == (best-1)) { desired++; }
                    index += 1;
                }
                Console.WriteLine("Param Size "+param+" is "+desired+" correct");

            }

                //var a = shreds[ii];
                //var b = shreds[ii+1];
                //var c = shreds[ii+2];

                //var dataAB = Data.CompareShred(a, b, Direction.FromRight, Orientation.Regular, Direction.FromLeft, Orientation.Regular);
                //var dataBA = Data.CompareShred(a, b, Direction.FromLeft, Orientation.Regular, Direction.FromRight, Orientation.Regular);
                ////Assert.IsTrue(dataAB.ChamferSimilarity > dataBA.ChamferSimilarity);

                //var dataBC = Data.CompareShred(b, c, Direction.FromRight, Orientation.Regular, Direction.FromLeft, Orientation.Regular);
                //var dataCB = Data.CompareShred(b, c, Direction.FromLeft, Orientation.Regular, Direction.FromRight, Orientation.Regular);
                ////Assert.IsTrue(dataBC.ChamferSimilarity > dataCB.ChamferSimilarity);

                //var dataAC = Data.CompareShred(a, c, Direction.FromRight, Orientation.Regular, Direction.FromLeft, Orientation.Regular);
                //var dataCA = Data.CompareShred(a, c, Direction.FromLeft, Orientation.Regular, Direction.FromRight, Orientation.Regular);
                ////Assert.IsTrue(dataBC.ChamferSimilarity > dataAC.ChamferSimilarity);
        }
    }
}
