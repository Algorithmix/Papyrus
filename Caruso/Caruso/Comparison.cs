using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithmix
{
    public class EdgeData
    {
        
        public long Id;
        public Direction Direction;
        public Orientation Orientation;
        
        public EdgeData( long id, Direction direction, Orientation orientation)
        {
            this.Id = id;
            this.Direction = direction;
            this.Orientation = orientation;
        }
    }

    public class Comparison
    {
        public readonly EdgeData First;
        public readonly EdgeData Second;
        public readonly double Similarity;
        public readonly int Offset;

        public Comparison(double chamferSimilarity, int offset, EdgeData first, EdgeData second)
        {
            this.First = first;
            this.Second = second;
            this.Similarity = chamferSimilarity;
            this.Offset = offset;
        }
    }
}
