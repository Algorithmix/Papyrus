#region

using System.Drawing;
using System.IO;
using Algorithmix.Preprocessing;
using Algorithmix.TestTools;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace Algorithmix.UnitTest
{
    [TestClass]
    public class HeuristicsTest
    {
        [TestMethod]
        public void DetectBackgroundTest()
        {
            var path1 = "PicassoUnitTest/DetectBackgroundTest/249-238-32.png";

            var filepath1 = Path.Combine(Drive.GetDriveRoot(), path1);
            Bitmap image1 = new Bitmap(filepath1);

            Bgr expectedBackGround = new Bgr(249, 238, 32);
            Bgr actualBackGround = Heuristics.DetectBackground(image1, 10);
            Assert.IsTrue(Utility.IsEqual(expectedBackGround, actualBackGround));
        }
    }
}