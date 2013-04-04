#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Algorithmix.Reconstruction;
using Algorithmix.TestTools;
using Algorithmix.Tools;

#endregion

namespace Algorithmix.Experiment
{
    public class Experiment
    {
        public static readonly string BenchmarkDirectory = "Benchmarks";
        public static bool UseOcr = true;

        public static void RunExperiment(string name , string folder, string prefix, string outputDirectory = "")
        {
            if (outputDirectory == String.Empty)
            {
                outputDirectory = Path.Combine(Drive.GetDriveRoot(), BenchmarkDirectory);
                //Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            var drive = new Drive(folder, Drive.Reason.Read);
            var experiment = new Experiment(drive.Files(prefix).ToList(),UseOcr);
            var mixed = experiment.MixedOrder;
            var normal = experiment.CorrectOrder;
            var results = Reconstructor.NaiveKruskalAlgorithm(mixed);
            var difference = experiment.Diff(results);

            var resultImg = Path.Combine(outputDirectory, name + "_result.png");
            Stitcher.ExportImage((Cluster)results.First().Root(), resultImg);

            var sb = new StringBuilder();

            sb.AppendLine(name);
            sb.AppendLine(folder);
            sb.AppendLine(difference.ToString());
            mixed.ForEach(shred => sb.Append(" " + shred.Id + ", "));
            sb.AppendLine();
            normal.ForEach(shred => sb.Append(" " + shred.Id + ", "));
            sb.AppendLine();
            results.ForEach(shred => sb.Append(" " + shred.Id + ", "));
            sb.AppendLine();

            Console.WriteLine(sb.ToString());
            File.WriteAllText(Path.Combine(outputDirectory,name+".txt"),sb.ToString());
        }

        #region Experiment Object

        public List<Tuple<Shred, int>> OrderedPair { get; private set; }
        public List<Shred> MixedOrder { get; private set; }
        public List<Shred> CorrectOrder { get; private set; }

        public Experiment(List<string> filenames, bool runOcr = true)
        {
            OrderedPair = LoadShredsRandomized(filenames, runOcr);
            MixedOrder = OrderedPair.Select(pair => pair.Item1).ToList();
            CorrectOrder = UnShuffle(OrderedPair);
        }

        public double Diff(List<Shred> shreds)
        {
            return Difference(CorrectOrder, shreds);
        }

        #endregion

        #region Comparison Helpers

        public static double Difference(List<Shred> first, List<Shred> second)
        {
            var firstId = first.Select(shred => shred.Id).ToList();
            var secondId = second.Select(shred => shred.Id).ToList();
            return Differ.DiffShredByOrder(firstId, secondId);
        }

        #endregion

        #region Shuffler and Unshuffler

        public static List<Shred> UnShuffle(List<Tuple<Shred, int>> orderedPairs)
        {
            var shred = orderedPairs.ToDictionary(pair => pair.Item2, pair => pair.Item1);
            var sortedShreds = new List<Shred>(orderedPairs.Count);
            for (int ii = 0; ii < orderedPairs.Count; ii++)
            {
                sortedShreds.Add(shred[ii]);
            }

            return sortedShreds;
        }

        public static List<Tuple<Shred, int>> LoadShredsRandomized(List<string> filenames, bool runOcr = true)
        {
            var randomizedFilenames = Shuffle(filenames);
            var shreds = Shred.Factory(randomizedFilenames.Select(pair => pair.Item1).ToList(),runOcr);
            return randomizedFilenames.Zip(shreds, (pair, shred) => Tuple.Create(shred, pair.Item2)).ToList();
        }

        public static List<Tuple<string, int>> Shuffle(List<string> shreds)
        {
            var orderedPair = new List<Tuple<string, int>>();
            for (int ii = 0; ii < shreds.Count; ii++)
            {
                var pair = Tuple.Create(shreds[ii], ii);
                orderedPair.Add(pair);
            }
            for (int ii = 0; ii < shreds.Count; ii++)
            {
                var swapNumber = random.Next(0, shreds.Count - 1);
                Swap(orderedPair, ii, swapNumber);
            }
            return orderedPair;
        }

        #endregion

        #region Helpers

        public static Random random = new Random((int) DateTime.Now.Ticks);

        public string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26*random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        public static void Swap<T>(List<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        #endregion
    }
}