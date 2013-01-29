namespace Algorithmix
{
    public struct Side
    {
        public Direction Direction;
        public Orientation Orientation;
        public Shred Shred;

        public Side(Shred shred, Direction direction, Orientation orientation)
        {
            Shred = shred;
            Direction = direction;
            Orientation = orientation;
        }
    }

    public struct Data
    {
        public readonly double[] ChamferScan;
        public readonly double ChamferSimilarity;
        public readonly Side First;
        public readonly int Offset;
        public readonly Side Second;

        public Data(double chamferSimilarity, int offset, double[] scan, Side first, Side second)
        {
            First = first;
            Second = second;
            ChamferSimilarity = chamferSimilarity;
            ChamferScan = scan;
            Offset = offset;
        }
    }
}