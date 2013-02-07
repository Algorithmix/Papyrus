#region

using System;
using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.Util;

#endregion

namespace Algorithmix.Forensics
{
// ReSharper disable InconsistentNaming
    public class OCR : DisposableObject
// ReSharper restore InconsistentNaming
    {
        private readonly Tesseract _tesseract;
        private readonly Stopwatch _timer;
        private Tesseract.Charactor[] _chars;
        private long _confidence;
        private string _text;

        public OCR(Accuracy engine_mode = Accuracy.High ,string language = "eng", bool startTimer = false)
        {
            _timer = new Stopwatch();
            if (startTimer)
            {
                _timer.Start();
            }
            Tesseract.OcrEngineMode mode = Tesseract.OcrEngineMode.OEM_TESSERACT_CUBE_COMBINED;
            switch (engine_mode)
            {
                case Accuracy.Low:
                    mode = Tesseract.OcrEngineMode.OEM_TESSERACT_ONLY;
                    break;
                case Accuracy.Medium:
                    mode = Tesseract.OcrEngineMode.OEM_CUBE_ONLY;
                    break;
                case Accuracy.High:
                    mode = Tesseract.OcrEngineMode.OEM_TESSERACT_CUBE_COMBINED;
                    break;
            }
            _tesseract = new Tesseract("tessdata", language, mode , "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            _text = null;
            _chars = null;
            _confidence = -1;
        }

        public Tesseract.Charactor[] Charactors()
        {
            return _chars;
        }

        public string Text()
        {
            return _text;
        }

        public long OverallConfidence()
        {
            if (_chars == null)
            {
                return -1;
            }
            _confidence = Confidence(_chars);
            return _confidence;
        }

        public static long Confidence(Tesseract.Charactor[] chars)
        {
            if (chars.Length < 1 )
            {
                return 0;
            }
            long total = 0;
            double current = -1;
            foreach (Tesseract.Charactor charactor in chars)
            {
                if (Math.Abs(current - charactor.Cost) > 0.0001)
                {
                    current = charactor.Cost;
                    total += (long) current;
                }
            }
            return total;
        }

        public Image<Gray, byte> Preprocess(Image<Bgra, byte> image)
        {
            Image<Gray, byte> gray = Filter.RgbToGray(image);
            Image<Gray, byte> thresh = Filter.Threshold(gray);
            //Filter.EmbossText(thresh);
            gray.Dispose();
            return thresh;
        }

        public string Scan(Image<Bgra, byte> image)
        {
            Image<Gray, byte> gray = Filter.RgbToGray(image);
            string text = Scan(gray);
            gray.Dispose();
            return text;
        }

        public string Scan(Image<Gray, byte> image)
        {
            _tesseract.Recognize(image);
            _text = _tesseract.GetText();
            _chars = _tesseract.GetCharactors();
            return _text;
        }

        public static OcrData Recognize(Bitmap original, Accuracy mode = Accuracy.High, string lang = "eng", bool enableTimer = false)
        {
            Image<Bgra, byte> img = new Image<Bgra, Byte>(original);
            Image<Gray, byte> processed;
            Tesseract.Charactor[] chars;
            String text;
            long confidence;
            long scantime;
            using (OCR ocr = new OCR(mode, lang, enableTimer))
            {
                processed = ocr.Preprocess(img);
                ocr.Scan(processed);
                confidence = ocr.OverallConfidence();
                chars = ocr.Charactors();
                text = ocr.Text();
                scantime = ocr.Elapsed();
                ocr.Stop();
            }
            img.Dispose();
            return new OcrData(processed, chars, text, confidence);
        }

        public bool Start()
        {
            if (_timer.IsRunning)
            {
                return false;
            }
            _timer.Start();
            return true;
        }

        public long Elapsed()
        {
            return _timer.ElapsedMilliseconds;
        }

        public long Stop()
        {
            _timer.Stop();
            return _timer.ElapsedMilliseconds;
        }

        protected override void DisposeObject()
        {
            _tesseract.Dispose();
        }
    }
}