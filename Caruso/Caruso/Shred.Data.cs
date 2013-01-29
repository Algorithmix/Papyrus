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

        public static Match Query( Data data )
        {
            Side first = data.First;
            Side second = data.Second;
            INode firstRoot = first.Shred.Root();
            INode secondRoot = second.Shred.Root();
            
            // Cannot match when you have the same root
            if (firstRoot == secondRoot)
            {
                return Match.Impossible;
            }

            Match firstFit = IsFit(firstRoot, first.Shred, first);
            if (firstFit == Match.Impossible)
            {
                return Match.Impossible;
            }
            Match secondFit = IsFit(secondRoot, second.Shred, second);
            if (secondFit == Match.Impossible)
            {
                return Match.Impossible;
            }

            return Enumeration.Combination(firstFit, secondFit);
        }
            
         private static Match IsFit(INode root, Shred shred, Side side)
         {
            Side query;
            Match match;
            
            if (side.Orientation != shred.Orientation )
            {
                query = new Side(shred, Enumeration.Opposite(side.Direction), shred.Orientation);
                match = Match.Inverted;
            }
            else {
                query = side;
                match = Match.NonInverted;
            }

            // If Directions match AND The Availablility then return match, otherwise impossible
            if ( query.Direction == Direction.FromLeft && root.LeftEdge() == shred.LeftEdge() )
            {
                return match;
            }
            if ( query.Direction == Direction.FromRight  && root.RightEdge()== shred.RightEdge() )
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
