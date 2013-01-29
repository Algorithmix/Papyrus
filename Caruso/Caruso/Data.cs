using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithmix
{
    public struct Side
    {

        public Shred Shred;
        public Direction Direction;
        public Orientation Orientation;
        
        public Side( Shred shred, Direction direction, Orientation orientation)
        {
            this.Shred = shred;
            this.Direction = direction;
            this.Orientation = orientation;
        }
    }

    public struct Data
    { 

        public readonly Side First;
        public readonly Side Second;
        public readonly double ChamferSimilarity;
        public readonly int Offset;
        public readonly double[] ChamferScan;

        public Data(double chamferSimilarity, int offset, double[] scan , Side first, Side second)
        {
            this.First = first;
            this.Second = second;
            this.ChamferSimilarity = chamferSimilarity;
            this.ChamferScan = scan;
            this.Offset = offset;
        }

        public Shred Compliment(Shred shred)
        {
            if (shred.Id == First.Shred.Id)
            {
                return Second;
            }
            return First;
        }
    }
}
