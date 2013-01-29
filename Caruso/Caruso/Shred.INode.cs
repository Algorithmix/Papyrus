using System.Collections.Generic;

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
            this._orientation = Orientation.Regular;
            this._parent = null;
            this._rightedge = Edge.New(this,Direction.FromRight);
            this._leftedge = Edge.New(this,Direction.FromLeft);
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
            return 1;
        }

        public void Mirror()
        {
            Edge swapEdge = this._leftedge;
            this._rightedge = this._leftedge;
            this._leftedge = swapEdge;

            this.Orientation = Enumeration.Opposite(this.Orientation);
        }

        public INode Root()
        {
            // Using Path Compression
            INode node = this;
            if (node.Parent() == null)
            {
                return node;
            }
            node.Root(node.Parent().Root());
            return node.Root();
        }

        public void Root(INode representative)
        {
            this._root = representative;
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
            return this._parent;
        }

        public void Parent(INode parent)
        {
            this._parent = parent;
        }

        public void Flatten(List<Shred> list)
        {
            // Just add myself to this list
            list.Add(this);
        }
    }
}
