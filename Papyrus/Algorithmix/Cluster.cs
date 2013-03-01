using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Algorithmix
{
    public partial class Cluster : INode
    {
        private static long _count;
        public long Id;

        /// <summary>
        ///   Clusters Two INodes together, sets new root/parent/left and right edges
        /// </summary>
        /// <param name="left"> Node on the left </param>
        /// <param name="right"> Node on the Right </param>
        /// <param name="match"> Inverted or Not Inverted </param>
        /// <param name="matchData"> Optional MatchData variable, store clustering information </param>
        public Cluster(INode left, INode right, Match match = Match.NonInverted, MatchData matchData = null)
        {
            if (match == Match.Impossible)
            {
                throw new ArgumentException("Match is apparently impossible why are you trying?");
            }

            // Now Build the new nodes
            _left = left;
            _right = right;
            _matchData = matchData;
            _size = left.Size() + right.Size();
            _leftedge = left.LeftEdge();
            _rightedge = right.RightEdge();

            // Set the parents accordingly
            left.Parent(this);
            right.Parent(this);
            _parent = null;

            // change the roots
            _left.Root(this);
            _right.Root(this);

            // Update Count
            Id = _count++;
        }

        public List<Shred> Flattened
        {
            get
            {
                List<Shred> flattened = new List<Shred>(this.Size());
                this.Flatten(flattened);
                return flattened;
            }
        }
    }
}
