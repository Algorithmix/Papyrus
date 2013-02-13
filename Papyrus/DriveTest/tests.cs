#region

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Algorithmix.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        public readonly string UnitTestFolder = "TestDriveUnitTest";
        public readonly string TestMaterialFolder = "RawShreds";

        [TestMethod]
        public void GetDriveRootTest()
        {
            Assert.IsNotNull(Drive.GetDriveRoot());
        }

        [TestMethod]
        public void TestSaveAndLoad()
        {
            const string saveDirectory = "TestSaving";
            RemoveDirectory(Path.Combine(Drive.GetDriveRoot(), UnitTestFolder, saveDirectory));
            var saveFolder = new Drive(Path.Combine(UnitTestFolder, saveDirectory), Drive.Reason.Save);
            Assert.IsTrue(saveFolder.FileCount() == 0);

            var loadFolder = new Drive(Path.Combine(UnitTestFolder, TestMaterialFolder), Drive.Reason.Read);
            var totalFileCount = loadFolder.FileCount();
            Assert.IsTrue(totalFileCount > 0);

            var bitmaps = new List<Bitmap>(totalFileCount);
            foreach (string filepath in loadFolder.FullFilePaths())
            {
                var bitmap = new Bitmap(filepath);
                bitmaps.Add(bitmap);
            }
            Assert.IsTrue(bitmaps.Count == totalFileCount);

            saveFolder.Save(bitmaps, "unittest");
            Assert.IsTrue(saveFolder.FileCount("unittest") == totalFileCount);
            Assert.IsTrue(saveFolder.FileCount() == totalFileCount);
        }

        [TestMethod]
        public void TestExceptions()
        {
        }

        [TestMethod]
        public void TestGetAllMatching()
        {
            const string dir = "TestRecursiveMatching";
            RemoveDirectory(Path.Combine(Drive.GetDriveRoot(), UnitTestFolder, dir));
            var expected = new List<string>();
            BuildNestedDirectory(Path.Combine(Drive.GetDriveRoot(), UnitTestFolder, dir), expected);
            var drive = new Drive(Path.Combine(UnitTestFolder, dir), Drive.Reason.Read);
            var actual = drive.GetAllMatching("image");
            expected.Sort();
            actual.Sort();
            var same = expected.Zip(actual, (first, second) => first == second).All(x => x);
            Assert.IsTrue(same);
        }

        private void BuildNestedDirectory(string dir, List<string> filenames, int size = 4)
        {
            if (size <= 0)
            {
                return;
            }
            Directory.CreateDirectory(dir);
            var file1 = Path.Combine(dir, "image_1" + size + ".jpg");
            var file2 = Path.Combine(dir, "image_2" + size + ".jpg");
            filenames.Add(file1);
            filenames.Add(file2);

            File.Create(file1).Dispose();
            File.Create(file2).Dispose();
            File.Create(Path.Combine(dir, "banana.jpg")).Dispose();

            var newdir1 = Path.Combine(dir, "fruits");
            var newdir2 = Path.Combine(dir, "vegetables");
            BuildNestedDirectory(newdir1, filenames, size - 1);
            BuildNestedDirectory(newdir2, filenames, size - 1);
        }

        private void RemoveDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            var files = Directory.EnumerateFiles(path);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            foreach (string dir in Directory.GetDirectories(path))
            {
                RemoveDirectory(dir);
            }
            Directory.Delete(path);
        }
    }
}