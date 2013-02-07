#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AForge.Imaging.Filters;
using Algorithmix.TestTools;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;

#endregion

namespace CarusoSample
{
    internal class OcrTest
    {
        // private static string path = @"OCRTest\Snips";
        private static readonly string path = @"PDFRequirement\Full1";

        public static void run()
        {
            var drive = new Drive(path, Drive.Reason.Read);
            var files = drive.Files("image6");
            var first = files.First();
            var bitmap = new Bitmap(first);
            var image = new Image<Bgra, byte>(bitmap);

            // tesseract.SetVariable("save_best_choices", "True");
            Filter(image);
            var filtered = Filter(image);
            var result = DoOcr(filtered);
        }

        public static Tesseract.Charactor[] DoOcr(Image<Gray, byte> img)
        {
            Tesseract.Charactor[] chars;
            string text;
            using (var tesseract = new Tesseract("tessdata", "eng", Tesseract.OcrEngineMode.OEM_TESSERACT_CUBE_COMBINED)
                )
            {
                tesseract.Recognize(img);
                text = tesseract.GetText();
                chars = tesseract.GetCharactors();
            }
            Console.WriteLine(text);
            Console.ReadLine();
            return chars;
        }

        public static Image<Gray, byte> Filter(Image<Bgra, byte> original)
        {
            var gray = original.Convert<Gray, byte>();
            var binary = new Image<Gray, byte>(new OtsuThreshold().Apply(gray.Bitmap));
            var canny = new Image<Gray, byte>(new CannyEdgeDetector().Apply(gray.Bitmap));
            var list = new List<Rectangle>();
            using (MemStorage stor = new MemStorage())
            {
                for (
                    Contour<Point> contours = canny.FindContours(
                        CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                        RETR_TYPE.CV_RETR_EXTERNAL,
                        stor);
                    contours != null;
                    contours = contours.HNext)
                {
                    Rectangle rect = contours.BoundingRectangle;
                    list.Add(rect);
                }
            }
            //list.Where(rect => rect.Height * rect.Width < 100)
            //    .ToList().ForEach( rect => binary.Draw(rect, new Gray(1.0) ,-1));

            binary._Erode(1);
            binary._Dilate(1);
            return binary;
        }
    }
}