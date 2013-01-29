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

        private List<Algorithmix.Shred> Initialize()
        {
            Console.WriteLine("Building Shreds");
            var path = Path.Combine(Drive.GetDriveRoot(), CarusoTestDirectory, FewTestMaterials);
            var shreds = Algorithmix.Shred.Factory("image", path);
            return shreds;
        }

        private INode Assemble(List<Algorithmix.Shred> shreds)
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

        [TestMethod]
        public void MirrorTest()
        {
            var shredsRegular = Initialize();
            var rootRegular = Assemble(shredsRegular);

            var shredsReversed = Initialize();
            var rootReversed = Assemble(shredsReversed);
            
            // Mirror this tree
            rootReversed.Mirror();

            Assert.IsTrue( IsMirror( rootRegular, rootReversed ) );
        }

        private static bool IsMirror( INode reg, INode rev)
        {
            if (reg.IsLeaf() ^ rev.IsLeaf())
            {
                throw new Exception("Mirrored nodes are not at the same height");
            }

            if ( reg.IsLeaf() && rev.IsLeaf() )
            {
                Assert.IsTrue(reg.Size() == rev.Size());
                Assert.IsTrue(reg.Leaf().Orientation == Enumeration.Opposite(rev.Leaf().Orientation));
                Assert.IsTrue(reg.LeftEdge().Direction == Enumeration.Opposite(rev.RightEdge().Direction));
                return true;
            }

            return IsMirror(reg.Left(), rev.Right()) && IsMirror(reg.Right(), rev.Left());
        }

        [TestMethod]
        public void FlattenTest()
        {
            var shreds = Initialize();
            var flattened = new List<Algorithmix.Shred>(shreds.Count());
            Assemble(shreds).Flatten(flattened);

            var ids = shreds.Select(shred => shred.Id).ToList();
            var actual = flattened.Select(shred => shred.Id).ToList();
            var expected = new List<long> { ids[6], ids[0], ids[1], ids[3], ids[4], ids[2], ids[5] };
            Assert.IsTrue(actual.Zip(expected, (first, second) => first == second).All(eq => eq == true));
        }

        [TestMethod]
        public void ValidateTree()
        {
            var shreds = Initialize();
            var root = Assemble(shreds);
            Assert.IsTrue(IsValid(root));
        }

        [TestMethod]
        public void NodeEdgeTest()
        {
            var shreds = Initialize();
            var root = Assemble(shreds);
            Assert.IsTrue( ValidateEdges(root) );
        }

        [TestMethod]
        public void NodeSizeTest()
        {
            var shreds = Initialize();
            var root = Assemble(shreds);
            int expected = root.Size();
            int actual = CalculateSize(root);
            Assert.IsTrue( actual == expected );
        }

        private static bool ValidateEdges(INode node)
        {
            if (node.IsLeaf())
            {
                return true;
            }

            // Get the expected
            var actualLeft = node.LeftEdge();
            var actualRight = node.RightEdge();

            INode left = node;
            INode right = node;
            while (!left.IsLeaf())
            {
                left = left.Left();
            }
            while (!right.IsLeaf())
            {
                right = right.Right();
            }

            var expectedLeft = left.LeftEdge();
            var expectedRight = right.RightEdge();

            Assert.IsTrue(actualLeft == expectedLeft);
            Assert.IsTrue(actualRight == expectedRight);

            if (actualLeft != expectedLeft || actualRight != expectedRight)
            {
                return false;
            }

            // Repeat for node children
            return ValidateEdges(node.Left()) && ValidateEdges(node.Right());
        }

        private static int CalculateSize(INode node)
        {
            if (node.Left()!= null && node.Right()!=null && !node.IsLeaf())
            {
                return CalculateSize(node.Left()) + CalculateSize(node.Right());
;           }
            
            if ( node.Left()==null && node.Right()==null && node.IsLeaf())
            {
                return node.Size();
            }

            throw new Exception("Badly Constructed INode Tree");
        }


        private static bool IsValid(INode parent)
        {
            if (parent.Left() == null && parent.Right() == null)
            {
                return true;
            }
            else if (parent.Left() == null ^ parent.Right() == null)
            {
                throw new ArgumentException("Invalid INode - Must have left/right members or both must be null");
            }
            else
            {
                bool validLeft = parent.Left().Parent() == parent;
                bool validRight = parent.Right().Parent() == parent;
                if (validLeft && validRight)
                {
                    return IsValid(parent.Left()) && IsValid(parent.Right());
                }
                else
                {
                    return false;
                }
            }
        }
    }
}