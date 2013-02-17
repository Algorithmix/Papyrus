#region

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace Algorithmix
{
    [Serializable]
    public partial class Cluster : INode
    {
        private INode _left;
        private INode _right;
        private INode _parent;
        private Edge _rightedge;
        private Edge _leftedge;
        private INode _root;
        private readonly int _size;
        private readonly MatchData _matchData;

        public void ToJson(JsonTextWriter writer)
        {
            if (_matchData == null) return;

            MatchData matchData = _matchData;
            string tip = matchData.First.Orientation + " " + matchData.First.Direction + " vs " +
                         matchData.Second.Orientation + " " + matchData.Second.Direction;
            string name = matchData.First.Shred.Id + " - " + matchData.Second.Shred.Id + " (" +
                          (Math.Truncate(matchData.ChamferSimilarity*1000)/1000) + ")";
            writer.WriteStartObject();
            {
                writer.WritePropertyName("id");
                writer.WriteValue("cluster" + matchData.First.Shred.Id + "_" + matchData.Second.Shred.Id);

                writer.WritePropertyName("name");
                writer.WriteValue(name);

                {
                    writer.WritePropertyName("data");

                    writer.WriteStartObject();
                    {
                        writer.WritePropertyName("tip");
                        writer.WriteValue(tip);

                        writer.WritePropertyName("first");
                        writer.WriteStartObject();
                        {
// ReSharper disable ImpureMethodCallOnReadonlyValueField
                            matchData.First.ToJson(writer);
// ReSharper restore ImpureMethodCallOnReadonlyValueField
                        }
                        writer.WriteEndObject();

                        writer.WritePropertyName("second");
                        writer.WriteStartObject();
                        {
// ReSharper disable ImpureMethodCallOnReadonlyValueField
                            matchData.Second.ToJson(writer);
// ReSharper restore ImpureMethodCallOnReadonlyValueField
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();

                    writer.WritePropertyName("children");
                    writer.WriteStartArray();
                    {
                        Left().ToJson(writer);
                        Right().ToJson(writer);
                    }
                    writer.WriteEndArray();
                }
                writer.WriteEndObject();
            }
        }

        public void OrphanChildren()
        {
            // Return if this not the root
            if (Parent() != this)
            {
                return;
            }

            // Severe ties with children
            Left().Parent(Left());
            Right().Parent(Right());
            _left = null;
            _right = null;
        }

        public MatchData MatchData()
        {
            return _matchData;
        }

        public INode Root()
        {
            INode node = this;
            if (node.Parent() == null)
            {
                return node;
            }
            return node.Parent().Root();
        }


        public void Root(INode root)
        {
            _root = root;
        }

        public Edge RightEdge()
        {
            return _rightedge;
        }

        public Edge LeftEdge()
        {
            return _leftedge;
        }

        public int Size()
        {
            return _size;
        }

        public void Mirror()
        {
            INode swapNode = _left;
            _left = _right;
            _right = swapNode;

            Edge swapEdge = _leftedge;
            _leftedge = _rightedge;
            _rightedge = swapEdge;

            _left.Mirror();
            _right.Mirror();
        }

        public INode Parent()
        {
            return _parent;
        }

        public INode Right()
        {
            return _right;
        }

        public INode Left()
        {
            return _left;
        }

        public bool IsLeaf()
        {
            return false;
        }

        public void Parent(INode parent)
        {
            _parent = parent;
        }

        public Shred Leaf()
        {
            return null;
        }

        public void Flatten(List<Shred> list)
        {
            Left().Flatten(list);
            Right().Flatten(list);
        }

        public Shred RightShred()
        {
            return _rightedge.Shred;
        }

        public Shred LeftShred()
        {
            return _leftedge.Shred;
        }
    }
}