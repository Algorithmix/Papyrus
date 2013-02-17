#region

using System;
using System.IO;
using System.Text;
using Algorithmix;
using Newtonsoft.Json;

#endregion

namespace Algorithmix.UnitTest
{
    public class ClusterExporter
    {
        public static void ExportJson(INode root, String path = @"..\..\visualizer\data.js")
        {
            File.WriteAllText(path, ClusterToJson(root));
        }

        public static string ClusterToJson(INode root)
        {
            var sb = new StringBuilder();
            sb.Append("var json = ");
            using (JsonTextWriter writer = new JsonTextWriter(new StringWriter(sb)))
            {
                writer.QuoteName = false;
                writer.Formatting = Formatting.Indented;
                BuildJson(root, writer);
            }
            sb.Append(";");
            return sb.ToString();
        }

        private static void BuildJson(INode node, JsonTextWriter writer)
        {
            if (node.IsLeaf())
            {
                writer.WriteStartObject();
                writer.WritePropertyName("id");
                writer.WriteValue("shred" + node.Leaf().Id);
                writer.WritePropertyName("name");
                writer.WriteValue(node.Leaf().Id);
                WriteLeaf(node.Leaf(),writer);
                writer.WriteEndObject();
                return;
            }
            var data = node.MatchData();
            writer.WriteStartObject();
            
            writer.WritePropertyName("id");
            writer.WriteValue("cluster" + data.First.Shred.Id + "_" + data.Second.Shred.Id);
            
            writer.WritePropertyName("name");
            writer.WriteValue(GetSummary(data));
            
            WriteNode(data, writer);

            writer.WritePropertyName("children");
            writer.WriteStartArray();
            
            BuildJson(node.Left(), writer);
            BuildJson(node.Right(), writer);
            
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        private static void WriteLeaf(Shred shred, JsonTextWriter writer)
        {
            writer.WritePropertyName("data");
            writer.WriteStartObject();
            writer.WritePropertyName("tip");
            writer.WriteValue(GetTip(shred));
            writer.WritePropertyName("filepath");
            writer.WriteValue(new Uri(shred.Filepath).AbsoluteUri);
            writer.WriteEndObject();
        }

        private static void WriteNode(MatchData matchData, JsonTextWriter writer)
        {
            writer.WritePropertyName("data");
            
            writer.WriteStartObject();
            
            writer.WritePropertyName("tip");
            writer.WriteValue(GetTip(matchData));
            
            writer.WritePropertyName("first");
            writer.WriteStartObject();
            WriteSide(matchData.First,writer);
            writer.WriteEndObject();
            
            writer.WritePropertyName("second");
            writer.WriteStartObject();
            WriteSide(matchData.Second, writer);
            writer.WriteEndObject();

            writer.WriteEndObject();
        }

        private static void WriteSide(Side side , JsonTextWriter writer)
        {
            writer.WritePropertyName("direction");
            writer.WriteValue(side.Direction);
            writer.WritePropertyName("orientation");
            writer.WriteValue(side.Orientation);
            writer.WritePropertyName("filepath");
            writer.WriteValue( new Uri(side.Shred.Filepath).AbsoluteUri);
        }

        private static string GetTip(Shred shred)
        {
            return Path.GetFileNameWithoutExtension(shred.Filepath);
        }

        private static string GetTip(MatchData matchData)
        {
            return matchData.First.Orientation + " " + matchData.First.Direction + " vs " +
                   matchData.Second.Orientation +" " + matchData.Second.Direction ;
        }

        private static string GetSummary(MatchData matchData)
        {
            return matchData.First.Shred.Id +" - "+ matchData.Second.Shred.Id + " ("+ (Math.Truncate(matchData.ChamferSimilarity*1000)/1000) +")";
        }
    }
}