using System;
using System.IO;
using System.Linq;
using Algorithmix.Reconstruction;
using Algorithmix.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Algorithmix.UnitTest;

namespace Algorithmix.Experiment
{
    [TestClass]
    public class PrimitiveExperiment
    {
        [TestMethod]
        public void PrimitiveBenchmark()
        {
            Shred.BUFFER = 0;
            Shred.SAMPLE_SIZE = 1;

            var drive = new Drive(Path.Combine(Dir.PrimitiveTestDirectory, Dir.PrimitiveTestSixNormal),
                                  Drive.Reason.Read);
            
            var experiment = new Experiment(drive.Files("Shred").ToList());
            
            var mixed = experiment.MixedOrder;
            var normal = experiment.CorrectOrder;
            var results = Reconstructor.NaiveKruskalAlgorithm(mixed);

            var difference = experiment.Diff(results);
            Stitcher.ExportImage((Cluster)results.First().Root(),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"result.png"));
            Console.WriteLine("Primitive Test Difference"+difference);
            mixed.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            normal.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
            results.ForEach(shred => Console.Write(" " + shred.Id + ", "));
            Console.WriteLine();
        }
    }
}
