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

        public static void Save(Shred shred , string filename)
        {
            _logger.Info("Serializing shred id={0} to filename={1}",shred.Id,filename);
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, shred);
            stream.Flush();
            stream.Close();
        }

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

        public void VisualizeChamfers(Direction direction)
        {
            Visualizer.Plot(Chamfer[(int) direction], "Convolution Result");
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