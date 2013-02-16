#region

using System;
using Algorithmix.Forensics;

#endregion

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
        /// <param name="orientationA"> Orientation of this shred to be compared </param>
        /// <param name="directionB"> Direction of the other shred to be compared </param>
        /// <param name="orientationB"> Orientiation of the other shred to be compared </param>
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

            double[] scan = Chamfer.ScanSimilarity(
                first.GetChamfer(directionA, orientationA),
                second.GetChamfer(directionB, orientationB));

            Tuple<double, int> maxData = Utility.Max(scan);
            double max = maxData.Item1;
            int best = maxData.Item2;

            return new Data(max, best, scan, sideA, sideB);
            //return new Tuple<double, int, double[]>(max, best, scan);
        }

        /// <summary>
        ///   IsMatch determines if two shreds can be placed next to each other and thus matched.
        ///   It returns a match.inverted or match.noninverted if possible or match.impossible if not.
        ///   You shouldn't need to call IsMatch, rather use the safe ClusterNodes() method which will 
        ///   do it for you
        /// </summary>
        /// <param name="data"> </param>
        /// <returns> </returns>
        private static ClusterData IsMatch(Data data, INode firstRoot, INode secondRoot)
        {
            Side first = data.First;
            Side second = data.Second;
            Tuple<Match, Direction> firstFit;
            Tuple<Match, Direction> secondFit;
            // Cannot match when you have the same root
            if (firstRoot == secondRoot)
            {
                return null;
            }

            firstFit = IsFit(firstRoot, first.Shred, first);
            if (firstFit.Item1 == Match.Impossible)
            {
                return null;
            }
            secondFit = IsFit(secondRoot, second.Shred, second);
            if (secondFit.Item1 == Match.Impossible)
            {
                return null;
            }

            //if (firstFit.Item1 == Match.Inverted && secondFit.Item1 == Match.Inverted)
            //{
            //    return new ClusterData(Match.NonInverted, secondFit.Item2, firstFit.Item2);
            //}
            //else if (firstFit.Item1 == Match.NonInverted && secondFit.Item1 == Match.NonInverted)
            if (firstFit.Item1 == secondFit.Item1)
            {
                return new ClusterData(Match.NonInverted, firstFit.Item2, secondFit.Item2);
            }
            else if (firstFit.Item1 == Match.Inverted && secondFit.Item1 == Match.NonInverted ||
                     firstFit.Item1 == Match.NonInverted && secondFit.Item1 == Match.Inverted)
            {
                return new ClusterData(Match.Inverted, firstFit.Item2, secondFit.Item2);
            }

            return null;
        }

        /// <summary>
        ///   IsFit checks if given a specified side, it can be fit with a shreds's edge
        ///   This Also returns a match object
        /// </summary>
        /// <param name="root"> </param>
        /// <param name="shred"> </param>
        /// <param name="side"> </param>
        /// <returns> </returns>
        private static Tuple<Match, Direction> IsFit(INode root, Shred shred, Side side)
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
                return new Tuple<Match, Direction>(match, Direction.FromLeft);
            }
            if (query.Direction == Direction.FromRight && root.RightEdge() == shred.RightEdge())
            {
                return new Tuple<Match, Direction>(match, Direction.FromRight);
            }
            return new Tuple<Match, Direction>(Match.Impossible, Direction.FromLeft);
        }

        /// <summary>
        ///   This Method ensures a match is possible and if so clusters two nodes
        /// </summary>
        /// <param name="data"> Data Object </param>
        /// <returns> </returns>
        public static Cluster ClusterNodes(Data data)
        {
            INode firstRoot = data.First.Shred.Root();
            INode secondRoot = data.Second.Shred.Root();
            ClusterData result = IsMatch(data, firstRoot, secondRoot);

            if (result == null)
            {
                return null;
            }

            // Mirror the smaller object if need be
            if (result.Match == Match.Inverted)
            {
                if (firstRoot.Size() < secondRoot.Size())
                {
                    firstRoot.Mirror();
                    result.FirstDirection = Enumeration.Opposite(result.FirstDirection);
                }
                else
                {
                    secondRoot.Mirror();
                    result.SecondDirection = Enumeration.Opposite(result.SecondDirection);
                }
            }

            // If the FirstNode's Edge is on the Right, it should go on the LEFT (make sense? )
            if (result.FirstDirection == Direction.FromRight && result.SecondDirection == Direction.FromLeft)
            {
                return new Cluster(firstRoot, secondRoot, result.Match, data);
            }

            return new Cluster(secondRoot, firstRoot, result.Match, data);
        }

        /// <summary>
        ///   This Method clusters two nodes forcibly with no match
        /// </summary>
        /// <param name="data"> Data Object </param>
        /// <param name="match"> Indicates how the Data is Matched </param>
        /// <returns> </returns>
        public static Cluster ForceClusterNodes(Data data, Match match = Match.NonInverted)
        {
            return new Cluster(data.First.Shred, data.Second.Shred, match);
        }

        #region Nested type: ClusterData

        public class ClusterData
        {
            public Direction FirstDirection;
            public Match Match;
            public Direction SecondDirection;

            public ClusterData(Match match, Direction first, Direction second)
            {
                Match = match;
                FirstDirection = first;
                SecondDirection = second;
            }
        }

        #endregion

        public static INode SmartClusterNodes(Data data)
        {
            throw new NotImplementedException();
        }
    }
}