using System;

namespace Algorithmix
{
    [Serializable]
    public class Edge
    {
        public static Edge New(Shred shred ,Direction direction)
        {
            return new Edge(shred,direction);
        }

        private readonly Shred _shred;
        private readonly Direction _direction;
        private readonly Orientation _orientation;

        public Shred Shred
        {
            get { return this._shred; }
        }

        public Direction Direction
        {
            get
            {
                if (this._shred.Orientation == this._orientation)
                {
                    return this._direction;
                }
                return Enumeration.Opposite(this._direction);
            }
        }

        public Orientation Orientation
        {
            get { return this._orientation; }
        }

        public Edge(Shred shred, Direction direction)
        {
            this._shred = shred;
            this._direction = direction;
            this._orientation = shred.Orientation;
        }
    }
}
