#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Emgu.CV;
using Emgu.CV.Structure;
using NLog;

#endregion

namespace Caruso
{
    [Serializable()]
    public class Shred
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private static long _count = 0;

        public static readonly double[] Kernel = {-1.0, 0.0, 1.0};
        public readonly string Filepath;
        public readonly long Id;
        public List<int[]> Chamfer;

        public List<double[]> Convolution;
        public List<double[]> Luminousity;
        public List<long> Sparsity;
        public List<double[]> Thresholded;

        /// <summary>
        ///   Create a shred object given a filepath to a bitmap image
        /// </summary>
        /// <param name="filepath"> destination path of the shred image </param>
        /// <param name="ignoreTopBottom"> Default is true, set to false to scan top and bottom aswell </param>
        public Shred(string filepath, bool ignoreTopBottom = true)
        {
            Filepath = filepath;
            Id = _count++;
            _logger.Trace("Starting Chamfer From Left");

            int directions = ignoreTopBottom ? 4 : 8;
            Convolution = new List<double[]>(directions);
            Luminousity = new List<double[]>(directions);
            Chamfer = new List<int[]>(directions);
            Thresholded = new List<double[]>(directions);
            Sparsity = new List<long>(directions);
            Bitmap source = new Bitmap(filepath);
            var image = new Image<Bgra, Byte>(source);

            foreach (int side in Enum.GetValues(typeof (Direction)))
            {
                if (side >= directions)
                {
                    continue;
                }

                int regularIndex = Index((Direction) side, Orientation.Regular);
                int reverseIndex = Index((Direction) side, Orientation.Reversed);

                _logger.Trace("Measuring Side no:" + side);

                double[] luminousity = Forensics.Luminousity.RepresentativeLuminousity(image, 2, 4, (Direction) side);
                Luminousity.Insert(regularIndex, luminousity);
                Luminousity.Insert(reverseIndex, Utility.Reverse(luminousity));

                int[] indicies = Utility.GetKernelIndicies(Kernel, -1);
                double[] convolutions = Utility.Convolute(Luminousity[regularIndex], Kernel, indicies);
                Convolution.Insert(regularIndex, convolutions);
                Convolution.Insert(reverseIndex, Utility.Reverse(convolutions));

                double[] thresholded = Utility.Threshold(Utility.Absolute(Convolution[regularIndex]), 0.3);
                Thresholded.Insert(regularIndex, thresholded);
                Thresholded.Insert(reverseIndex, Utility.Reverse(thresholded));

                int[] chamfer = Forensics.Chamfer.Measure(Thresholded[regularIndex]);
                Chamfer.Insert(regularIndex, chamfer);
                Chamfer.Insert(reverseIndex, Utility.Reverse(chamfer));

                long sparsity = Forensics.Chamfer.Sparsity(Chamfer[regularIndex]);
                Sparsity.Insert(regularIndex, sparsity);
                Sparsity.Insert(reverseIndex, sparsity);
            }
        }

        private int Index(Direction direction, Orientation orientation)
        {
            return ((int) direction*2) + ((int) orientation);
        }

        /// <summary>
        ///   Serialize Shred to binary file on disk
        /// </summary>
        /// <param name="shred"> Shred Object </param>
        /// <param name="filename"> Destination File path </param>
        public static void Save(Shred shred, string filename)
        {
            _logger.Info("Serializing shred id={0} to filename={1}", shred.Id, filename);
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
            _logger.Info("Deserializing shred id={0} from filename={1}", objectToDeserialize.Id, filepath);
            return objectToDeserialize;
        }

        /// <summary>
        ///   Given two shreds, calculate the offset value at which the two shreds are most similar
        /// </summary>
        /// <param name="other"> The other shred to be compared to </param>
        /// <param name="directionA"> Direction of this shred to be compared </param>
        /// <param name="directionB"> Direction of the other shred to be compared </param>
        /// <returns> Tuple containing the max similarity value and the offset at which that occured </returns>
        public Tuple<double, int, double[]> ChamferSimilarity(Shred other, Direction directionA,
                                                              Orientation orientationA, Direction directionB,
                                                              Orientation orientationB)
        {
            double[] scan = Forensics.Chamfer.ScanSimilarity(GetChamfer(directionA), other.GetChamfer(directionB));
            double max = scan[0];
            int index;
            int best = 0;

            for (index = 0; index < scan.Length; index++)
            {
                if (scan[index] > max)
                {
                    max = scan[index];
                    best = index;
                }
            }
            return new Tuple<double, int, double[]>(max, best, scan);
        }


        public static List<Shred> Factory(string prefix, string directory, bool ignoreTopAndBottom = true)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException("could not find " + directory);
            }

            var files = Directory.EnumerateFiles(directory);
            var shreds = new List<Shred>();
            foreach (string file in files)
            {
                if (file.StartsWith(Path.Combine(directory, prefix)))
                {
                    shreds.Add(Create(file, ignoreTopAndBottom));
                }
            }
            return shreds;
        }


        private static Shred Create(string file, bool ignoreTopAndBottom = true)
        {
            return new Shred(file, ignoreTopAndBottom);
        }

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
                if (processed[ii] != 0.0)
                {
                    result[ii] = processed[ii];
                }
            }
            Visualizer.Plot(result, "Thresholded Convolutions");
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
    }

    /// <summary>
    ///   Scan Direction Enum
    /// </summary>
    public enum Direction
    {
        FromLeft,
        FromRight,
        FromTop,
        FromBottom
    }

    public enum Orientation
    {
        Regular,
        Reversed
    }
}