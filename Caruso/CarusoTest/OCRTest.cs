#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            var images = drive.Files("reg").Concat(drive.Files("rev"));

            var cMap = new ConcurrentDictionary<string, OcrData>();
            Parallel.ForEach(images, img =>
                {
                    var regData = OCR.Recognize( new Bitmap(img) , Accuracy.Low );
                    cMap[img] = regData;
                });
            var results = cMap.Aggregate(new Tuple<long,long> (0,0), (total, kvpair) =>
                {
                    var filename = Path.GetFileName(kvpair.Key);
                    var data = kvpair.Value;
                    Console.WriteLine("-------------- "+ filename +" ----------");
                    Console.WriteLine(data.Text);
                    Console.WriteLine(data.Confidence);
                    Console.WriteLine();

                    if ( filename!=null && filename.StartsWith("rev"))
                    {
                        return new Tuple<long, long>(total.Item1,total.Item2+data.Confidence);
                    }
                    return new Tuple<long, long>(total.Item1 + data.Confidence, total.Item2);
                });

            Console.WriteLine("-------------------------------------");
            Console.WriteLine("-------------------------------------");        
            Console.WriteLine( "Regular: " + results.Item1 + "\t Reversed: "+ results.Item2);
            Console.WriteLine("Diff: "+ (results.Item1- results.Item2));
            Assert.IsTrue(results.Item1 < results.Item2 );            
        }

        [TestMethod]
        public void OcrOrientationTest()
        {
            // Init Drive
            var relpath = Path.Combine(Dir.OcrDirectory, Dir.OcrParallelizationTesting);
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
                var regData = OCR.Recognize(imgReg,Accuracy.High);

                var imgRev = new Bitmap(rev);
                var revData = OCR.Recognize(imgRev,Accuracy.High);

                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine(StripNewLine(regData.Text + " vs " + revData.Text));
                Console.WriteLine(regData.Confidence + " vs " + revData.Confidence);
                Console.WriteLine("Diff: " + (regData.Confidence - revData.Confidence));
                Console.WriteLine();
                return regData.Confidence - revData.Confidence;
            }).ToList();
            difference.ForEach(diff => Console.WriteLine("Diff : " + diff));
            Assert.IsTrue(difference.Aggregate((long)0, (total, item) => total + item) < 0);
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
                    Console.WriteLine(regData.Confidence + " vs " + revData.Confidence);
                    Console.WriteLine("Diff: " + (regData.Confidence - revData.Confidence));
                    Console.WriteLine();
                    return regData.Confidence - revData.Confidence;
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
                Console.WriteLine("COST: " + ocrdata.Confidence);
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