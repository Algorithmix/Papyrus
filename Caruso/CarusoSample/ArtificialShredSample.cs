using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;

namespace CarusoSample
{
    class ArtificialShred
    {
        public static void DisplayTestShred()
        {
            // Create a Square
            Point[] patch1 = new Point[4];
            patch1[0] = new Point(0, 0);
            patch1[1] = new Point(0, 10);
            patch1[2] = new Point(99, 10);
            patch1[3] = new Point(99, 0);

            Point[] patch2 = new Point[4];
            patch2[0] = new Point(0, 50);
            patch2[1] = new Point(0, 60);
            patch2[2] = new Point(99, 60);
            patch2[3] = new Point(99, 50);

            // Create an Original Image
            var original = new Image<Bgr, Byte>(100, 100, new Bgr(Color.HotPink));
            original.FillConvexPoly(patch1, new Bgr(Color.Gray));
            original.FillConvexPoly(patch2, new Bgr(Color.Gray));
            ImageViewer display = new ImageViewer(original, "TestBitmap");
            display.ShowDialog();

        }
    }
}
