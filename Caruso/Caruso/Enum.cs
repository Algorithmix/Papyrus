namespace Algorithmix
{
    public class Enumeration
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

        public static Match Combination(Match firstFit, Match secondFit)
        {
            if ((firstFit == Match.Inverted && secondFit == Match.NonInverted) || (firstFit == Match.NonInverted && secondFit == Match.Inverted))
            {
                return Match.Inverted;
            }
            if ( (firstFit==Match.Inverted && secondFit == Match.Inverted) || ( firstFit==Match.NonInverted&& secondFit==Match.NonInverted))
            {
                return Match.NonInverted;
            }
            return Match.Impossible;
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

    public enum Match
    {
        Impossible,
        Inverted,
        NonInverted
    }
}
