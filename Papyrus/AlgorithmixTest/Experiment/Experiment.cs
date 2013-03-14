#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Algorithmix.Reconstruction;
using Algorithmix.TestTools;

#endregion

namespace Algorithmix.Experiment
{
    public class Experiment
    {

        public static void RunExperiment(string folder, string prefix, string outputDirectory = "")
        {
            if (outputDirectory == String.Empty)
            {
                outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            var drive = new Drive(folder, Drive.Reason.Read);
            var experiment = new Experiment(drive.Files(prefix).ToList());
            var mixed = experiment.MixedOrder;
            var normal = experiment.CorrectOrder;
            var results = Reconstructor.NaiveKruskalAlgorithm(mixed);
            var difference = experiment.Diff(results);
            
            Console.WriteLine( folder + difference);
            mixed.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            normal.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            results.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
        }

        #region Experiment Object

        public List<Tuple<Shred, int>> OrderedPair { get; private set; }
        public List<Shred> MixedOrder { get; private set; }
        public List<Shred> CorrectOrder { get; private set; }

        public Experiment(List<string> filenames)
        {
            OrderedPair = LoadShredsRandomized(filenames);
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
            var firstId = first.Select(shred => shred.Filepath).ToList();
            var secondId = second.Select(shred => shred.Filepath).ToList();
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

        public static List<Tuple<Shred, int>> LoadShredsRandomized(List<string> filenames)
        {
            var randomizedFilenames = Shuffle(filenames);
            var shreds = Shred.Factory(randomizedFilenames.Select(pair => pair.Item1).ToList());
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