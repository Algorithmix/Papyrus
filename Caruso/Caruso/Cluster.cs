﻿using System;
using System.Collections.Generic;

namespace Algorithmix
{
    public enum Relationship
    {
        SameLeftRight,
        SameRightLeft,
        OppositeLeftRight,
        OppositeRightLeft
    }

    public class Cluster : INode
    {

        private readonly INode _left;
        private readonly INode _right;
        private readonly Shred _leftshred;
        private readonly Shred _rightshred;
        private INode _parent;

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
            throw new NotImplementedException("It is not possible to create a Shred from a Cluster");
        }

        public void Flatten(List<Shred> list)
        {
            Left().Flatten(list);
            Right().Flatten(list);
        }

        public Shred RightShred()
        {
            return this._rightshred;
        }

        public Shred LeftShred()
        {
            return this._leftshred;
        }

        public Cluster(INode left, INode right)
        {
            this._left = left;
            this._right = right;
            this._leftshred = left.LeftShred();
            this._rightshred = right.RightShred();
            left.Parent(this);
            right.Parent(this);
        }
    }
}
