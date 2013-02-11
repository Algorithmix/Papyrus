#region

using System;
using System.IO;
using System.Text;
using Algorithmix;
using Newtonsoft.Json;

#endregion

namespace JigsawTest
{
    public class ClusterExporter
    {
        public static void ExportJson(INode root, String path = @"visualizer\data.js")
        {
            File.WriteAllText(path,ClusterToJson(root));
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
                writer.WriteValue("shred"+node.Leaf().Id);
                writer.WritePropertyName("name");
                writer.WriteValue(node.Leaf().Id) ;
                writer.WritePropertyName("data");
                writer.WriteStartObject();
                writer.WriteEndObject();
                writer.WriteEndObject();
                return;
            }
            var data = node.MatchData();
            writer.WriteStartObject();
            writer.WritePropertyName("id");
            writer.WriteValue("cluster"+data.First.Shred.Id+"_"+data.Second.Shred.Id);
            writer.WritePropertyName("name");
            writer.WriteValue( Summary(data));
            writer.WritePropertyName("data");
            writer.WriteStartObject();
            writer.WritePropertyName("tip");
            writer.WriteValue(Tip(data));
            writer.WriteEndObject();
            writer.WritePropertyName("children");
            writer.WriteStartArray();
            BuildJson(node.Left(),writer);
            BuildJson(node.Right(),writer);
            writer.WriteEndArray();
            writer.WriteEndObject(); 
        }

        private static string Tip(Data data)
        {
            return data.First.Orientation + " " + data.First.Direction + " vs " +
                   data.Second.Orientation +" " + data.Second.Direction ;
        }

        private static string Summary(Data data)
        {
            return data.First.Shred.Id +" - "+ data.Second.Shred.Id + " ("+ (Math.Truncate(data.ChamferSimilarity*100)/100) +")";
        }
    }
}