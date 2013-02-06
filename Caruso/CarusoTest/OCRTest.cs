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
                Console.WriteLine("OCR: " + ocrdata.Text.Replace("\r\n",""));
                Console.WriteLine("REAL: " + checker[filename].ToLower());
                Console.WriteLine("COST: "+ ocrdata.Confidence);
                Console.WriteLine();
            }

            Assert.IsTrue(correct.All(x => x) == true);
        }
    }
}