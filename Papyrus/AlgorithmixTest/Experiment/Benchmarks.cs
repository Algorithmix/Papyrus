using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Algorithmix.UnitTest;

namespace Algorithmix.Experiment
{
    [TestClass]
    public class Benchmarks
    {
        [TestMethod]
        public void PrimitiveBenchmark()
        {
            Shred.BUFFER = 0;
            Shred.SAMPLE_SIZE = 1;

            var sixtest = Path.Combine(Dir.PrimitiveTestDirectory, Dir.PrimitiveTestSixNormal);
            var tentest = Path.Combine(Dir.PrimitiveTestDirectory, Dir.PrimitiveTestTenNormal);
            Experiment.RunExperiment(sixtest,"Shred");
            Experiment.RunExperiment(tentest,"Shred");
        }
    }
}
