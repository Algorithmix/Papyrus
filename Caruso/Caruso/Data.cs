using System;

namespace Algorithmix
{
    public class Data
    {
        public readonly double[] ChamferScan;
        public readonly double ChamferSimilarity;
        public readonly Side First;
        public readonly int Offset;
        public readonly Side Second;

        /// <summary>
        ///   Creates a Data Object for encapsulating all the comparison data
        /// </summary>
        /// <param name="chamferSimilarity"> </param>
        /// <param name="offset"> </param>
        /// <param name="scan"> </param>
        /// <param name="first"> </param>
        /// <param name="second"> </param>
        public Data(double chamferSimilarity, int offset, double[] scan, Side first, Side second)
        {
            First = first;
            Second = second;
            ChamferSimilarity = chamferSimilarity;
            ChamferScan = scan;
            Offset = offset;
        }

        /// <summary>
        ///   Given two shreds, calculate the offset value at which the two shreds are most similar
        /// </summary>
        /// <param name="first"> The first shred staged for comparison </param>
        /// <param name="second"> The other shred staged for comparison </param>
        /// <param name="directionA"> Direction of this shred to be compared </param>
        /// <param name="orientationA">Orientation of this shred to be compared</param>
        /// <param name="directionB"> Direction of the other shred to be compared </param>
        /// <param name="orientationB">Orientiation of the other shred to be compared</param>
        /// <returns> Tuple containing the max similarity value and the offset at which that occured </returns>
        public static Data CompareShred(Shred first, 
                                        Shred second,
                                        Direction directionA,
                                        Orientation orientationA,
                                        Direction directionB,
                                        Orientation orientationB)
        {
            Side sideA = new Side(first, directionA, orientationA);
            Side sideB = new Side(second, directionB, orientationB);

            double[] scan = Forensics.Chamfer.ScanSimilarity(
                                first.GetChamfer(directionA, orientationA),
                                second.GetChamfer(directionB, orientationB));

            Tuple<double, int> maxData = Utility.Max(scan);
            double max = maxData.Item1;
            int best = maxData.Item2;

            return new Data(max, best, scan, sideA, sideB);
            //return new Tuple<double, int, double[]>(max, best, scan);
        }


        /// <summary>
        /// IsMatch determines if two shreds can be placed next to each other and thus matched.
        /// It returns a match.inverted or match.noninverted if possible or match.impossible if not.
        /// You shouldn't need to call IsMatch, rather use the safe ClusterNodes() method which will 
        /// do it for you 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Match IsMatch(Data data)
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

        /// <summary>
        /// IsFit checks if given a specified side, it can be fit with a shreds's edge
        /// This Also returns a match object
        /// </summary>
        /// <param name="root"></param>
        /// <param name="shred"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        private static Match IsFit(INode root, Shred shred, Side side)
        {
            Side query;
            Match match;

            if (side.Orientation != shred.Orientation)
            {
                query = new Side(shred, Enumeration.Opposite(side.Direction), shred.Orientation);
                match = Match.Inverted;
            }
            else
            {
                query = side;
                match = Match.NonInverted;
            }

            // If Directions match AND The Availablility then return match, otherwise impossible
            if (query.Direction == Direction.FromLeft && root.LeftEdge() == shred.LeftEdge())
            {
                return match;
            }
            if (query.Direction == Direction.FromRight && root.RightEdge() == shred.RightEdge())
            {
                return match;
            }
            return Match.Impossible;
        }

        /// <summary>
        /// This Method ensures a match is possible and if so clusters two nodes
        /// </summary>
        /// <param name="data">Data Object</param>
        /// <returns></returns>
        public static Cluster ClusterNodes(Data data)
        {
            Match match = IsMatch(data);
            return match == Match.Impossible ? null : new Cluster(data.First.Shred, data.Second.Shred, match);
        }

        /// <summary>
        /// This Method clusters two nodes forcibly with no match
        /// </summary>
        /// <param name="data">Data Object</param>
        /// <param name="match">Indicates how the Data is Matched</param>
        /// <returns></returns>
        public static Cluster ForceClusterNodes(Data data, Match match = Match.NonInverted )
        {
            return new Cluster(data.First.Shred, data.Second.Shred, match);
        }
    }
}