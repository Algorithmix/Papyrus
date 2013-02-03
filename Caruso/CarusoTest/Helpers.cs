using System;
using System.Collections.Generic;
using System.IO;
using Algorithmix;
using Algorithmix.TestTools;

namespace CarusoTest
{
    public class Helpers
    {
        public static readonly string CarusoTestDirectory = "CarusoUnitTest";
        public static readonly string FewTestMaterials = "GettysburgAddressFew";
        public static readonly string PrimitiveTestDirectory = "PrimitiveTest";
        public static readonly string PrimitiveTestThreeNormal = @"3ShredTest\NormalOrder";

        public static List<Algorithmix.Shred> BootstrapPrimitiveThree() 
        {
            Console.WriteLine("Building Shreds");
            var path = Path.Combine(Drive.GetDriveRoot(), PrimitiveTestDirectory, PrimitiveTestThreeNormal);
            var shreds = Algorithmix.Shred.Factory("Shred", path);
            return shreds;
        }

        public static List<Algorithmix.Shred> InitializeShreds()
        {
            Console.WriteLine("Building Shreds");
            var path = Path.Combine(Drive.GetDriveRoot(), CarusoTestDirectory, FewTestMaterials);
            var shreds = Algorithmix.Shred.Factory("image", path);
            return shreds;
        }
        
        public static INode BuildCluster(List<Algorithmix.Shred> shreds)
        {
            // Lets join them in this order (6,((0,((1,3),4)),(2,5)))
            var cluster13 = new Cluster(shreds[1], shreds[3]);
            var cluster134 = new Cluster(cluster13, shreds[4]);
            var cluster0134 = new Cluster(shreds[0], cluster134);
            var cluster25 = new Cluster(shreds[2], shreds[5]);
            var cluster013425 = new Cluster(cluster0134, cluster25);
            var cluster6013425 = new Cluster(shreds[6], cluster013425);
            return cluster6013425;
        }

        public static void PrintTree(INode root)
        {
            var list = new List<Algorithmix.Shred>(root.Size());
            root.Flatten(list);
            list.ForEach(item => Console.Write(item.Id + " , "));
            Console.WriteLine("");
        }
    }
}
