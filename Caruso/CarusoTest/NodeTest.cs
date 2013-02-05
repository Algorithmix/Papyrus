#region

using System;
using System.Collections.Generic;
using System.Linq;
using Algorithmix;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class NodeTest
    {
        [TestMethod]
        public void MirrorTest()
        {
            var shredsRegular = Helpers.InitializeShreds();
            var rootRegular = Helpers.BuildCluster(shredsRegular);

            var shredsReversed = Helpers.InitializeShreds();
            var rootReversed = Helpers.BuildCluster(shredsReversed);

            // Mirror this tree
            rootReversed.Mirror();
            Assert.IsTrue(IsMirror(rootRegular, rootReversed));

            rootReversed.Mirror();
            Assert.IsTrue(IsEqual(rootRegular, rootReversed));
        }

        private static bool IsEqual(INode nodeA, INode nodeB)
        {
            if (nodeA.IsLeaf() ^ nodeB.IsLeaf())
            {
                throw new Exception("Identical nodes should be at the same height");
            }

            Assert.IsTrue(nodeA.LeftEdge().Direction == nodeB.LeftEdge().Direction);
            Assert.IsTrue(nodeA.RightEdge().Direction == nodeB.RightEdge().Direction);
            Assert.IsTrue(nodeA.LeftEdge().Orientation == nodeB.LeftEdge().Orientation);
            Assert.IsTrue(nodeA.RightEdge().Orientation == nodeB.RightEdge().Orientation);

            if (nodeA.IsLeaf() && nodeB.IsLeaf())
            {
                return true;
            }

            return IsEqual(nodeA.Left(), nodeB.Left()) && IsEqual(nodeA.Right(), nodeB.Right());
        }

        private static bool IsMirror(INode reg, INode rev)
        {
            if (reg.IsLeaf() ^ rev.IsLeaf())
            {
                throw new Exception("Mirrored nodes are not at the same height");
            }

            if (reg.IsLeaf() && rev.IsLeaf())
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
            var shreds = Helpers.InitializeShreds();
            var flattened = new List<Algorithmix.Shred>(shreds.Count());
            Helpers.BuildCluster(shreds).Flatten(flattened);

            var ids = shreds.Select(shred => shred.Id).ToList();
            var actual = flattened.Select(shred => shred.Id).ToList();
            var expected = new List<long> {ids[6], ids[0], ids[1], ids[3], ids[4], ids[2], ids[5]};
            Assert.IsTrue(actual.Zip(expected, (first, second) => first == second).All(eq => eq == true));
        }

        [TestMethod]
        public void ValidateTree()
        {
            var shreds = Helpers.InitializeShreds();
            var root = Helpers.BuildCluster(shreds);
            Assert.IsTrue(IsValid(root));
        }

        [TestMethod]
        public void NodeEdgeTest()
        {
            var shreds = Helpers.InitializeShreds();
            var root = Helpers.BuildCluster(shreds);
            Assert.IsTrue(ValidateEdges(root));
        }

        [TestMethod]
        public void NodeSizeTest()
        {
            var shreds = Helpers.InitializeShreds();
            var root = Helpers.BuildCluster(shreds);
            int expected = root.Size();
            int actual = CalculateSize(root);
            Assert.IsTrue(actual == expected);
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
            if (node.Left() != null && node.Right() != null && !node.IsLeaf())
            {
                return CalculateSize(node.Left()) + CalculateSize(node.Right());
                ;
            }

            if (node.Left() == null && node.Right() == null && node.IsLeaf())
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