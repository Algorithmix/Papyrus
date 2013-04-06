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
        public void BenchmarkBla()
        {
            var prefix = "image";
            //const string pdf1 = @"C:\Users\Algorithmix\Desktop\30";
            const string pdf2 = @"C:\Users\Algorithmix\Desktop\41";
            const string pdf7 = @"C:\Users\Algorithmix\Dropbox\Algorthmics-TestDrive\Documents\W2Form_ECE\Full1";
            const string pdf8 = @"C:\Users\Algorithmix\Desktop\61";
            //const string pdf3 = @"C:\Users\Algorithmix\Desktop\50";
            //const string pdf4 = @"C:\Users\Algorithmix\Desktop\60";
            //const string pdf5 = @"C:\Users\Algorithmix\Desktop\75";
            //const string pdf6 = @"C:\Users\Algorithmix\Desktop\85";


            //Experiment.RunExperiment("30", pdf1, prefix);
            Experiment.RunExperiment("41", pdf2, prefix);
            //Experiment.RunExperiment("50", pdf3, prefix);
            //Experiment.RunExperiment("60", pdf4, prefix);
            //Experiment.RunExperiment("75", pdf5, prefix);
            //Experiment.RunExperiment("85", pdf6, prefix);
            Experiment.RunExperiment("FULL1", pdf7, prefix);
            Experiment.RunExperiment("61", pdf8, prefix);

        }
        

        [TestMethod]
        public void BenchmarkAuthentic()
        {
            //Shred.BUFFER = 3;
            //Shred.SAMPLE_SIZE = 4;
            MatchData.ORIENTATION_PENALTY = true;
            MatchData.NORMALIZATION_ENABLED = true;

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

            const string w2Form1 = "w2form1";
            const string w2Form2 = "w2form2";
            var w2Formpath1 = Path.Combine(Dir.DocumentDirectory, Dir.W2FormFolder, Dir.Full1);
            var w2Formpath2 = Path.Combine(Dir.DocumentDirectory, Dir.W2FormFolder, Dir.Full2);

            Experiment.RunExperiment(w2Form1, w2Formpath1, prefix);
            Experiment.RunExperiment(w2Form2, w2Formpath2, prefix);

            const string challenger1 = "challenger1";
            const string challenger2 = "challenger2";
            var challengerpath1 = Path.Combine(Dir.DocumentDirectory, Dir.ChallengerFolder, Dir.Full1);
            var challengerpath2 = Path.Combine(Dir.DocumentDirectory, Dir.ChallengerFolder, Dir.Full2);

            Experiment.RunExperiment(challenger1, challengerpath1, prefix);
            Experiment.RunExperiment(challenger2, challengerpath2, prefix);

            const string w2Complete = "w2complete";
            var w2Pathcomplete = Path.Combine(Dir.DocumentDirectory, Dir.W2FormFolder, Dir.Complete);
            
            Experiment.RunExperiment(w2Complete, w2Pathcomplete, prefix);

         
        }
    }
}
