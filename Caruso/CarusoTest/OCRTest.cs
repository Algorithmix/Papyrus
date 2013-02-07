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
        private const string CheckFile = "check.txt";

        [TestMethod]
        public void OcrParallelizationTest()
        {
            // Init Drive
            var relpath = Path.Combine(Dir.OcrDirectory, Dir.OcrParallelizationTesting);
            var drive = new Drive(relpath, Drive.Reason.Read);
            var regs = drive.Files("reg").ToList();
            var revs = drive.Files("rev").ToList();

            // Sort to ensure correct pairing
            regs.Sort();
            revs.Sort();

            // Concatenate and Recognize
            var images = regs.Concat(revs).Select(path => new Bitmap(path)).ToList();
            int revStart = regs.Count;
            var datas = new List<OcrData>(OCR.ParallelRecognize(images, images.Count(), Accuracy.Low));
            var regdata = datas.Take(revStart);
            var revdata = datas.Skip(revStart);

            var difference = regdata.Zip(revdata, (reg, rev) =>
                {
                    Console.WriteLine("-----------------------------------------------");
                    Console.WriteLine(StripNewLine(reg.Text + " vs " + rev.Text));
                    Console.WriteLine(reg.Cost + " vs " + rev.Cost);
                    Console.WriteLine("Diff: " + (reg.Cost - rev.Cost));
                    Console.WriteLine();
                    return reg.Cost - rev.Cost;
                }).ToList();

            difference.ForEach(diff => Assert.IsTrue(diff < 0));
        }

        [TestMethod]
        public void OcrOrientationTest()
        {
            // Init Drive
            var relpath = Path.Combine(Dir.OcrDirectory, Dir.OcrOrientationTesting);
            var drive = new Drive(relpath, Drive.Reason.Read);
            var regs = drive.Files("img").ToList();
            var revs = drive.Files("rev").ToList();

            // Sort them so that the names align correctly
            regs.Sort();
            revs.Sort();

            // Zip and Diff
            var difference = regs.Zip(revs, (reg, rev) =>
                {
                    var imgReg = new Bitmap(reg);
                    var regData = OCR.Recognize(imgReg, Accuracy.High);

                    var imgRev = new Bitmap(rev);
                    var revData = OCR.Recognize(imgRev, Accuracy.High);

                    Console.WriteLine("-----------------------------------------------");
                    Console.WriteLine(StripNewLine(regData.Text + " vs " + revData.Text));
                    Console.WriteLine(regData.Cost + " vs " + revData.Cost);
                    Console.WriteLine("Diff: " + (regData.Cost - revData.Cost));
                    Console.WriteLine();
                    return regData.Cost - revData.Cost;
                }).ToList();
            difference.ForEach(diff => Console.WriteLine("Diff : " + diff));
            difference.ForEach(diff => Assert.IsTrue(diff < 0));
            Assert.IsTrue(difference.Aggregate((long) 0, (total, item) => total + item) < 0);
        }

        [TestMethod]
        public void OcrSimpleReversalTest()
        {
            // Init Drive
            var relpath = Path.Combine(Dir.OcrDirectory, Dir.OcrSimple);
            var drive = new Drive(relpath, Drive.Reason.Read);
            var regs = drive.Files("snip").ToList();
            var revs = drive.Files("rev").ToList();

            // Sort so that the correct files will pair up when zipped
            regs.Sort();
            revs.Sort();

            // Zip and diff
            var difference = regs.Zip(revs, (reg, rev) =>
                {
                    var imgReg = new Bitmap(reg);
                    var regData = OCR.Recognize(imgReg);

                    var imgRev = new Bitmap(rev);
                    var revData = OCR.Recognize(imgRev);

                    Console.WriteLine("-----------------------------------------------");
                    Console.WriteLine(StripNewLine(regData.Text + " vs " + revData.Text));
                    Console.WriteLine(regData.Cost + " vs " + revData.Cost);
                    Console.WriteLine("Diff: " + (regData.Cost - revData.Cost));
                    Console.WriteLine();
                    return regData.Cost - revData.Cost;
                }).ToList();
            difference.ForEach(diff => Console.WriteLine("Diff : " + diff));
            Assert.IsTrue(difference.Aggregate((long) 0, (total, item) => total + item) < 0);
        }

        [TestMethod]
        public void OcrSimpleOcrTest()
        {
            // Init Drive and a Checker (Hashmap)
            var relpath = Path.Combine(Dir.OcrDirectory, Dir.OcrSimple);
            var fullpath = Path.Combine(Drive.GetDriveRoot(), relpath);
            var checker = GetChecker(Path.Combine(fullpath, CheckFile));
            var drive = new Drive(relpath, Drive.Reason.Read);

            // Init Correct List
            var correct = new List<bool>(drive.FileCount("snip"));

            // Scan each snippet, check if they are substrings, save result in correct
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
                Console.WriteLine("CORRECT: " + isCorrect);
                Console.WriteLine("OCR: " + StripNewLine(ocrdata.Text));
                Console.WriteLine("REAL: " + checker[filename].ToLower());
                Console.WriteLine("COST: " + ocrdata.Cost);
                Console.WriteLine();
            }

            // Ensure that all the OCR scans were correct
            Assert.IsTrue(correct.All(x => x));
        }

        private static string StripNewLine(string text)
        {
            return text.Replace("\r\n", "");
        }

        private Dictionary<string, string> GetChecker(string filepath)
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
    }
}