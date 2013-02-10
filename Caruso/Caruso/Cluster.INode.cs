#region

using System;
using System.Collections.Generic;

#endregion

namespace Algorithmix
{
    [Serializable]
    public class Cluster : INode
    {
        private INode _left;
        private INode _right;
        private INode _parent;
        private Edge _rightedge;
        private Edge _leftedge;
        private INode _root;
        private readonly int _size;
        private Data _data;

        public Data Data()
        {
            return _data;
        }

        public INode Root()
        {
            INode node = this;
            if (node.Parent() == null)
            {
                return node;
            }
            return node.Parent().Root();
        }


        public void Root(INode root)
        {
            _root = root;
        }

        public Edge RightEdge()
        {
            return _rightedge;
        }

        public Edge LeftEdge()
        {
            return _leftedge;
        }

        public int Size()
        {
            return _size;
        }

        public void Mirror()
        {
            INode swapNode = _left;
            _left = _right;
            _right = swapNode;

            Edge swapEdge = _leftedge;
            _leftedge = _rightedge;
            _rightedge = swapEdge;

            _left.Mirror();
            _right.Mirror();
        }

        public INode Parent()
        {
            return _parent;
        }

        public INode Right()
        {
            return _right;
        }

        public INode Left()
        {
            return _left;
        }

        public bool IsLeaf()
        {
            return false;
        }

        public void Parent(INode parent)
        {
            _parent = parent;
        }

        public Shred Leaf()
        {
            return null;
        }

        public void Flatten(List<Shred> list)
        {
            Left().Flatten(list);
            Right().Flatten(list);
        }

        public Shred RightShred()
        {
            return _rightedge.Shred;
        }

        public Shred LeftShred()
        {
            return _leftedge.Shred;
        }

        /// <summary>
        ///   Clusters Two INodes together, sets new root/parent/left and right edges
        /// </summary>
        /// <param name="left"> Node on the left </param>
        /// <param name="right"> Node on the Right </param>
        /// <param name="match"> Inverted or Not Inverted </param>
        public Cluster(INode left, INode right, Match match = Match.NonInverted, Data data = null)
        {
            if (match == Match.Impossible)
            {
                throw new ArgumentException("Match is apparently impossible why are you trying?");
            }

            // Now Build the new nodes
            _left = left;
            _right = right;
            _data = data;
            _size = left.Size() + right.Size();
            _leftedge = left.LeftEdge();
            _rightedge = right.RightEdge();

            // Set the parents accordingly
            left.Parent(this);
            right.Parent(this);
            _parent = null;

            // change the roots
            _left.Root(this);
            _right.Root(this);
        }
    }
}