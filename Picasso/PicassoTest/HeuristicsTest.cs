using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Picasso;
using System.Drawing;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Windows.Forms;
namespace PicassoTest
{
    [TestClass]
    public class HeuristicsTest
    {
        [TestMethod]
        public void DetectBackgroundTest()
        {
            // Create a Square
            Point[] shred = new Point[4];
            shred[0] = new Point(0,0);
            shred[1] = new Point(0, 99);
            shred[2] = new Point(10, 99);
            shred[3] = new Point(10, 0);

            // Create an Original Image
            var original = new Image<Bgr, Byte>(100, 100, new Bgr(Color.HotPink));
            original.FillConvexPoly(shred, new Bgr(Color.Gray));

            var expected = new Bgr(Color.HotPink);

            Console.WriteLine("Performing Heuristic Background Detection");
            var actual = Heuristics.DetectBackground(original.ToBitmap());
            //Emgu.CV.UI.ImageViewer.Show(original, "Original");
            Assert.IsTrue( Picasso.Utility.IsEqual(expected,actual) );

        }
    }
}
