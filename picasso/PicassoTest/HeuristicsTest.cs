using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Picasso;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Algorithmix.TestTools;

namespace PicassoTest
{
    [TestClass]
    public class HeuristicsTest
    {
        [TestMethod]
        public void DetectBackgroundTest()
        {
            //var path1 = "PicassoUnitTest/DetectBackgroundTest/14-211-222.png";
            //var path1 = "PicassoUnitTest/DetectBackgroundTest/107-183-51.png";
            var path1 = "PicassoUnitTest/DetectBackgroundTest/249-238-32.png";

            var filepath1 = Path.Combine(Drive.GetDriveRoot(), path1);
            Bitmap image1 = new Bitmap(filepath1);

            Bgr expectedBackGround = new Bgr(249,238,32);
            Bgr actualBackGround = Picasso.Heuristics.DetectBackground(image1, 10);
            Assert.IsTrue(Picasso.Utility.IsEqual(expectedBackGround,actualBackGround));
        }
    }
}
