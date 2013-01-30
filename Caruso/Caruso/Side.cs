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
}