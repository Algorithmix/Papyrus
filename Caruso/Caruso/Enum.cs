namespace Algorithmix
{
    public class Enumeration
    { 
        /// <summary>
        /// Deteremines the Opposite Orientation,
        /// that is if Regular, the opposite is reversed and vice versa
        /// </summary>
        /// <param name="orientation">Orientation to find opposite</param>
        /// <returns>Opposite Orientation</returns>
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

        /// <summary>
        /// Determine the Opposite direction
        /// Left/Right are opposites as are Up and Down
        /// </summary>
        /// <param name="direction">Direction to be toggled</param>
        /// <returns>Opposite Direction</returns>
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

        /// <summary>
        /// Combines two Matches to give you an Aggregate
        /// If Both are Inverted or Noninverted it will return NonInverted;
        /// If Both are different it will return Inverted;
        /// If Either are Impossible it will return Impossible;
        /// </summary>
        /// <param name="firstFit">The first Match Type</param>
        /// <param name="secondFit">The Second Match Type</param>
        /// <returns>The Aggregate match</returns>
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
