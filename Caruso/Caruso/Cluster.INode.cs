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
            if (node.Parent() != null)
            {
                node.Root( node.Parent().Root() );
            }
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
            this._right = this._left;
            this._left = swapNode;

            Edge swapEdge = this._leftedge;
            this._rightedge = this._leftedge;
            this._leftedge = swapEdge;

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
            // Build the information
            this._left = left;
            this._right = right;
            this._size = left.Size() + right.Size();
            this._leftedge = left.LeftEdge();
            this._rightedge = right.RightEdge();

            // Set the parents
            left.Parent(this);
            right.Parent(this);
            this._parent = null;

            // Set TopMost Cluster;
            INode leftRepresentative = this._left.Root();
            INode rightRepresentative = this._right.Root();
            if (leftRepresentative == rightRepresentative )
            {
                throw new ArgumentException("Both Nodes have same representative");
            }

            //this._root = this;
            this._left.Root(this);
            this._right.Root(this);
        }
    }
}
