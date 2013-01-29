using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithmix
{
    public partial class Shred
    {
        private Tuple<Side,Side> MatchSide( Data data )
        {
            if ( data.First.Shred == this )
            {
                return new Tuple<Side, Side>( data.First, data.Second);
            }
            return new Tuple<Side, Side>( data.Second, data.First );
        }

        public Match Query(Data data)
        {
            Tuple<Side, Side> sides = MatchSide(data);
            Side mySide = sides.Item1;
            Side otherSide = sides.Item2;
            Shred other = otherSide.Shred;

            INode myRoot = this.Root();
            
            // Cannot match when you have the same root
            if ( myRoot == other.Root() )
            {
                return Match.Impossible;
            }

            // Assign the Query -> If the orientation is different we have to invert the query
            Side query;
            Match match;
            if (mySide.Orientation != this.Orientation )
            {
                query = new Side(this, Enumeration.Opposite(mySide.Direction), this.Orientation);
                match = Match.Inverted;
            }
            else {
                query = mySide;
                match = Match.NonInverted;
            }

            // If Directions match AND The Availablility then return match, otherwise impossible
            if ( query.Direction == Direction.FromLeft && myRoot.LeftEdge() == this.LeftEdge() )
            {
                return match;
            }
            if ( query.Direction == Direction.FromRight  && myRoot.RightEdge()== this.RightEdge() )
            {
                return match;
            }
            return Match.Impossible;
        }

        /// <summary>
        ///   Given two shreds, calculate the offset value at which the two shreds are most similar
        /// </summary>
        /// <param name="other"> The other shred to be compared to </param>
        /// <param name="directionA"> Direction of this shred to be compared </param>
        /// <param name="orientationA">Orientation of this shred to be compared</param>
        /// <param name="directionB"> Direction of the other shred to be compared </param>
        /// <param name="orientationB">Orientiation of the other shred to be compared</param>
        /// <returns> Tuple containing the max similarity value and the offset at which that occured </returns>
        public Data Compare(Shred other, 
                                                    Direction directionA,
                                                    Orientation orientationA, 
                                                    Direction directionB,
                                                    Orientation orientationB)
        {
            Side sideA = new Side(this,directionA,orientationA);
            Side sideB = new Side(other,directionB,orientationB);

            double[] scan = Forensics.Chamfer.ScanSimilarity(
                                this.GetChamfer(directionA, orientationA), 
                                other.GetChamfer(directionB, orientationB));

            Tuple<double, int> maxData = Utility.Max(scan);
            double max = maxData.Item1;
            int best = maxData.Item2;

            return new Data(max, best, scan, sideA, sideB);
            //return new Tuple<double, int, double[]>(max, best, scan);
        }
    }
}
