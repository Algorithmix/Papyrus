#region

using System;
using System.IO;
using System.Text;
using Algorithmix;
using Newtonsoft.Json;

#endregion

namespace Algorithmix.Tools
{
    public class Exporter
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
                root.ToJson(writer);
            }
            sb.Append(";");
            return sb.ToString();
        }
    }
}