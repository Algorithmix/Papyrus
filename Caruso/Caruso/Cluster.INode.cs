using System;
using System.Collections.Generic;

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

        public INode Root()
        {
            // Using Path Compression
            INode node = this;
            if (node.Parent() == null)
            {
                return node;
            }
            node.Root( node.Parent().Root());
            return node.Root();
        }

        public void Root(INode root)
        {
            this._root = root;
        }

        public Edge RightEdge()
        {
            return this._rightedge;
        }

        public Edge LeftEdge()
        {
            return this._leftedge;
        }

        public int Size()
        {
            return this._size;
        }

        public void  Mirror()
        {
            INode swapNode = this._left;
            this._left = this._right;
            this._right = swapNode;

            Edge swapEdge = this._leftedge;
            this._leftedge = this._rightedge;
            this._rightedge = swapEdge;

            this._left.Mirror();
            this._right.Mirror();
        }

        public INode Parent()
        {
            return this._parent;
        }

        public INode Right()
        {
            return this._right;
        }

        public INode Left()
        {
            return this._left;
        }

        public bool IsLeaf()
        {
            return false;
        }

        public void Parent(INode parent)
        {
            this._parent = parent;
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
            return this._rightedge.Shred;
        }

        public Shred LeftShred()
        {
            return this._leftedge.Shred;
        }

        public Cluster(INode left, INode right)
        {
            // First check to ensure roots are not same
            // Set TopMost Cluster;
            INode leftroot = left.Root();
            INode rightroot = right.Root();
            if (leftroot == rightroot)
            {
                throw new ArgumentException("Both Nodes have same representative");
            }
            
            // Now Build the new nodes
            this._left = left;
            this._right = right;
            this._size = left.Size() + right.Size();
            this._leftedge = left.LeftEdge();
            this._rightedge = right.RightEdge();

            // Set the parents accordingly
            left.Parent(this);
            right.Parent(this);
            this._parent = null;

            // change the roots
            this._left.Root(this);
            this._right.Root(this);
        }
    }
}
