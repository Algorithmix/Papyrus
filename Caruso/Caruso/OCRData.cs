﻿#region

using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;

#endregion

namespace Algorithmix.Forensics
{
    public class OcrData
    {
        public readonly Tesseract.Charactor[] Charactors;
        public readonly long Cost;
        public readonly Image<Gray, byte> Source;
        public readonly string Text;
        public readonly long ScanTime;
        
        public OcrData(Image<Gray, byte> source,
                       Tesseract.Charactor[] charactors,
                       string text,
                       long cost,
                       long scantime = long.MinValue)
        {
            Charactors = charactors;
            Text = text;
            Cost = cost;
            Source = source;
            ScanTime = scantime;
        }

        public static List<Image<TColor, TDepth>>
            GetBoundedImages<TColor, TDepth>
            (Image<TColor, TDepth> source, Rectangle[] rects)
            where TColor : struct, IColor
            where TDepth : new()
        {
            List<Image<TColor, TDepth>> result = new List<Image<TColor, TDepth>>(rects.Length);
            foreach (Rectangle rect in rects)
            {
                result.Add(source.Copy(rect));
            }
            return result;
        }

        public static Rectangle[] BoundingBoxes(Tesseract.Charactor[] chars)
        {
            Rectangle[] rects = new Rectangle[chars.Length];
            for (int ii = 0; ii < chars.Length; ii++)
            {
                rects[ii] = chars[ii].Region;
            }
            return rects;
        }
        
        public static bool IsBetter( OcrData first, OcrData second )
        {
            return first.Cost < second.Cost;
        }
    }
}