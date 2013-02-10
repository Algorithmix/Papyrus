#region

using System;

#endregion

namespace Algorithmix
{
    [Serializable]
    public class Edge
    {
        public static Edge New(Shred shred, Direction direction)
        {
            return new Edge(shred, direction);
        }

        //public Match Fits(Side side)
        //{
        //    if (side.Shred.Id != this.Shred.Id)
        //    {
        //        return Match.Impossible;
        //    }
        //    if (this.Direction == side.Direction && this.Orientation == side.Orientation )
        //    {
        //        return Match.NonInverted;
        //    }
        //    else if (this.Direction == Enumeration.Opposite(side.Direction) && this.Orientation == Enumeration.Opposite(side.Orientation))
        //    {
        //        return Match.Inverted;
        //    }
        //    return Match.Impossible;
        //}

        private readonly Shred _shred;
        private readonly Direction _direction;
        private readonly Orientation _orientation;

        public Shred Shred
        {
            get { return _shred; }
        }

        public Direction Direction
        {
            get
            {
                if (_shred.Orientation == _orientation)
                {
                    return _direction;
                }
                return Enumeration.Opposite(_direction);
            }
        }

        public Orientation Orientation
        {
            get { return _orientation; }
        }

        public Edge(Shred shred, Direction direction)
        {
            _shred = shred;
            _direction = direction;
            _orientation = shred.Orientation;
        }
    }
}