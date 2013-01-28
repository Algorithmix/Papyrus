using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Algorithmix;
using Algorithmix.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarusoTest
{
    [TestClass]
    public class INodeTest
    {
        public static readonly string CarusoTestDirectory = "CarusoUnitTest";
        public static readonly string FewTestMaterials = "GettysburgAddressFew";

        [TestMethod]
        public void FlattenTest()
        {
            Console.WriteLine("Building 10 Shreds");
            var path = Path.Combine(Drive.GetDriveRoot(), CarusoTestDirectory, FewTestMaterials);
            var shreds = Algorithmix.Shred.Factory("image",path);
            var ids = shreds.Select(shred => shred.Id).ToList();
            
            // Now we have a list of Shreds
            // Lets join them in this order (6,((0,((1,3),4)),(2,5)))
            var cluster13 = new Cluster(shreds[1], shreds[3]);
            var cluster134 = new Cluster(cluster13, shreds[4]);
            var cluster0134 = new Cluster(shreds[0], cluster134);
            var cluster25 = new Cluster(shreds[2], shreds[5]);
            var cluster013425 = new Cluster(cluster0134, cluster25);
            var cluster6013425 = new Cluster(shreds[6], cluster013425);
            var flattened = new List<Algorithmix.Shred>(shreds.Count()); 
            cluster6013425.Flatten( flattened);
            
            var actual = flattened.Select(shred => shred.Id).ToList();
            var expected = new List<long> {ids[6], ids[0], ids[1], ids[3], ids[4], ids[2], ids[5]};
            Assert.IsTrue(actual.Zip(expected,(first,second)=> first==second).All(eq=>eq==true));
        }
    }
}
