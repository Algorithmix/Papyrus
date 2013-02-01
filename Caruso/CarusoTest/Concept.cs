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
        public void ScannedDocumentSequentialTest()
        {
            ParameterTest(Helpers.PrimitiveTestScanned, "image");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void ScannedDocumentSequentialContrastTest()
        {
            ParameterTest(Helpers.PrimitiveTestScannedContrast, "shred");
        }


        /// <summary>
        /// This Test Method 
        /// </summary>
        [TestMethod]
        public void PDFRequirementScannedTests()
        {
            BestMatch(Helpers.PDFRequiremnetTestFull1, "image");
        }


         public void BestMatch(string directory, string prefix )
        {  
            Algorithmix.Shred.BUFFER = 6;
            Algorithmix.Shred.SAMPLE_SIZE = 4;
            Algorithmix.Shred.PEAK_THRESHOLD = 0.15;

            var shreds = Helpers.BootStrapPDFRequiementScanned1(directory, prefix);
            var bestRight = new List<Data>(shreds.Count);
            var bestLeft = new List<Data>(shreds.Count);
            var correctRight = new List<Data>(shreds.Count);
            var correctLeft = new List<Data>(shreds.Count);
             foreach (var shred in shreds)
            {
                var datas = new List<Data>((shreds.Count-1)*4);
                for (int ii = 0; ii < shreds.Count; ii++)
                {
                    var other = shreds[ii];
                    if (shred.Id == other.Id)
                    {
                        continue;
                    }

                    var dataForwadsRegular = Data.CompareShred(shred, other, Direction.FromRight, Orientation.Regular, Direction.FromLeft, Orientation.Regular);
                    var dataBackwardsRegular = Data.CompareShred(shred, other, Direction.FromLeft, Orientation.Regular, Direction.FromRight, Orientation.Regular);
                    var dataForwardsReverse = Data.CompareShred(shred, other, Direction.FromRight, Orientation.Reversed, Direction.FromLeft, Orientation.Regular);
                    var dataBackwardsReverse = Data.CompareShred(shred, other, Direction.FromLeft, Orientation.Reversed, Direction.FromRight, Orientation.Regular);
                    datas.Add(dataForwadsRegular);
                    datas.Add(dataBackwardsRegular);
                    datas.Add(dataForwardsReverse);
                    datas.Add(dataBackwardsReverse);
                    
                    if (shred.Id == (other.Id - 1) && shred.Id != (shreds.Count-1) )
                    {
                        correctRight.Add(dataForwadsRegular);
                    }
                    //else if (shred.Id == (other.Id + 1) && shred.Id != (shreds.First().Id))
                    //{
                    //    correctLeft.Add(dataBackwardsRegular);
                    //}
                }


                var maxFirst = datas.Where( data => 
                    (data.First.Direction==Direction.FromRight && data.First.Orientation==Orientation.Regular) ||
                    (data.First.Direction == Enumeration.Opposite(Direction.FromRight) && data.First.Orientation == Enumeration.Opposite(Orientation.Regular) ))
                    .Max(x => x.ChamferSimilarity);
                
                 bestRight.Add(datas.Where( data => 
                    (data.First.Direction==Direction.FromRight && data.First.Orientation==Orientation.Regular) ||
                    (data.First.Direction == Enumeration.Opposite(Direction.FromRight) && data.First.Orientation == Enumeration.Opposite(Orientation.Regular) ))
                    .First(x => x.ChamferSimilarity == maxFirst));
                 
                 //var maxSecond = datas.Where(data =>
                 //   (data.First.Direction == Direction.FromLeft && data.First.Orientation == Orientation.Regular) ||
                 //   (data.First.Direction == Enumeration.Opposite(Direction.FromLeft) && data.First.Orientation == Enumeration.Opposite(Orientation.Regular)))
                 //   .Max(x => x.ChamferSimilarity);

                 //bestLeft.Add(datas.Where(data =>
                 //   (data.First.Direction == Direction.FromLeft && data.First.Orientation == Orientation.Regular) ||
                 //   (data.First.Direction == Enumeration.Opposite(Direction.FromLeft) && data.First.Orientation == Enumeration.Opposite(Orientation.Regular)))
                 //   .First(x => x.ChamferSimilarity == maxSecond));

            }            
            int index = (int)shreds.First().Id;
            int desired = 0;
            for (int  ii = 0; ii< shreds.Count()-1 ;ii++)
            {
                if ( bestRight[ii] == correctRight[ii] )
                {
                    desired++;
                    Console.WriteLine("Best Match to the shred " + ii + " is CORRECTLY " + bestRight[ii].Second.Shred.Id);
                    Console.WriteLine("Certainty " + bestRight[ii].ChamferSimilarity);

                }
                else 
                {
                    Console.WriteLine("Best Match to the shred " + ii + " is really " + bestRight[ii].Second.Shred.Id);
                    Console.WriteLine("Best " + bestRight[ii].ChamferSimilarity + " vs Correct :" + correctRight[ii].ChamferSimilarity);
                }
            }

            //for (int ii = 1; ii < shreds.Count() ; ii++)
            //{
            //    if (bestLeft[ii] == correctLeft[ii-1])
            //    {
            //        desired++;
            //        Console.WriteLine("Best Match to the shred " + ii + " is CORRECTLY " + bestLeft[ii].Second.Shred.Id);
            //        Console.WriteLine("Certainty " + bestLeft[ii].ChamferSimilarity);

            //    }
            //    else
            //    {
            //        Console.WriteLine("Best Match to the shred " + ii + " is really " + bestLeft[ii].Second.Shred.Id);
            //        Console.WriteLine("Best " + bestLeft[ii].ChamferSimilarity + " vs Correct :" + correctLeft[ii].ChamferSimilarity);
            //    }
            //}
            Console.WriteLine("We go " + desired + " correct");

            }

        public void ParameterTest(string directory, string prefix )
        {  
            Algorithmix.Shred.BUFFER = 6;
            Algorithmix.Shred.SAMPLE_SIZE = 4;
            for (double param = 0.01; param < 0.5; param+=0.01)
            {
                Algorithmix.Shred.PEAK_THRESHOLD = param;


                //var shreds = Helpers.BootstrapPrimitiveScanned(directory, prefix);

                //For [TestMethod]
                //public void PDFRequirementScannedTests()
                var shreds = Helpers.BootStrapPDFRequiementScanned1(directory, prefix);

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
