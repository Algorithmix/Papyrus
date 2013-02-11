﻿#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Algorithmix.TestTools;
using JigsawTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class NaiveKruskalTests
    {
        public static readonly string NaiveThree = "Three";
        public static readonly string NaiveSix = "Six";
        public static readonly string NaiveTen = "Ten";

        [TestMethod]
        public void NaiveKruskalPrimitive()
        {
            Shred.BUFFER = 0;
            Shred.SAMPLE_SIZE = 1;

            var folder = Path.Combine(Drive.GetDriveRoot(), Dir.NaiveKruskalTestDirectory);
            var paths = new List<string>();
            paths.Add(Path.Combine(folder, NaiveThree));
            paths.Add(Path.Combine(folder, NaiveSix ));
            paths.Add(Path.Combine(folder, NaiveTen ));

            foreach (var path in paths)
            {
                var shreds = Shred.Factory("Shred", path, false);
                var checker = Helpers.BuildChecker(Path.Combine( path , Helpers.CheckFile) );
                
                var results = Reconstructor.NaiveKruskalAlgorithm(shreds);

                var indicies = results.Select((t, pos) => new Tuple<int, Shred>(pos, t)).ToList();

                Console.WriteLine(" ---- "+ path +"----");
                var result = indicies.Select(pair =>
                    {
                        var filename = Path.GetFileName(pair.Item2.Filepath);
                        Assert.IsNotNull(filename);
                        Assert.IsNotNull(checker[filename]);
                        var expected = checker[filename];
                        var actual = pair.Item1;
                        Console.WriteLine("Actual " + actual + " vs. "+ expected + " | File: " + filename );
                        return actual.ToString(CultureInfo.InvariantCulture) == expected;
                    }).ToList();

                Console.WriteLine();
                result.ForEach( Assert.IsTrue );
            }
        }

        [TestMethod]
        public void NaiveKruskalArtificial()
        {
            var path = Path.Combine(Drive.GetDriveRoot(), Dir.ArtificialTestDirectory,Dir.ArtificialHttpDocument);
            var shreds = Shred.Factory("image", path, false);
            var results = Reconstructor.NaiveKruskalAlgorithm(shreds);


            
            shreds.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            results.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            ClusterExporter.ExportJson(shreds.First().Root());
        }

        [TestMethod]
        public void NaiveKruskalAuthentic()
        {
            var path = Path.Combine(Drive.GetDriveRoot(), Dir.PdfRequiremnetTestDirectory,
                                    Dir.PdfRequiremnetTestFullOne);
            var shreds = Shred.Factory("image", path, false);
            var results = Reconstructor.NaiveKruskalAlgorithm(shreds);
            shreds.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            results.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            Helpers.PrintTree(results.First().Root());
        }
    }
}