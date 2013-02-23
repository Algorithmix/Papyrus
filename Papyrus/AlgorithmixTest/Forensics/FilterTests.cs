#region

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Algorithmix.Forensics;
using Algorithmix.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    /// <summary>
    ///   Summary description for FilterTests
    /// </summary>
    [TestClass]
    public class FilterTests
    {
        [TestMethod]
        public void FilterEmptyDetectionTest()
        {
            //Load Shreds
            var path = Path.Combine(Dir.OcrDirectory, Dir.OcrEmptyTestDirectory);
            var shreds = (new Drive(path, Drive.Reason.Read)).Files("image");
            var checker = Helpers.BuildChecker(Path.Combine(Drive.GetDriveRoot(), path, Helpers.CheckFile));
            var results = shreds.Select(shred =>
                {
                    var filename = Path.GetFileName(shred);
                    bool? empty;
                    using (Bitmap bmp = new Bitmap(shred))
                    {
                        Assert.IsTrue(filename != null);
                        empty = Filter.IsEmpty(bmp);
                        Console.WriteLine("--------------------------------------");
                        Console.WriteLine("Shred Name" + Path.GetFileNameWithoutExtension(shred));
                        Console.WriteLine("IsEmpty? = " + empty);
                        Console.WriteLine("Is Really Empty? = " + checker[filename]);
                        Console.WriteLine("Correct: " + (checker[filename] == ((bool) empty ? "y" : "n")));
                        Console.WriteLine();
                    }
                    return (checker[filename] == ((bool) empty ? "y" : "n"));
                });
            results.ToList().ForEach(Assert.IsTrue);
        }
    }
}