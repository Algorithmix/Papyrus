using System.Collections.Generic;

namespace Algorithmix
{
    public interface INode
    {
        /// <summary>
        /// Tests whether to given INode is a leaf. A leaf INode is a shred object
        /// </summary>
        /// <returns>true if this is a leaf</returns>
        bool IsLeaf();
        
        Shred Leaf();
        
        INode Left();
        
        INode Right();
        
        INode Parent();

        Shred LeftShred();

        Shred RightShred();

        void Flatten(List<Shred> list);
        
        void Parent(INode parent);
    }
}
