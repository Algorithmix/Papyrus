﻿#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
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

        /// <summary>
        ///   Initialize a new OCR Object.
        /// 
        ///   This object is a wrapper for the Emgu Tesseract Wrapper to give a level of abstraction
        ///   necessary for scanning shreds
        /// </summary>
        /// <param name="accuracy"> Desired Accuracy setting </param>
        /// <param name="language"> Language of text on image used for OCR model </param>
        /// <param name="enableTimer"> Set enable Timer to true to measure scan time for diagnostic purposes </param>
        public OCR(Accuracy accuracy = Accuracy.High, string language = "eng", bool enableTimer = false)
        {
            _timer = new Stopwatch();
            if (enableTimer)
            {
                _timer.Start();
            }
            Tesseract.OcrEngineMode mode = Tesseract.OcrEngineMode.OEM_TESSERACT_CUBE_COMBINED;
            switch (accuracy)
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
            _tesseract = new Tesseract("tessdata", language, mode);
            //"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");
            _tesseract.SetVariable("tessedit_unrej_any_wd", "T");
            //_tesseract.SetVariable("tessedit_char_whitelist","abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWZ0123456789.,");
            _text = null;
            _chars = null;
            _confidence = -1;
        }

        /// <summary>
        ///   Returns an array of Tesseract Charactors after running the Scan Method
        /// </summary>
        /// <returns> Tesseract Charactors generated after scan </returns>
        public Tesseract.Charactor[] Charactors()
        {
            return _chars;
        }

        /// <summary>
        ///   Getter for the text generated after running the Scan Method
        /// </summary>
        /// <returns> String of resulting text from ocr, often with newline and carriage return characters </returns>
        public string Text()
        {
            return _text;
        }

        /// <summary>
        ///   Returns the of cos
        /// </summary>
        /// <returns> </returns>
        public long OverallCost()
        {
            if (_chars == null)
            {
                return -1;
            }
            _confidence = Cost(_chars);
            return _confidence;
        }

        /// <summary>
        ///   OCR Preprocessing, currently this involves binary threholding 
        ///   a gray scaled image using the Otsu Method
        /// </summary>
        /// <param name="image"> Image to be preprocessed </param>
        /// <returns> new binary BW image </returns>
        public Image<Gray, byte> Preprocess(Image<Bgra, byte> image)
        {
            Image<Gray, byte> gray = Filter.RgbToGray(image);
            Image<Gray, byte> thresh = Filter.Threshold(gray);
            //Filter.EmbossText(thresh);
            gray.Dispose();
            return thresh;
        }

        /// <summary>
        ///   Given a Color image, it is coverted to grayscale OCR-ed and returned
        /// </summary>
        /// <param name="image"> Source Image to be OCR-ed </param>
        /// <returns> Text from Image </returns>
        public string Scan(Image<Bgra, byte> image)
        {
            Image<Gray, byte> gray = Filter.RgbToGray(image);
            string text = Scan(gray);
            gray.Dispose();
            return text;
        }

        /// <summary>
        ///   Invokes Tesseract OCR Recognize on given image
        ///   Stores the resulting data in the Text,Confidence and ScanTime data members
        /// </summary>
        /// <param name="image"> Source Image to be OCR-ed </param>
        /// <returns> Text from Image </returns>
        public string Scan(Image<Gray, byte> image)
        {
            _tesseract.Recognize(image);
            _text = _tesseract.GetText();
            _chars = _tesseract.GetCharactors();
            return _text;
        }

        /// <summary>
        ///   Execute OCR on a given image, this static member will process the image,
        ///   Safely open, execute and dispose a Tesseract Object and store the result
        ///   in a new OcrData object.
        /// </summary>
        /// <param name="original"> Image to be OCR-ed </param>
        /// <param name="mode"> Accuracy setting </param>
        /// <param name="lang"> Language of text for OCR Language Model </param>
        /// <param name="enableTimer"> Measure the Scantime for Diagnostic purposes </param>
        /// <returns> </returns>
        public static OcrData Recognize(Bitmap original,
                                        Accuracy mode = Accuracy.High,
                                        string lang = "eng",
                                        bool enableTimer = false)
        {
            Image<Bgra, byte> img = new Image<Bgra, byte>(original);
            Image<Gray, byte> processed;
            Tesseract.Charactor[] chars;
            String text;
            long confidence;
            long scantime = Int64.MinValue;
            using (OCR ocr = new OCR(mode, lang, enableTimer))
            {
                processed = ocr.Preprocess(img);
                ocr.Scan(processed);
                confidence = ocr.OverallCost();
                chars = ocr.Charactors();
                text = ocr.Text();
                if (enableTimer)
                {
                    scantime = ocr.Elapsed();
                    ocr.Stop();
                }
            }
            img.Dispose();
            if (scantime == Int64.MinValue)
            {
                return new OcrData(processed, chars, text, confidence);
            }
            return new OcrData(processed, chars, text, confidence, scantime);
        }

        /// <summary>
        ///   Parallelized Recognize Function takes in a list or array of images,
        ///   A specified length and for each image returns an OCRData object
        /// </summary>
        /// <param name="images"> Array or List of Bitmaps </param>
        /// <param name="length"> Number of items to be Recognized from the array </param>
        /// <param name="mode"> Accuracy Mode </param>
        /// <param name="lang"> Desired OCR Language </param>
        /// <param name="enableTimer"> Enables OCR Scan Timer if true </param>
        /// <returns> </returns>
        public static OcrData[] ParallelRecognize(IEnumerable<Bitmap> images,
                                                  int length,
                                                  Accuracy mode = Accuracy.High,
                                                  string lang = "eng",
                                                  bool enableTimer = false)
        {
            Tuple<int, Bitmap>[] indexedImages = new Tuple<int, Bitmap>[length];
            int index = 0;
            foreach (Bitmap image in images)
            {
                if (index >= length)
                {
                    break;
                }
                indexedImages[index] = new Tuple<int, Bitmap>(index, image);

                index += 1;
            }

            ConcurrentDictionary<int, OcrData> safeMap = new ConcurrentDictionary<int, OcrData>();

            Parallel.ForEach(indexedImages, pair =>
                {
                    int position = pair.Item1;
                    Bitmap image = pair.Item2;
                    safeMap[position] = Recognize(image, mode, lang, enableTimer);
                });

            OcrData[] data = new OcrData[length];
            foreach (KeyValuePair<int, OcrData> kvpair in safeMap)
            {
                data[kvpair.Key] = kvpair.Value;
            }

            return data;
        }

        /// <summary>
        ///   Calculates the cost by summing the unique cost of each word
        /// </summary>
        /// <param name="chars"> Tesseract OCR Charactor results </param>
        /// <returns> Cost, where zero is perfect and long.MaxValue is worst </returns>
        public static long Cost(Tesseract.Charactor[] chars)
        {
            if (chars.Length < 1)
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
                    total += (long)current;
                }
            }
            return total;
        }

        /// <summary>
        /// Given an array of shreds, 
        /// we OCR them all and save results to the shred object
        /// </summary>
        /// <param name="shreds">Array of initialized shred objects</param>
        /// <param name="lang">Desired OCR langauge</param>
        public static void ShredOcr(Shred[] shreds, string lang = "eng")
        {
            Bitmap[] images = new Bitmap[shreds.Length];
            Bitmap[] reversed = new Bitmap[shreds.Length];
            int index = 0;
            foreach (Shred shred in shreds)
            {
                images[index] = new Bitmap(shred.Filepath);
                reversed[index] = Filter.Reverse(images[index]);
                index += 1;
            }
            Tuple<long, OcrData, OcrData>[] results = ParallelDetectOrientation(images, reversed, Accuracy.Low, lang);

            for (int ii = 0; ii < results.Length; ii++)
            {
                long confidence = results[ii].Item1;
                if (confidence > 0)
                {
                    shreds[ii].AddOcrData(results[ii].Item2, Math.Abs(confidence), false);
                }
                else if (confidence <= 0)
                {
                    shreds[ii].AddOcrData(results[ii].Item3, Math.Abs(confidence), true);
                }
            }
        }

        /// <summary>
        /// Determines runs OCR and determines if a shred is empty or not
        /// </summary>
        /// <param name="shreds">A list of Shred Objects</param>
        public static void IsEmpty(Shred[] shreds)
        {
            // Get Images from Shreds
            Bitmap[] images = new Bitmap[shreds.Length];

            for (int ii = 0; ii < shreds.Length; ii++)
            {
                images[ii] = new Bitmap(shreds[ii].Filepath);
            }

            // Run Fast Recognition
            OcrData[] datas = ParallelRecognize(images, images.Length, Accuracy.Low);
            for (int ii = 0; ii < datas.Length; ii++)
            {
                shreds[ii].AddOcrData(datas[ii]);
            }
        }

        /// <summary>
        /// Parallelized OCR Orientation Confidence Detector, this method will run ocr on 
        /// an image and its corresponding reversed images and return the confidence and 
        /// both the ocrdata objects
        /// </summary>
        /// <param name="regs">Images with default regular orientation</param>
        /// <param name="revs">Images with reversed orientation to default</param>
        /// <param name="mode">OCR accuracy mode</param>
        /// <param name="lang">OCR languages</param>
        /// <param name="enableTimer">Enable timer for diagnostic purposes</param>
        /// <returns>Tuple containing confidence, regular ocrdata and reversed ocrdata.
        ///  Positive confidence is for Regular where as negative 
        ///  confidences is for Reversed</returns>
        public static Tuple<long, OcrData, OcrData>[] ParallelDetectOrientation(
            Bitmap[] regs,
            Bitmap[] revs,
            Accuracy mode = Accuracy.High,
            string lang = "eng",
            bool enableTimer = false)
        {
            if (regs.Length != revs.Length)
            {
                throw new ArgumentException("Input Arrays must be same length!");
            }

            // create new array and copy over image references
            int pivot = regs.Length;
            Bitmap[] images = new Bitmap[regs.Length + revs.Length];
            Array.Copy(regs, images, pivot);
            Array.Copy(revs, 0, images, pivot, pivot);

            // Run Parallel Recognition on the arrays
            OcrData[] datas = ParallelRecognize(images, pivot + pivot, mode, lang, enableTimer);

            // Extract results and calculate confidence
            Tuple<long, OcrData, OcrData>[] results = new Tuple<long, OcrData, OcrData>[pivot];
            for (int ii = 0; ii < pivot; ii++)
            {
                OcrData reg = datas[ii];
                OcrData rev = datas[ii + pivot];

                // If postive we are confident about the current orientation
                // if negative we are not confident about the current orientation
                long confidence = rev.Cost - reg.Cost;
                results[ii] = new Tuple<long, OcrData, OcrData>(confidence, reg, rev);
            }
            return results;
        }

        #region Diagnostics Timer Methods
        /// <summary>
        ///   Explicitly starts the diagnostics timer
        /// </summary>
        /// <returns> true if timer is start, false if timer is running </returns>
        private bool Start()
        {
            if (_timer.IsRunning)
            {
                return false;
            }
            _timer.Start();
            return true;
        }

        /// <summary>
        ///   Retrieve the time elapsed from the diagnostics Timer
        /// </summary>
        /// <returns> Milliseconds elapsed since start() was called </returns>
        private long Elapsed()
        {
            return _timer.ElapsedMilliseconds;
        }

        /// <summary>
        ///   Stops the diagnostics timer
        /// </summary>
        private void Stop()
        {
            _timer.Stop();
        }

        #endregion

        /// <summary>
        ///   Disposes all the necessary objects
        /// </summary>
        protected override void DisposeObject()
        {
            _tesseract.Dispose();
        }

        public static string StripNewLine(string text)
        {
            return text.Replace("\r\n", "");
        }
    }
}