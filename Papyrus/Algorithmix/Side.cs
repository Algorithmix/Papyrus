using System;
using Newtonsoft.Json;

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

        public void ToJson(JsonTextWriter writer)
        {
            writer.WritePropertyName("direction");
            writer.WriteValue(this.Direction);
            writer.WritePropertyName("orientation");
            writer.WriteValue(this.Orientation);
            writer.WritePropertyName("filepath");
            writer.WriteValue( new Uri(this.Shred.Filepath).AbsoluteUri);
        }
    }
}