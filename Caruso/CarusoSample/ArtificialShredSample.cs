#region

using Caruso;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;

#endregion

namespace CarusoSample
{
    internal class ArtificialShred
    {
        public static void DisplayTestShred()
        {
            //Create original Square
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

            // Create a Tester Square to compare
            Point[] patch3 = new Point[4];
            patch3[0] = new Point(0, 110);
            patch3[1] = new Point(0, 120);
            patch3[2] = new Point(99, 120);
            patch3[3] = new Point(99, 110);

            Point[] patch4 = new Point[4];
            patch4[0] = new Point(0, 160);
            patch4[1] = new Point(0, 170);
            patch4[2] = new Point(99, 170);
            patch4[3] = new Point(99, 160);

            // Create an Original Image
            var original = new Image<Bgr, Byte>(100, 100, new Bgr(Color.HotPink));
            original.FillConvexPoly(patch1, new Bgr(Color.Gray));
            original.FillConvexPoly(patch2, new Bgr(Color.Gray));

            //Create Image to compare with
            var tester = new Image<Bgr, Byte>(100, 200, new Bgr(Color.HotPink));
            tester.FillConvexPoly(patch3, new Bgr(Color.Gray));
            tester.FillConvexPoly(patch4, new Bgr(Color.Gray));

            //ImageViewer display = new ImageViewer(original, "TestBitmap");
            //display.ShowDialog();
            //ImageViewer display2 = new ImageViewer(tester, "Test Image");
            //display2.ShowDialog();

            const string filepath = "originalshrd.bmp";
            const string filepath2 = "testshred.bmp";

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            if (File.Exists(filepath2))
            {
                File.Delete(filepath2);
            }
            original.ToBitmap().Save(filepath);
            tester.ToBitmap().Save(filepath2);
            Shred originalshred = new Shred(filepath);
            Shred testershred = new Shred(filepath2);
            originalshred.VisualizeLuminousity(Direction.FromLeft);
            originalshred.VisualizeThresholded(Direction.FromLeft);
            originalshred.VisualizeChamfers(Direction.FromLeft);
            testershred.VisualizeThresholded(Direction.FromLeft);
            testershred.VisualizeLuminousity(Direction.FromLeft);
            testershred.VisualizeChamfers(Direction.FromRight);
        }
    }
}