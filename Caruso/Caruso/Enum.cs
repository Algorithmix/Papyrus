namespace Algorithmix
{
    class Enumeration
    { 
        public static Orientation Opposite(Orientation orientation)
        {
            Orientation opposite = Orientation.Regular;
            switch (orientation)
            {
                case Orientation.Regular:
                    opposite = Orientation.Reversed;
                    break;
                case Orientation.Reversed:
                    opposite = Orientation.Regular;
                    break;
            }
            return opposite;
        }

        public static Direction Opposite(Direction direction)
        {
            // Default case
            Direction opposite = Direction.FromLeft;
            switch (direction)
            {
                case Direction.FromLeft:
                    opposite = Direction.FromRight;
                    break;
                case Direction.FromRight:
                    opposite = Direction.FromLeft;
                    break;
                case Direction.FromTop:
                    opposite = Direction.FromBottom;
                    break;
                case Direction.FromBottom:
                    opposite = Direction.FromTop;
                    break;
            }
            return opposite;
        }
    }

    /// <summary>
    ///   Scan Direction Enum
    /// </summary>
    public enum Direction
    {
        FromLeft,
        FromRight,
        FromTop,
        FromBottom
    }

    public enum Orientation
    {
        Regular,
        Reversed
    }
}
