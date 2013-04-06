﻿#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Algorithmix.Forensics;
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

#endregion

namespace Algorithmix
{
    [Serializable]
    public partial class Shred : INode
    {
        #region DataMembers

        public static double THRESHOLD = 0.2;
        public static int BUFFER = 3;
        public static int SAMPLE_SIZE = 4;
        public static int OCR_EMPTY_THRESHOLD = 3;
        public static bool JACCARD = true;
        public static string CsvDestination = "";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static long _count;
        public static readonly double[] ConvolutionKernel = {-1.0, 0.0, 1.0};

        private Orientation _orientation;
        private Orientation _trueOrientation = Orientation.Regular;
        public OcrData OcrResult { get; private set; }
        public long OrientationConfidence { get; private set; }
        public bool IsEmpty { get; private set; }

        public readonly string Filepath;
        public readonly long Id;
        public Image<Bgra, byte> RawImage;

        public List<int[]> Chamfer;
        public List<double[]> Convolution;
        public List<double[]> Luminousity;
        public List<long> Sparsity;
        public List<double[]> Thresholded;
        public List<int[]> Offsets;
        public List<int[]> Jaccard;
 
        #endregion

        #region Constructor and Factory Methods

        public static void Dump(List<Shred> list, string directory )
        {
            foreach (Shred shred in list)
            {
                ShredToCsv(shred,Path.Combine(directory,Path.GetFileNameWithoutExtension(shred.Filepath)+".csv"));
                shred.RawImage.ToBitmap().Save(Path.Combine(directory,Path.GetFileName(shred.Filepath)));
            }
        }

        public static void ShredToCsv(Shred shred, String filepath)
        {
            StringBuilder sb= new StringBuilder();
            var left = Forensics.Chamfer.ScaleJaccard(shred.GetJaccard(Direction.FromLeft),10000);
            var right = Forensics.Chamfer.ScaleJaccard(shred.GetJaccard(Direction.FromRight),10000);
            foreach (int value in left)
            {
                sb.Append(value);
                sb.Append(",");
            }
            sb.AppendLine("");

            foreach (int value in right)
            {
                sb.Append(value);
                sb.Append(",");
            }
            sb.AppendLine("");
            
            File.WriteAllText(filepath, sb.ToString());
        }

        /// <summary>
        ///   Create a shred object given a filepath to a bitmap image
        /// </summary>
        /// <param name="filepath"> destination path of the shred image </param>
        /// <param name="ignoreTopBottom"> Default is true, set to false to scan top and bottom aswell </param>
        public Shred(string filepath, bool ignoreTopBottom = true)
        {
            InitializeINode();
            Filepath = filepath;
            Id = _count++;

            Logger.Trace("Starting Chamfer From Left");

            int directions = ignoreTopBottom ? 4 : 8;
            Convolution = new List<double[]>(directions);
            Luminousity = new List<double[]>(directions);
            Chamfer = new List<int[]>(directions);
            Thresholded = new List<double[]>(directions);
            Sparsity = new List<long>(directions);
            Offsets = new List<int[]>(directions);
            Jaccard = new List<int[]>(directions);

            using (Bitmap source = new Bitmap(filepath))
            {
                this.RawImage = new Image<Bgra, Byte>(source);

                // Initialize List for Random Access
                for (int ii = 0; ii < directions; ii++)
                {
                    Convolution.Add(new double[0]);
                    Luminousity.Add(new double[0]);
                    Thresholded.Add(new double[0]);
                    Chamfer.Add(new int[0]);
                    Sparsity.Add((long) -1.0);
                    Offsets.Add(new int[0]);
                    Jaccard.Add(new int[0]);
                }

                foreach (int side in Enum.GetValues(typeof (Direction)))
                {
                    // 2 per side
                    if (side*2 >= directions)
                    {
                        continue;
                    }

                    int regularIndex = Index((Direction) side, Orientation.Regular);
                    int reverseIndex = regularIndex + 1; //Index((Direction) side, Orientation.Reversed);

                    Logger.Trace("Measuring Side no:" + side);

                    int[] offset = EdgeDetector.EdgePoints(source, (Direction) side);
                    Offsets[regularIndex] = offset;
                    Offsets[reverseIndex] = Utility.Reverse(offset);

                    double[] luminousity = Forensics.Luminousity.RepresentativeLuminousity(RawImage, BUFFER, SAMPLE_SIZE,
                                                                                             (Direction) side);
                    Luminousity[regularIndex] = luminousity;
                    Luminousity[reverseIndex] = Utility.Reverse(luminousity);

                    int[] jaccard = Forensics.Luminousity.Jaccard(luminousity);
                    Jaccard[regularIndex] = jaccard;
                    Jaccard[reverseIndex] = Utility.Reverse(jaccard);

                    int[] indicies = Utility.GetKernelIndicies(ConvolutionKernel, -1);
                    double[] convolutions = Utility.Convolute(Luminousity[regularIndex], ConvolutionKernel, indicies);
                    Convolution[regularIndex] = convolutions;
                    Convolution[reverseIndex] = Utility.Reverse(convolutions);

                    double[] thresholded = Utility.Threshold(Utility.Absolute(Convolution[regularIndex]), THRESHOLD);
                    Thresholded[regularIndex] = thresholded;
                    Thresholded[reverseIndex] = Utility.Reverse(thresholded);

                    int[] chamfer = Forensics.Chamfer.Measure(Thresholded[regularIndex]);
                    Chamfer[regularIndex] = chamfer;
                    Chamfer[reverseIndex] = Utility.Reverse(chamfer);

                    long sparsity = Forensics.Chamfer.Sparsity(Chamfer[regularIndex]);
                    Sparsity[regularIndex] = sparsity;
                    Sparsity[reverseIndex] = sparsity;
                }
            }
        }

        public static List<Shred> Factory(List<string> files, bool runOcr = true)
        {
            List<Shred> shreds = new List<Shred>();
            foreach (string file in files)
            {
                shreds.Add(Create(file, true));
            }

            if (runOcr)
            {
                OCR.ShredOcr(shreds.ToArray());
            }

            if (CsvDestination!="")
            {
                Dump(shreds,CsvDestination);
            }
            return shreds;
        }

        /// <summary>
        ///   Factory Method loads a bunch of shreds from a directory given a prefix to match
        /// </summary>
        /// <param name="prefix"> prefix to match within shred folder </param>
        /// <param name="directory"> path where folder is located </param>
        /// <param name="runOcr"> Run Optical Character Recognition for Page Text and Orientation Detection </param>
        /// <returns> A list of Shreds </returns>
        public static List<Shred> Factory(string prefix, string directory, bool runOcr = true)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException("could not find " + directory);
            }

            IEnumerable<string> files = Directory.EnumerateFiles(directory);
            List<string> matchingFiles = new List<string>(100);
            foreach (string filepath in files)
            {
                if (filepath.StartsWith(Path.Combine(directory, prefix)))
                {
                    matchingFiles.Add(filepath);
                }
            }
            return Factory(matchingFiles, runOcr);
        }

        private static Shred Create(string file, bool ignoreTopAndBottom = true)
        {
            return new Shred(file, ignoreTopAndBottom);
        }

        #endregion

        # region Getter and Setters

        public Bitmap Bitmap
        {
            get { return GetBitmap(Orientation); }
        }

        /// <summary>
        ///   Load a bitmap image of the shred with a specific orientation
        /// </summary>
        /// <param name="orientation"> Specific Orientation, regular or reversed, default is regular </param>
        /// <returns> Bitmap Image of SHred </returns>
        public Bitmap GetBitmap(Orientation orientation = Orientation.Regular)
        {
            Bitmap bitmap = new Bitmap(Filepath);
            if (orientation == Orientation.Reversed)
            {
                Filter.ReverseInPlace(bitmap);
            }
            return bitmap;
        }

        /// <summary>
        ///   Standard Add OCR will filter a shred if it is empty
        /// </summary>
        /// <param name="results"> Shreds Ocr Data </param>
        public void AddOcrData(OcrData results)
        {
            OcrResult = results;
            if (OCR.StripNewLine(OcrResult.Text).Length <= OCR_EMPTY_THRESHOLD)
            {
                using (Bitmap bmp = new Bitmap(Filepath))
                {
                    IsEmpty = Filter.IsEmpty(bmp);
                }
            }
            else
            {
                IsEmpty = false;
            }
        }

        /// <summary>
        ///   Set OCR Results, and Orientation Confidence on an Object
        /// </summary>
        /// <param name="results"> The Orientation Results from the OCR execution </param>
        /// <param name="orienationConfidence"> Absolute Orientation Confidence </param>
        /// <param name="isUpsideDown"> Indicates if True orientation is different than the current </param>
        public void AddOcrData(OcrData results, long orienationConfidence, bool isUpsideDown)
        {
            AddOcrData(results);
            _trueOrientation = isUpsideDown ? Enumeration.Opposite(Orientation) : Orientation;
            OrientationConfidence = orienationConfidence;
        }

        /// <summary>
        ///   Returns the true orientation of the object
        /// </summary>
        public Orientation? TrueOrienation
        {
            get
            {
                if (OrientationConfidence != long.MinValue && OrientationConfidence > 100)
                {
                    return _trueOrientation;
                }
                return null;
            }
        }

        /// <summary>
        ///   Returns the current orientation with respect to the default (from Fileload)
        /// </summary>
        public Orientation Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        /// <summary>
        ///   Helper for converting Orientation + Direction into an index number
        /// </summary>
        /// <param name="direction"> </param>
        /// <param name="orientation"> </param>
        /// <returns> </returns>
        private int Index(Direction direction, Orientation orientation)
        {
            if (orientation == Orientation.Reversed)
            {
                return ((int) Enumeration.Opposite(direction)*2) + 1;
            }
            return ((int) direction*2);
        }

        /// <summary>
        ///   Plots a trace of the Chamfering
        /// </summary>
        /// <param name="direction"> Direction to be traced </param>
        /// <param name="orientation"> Orientation to be traced </param>
        public void VisualizeChamfers(Direction direction, Orientation orientation = Orientation.Regular)
        {
            Visualizer.Plot(Chamfer[Index(direction, orientation)], "Chamfer Trace");
        }

        public int[] GetJaccard(Direction direction, Orientation orientation = Orientation.Regular)
        {
            return Jaccard[Index(direction, orientation)];
        }

        public int[] GetChamfer(Direction direction, Orientation orientation = Orientation.Regular)
        {
            return Chamfer[Index(direction, orientation)];
        }

        public double[] GetLuminousity(Direction direction, Orientation orientation = Orientation.Regular)
        {
            return Luminousity[Index(direction, orientation)];
        }

        public double[] GetThresholded(Direction direction, Orientation orientation = Orientation.Regular)
        {
            return Thresholded[Index(direction, orientation)];
        }

        public double[] GetConvolution(Direction direction, Orientation orientation = Orientation.Regular)
        {
            return Convolution[Index(direction, orientation)];
        }

        public long GetSparsity(Direction direction, Orientation orientation = Orientation.Regular)
        {
            return Sparsity[Index(direction, orientation)];
        }

        #endregion

        #region Static Helpers

        /// <summary>
        ///   Serialize Shred to binary file on disk
        /// </summary>
        /// <param name="shred"> Shred Object </param>
        /// <param name="filename"> Destination File path </param>
        public static void Save(Shred shred, string filename)
        {
            Logger.Info("Serializing shred id={0} to filename={1}", shred.Id, filename);
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, shred);
            stream.Flush();
            stream.Close();
        }

        /// <summary>
        ///   Deserialize binary shred on disk into memory
        /// </summary>
        /// <param name="filepath"> filepath of the deserialized shred </param>
        /// <returns> A new shred object from the serialized binary </returns>
        public static Shred Load(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileNotFoundException();
            }
            Stream stream = File.Open(filepath, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            Shred objectToDeserialize = (Shred) binaryFormatter.Deserialize(stream);
            stream.Flush();
            stream.Close();
            Logger.Info("Deserializing shred id={0} from filename={1}", objectToDeserialize.Id, filepath);
            return objectToDeserialize;
        }

        #endregion

        #region Helpers

        /// <summary>
        ///   Plots a trace of the Luminousity
        /// </summary>
        /// <param name="direction"> Direction to be traced </param>
        /// <param name="orientation"> Orientation to be traced </param>
        public void VisualizeLuminousity(Direction direction, Orientation orientation = Orientation.Regular)
        {
            Visualizer.Plot(Luminousity[Index(direction, orientation)], "Luminousity Trace");
        }

        /// <summary>
        ///   Plots a trace of the threshold
        /// </summary>
        /// <param name="direction"> Direction to be traced </param>
        /// <param name="orientation"> Orientation to be traced </param>
        public void VisualizeThresholded(Direction direction, Orientation orientation = Orientation.Regular)
        {
            var processed = Thresholded[Index(direction, orientation)];
            var result = Utility.Absolute(Convolution[Index(direction, orientation)]);
            for (int ii = 0; ii < processed.Length; ii++)
            {
                if (Math.Abs(processed[ii] - 0.0) > 0.01)
                {
                    result[ii] = processed[ii];
                }
            }
            Visualizer.Plot(result, "Thresholded Convolutions");
        }

        #endregion
    }
}