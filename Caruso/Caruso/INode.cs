#region

using System.Collections.Generic;

#endregion

namespace Algorithmix
{
    public interface INode
    {
        Data MatchData();

        /// <summary>
        ///   Tests whether to given INode is a leaf. A leaf INode is a shred object
        /// </summary>
        /// <returns> true if this is a leaf </returns>
        bool IsLeaf();

        /// <summary>
        ///   Returns the shred objet in the leaf of the INode. If the node is not a leaf node, the it returns null;
        /// </summary>
        /// <returns> Shred Object or NULL if not Leaf </returns>
        Shred Leaf();

        /// <summary>
        ///   Returns the Child node on the left
        /// </summary>
        /// <returns> Another node or NULL if leaf </returns>
        INode Left();

        /// <summary>
        ///   Returns the Child Node on the right
        /// </summary>
        /// <returns> Another Node or NULL if leaf </returns>
        INode Right();

        /// <summary>
        ///   Returns the immediate parent object of the given node
        /// </summary>
        /// <param name="parent"> Parent Node, NULL if it doesn't exist </param>
        INode Parent();

        /// <summary>
        ///   Returns the left most Shred in teh given Inode tree
        /// </summary>
        /// <returns> Shred Object </returns>
        Shred LeftShred();

        /// <summary>
        ///   Returns the right most shred in the given INode tree
        /// </summary>
        /// <returns> Shred object </returns>
        Shred RightShred();

        /// <summary>
        ///   Given an Inode, Flatten will return a list of the Shreds in the order from left to right
        /// </summary>
        /// <param name="list"> </param>
        void Flatten(List<Shred> list);

        /// <summary>
        ///   Explicitly sets the parent of node, if set at the wrong time, the Node tree will be broken.
        /// </summary>
        /// <param name="parent"> node object to be set as parent </param>
        void Parent(INode parent);

        /// <summary>
        ///   Flips the Left and Right Children of the node recursively until the leaf.
        ///   The Leafs will have their edges flipped as well as their orientation toggled.
        /// </summary>
        void Mirror();

        /// <summary>
        ///   Number of Nodes below and including this node
        /// </summary>
        /// <returns> The size of the subtree </returns>
        int Size();

        /// <summary>
        ///   Determines the Root of the INode Tree the node belongs to
        /// </summary>
        /// <returns> Root Node </returns>
        INode Root();

        /// <summary>
        ///   Explictly Sets the Root of the top Most Node
        ///   This will get overwritten at every query and is
        ///   therefore probably useless
        /// </summary>
        /// <param name="root"> </param>
        void Root(INode representative);

        /// <summary>
        ///   Determines the LeftMost Edge
        ///   of the Given Cluster of Shreds
        /// </summary>
        /// <returns> Leftmost Edge object </returns>
        Edge LeftEdge();

        /// <summary>
        ///   Retrieves the Right Most Edge of the Given Cluster of Shreds
        /// </summary>
        /// <returns> Edge Object on the right </returns>
        Edge RightEdge();
    }
}