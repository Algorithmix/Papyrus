using System;
using System.Drawing.Imaging;
using Algorithmix;
using System.IO;
using System.Drawing;
using System.Text;
using System.Collections.Generic;
using Algorithmix.TestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algorithmix.UnitTest
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class StitcherTest
    {
        [TestMethod]
        public void StitcherPrimitiveTest()
        {
            var shreds = Shred.Factory("Shred",Path.Combine(Drive.GetDriveRoot(),Dir.StitcherTestDirectory, "PrimitiveTest"),false);
            var bitmap = Stitcher.Merge(shreds);
            bitmap.Save("StitcherPrimitiveTest.png",ImageFormat.Png);
        }

        [TestMethod]
        public void StitcherArtificialTest()
        {
            var shreds = Shred.Factory("image", Path.Combine(Drive.GetDriveRoot(), Dir.ArtificialTestDirectory, Dir.ArtificialHttpDocument), false);
            var bitmap = Stitcher.Merge(shreds);
            bitmap.Save("StitcherArtificialTest.png", ImageFormat.Png);
        }
    }
}
