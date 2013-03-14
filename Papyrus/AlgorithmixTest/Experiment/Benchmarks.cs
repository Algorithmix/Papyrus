using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Algorithmix.UnitTest;

namespace Algorithmix.Experiment
{
    [TestClass]
    public class Benchmarks
    {
        [TestMethod]
        public void BenchmarkPrimitive()
        {
            Shred.BUFFER = 0;
            Shred.SAMPLE_SIZE = 1;

            var sixtest = Path.Combine(Dir.PrimitiveTestDirectory, Dir.PrimitiveTestSixNormal);
            var tentest = Path.Combine(Dir.PrimitiveTestDirectory, Dir.PrimitiveTestTenNormal);
            Experiment.RunExperiment("Primative Six Test",sixtest,"Shred");
            Experiment.RunExperiment("Primative Ten Test",tentest,"Shred");
        }

        [TestMethod]
        public void BenchmarkAuthentic()
        {
            //Shred.BUFFER = 2;
            //Shred.SAMPLE_SIZE = 4;

            var prefix = "image";
            const string pdf1 = "pdf1";
            const string pdf2 = "pdf2";
            var pdfpath1 = Path.Combine(Dir.DocumentDirectory, Dir.PdfRequiremnetTestDirectory, Dir.Full1);
            var pdfpath2 = Path.Combine(Dir.DocumentDirectory, Dir.PdfRequiremnetTestDirectory, Dir.Full2);

            Experiment.RunExperiment(pdf1, pdfpath1, prefix);
            Experiment.RunExperiment(pdf2, pdfpath2, prefix);

            const string management1 = "Management1";
            const string management2 = "Management2";
            var managementpath1 = Path.Combine(Dir.DocumentDirectory ,Dir.ManagementDiscussionFolder, Dir.Full1);
            var managementpath2 = Path.Combine(Dir.DocumentDirectory, Dir.ManagementDiscussionFolder, Dir.Full2);

            Experiment.RunExperiment(management1,managementpath1,prefix);
            Experiment.RunExperiment(management2, managementpath2, prefix);

            const string w2form1 = "w2form1";
            const string w2form2 = "w2form2";
            var w2formpath1 = Path.Combine(Dir.DocumentDirectory, Dir.W2FormFolder, Dir.Full1);
            var w2formpath2 = Path.Combine(Dir.DocumentDirectory, Dir.W2FormFolder, Dir.Full2);

            Experiment.RunExperiment(w2form1, w2formpath1, prefix);
            Experiment.RunExperiment(w2form2, w2formpath2, prefix);

            const string challenger1 = "challenger1";
            const string challenger2 = "challenger2";
            var challengerpath1 = Path.Combine(Dir.DocumentDirectory, Dir.ChallengerFolder, Dir.Full1);
            var challengerpath2 = Path.Combine(Dir.DocumentDirectory, Dir.ChallengerFolder, Dir.Full2);

            Experiment.RunExperiment(challenger1, challengerpath1, prefix);
            Experiment.RunExperiment(challenger2, challengerpath2, prefix);
        }
    }
}
