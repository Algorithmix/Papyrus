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
        [TestMethod]
        public void OcrEmptyDetectionTest()
        {
            //Load Shreds
            var abspath = Path.Combine(Drive.GetDriveRoot(), Dir.OcrDirectory, Dir.OcrEmptyTestDirectory);
            var shreds = Shred.Factory("empty", abspath, true);
            var checker = Helpers.BuildChecker( Path.Combine(abspath,Helpers.CheckFile));

            var results = shreds.Select(shred =>
                {
                    var filename = Path.GetFileName(shred.Filepath);
                    Assert.IsTrue(filename != null);
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine("Shred Name" + Path.GetFileNameWithoutExtension(shred.Filepath));
                    Console.WriteLine("IsEmpty? = " + shred.IsEmpty);
                    Console.WriteLine("Is Really Empty? = " + checker[filename]);
                    Console.WriteLine("Correct: " + (checker[filename] == (shred.IsEmpty ? "y" : "n")));
                    Console.WriteLine();
                    return (checker[filename] == (shred.IsEmpty ? "y" : "n"));
                });

            results.ToList().ForEach( Assert.IsTrue);


        }

        [TestMethod]
        public void OcrParallelizationTest()
        {
            // Init Drive
            var relpath = Path.Combine(Dir.OcrDirectory, Dir.OcrParallelizationTesting);
            var drive = new Drive(relpath, Drive.Reason.Read);
            var currents = drive.Files("reg").Concat(drive.Files("rev")).Select(path => new Bitmap(path)).ToArray();
            var opposites = currents.Select(Filter.Reverse).ToArray();
            var regularCount = drive.FileCount("reg");

            var results = OCR.ParallelDetectOrientation(currents, opposites, Accuracy.Low, "eng", true).ToList();
            results.ToList().ForEach(result =>
                {
                    var current = result.Item2;
                    var opposite = result.Item3;
                    Console.WriteLine("-----------------------------------------------");
                    Console.WriteLine(OCR.StripNewLine(current.Text + " vs " + opposite.Text));
                    Console.WriteLine(current.Cost + " vs " + opposite.Cost);
                    Console.WriteLine("Diff: " + (current.Cost - opposite.Cost));
                    Console.WriteLine("scantime: " + current.ScanTime + "ms and " + opposite.ScanTime + "ms");
                    Console.WriteLine();
                });
            results.Take(regularCount).ToList().ForEach(pair => Assert.IsTrue(pair.Item1 >= 0));
            results.Skip(regularCount).ToList().ForEach(pair => Assert.IsTrue(pair.Item1 <= 0));
        }

        [TestMethod]
        public void OcrParallelizationExperiment()
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
            var datas = new List<OcrData>(OCR.ParallelRecognize(images, images.Count(), Accuracy.Low, "eng", true));
            var regdata = datas.Take(revStart);
            var revdata = datas.Skip(revStart);

            var difference = regdata.Zip(revdata, (reg, rev) =>
                {
                    Console.WriteLine("-----------------------------------------------");
                    Console.WriteLine(OCR.StripNewLine(reg.Text + " vs " + rev.Text));
                    Console.WriteLine(reg.Cost + " vs " + rev.Cost);
                    Console.WriteLine("Diff: " + (reg.Cost - rev.Cost));
                    Console.WriteLine("scantime: " + reg.ScanTime + "ms and " + rev.ScanTime + "ms");
                    Console.WriteLine();
                    return reg.Cost - rev.Cost;
                }).ToList();

            difference.ForEach(diff => Assert.IsTrue(diff < 0));
        }

        [TestMethod]
        public void OcrFullOrientationTest()
        {
            const string docRev = @"Documents\EnergyInfrastructure\Full2";
            const string docReg = @"Documents\IncomeTax_Ingalls\Full1";
            var driveRev = new Drive(docRev, Drive.Reason.Read);
            var driveReg = new Drive(docReg, Drive.Reason.Read);

            var revs = Shred.Factory(driveRev.Files("image").ToList(), true);
            var regs = Shred.Factory(driveReg.Files("image").ToList(), true);

            var resV = revs.Select(shred =>
                {
                    Console.Write(" | Expected:" + Orientation.Reversed);
                    Console.Write(" | Actual: " + (shred.TrueOrienation.ToString()));
                    Console.Write(" | Orientation Confidence: " + shred.OrientationConfidence);
                    if (shred.TrueOrienation == null)
                    {
                        Console.WriteLine(" | Empty? =" + shred.IsEmpty);
                        Console.WriteLine("Correct? Indeterminable");
                        return true;
                    }

                    var passed = (Orientation.Reversed == shred.TrueOrienation || shred.IsEmpty);
                    Console.WriteLine(" | Empty? =" + shred.IsEmpty);
                    Console.WriteLine("Correct? = " + passed);
                    return passed;
                }).ToList();
            var resG = regs.Select(shred =>
                {
                    Console.Write(" | Expected:" + Orientation.Regular);
                    Console.Write(" | Actual: " + (shred.TrueOrienation.ToString()));
                    Console.Write(" | Orientation Confidence: " + shred.OrientationConfidence);
                    if (shred.TrueOrienation == null)
                    {
                        Console.WriteLine(" | Empty? =" + shred.IsEmpty);
                        Console.WriteLine("Correct? Indeterminable");
                        return true;
                    }

                    var passed = Orientation.Regular == shred.TrueOrienation || shred.IsEmpty;
                    Console.WriteLine(" | Empty? =" + shred.IsEmpty);
                    Console.WriteLine("Correct? = " + passed);
                    return passed;
                }).ToList();
            Assert.IsTrue(resV.All(item => item));
            Assert.IsTrue(resG.All(item => item));
            }

        [TestMethod]
        public void OcrQuickOrientationTest()
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
                    Console.WriteLine(OCR.StripNewLine(regData.Text + " vs " + revData.Text));
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
                    Console.WriteLine(OCR.StripNewLine(regData.Text + " vs " + revData.Text));
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
            var checker = Helpers.BuildChecker(Path.Combine(fullpath, Helpers.CheckFile));
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
                Console.WriteLine("OCR: " + OCR.StripNewLine(ocrdata.Text));
                Console.WriteLine("REAL: " + checker[filename].ToLower());
                Console.WriteLine("COST: " + ocrdata.Cost);
                Console.WriteLine();
            }

            // Ensure that all the OCR scans were correct
            Assert.IsTrue(correct.All(x => x));
        }
    }
}