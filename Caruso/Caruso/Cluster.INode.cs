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
        private Data _data;

        public Data Data() 
        {
            return this._data;
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

        /// <summary>
        /// Clusters Two INodes together, sets new root/parent/left and right edges
        /// </summary>
        /// <param name="left">Node on the left</param>
        /// <param name="right">Node on the Right</param>
        /// <param name="match">Inverted or Not Inverted</param>
        public Cluster(INode left, INode right, Match match = Match.NonInverted , Data data = null)
        {
            if (match == Match.Impossible)
            {
                throw new ArgumentException("Match is apparently impossible why are you trying?");
            }

            // Now Build the new nodes
            this._left = left;
            this._right = right;
            this._data = data;
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
