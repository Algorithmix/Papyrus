using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithmix
{
    public partial class Shred
    {

        /// <summary>
        ///   Given two shreds, calculate the offset value at which the two shreds are most similar
        /// </summary>
        /// <param name="other"> The other shred to be compared to </param>
        /// <param name="directionA"> Direction of this shred to be compared </param>
        /// <param name="orientationA">Orientation of this shred to be compared</param>
        /// <param name="directionB"> Direction of the other shred to be compared </param>
        /// <param name="orientationB">Orientiation of the other shred to be compared</param>
        /// <returns> Tuple containing the max similarity value and the offset at which that occured </returns>
        public Tuple<double, int, double[]> Compare(Shred other, 
                                                    Direction directionA,
                                                    Orientation orientationA, 
                                                    Direction directionB,
                                                    Orientation orientationB)
        {
            double[] scan = Forensics.Chamfer.ScanSimilarity(
                                this.GetChamfer(directionA, orientationA), 
                                other.GetChamfer(directionB, orientationB));

            Tuple<double, int> maxData = Utility.Max(scan);
            double max = maxData.Item1;
            int best = maxData.Item2;

            return new Tuple<double, int, double[]>(max, best, scan);
        }
    }
}
