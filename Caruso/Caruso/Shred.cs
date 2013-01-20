#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
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
        /// Create a shred object given a filepath to a bitmap image
        /// </summary>
        /// <param name="filepath">destination path of the shred image</param>
        /// <param name="ignoreTopBottom">Default is true, set to false to scan top and bottom aswell</param>
        public Shred(string filepath, bool ignoreTopBottom = true)
        {
            Filepath = filepath;
            Id = Shred._count++;
            _logger.Trace("Starting Chamfer From Left");

            int directions = ignoreTopBottom ? 2 : 4;
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

                _logger.Trace("Measuring Side no:" + side);
                Forensics.Luminousity.RepresentativeLuminousity(image, 2, 4, (Direction) side);
                Luminousity.Add(Forensics.Luminousity.RepresentativeLuminousity(image, 2, 4, (Direction) side));

                int[] indicies = Utility.GetKernelIndicies(Kernel, -1);
                Convolution.Add(Utility.Convolute(Luminousity[side], Kernel, indicies));
                Thresholded.Add(Utility.Threshold(Utility.Absolute(Convolution[side]), 0.3));
                Chamfer.Add(Forensics.Chamfer.Measure(Thresholded[side]));
                Sparsity.Add(Forensics.Chamfer.Sparsity(Chamfer[side]));
            }
        }

        /// <summary>
        /// Serialize Shred to binary file on disk
        /// </summary>
        /// <param name="shred">Shred Object</param>
        /// <param name="filename">Destination File path</param>
        public static void Save(Shred shred , string filename)
        {
            _logger.Info("Serializing shred id={0} to filename={1}",shred.Id,filename);
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, shred);
            stream.Flush();
            stream.Close();
        }

        /// <summary>
        /// Deserialize binary shred on disk into memory
        /// </summary>
        /// <param name="filepath">filepath of the deserialized shred</param>
        /// <returns>A new shred object from the serialized binary</returns>
        public static Shred Load(string filepath)
        {
            if( !File.Exists(filepath))
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
        /// Given two shreds, calculate the offset value at which the two shreds are most similar
        /// </summary>
        /// <param name="other">The other shred to be compared to</param>
        /// <param name="directionA">Direction of this shred to be compared</param>
        /// <param name="directionB">Direction of the other shred to be compared</param>
        /// <returns>Tuple containing the max similarity value and the offset at which that occured</returns>
        public Tuple<double,int,double[]> ChamferSimilarity(Shred other, Direction directionA, Direction directionB)
        {
            double[] scan =  Caruso.Forensics.Chamfer.ScanSimilarity(this.GetChamfer(directionA),other.GetChamfer(directionB));
            double max = scan[0];
            int index;
            int best = 0;
            
            for( index = 0; index < scan.Length ; index++ )
            {
                if( scan[index] > max )
                {
                    max = scan[index];
                    best = index;
                }
            }
            return new Tuple<double,int,double[]>(max,best,scan);
        }

        /// <summary>
        /// Plots a trace of the Luminousity
        /// </summary>
        /// <param name="direction">Direction to be traced</param>
        public void VisualizeLuminousity(Direction direction)
        {
            Visualizer.Plot(Luminousity[(int)direction], "Luminousity Trace");
        }

        /// <summary>
        /// Plots a trace of the threshold 
        /// </summary>
        /// <param name="direction">Direction to be traced</param>
        public void VisualizeThresholded(Direction direction)
        {
            var processed = this.Thresholded[(int)direction];
            var result = Utility.Absolute(this.Convolution[(int)direction]);
            for (int ii = 0; ii < processed.Length; ii++)
            {
                if (processed[ii] != 0.0)
                {
                    result[ii] = processed[ii];
                }
            }
            Caruso.Visualizer.Plot(result, "Thresholded Convolutions");
        }

        /// <summary>
        /// Plots a trace of the Chamfering
        /// </summary>
        /// <param name="direction">Direction to be traced</param>
        public void VisualizeChamfers(Direction direction)
        {
            Visualizer.Plot(Chamfer[(int) direction], "Chamfer Trace");
        }

        public int[] GetChamfer(Direction direction)
        {
            return Chamfer[(int) direction];
        }

        public double[] GetLuminousity(Direction direction)
        {
            return Luminousity[(int) direction];
        }

        public double[] GetThresholded(Direction direction)
        {
            return Thresholded[(int) direction];
        }

        public double[] GetConvolution(Direction direction)
        {
            return Convolution[(int) direction];
        }

        public long GetSparsity(Direction direction)
        {
            return Sparsity[(int) direction];
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
}