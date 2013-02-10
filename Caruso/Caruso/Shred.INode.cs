﻿#region

using System.Collections.Generic;

#endregion

namespace Algorithmix
{
    // SHRED INODE IMPLEMENTATION FILE
    public partial class Shred
    {
        private INode _parent;
        private INode _root;
        private Edge _leftedge;
        private Edge _rightedge;

        private void InitializeINode()
        {
            _orientation = Orientation.Regular;
            _parent = null;
            _rightedge = Edge.New(this, Direction.FromRight);
            _leftedge = Edge.New(this, Direction.FromLeft);
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
            return 1;
        }

        public void Mirror()
        {
            Edge swapEdge = _leftedge;
            _leftedge = _rightedge;
            _rightedge = swapEdge;

            Orientation = Enumeration.Opposite(Orientation);
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

        public void Root(INode representative)
        {
            _root = representative;
        }

        public bool IsLeaf()
        {
            // A Shred is a leaf
            return true;
        }

        public Shred Leaf()
        {
            // A Shred is a leaf therefore
            return this;
        }

        public INode Left()
        {
            // A Shred doesn't have a Left
            return null;
        }

        public INode Right()
        {
            // A Shred doesn't have a Right
            return null;
        }

        public Shred LeftShred()
        {
            // leftshred is this shred
            return this;
        }

        public Shred RightShred()
        {
            // rightshred is this shred
            return this;
        }

        public INode Parent()
        {
            return _parent;
        }

        public void Parent(INode parent)
        {
            _parent = parent;
        }

        public void Flatten(List<Shred> list)
        {
            // Just add myself to this list
            list.Add(this);
        }
    }
}