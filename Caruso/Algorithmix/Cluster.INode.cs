#region

using System;
using System.Collections.Generic;

#endregion

namespace Algorithmix
{
    [Serializable]
    public partial class Cluster : INode
    {
        private INode _left;
        private INode _right;
        private INode _parent;
        private Edge _rightedge;
        private Edge _leftedge;
        private INode _root;
        private readonly int _size;
        private readonly Data _data;

        public Data MatchData()
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

    }
}