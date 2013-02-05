using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static void PrintFlatTree(INode root)
        {
            var list = new List<Algorithmix.Shred>(root.Size());
            root.Flatten(list);
            list.ForEach(item => Console.Write(item.Id + " , "));
            Console.WriteLine("");
        }

        public static void PrintTree(INode root)
        {
            int maxLevel = GetNodeHeight(root);
            var list = new Queue<INode>();
            list.Enqueue(root);
            PrintTree(list,1,maxLevel);
        }

        public static void PrintTree(Queue<INode> queue, int level, int maxLevel)
        {
            if ( queue.Count == 0 || queue.All(item => item == null ) )
            {
                return;
            }

            int floor = maxLevel - level;
            int edgeLines = (int) Math.Pow(2, (Math.Max(floor - 1, 0)));
            int firstSpaces = (int) Math.Pow(2, floor) - 1;
            int betweenSpaces = (int) Math.Pow(2, (floor + 1)) - 1;

            PrintWhiteSpace(firstSpaces);
            
            List<INode> nodes = new List<INode>(queue.Count);
            while( queue.Count > 0  )
            {
                nodes.Add(queue.Dequeue());
            }

            foreach ( INode node in nodes)
            {   
                if (node != null )
                {
                    if (node.IsLeaf())
                    {
                        Console.Write(node.Leaf().Id);
                    }
                    else
                    {
                        Console.Write(".");
                    }

                    queue.Enqueue(node.Left());
                    queue.Enqueue(node.Right());
                }
                else
                {
                    queue.Enqueue(null);
                    queue.Enqueue(null);
                    PrintWhiteSpace(1);
                }
                PrintWhiteSpace(betweenSpaces);
            }

            Console.WriteLine();

            for (int ii = 1; ii <= edgeLines; ii++) 
            {
                for (int jj = 0; jj < nodes.Count; jj++) 
                {
                    PrintWhiteSpace(firstSpaces - ii);
                    if (nodes[jj] == null) {
                        PrintWhiteSpace(edgeLines + edgeLines + ii + 1);
                        continue;
                    }

                    if (nodes[jj].Left() != null)
                    {
                        Console.Write("/");
                    }
                    else
                    {
                        PrintWhiteSpace(1);
                    }

                    PrintWhiteSpace(ii + ii - 1);

                    if (nodes[jj].Right() != null)
                    {
                        Console.Write("\\");
                    }
                    else
                    {
                        PrintWhiteSpace(1);
                    }

                    PrintWhiteSpace(edgeLines + edgeLines - ii);
                }

                Console.WriteLine("");
            }

        PrintTree(queue, level+1 , maxLevel);

        }

        public static void PrintWhiteSpace(int number)
        {
            for(int ii =0; ii < number; ii++)
            {
                Console.Write(" ");
            }
        }

        public static int GetNodeHeight(INode root)
        {
            return root != null ? 0 : Math.Max(GetNodeHeight(root.Left()), GetNodeHeight(root.Right()))+1;
        }
    }
}
