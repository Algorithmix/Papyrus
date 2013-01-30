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

        public static Match Query(Data data)
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

        private static Cluster Merge(Data data)
        {
            // Check if they are matchable
            Match match = Data.Query(data);
            if (match == Match.Impossible)
            {
                return null;
            }
            return new Cluster(data.First.Shred, data.Second.Shred, match);
        }
    }
}