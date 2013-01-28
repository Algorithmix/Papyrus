using System.Collections.Generic;

namespace Algorithmix
{

    // SHRED INODE IMPLEMENTATION FILE
    public partial class Shred
    {
        private INode _parent;

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
