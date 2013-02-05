using System;
using System.Collections.Generic;
using System.Linq;
using Algorithmix;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarusoTest
{
    [TestClass]
    public class DataTest
    {

        [TestMethod]
        public void DataMatchTest()
        {
            var shreds = Helpers.InitializeShreds();
            
            var a = shreds[0];
            var b = shreds[1];
            var c = shreds[2];
            var d = shreds[3];
            var e = shreds[4];

            // Merge A-B
            var abData = Data.CompareShred(a, b, 
                                           Direction.FromRight, 
                                           Orientation.Regular,
                                           Direction.FromLeft, 
                                           Orientation.Regular);
            
            var cluster1 = Data.ClusterNodes(abData);
            Assert.IsNotNull(cluster1);
            Assert.IsTrue(cluster1.Right() == b);
            Assert.IsTrue(cluster1.Left() == a);
            Assert.IsTrue(a.Orientation == Orientation.Regular);
            Assert.IsTrue(b.Orientation == Orientation.Regular);
            Helpers.PrintFlatTree(cluster1);
            
            // Merge D'-C' should flip to a C-D merge
            var cdData = Data.CompareShred(c, d,
                                           Direction.FromLeft, 
                                           Orientation.Reversed,
                                           Direction.FromRight, 
                                           Orientation.Reversed);
            
            var cluster2 = Data.ClusterNodes(cdData);
            Assert.IsNotNull(cluster2);
            Assert.IsTrue(cluster2.Right()==d);
            Assert.IsTrue(cluster2.Left() ==c);
            Assert.IsTrue(c.Orientation == Orientation.Regular);
            Assert.IsTrue(d.Orientation == Orientation.Regular);
            Helpers.PrintFlatTree(cluster2);

            // Merge with (A-B)-E by NonInverted
            var beData = Data.CompareShred(b, e,
                                            Direction.FromLeft,
                                            Orientation.Reversed,
                                            Direction.FromRight,
                                            Orientation.Reversed);
            var cluster3 = Data.ClusterNodes(beData);
            Assert.IsNotNull(cluster3);
            Assert.IsTrue(cluster3.Right() == e);
            Assert.IsTrue(cluster3.Left() == cluster1);
            Assert.IsTrue(a.Orientation == Orientation.Regular);
            Assert.IsTrue(b.Orientation == Orientation.Regular);
            Assert.IsTrue(e.Orientation == Orientation.Regular);
            Helpers.PrintFlatTree(cluster3);

            // (C-D)->(D'-C') and merge C and A essentially (D'-C')-((A-B)-E)
            var caData = Data.CompareShred(c,a,
                                            Direction.FromRight, 
                                            Orientation.Reversed,
                                            Direction.FromLeft,
                                            Orientation.Regular);
            var cluster4 = Data.ClusterNodes(caData);

            Assert.IsNotNull(cluster4);
            Assert.IsTrue(cluster4.Size()==5);
            Assert.IsTrue(cluster4.Right()==cluster3);
            Assert.IsTrue(cluster4.Left()==cluster2);
            Assert.IsTrue(c.Orientation == Orientation.Reversed);
            Assert.IsTrue(d.Orientation == Orientation.Reversed);
            Assert.IsTrue(a.Orientation == Orientation.Regular);
            Assert.IsTrue(b.Orientation == Orientation.Regular);
            Assert.IsTrue(e.Orientation == Orientation.Regular);
            Helpers.PrintFlatTree(cluster4);

            // Ensure it Flattens correctly;
            var actual = new List<Algorithmix.Shred>(cluster4.Size());
            cluster4.Flatten(actual);

            var expected = new List<Algorithmix.Shred>(5);
            expected.Add(d);
            expected.Add(c);
            expected.Add(a);
            expected.Add(b);
            expected.Add(e);

            Assert.IsTrue(actual.Zip(expected, (aa, ee) => aa.Id == ee.Id).All(item => item == true) == true);
          }

        // Create (D'-C')-(A-B)
        
    }
}
