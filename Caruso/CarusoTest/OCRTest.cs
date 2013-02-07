#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Algorithmix.Forensics;
using Algorithmix.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class OcrTest
    {
        private static readonly string CheckFile = "check.txt";

        public Dictionary<string, string> GetChecker(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException("Could not find " + filepath);
            }
            var map = new Dictionary<string, string>();
            var lines = new List<string>(File.ReadAllLines(filepath));
            lines.Where(line => line.Contains(':')).ToList()
                .ForEach(line => map[line.Split(':').First()] = line.Split(':').Last());
            return map;
        }

        [TestMethod]
        public void SimpleReversalTest()
        {
            var relpath = Path.Combine(Dir.OcrDirectory, Dir.OcrSimple);
            var fullpath = Path.Combine(Drive.GetDriveRoot(), relpath);
            var drive = new Drive(relpath, Drive.Reason.Read);
            var regs = drive.Files("snip").ToList();
            var revs = drive.Files("rev").ToList();
            var difference = regs.Zip(revs, (reg, rev) =>
                {
                    var imgReg = new Bitmap(reg);
                    var regData = OCR.Recognize(imgReg);

                    var imgRev = new Bitmap(rev);
                    var revData = OCR.Recognize(imgRev);

                    Console.WriteLine("-----------------------------------------------");
                    Console.WriteLine( StripNewLine(regData.Text + " vs " + revData.Text ));
                    Console.WriteLine( regData.Confidence + " vs "+ revData.Confidence );
                    Console.WriteLine("Diff: " + (regData.Confidence - revData.Confidence) );
                    Console.WriteLine();
                    return regData.Confidence - revData.Confidence;
                }).ToList();
            difference.ForEach(diff => Console.WriteLine("Diff : "+diff));
            Assert.IsTrue( difference.Aggregate((long)0, (total, item) => total+item ) < 0 ) ;
        }


        private static string StripNewLine(string text)
        {
            return text.Replace("\r\n","");
        }

        [TestMethod]
        public void SimpleOcrTest()
        {
            var relpath = Path.Combine(Dir.OcrDirectory, Dir.OcrSimple);
            var fullpath = Path.Combine(Drive.GetDriveRoot(), relpath);
            var checker = GetChecker(Path.Combine(fullpath, CheckFile));
            var drive = new Drive(relpath, Drive.Reason.Read);

            var correct = new List<bool>(drive.FileCount("snip"));
            foreach (var file in drive.Files("snip"))
            {
                var img = new Bitmap(file);
                var filename = Path.GetFileName(file);
                var ocrdata = OCR.Recognize(img);
                Assert.IsTrue(filename != null);
                Assert.IsTrue(checker[filename] != null);
                var isCorrect = ocrdata.Text.ToLower().Contains(checker[filename].ToLower());
                correct.Add(isCorrect);
                Console.WriteLine("-------------------------------------------");
                Console.WriteLine("CORRECT: "+ isCorrect);
                Console.WriteLine("OCR: " + StripNewLine(ocrdata.Text));
                Console.WriteLine("REAL: " + checker[filename].ToLower());
                Console.WriteLine("COST: "+ ocrdata.Confidence);
                Console.WriteLine();
            }

            Assert.IsTrue(correct.All(x => x) == true);
        }
    }
}