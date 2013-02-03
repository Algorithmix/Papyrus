using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Algorithmix.TestTools;

namespace CarusoSample
{
    class OCRTest
    {
        private static string path = @"OCRTest\Snips";

        public static void run()
        {
            var drive = new Drive(path, Drive.Reason.Read);
            var files = drive.Files("snip_0");
            var first = files.First();
            var bitmap = new System.Drawing.Bitmap(first);
            var image = new Image<Bgra, byte>(bitmap);
            var tesseract = new Tesseract("tessdata", "eng", Tesseract.OcrEngineMode.OEM_TESSERACT_ONLY);
            tesseract.SetVariable("save_best_choices", "1"); 
            tesseract.Recognize(image);
            var charactors = tesseract.GetCharactors();

        }
    }
}
